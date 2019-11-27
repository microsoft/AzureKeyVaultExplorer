// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 

namespace Microsoft.Vault.Library
{
    using Core;
    using Microsoft.Azure.KeyVault;
    using Microsoft.Azure.KeyVault.Models;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Single or dual Vault class to manage secrets
    /// For HA and DR story this supports up to two Azure Key Vaults, one in each region in the specified geo 
    /// </summary>
    /// <remarks>
    /// Based on vault name and <see cref="Vault.VaultsConfig"/> appropriate access will be picked in the following order:
    /// 1. <see cref="VaultAccessClientCertificate"/> - client id (application id) in AzureAD will be selected with right certificate thumbprint (sha1) of the application's principal to get the access
    /// 2. <see cref="VaultAccessClientCredential"/> - client id and client secret will be used to get the access
    /// 3. <see cref="VaultAccessUserInteractive"/> - client id (powershell app id) and user credentials will be used to get the access
    /// </remarks>
    public class Vault
    {
        private readonly KeyVaultClientEx[] _keyVaultClients;
        private bool Secondary => (_keyVaultClients.Length == 2);

        public readonly string VaultsConfigFile;
        public readonly string[] VaultNames;
        public readonly VaultsConfig VaultsConfig;

        private static readonly Task CompletedTask = Task.FromResult(0); // Dummy completed task to be used for secondary operations, in case we work with only Primary vault
        private static readonly object Lock = new object();

        /// <summary>
        /// UserPrincipalName, in UPN format of the currently authenticated user, in case of cert based access the value will be: {Environment.UserDomainName}\{Environment.UserName}
        /// The value will be set only after successful opertaion to vault, like: <see cref="ListSecretsAsync(int, ListOperationProgressUpdate, CancellationToken)"/>
        /// </summary>
        public string AuthenticatedUserName { get; private set; }

        /// <summary>
        /// Delegate to indicate progress
        /// </summary>
        /// <param name="position">Current position in the list of secrets, keys or certificates</param>
        public delegate void ListOperationProgressUpdate(int position);

        #region Constructors

        /// <summary>
        /// Creates the vault management instance based on provided Vaults Config dictionary
        /// </summary>
        /// <param name="vaultsConfig">Vaults Config dictionary</param>
        /// <param name="accessType">ReadOnly or ReadWrite</param>
        /// <param name="vaultNames">Single or Dual</param>
        public Vault(VaultsConfig vaultsConfig, VaultAccessTypeEnum accessType, params string[] vaultNames)
        {
            Guard.ArgumentNotNull(vaultsConfig, nameof(vaultsConfig));
            Guard.ArgumentCollectionNotEmpty(vaultNames, nameof(vaultNames));
            VaultsConfig = vaultsConfig;
            VaultNames = (from v in vaultNames where !string.IsNullOrEmpty(v) select v).ToArray();
            switch (VaultNames.Length)
            {
                case 1:
                    _keyVaultClients = new KeyVaultClientEx[1]
                    {
                        CreateKeyVaultClientEx(accessType, VaultNames[0]),
                    };
                    break;
                case 2:
                    string primaryVaultName = VaultNames[0];
                    string secondaryVaultName = VaultNames[1];
                    if (0 == string.Compare(primaryVaultName, secondaryVaultName, true))
                    {
                        throw new ArgumentException($"Primary vault name {primaryVaultName} is equal to secondary vault name {secondaryVaultName}");
                    }
                    _keyVaultClients = new KeyVaultClientEx[2]
                    {
                        CreateKeyVaultClientEx(accessType, primaryVaultName),
                        CreateKeyVaultClientEx(accessType, secondaryVaultName),
                    };
                    break;
                default:
                    throw new ArgumentException($"Vault names length must be 1 or 2 only", nameof(VaultNames));
            }
        }

        /// <summary>
        /// Load specified Vaults.json configuration file and creates the vault management instance
        /// </summary>
        /// <param name="vaultsConfigFile">
        /// Optional path to Vaults.json file, if NULL or empty default Vaults.json will be used, in such case Vaults.json location will be resolved in the following order:
        /// 1. Side-by-side with the current process location
        /// 2. Side-by-side with the current (executing) assembly
        /// </param>
        /// <param name="accessType">ReadOnly or ReadWrite</param>
        /// <param name="vaultNames">Single or Dual</param>
        public Vault(string vaultsConfigFile, VaultAccessTypeEnum accessType, params string[] vaultNames)
            : this(DeserializeVaultsConfigFromFile(ref vaultsConfigFile), accessType, vaultNames)
        {
            VaultsConfigFile = vaultsConfigFile;
        }

        /// <summary>
        /// Single (primary) vault management constructor
        /// </summary>
        /// <param name="accessType">ReadOnly or ReadWrite</param>
        /// <param name="vaultNames">Single or pair</param>
        public Vault(VaultAccessTypeEnum accessType, params string[] vaultNames)
            : this(string.Empty, accessType, vaultNames) { }

        /// <summary>
        /// Single (primary) or Dual (primary and secondary) vault management constructor
        /// </summary>
        /// <param name="accessType">ReadOnly or ReadWrite</param>
        /// <param name="vaultName">Vault name</param>
        public Vault(VaultAccessTypeEnum accessType, string vaultName)
            : this(accessType, new string[] { vaultName }) { }

        /// <summary>
        /// Dual (primary and secondary) vault management constructor
        /// </summary>
        /// <param name="accessType">ReadOnly or ReadWrite</param>
        /// <param name="primaryVaultName">Primary vault name</param>
        /// <param name="secondaryVaultName">Secodnary vault name</param>
        public Vault(VaultAccessTypeEnum accessType, string primaryVaultName, string secondaryVaultName)
            : this(accessType, new string[] { primaryVaultName, secondaryVaultName }) { }

        private static VaultsConfig DeserializeVaultsConfigFromFile(ref string vaultsConfigFile)
        {
            var settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto
            };
            if (string.IsNullOrWhiteSpace(vaultsConfigFile)) // Config file was not provied, use the default one
            {
                // Vaults.json location will be resolved in the following order:
                // 1. Side-by-side with the current process location
                // 2. Side-by-side with the current (executing) assembly
                vaultsConfigFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Consts.VaultsJsonConfig);
                if (!File.Exists(vaultsConfigFile))
                {
                    vaultsConfigFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Consts.VaultsJsonConfig);
                }
            }
            return JsonConvert.DeserializeObject<VaultsConfig>(File.ReadAllText(vaultsConfigFile), settings);
        }

        private KeyVaultClientEx CreateKeyVaultClientEx(VaultAccessTypeEnum accessType, string vaultName) =>
            new KeyVaultClientEx(vaultName, (authority, resource, scope) =>
        {
            lock (Lock)
            { 
                Utils.GuardVaultName(vaultName);
                if (false == VaultsConfig.ContainsKey(vaultName))
                {
                    throw new KeyNotFoundException($"{vaultName} is not found in {VaultsConfigFile}");
                }
                VaultAccessType vat = VaultsConfig[vaultName];
                VaultAccess[] vas = (accessType == VaultAccessTypeEnum.ReadOnly) ? vat.ReadOnly : vat.ReadWrite;

                // Order possible VaultAccess options by Order property
                IEnumerable<VaultAccess> vaSorted = from va in vas orderby va.Order select va;

                // In case VaultAccessUserInteractive is in the list, we will use our FileTokenCache with provided domainHint, otherwise use MemoryTokenCache
                string domainHint = (from va in vaSorted where va is VaultAccessUserInteractive select (VaultAccessUserInteractive)va).FirstOrDefault()?.DomainHint;
                string userAliasType = (from va in vaSorted where va is VaultAccessUserInteractive select (VaultAccessUserInteractive)va).FirstOrDefault()?.UserAliasType;

                // Token cache name is unique per login credentials as it uses alias type or env user name and domain hint.
                string tokenCacheName = $"{(userAliasType ?? Environment.UserName)}@{domainHint ?? "microsoft.com"}";

                // If either user alias or domain hint are empty, cache in memory instead.
                var authenticationContext = new AuthenticationContext(authority, string.IsNullOrEmpty(domainHint) && string.IsNullOrEmpty(userAliasType) ? new MemoryTokenCache() : (TokenCache)new FileTokenCache(tokenCacheName));

                Queue<Exception> exceptions = new Queue<Exception>();
                string vaultAccessTypes = "";
                foreach (VaultAccess va in vaSorted)
                {
                    try
                    {
                        // If user alias type is different from environment, force login prompt, otherwise silently login
                        var authResult = va.AcquireToken(authenticationContext, resource, userAliasType == Environment.UserName ? Environment.UserName:"");
                        AuthenticatedUserName = authResult.UserInfo?.DisplayableId ?? $"{Environment.UserDomainName}\\{Environment.UserName}";
                        return Task.FromResult(authResult.AccessToken);
                    }
                    catch (Exception e)
                    {
                        vaultAccessTypes += $" {va}";
                        exceptions.Enqueue(e);
                    }
                }
                throw new VaultAccessException($"Failed to get access to {vaultName} with all possible vault access type(s){vaultAccessTypes}", exceptions.ToArray());
            }
        });

        #endregion

        #region Secrets

        /// <summary>
        /// Gets specified secret by name from vault
        /// This function will prefer vault in the same region, in case we failed (including secret not found) it will fallback to other region
        /// In case we failed in both regions it will throw aggregated SecretException
        /// </summary>
        /// <param name="secretName">The name the secret in the given vault</param>
        /// <param name="secretVersion">The version of the secret (optional)</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns>SecretBundle</returns>
        public async Task<SecretBundle> GetSecretAsync(string secretName, string secretVersion = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            Queue<Exception> exceptions = new Queue<Exception>();
            string vaults = "";
            secretVersion = secretVersion ?? string.Empty;
            foreach (var kv in _keyVaultClients)
            {
                try
                {
                    return await kv.GetSecretAsync(kv.VaultUri, secretName, secretVersion, cancellationToken).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    vaults += $" {kv}";
                    exceptions.Enqueue(e);
                }
            }
            throw new SecretException($"Failed to get secret {secretName} from vault(s){vaults}", exceptions.ToArray());
        }

        /// <summary>
        /// Sets a secret in both vaults
        /// </summary>
        /// <param name="secretName">The name the secret in the given vault</param>
        /// <param name="value">The value of the secret</param>
        /// <param name="tags">Application-specific metadata in the form of key-value pairs</param>
        /// <param name="contentType">Type of the secret value such as a password</param>
        /// <param name="secretAttributes">Attributes for the secret. For more information on possible attributes, <see cref="SecretAttributes"/></param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns>A response message containing the updated secret from first vault</returns>
        public async Task<SecretBundle> SetSecretAsync(string secretName, string value, Dictionary<string, string> tags = null, string contentType = null, SecretAttributes secretAttributes = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            tags = Utils.AddMd5ChangedBy(tags, value, AuthenticatedUserName);
            var t0 = _keyVaultClients[0].SetSecretAsync(_keyVaultClients[0].VaultUri, secretName, value, tags, contentType, secretAttributes, cancellationToken);
            var t1 = Secondary ? _keyVaultClients[1].SetSecretAsync(_keyVaultClients[1].VaultUri, secretName, value, tags, contentType, secretAttributes, cancellationToken) : CompletedTask;
            await Task.WhenAll(t0, t1).ContinueWith((t) =>
            {
                if (t0.IsFaulted && t1.IsFaulted)
                {
                    throw new SecretException($"Failed to set secret {secretName} in both vaults {_keyVaultClients[0]} and {_keyVaultClients[1]}", t0.Exception, t1.Exception);
                }
                if (t0.IsFaulted)
                {
                    throw new SecretException($"Failed to set secret {secretName} in vault {_keyVaultClients[0]}", t0.Exception);
                }
                if (t1.IsFaulted)
                {
                    throw new SecretException($"Failed to set secret {secretName} in vault {_keyVaultClients[1]}", t1.Exception);
                }
            });
            return t0.Result;
        }

        /// <summary>
        /// Updates the attributes associated with the specified secret in both vaults
        /// </summary>
        /// <param name="secretName">The name of the secret in the given vault</param>
        /// <param name="secretVersion">The secret version (optional)</param>
        /// <param name="tags">Application-specific metadata in the form of key-value pairs</param>
        /// <param name="contentType">Type of the secret value such as a password</param>
        /// <param name="secretAttributes">Attributes for the secret. For more information on possible attributes, <see cref="SecretAttributes"/></param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns>A response message containing the updated secret from first vault</returns>
        public async Task<SecretBundle> UpdateSecretAsync(string secretName, string secretVersion = null, Dictionary<string, string> tags = null, string contentType = null, SecretAttributes secretAttributes = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            tags = Utils.AddMd5ChangedBy(tags, null, AuthenticatedUserName);
            secretVersion = secretVersion ?? string.Empty;
            var t0 = _keyVaultClients[0].UpdateSecretAsync(_keyVaultClients[0].VaultUri, secretName, secretVersion, contentType, secretAttributes, tags, cancellationToken);
            var t1 = Secondary ? _keyVaultClients[1].UpdateSecretAsync(_keyVaultClients[1].VaultUri, secretName, secretVersion, contentType, secretAttributes, tags, cancellationToken) : CompletedTask;
            await Task.WhenAll(t0, t1).ContinueWith((t) =>
            {
                if (t0.IsFaulted && t1.IsFaulted)
                {
                    throw new SecretException($"Failed to update secret {secretName} in both vaults {_keyVaultClients[0]} and {_keyVaultClients[1]}", t0.Exception, t1.Exception);
                }
                if (t0.IsFaulted)
                {
                    throw new SecretException($"Failed to update secret {secretName} in vault {_keyVaultClients[0]}", t0.Exception);
                }
                if (t1.IsFaulted)
                {
                    throw new SecretException($"Failed to update secret {secretName} in vault {_keyVaultClients[1]}", t1.Exception);
                }
            });
            return t0.Result;
        }

        /// <summary>
        /// List all secrets from specified vault
        /// This function will only look in single specified Azure Key Vault. It will not fallback to other region.
        /// </summary>
        /// <param name="regionIndex">0 - current region, 1 - other region</param>
        /// <param name="listSecretsProgressUpdate">Optional progress update delegate</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns>IEnumerable of SecretItem</returns>
        public async Task<IEnumerable<SecretItem>> ListSecretsAsync(int regionIndex = 0, ListOperationProgressUpdate listSecretsProgressUpdate = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            Guard.ArgumentIsValidRegion(regionIndex, nameof(regionIndex));
            Guard.ArgumentInRange(regionIndex, 0, _keyVaultClients.Length - 1, nameof(regionIndex));
            var listResponse = await _keyVaultClients[regionIndex].GetSecretsAsync(_keyVaultClients[regionIndex].VaultUri, Consts.ListSecretsMaxResults, cancellationToken: cancellationToken).ConfigureAwait(false);
            Dictionary<string, SecretItem> result = new Dictionary<string, SecretItem>(StringComparer.InvariantCulture);
            if (listResponse == null) // No secrets in the vault
            {
                return result.Values;
            }
            foreach (SecretItem si in listResponse)
            {
                result[si.Identifier.Name] = si;
            }
            listSecretsProgressUpdate?.Invoke(result.Count);

            while (!string.IsNullOrEmpty(listResponse.NextPageLink))
            {
                listResponse = await _keyVaultClients[regionIndex].GetSecretsNextAsync(listResponse.NextPageLink, cancellationToken).ConfigureAwait(false);
                foreach (SecretItem si in listResponse)
                {
                    result[si.Identifier.Name] = si;
                }
                listSecretsProgressUpdate?.Invoke(result.Count);
            }

            return result.Values;
        }


        /// <summary>
        /// List all the versions of a specified secret
        /// This function will only look in single specified Azure Key Vault. It will not fallback to other region.
        /// </summary>
        /// <param name="secretName">The name of the secret in the given vault</param>
        /// <param name="regionIndex">0 - current region, 1 - other region</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns></returns>
        public async Task<IEnumerable<SecretItem>> GetSecretVersionsAsync(string secretName, int regionIndex = 0, CancellationToken cancellationToken = default(CancellationToken))
        {
            Guard.ArgumentNotNullOrWhitespace(secretName, nameof(secretName));
            Guard.ArgumentIsValidRegion(regionIndex, nameof(regionIndex));
            Guard.ArgumentInRange(regionIndex, 0, _keyVaultClients.Length - 1, nameof(regionIndex));

            var listResponse = await _keyVaultClients[regionIndex].GetSecretVersionsAsync(_keyVaultClients[regionIndex].VaultUri, secretName, Consts.GetSecretVersionsMaxResults, cancellationToken: cancellationToken).ConfigureAwait(false);
            Dictionary<string, SecretItem> result = new Dictionary<string, SecretItem>(StringComparer.InvariantCulture);
            if (listResponse == null) // No secrets in the vault
            {
                return result.Values;
            }
            foreach (SecretItem si in listResponse)
            {
                result[si.Identifier.Identifier] = si;
            }

            while (!string.IsNullOrEmpty(listResponse.NextPageLink))
            {
                listResponse = await _keyVaultClients[regionIndex].GetSecretVersionsNextAsync(listResponse.NextPageLink, cancellationToken).ConfigureAwait(false);
                foreach (SecretItem si in listResponse)
                {
                    result[si.Identifier.Identifier] = si;
                }
            }

            return result.Values;
        }

        /// <summary>
        /// Deletes a secret from both vaults
        /// </summary>
        /// <param name="secretName">The name of the secret in the given vault</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns>The deleted secret</returns>
        public async Task<SecretBundle> DeleteSecretAsync(string secretName, CancellationToken cancellationToken = default(CancellationToken))
        {
            var t0 = _keyVaultClients[0].DeleteSecretAsync(_keyVaultClients[0].VaultUri, secretName, cancellationToken);
            var t1 = Secondary ? _keyVaultClients[1].DeleteSecretAsync(_keyVaultClients[1].VaultUri, secretName, cancellationToken) : CompletedTask;
            await Task.WhenAll(t0, t1).ContinueWith((t) =>
            {
                if (t0.IsFaulted && t1.IsFaulted)
                {
                    throw new SecretException($"Failed to delete secret {secretName} from both vaults {_keyVaultClients[0]} and {_keyVaultClients[1]}", t0.Exception, t1.Exception);
                }
                if (t0.IsFaulted)
                {
                    throw new SecretException($"Failed to delete secret {secretName} from vault {_keyVaultClients[0]}", t0.Exception);
                }
                if (t1.IsFaulted)
                {
                    throw new SecretException($"Failed to delete secret {secretName} from vault {_keyVaultClients[1]}", t1.Exception);
                }
            });

            return t0.Result;
        }

        #endregion

        #region Certificates

        /// <summary>
        /// Gets a certificate with private and public keys. Keys are exportable.
        /// We are NOT using here <see cref="KeyVaultClient.GetCertificateWithPrivateKeyAsync(string, string, string, CancellationToken)"/> as this returns a certificate with non-exportable keys
        /// </summary>
        /// <param name="certificateName">The name of the certificate in the given vault</param>
        /// <param name="certificateVersion">The version of the certificate (optional)</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns>A response message containing the certificate with private key.</returns>
        public async Task<X509Certificate2> GetCertificateWithExportableKeysAsync(string certificateName, string certificateVersion = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            SecretBundle s = await GetSecretAsync(certificateName, certificateVersion, cancellationToken);
            var cert = new X509Certificate2();
            cert.Import(Convert.FromBase64String(s.Value), string.Empty, X509KeyStorageFlags.Exportable);
            return cert;
        }

        /// <summary>
        /// Gets a certificate.
        /// </summary>
        /// <param name="certificateName">The name of the certificate in the given vault</param>
        /// <param name="certificateVersion">The version of the certificate (optional)</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns>A response message containing the certificate</returns>
        public async Task<CertificateBundle> GetCertificateAsync(string certificateName, string certificateVersion = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            Queue<Exception> exceptions = new Queue<Exception>();
            string vaults = "";
            certificateVersion = certificateVersion ?? string.Empty;
            foreach (var kv in _keyVaultClients)
            {
                try
                {
                    return await kv.GetCertificateAsync(kv.VaultUri, certificateName, certificateVersion, cancellationToken).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    vaults += $" {kv}";
                    exceptions.Enqueue(e);
                }
            }
            throw new SecretException($"Failed to get certificate {certificateName} from vault(s){vaults}", exceptions.ToArray());
        }


        /// <summary>
        /// List all certificates from specified vault
        /// This function will only look in single specified Azure Key Vault. It will not fallback to other region.
        /// </summary>
        /// <param name="regionIndex">0 - current region, 1 - other region</param>
        /// <param name="listCertificatesProgressUpdate">Optional progress update delegate</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns>IEnumerable of CertificateItem</returns>
        public async Task<IEnumerable<CertificateItem>> ListCertificatesAsync(int regionIndex = 0, ListOperationProgressUpdate listCertificatesProgressUpdate = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            Guard.ArgumentIsValidRegion(regionIndex, nameof(regionIndex));
            Guard.ArgumentInRange(regionIndex, 0, _keyVaultClients.Length - 1, nameof(regionIndex));
            var listResponse = await _keyVaultClients[regionIndex].GetCertificatesAsync(_keyVaultClients[regionIndex].VaultUri, Consts.ListCertificatesMaxResults, cancellationToken: cancellationToken).ConfigureAwait(false);
            Dictionary<string, CertificateItem> result = new Dictionary<string, CertificateItem>(StringComparer.InvariantCulture);
            if (listResponse == null) // No certificates in the vault
            {
                return result.Values;
            }
            foreach (CertificateItem ci in listResponse)
            {
                result[ci.Identifier.Name] = ci;
            }
            listCertificatesProgressUpdate?.Invoke(result.Count);

            while (!string.IsNullOrEmpty(listResponse.NextPageLink))
            {
                listResponse = await _keyVaultClients[regionIndex].GetCertificatesNextAsync(listResponse.NextPageLink, cancellationToken).ConfigureAwait(false);
                foreach (CertificateItem ci in listResponse)
                {
                    result[ci.Identifier.Name] = ci;
                }
                listCertificatesProgressUpdate?.Invoke(result.Count);
            }

            return result.Values;
        }

        /// <summary>
        /// Imports a new certificate version. If this is the first version, the certificate resource is created.
        /// </summary>
        /// <param name="certificateName">The name of the certificate</param>
        /// <param name="certificateCollection">The certificate collection with the private key</param>
        /// <param name="certificatePolicy">The management policy for the certificate</param>
        /// <param name="certificateAttributes">The attributes of the certificate (optional)</param>
        /// <param name="tags">Application-specific metadata in the form of key-value pairs</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns>A response message containing the imported certificate.</returns>
        public async Task<CertificateBundle> ImportCertificateAsync(string certificateName, X509Certificate2Collection certificateCollection, CertificatePolicy certificatePolicy, CertificateAttributes certificateAttributes = null, IDictionary<string, string> tags = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            string thumbprint = certificateCollection.Cast<X509Certificate2>().FirstOrDefault()?.Thumbprint.ToLowerInvariant();
            tags = Utils.AddMd5ChangedBy(tags, thumbprint, AuthenticatedUserName);
            var t0 = _keyVaultClients[0].ImportCertificateAsync(_keyVaultClients[0].VaultUri, certificateName, certificateCollection, certificatePolicy, certificateAttributes, tags, cancellationToken);
            var t1 = Secondary ? _keyVaultClients[1].ImportCertificateAsync(_keyVaultClients[1].VaultUri, certificateName, certificateCollection, certificatePolicy, certificateAttributes, tags, cancellationToken) : CompletedTask;
            await Task.WhenAll(t0, t1).ContinueWith((t) =>
            {
                if (t0.IsFaulted && t1.IsFaulted)
                {
                    throw new SecretException($"Failed to import certificate {certificateName} to both vaults {_keyVaultClients[0]} and {_keyVaultClients[1]}", t0.Exception, t1.Exception);
                }
                if (t0.IsFaulted)
                {
                    throw new SecretException($"Failed to import certificate {certificateName} to vault {_keyVaultClients[0]}", t0.Exception);
                }
                if (t1.IsFaulted)
                {
                    throw new SecretException($"Failed to import certificate {certificateName} to vault {_keyVaultClients[1]}", t1.Exception);
                }
            });

            return t0.Result;
        }

        /// <summary>
        /// Deletes a certificate from the specified vault.
        /// </summary>
        /// <param name="certificateName">The name of the certificate in the given vault.</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns>The deleted certificate</returns>
        public async Task<CertificateBundle> DeleteCertificateAsync(string certificateName, CancellationToken cancellationToken = default(CancellationToken))
        {
            var t0 = _keyVaultClients[0].DeleteCertificateAsync(_keyVaultClients[0].VaultUri, certificateName, cancellationToken);
            var t1 = Secondary ? _keyVaultClients[1].DeleteCertificateAsync(_keyVaultClients[1].VaultUri, certificateName, cancellationToken) : CompletedTask;
            await Task.WhenAll(t0, t1).ContinueWith((t) =>
            {
                if (t0.IsFaulted && t1.IsFaulted)
                {
                    throw new SecretException($"Failed to delete certificate {certificateName} from both vaults {_keyVaultClients[0]} and {_keyVaultClients[1]}", t0.Exception, t1.Exception);
                }
                if (t0.IsFaulted)
                {
                    throw new SecretException($"Failed to delete certificate {certificateName} from vault {_keyVaultClients[0]}", t0.Exception);
                }
                if (t1.IsFaulted)
                {
                    throw new SecretException($"Failed to delete certificate {certificateName} from vault {_keyVaultClients[1]}", t1.Exception);
                }
            });

            return t0.Result;
        }

        /// <summary>
        /// Updates a certificate
        /// </summary>
        /// <param name="certificateName">The name of the certificate in the given vault.</param>
        /// <param name="certificateVersion">The certificate version (optional)</param>
        /// <param name="certificatePolicy">The certificate policy (optional)</param>
        /// <param name="certificateAttributes">The attributes of the certificate (optional)</param>
        /// <param name="tags">Application-specific metadata in the form of key-value pairs</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns>A response message containing the updated certificate.</returns>
        public async Task<CertificateBundle> UpdateCertificateAsync(string certificateName, string certificateVersion = null, CertificatePolicy certificatePolicy = null, CertificateAttributes certificateAttributes = null, IDictionary<string, string> tags = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            tags = Utils.AddMd5ChangedBy(tags, null, AuthenticatedUserName);
            certificateVersion = certificateVersion ?? string.Empty;
            var t0 = _keyVaultClients[0].UpdateCertificateAsync(_keyVaultClients[0].VaultUri, certificateName, certificateVersion, certificatePolicy, certificateAttributes, tags, cancellationToken);
            var t1 = Secondary ? _keyVaultClients[1].UpdateCertificateAsync(_keyVaultClients[1].VaultUri, certificateName, certificateVersion, certificatePolicy, certificateAttributes, tags, cancellationToken) : CompletedTask;
            await Task.WhenAll(t0, t1).ContinueWith((t) =>
            {
                if (t0.IsFaulted && t1.IsFaulted)
                {
                    throw new SecretException($"Failed to update certificate {certificateName} in both vaults {_keyVaultClients[0]} and {_keyVaultClients[1]}", t0.Exception, t1.Exception);
                }
                if (t0.IsFaulted)
                {
                    throw new SecretException($"Failed to update certificate {certificateName} in vault {_keyVaultClients[0]}", t0.Exception);
                }
                if (t1.IsFaulted)
                {
                    throw new SecretException($"Failed to update certificate {certificateName} in vault {_keyVaultClients[1]}", t1.Exception);
                }
            });

            return t0.Result;
        }

        /// <summary>
        /// Updates the policy for a certificate. Set appropriate members in the certificatePolicy that must be updated. Leave others as null.
        /// </summary>
        /// <param name="certificateName">The name of the certificate in the given vault.</param>
        /// <param name="certificatePolicy">The policy for the certificate.</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns>A response message containing the updated certificate policy.</returns>
        public async Task<CertificatePolicy> UpdateCertificatePolicyAsync(string certificateName, CertificatePolicy certificatePolicy, CancellationToken cancellationToken = default(CancellationToken))
        {
            var t0 = _keyVaultClients[0].UpdateCertificatePolicyAsync(_keyVaultClients[0].VaultUri, certificateName, certificatePolicy, cancellationToken);
            var t1 = Secondary ? _keyVaultClients[1].UpdateCertificatePolicyAsync(_keyVaultClients[1].VaultUri, certificateName, certificatePolicy, cancellationToken) : CompletedTask;
            await Task.WhenAll(t0, t1).ContinueWith((t) =>
            {
                if (t0.IsFaulted && t1.IsFaulted)
                {
                    throw new SecretException($"Failed to update certificate policy for {certificateName} in both vaults {_keyVaultClients[0]} and {_keyVaultClients[1]}", t0.Exception, t1.Exception);
                }
                if (t0.IsFaulted)
                {
                    throw new SecretException($"Failed to update certificate policy for {certificateName} in vault {_keyVaultClients[0]}", t0.Exception);
                }
                if (t1.IsFaulted)
                {
                    throw new SecretException($"Failed to update certificate policy for {certificateName} in vault {_keyVaultClients[1]}", t1.Exception);
                }
            });

            return t0.Result;
        }

        /// <summary>
        /// List the versions of a certificate.
        /// </summary>
        /// <param name="certificateName">The name of the certificate</param>
        /// <param name="regionIndex">0 - current region, 1 - other region</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns>IEnumerable of CertificateItem</returns>
        public async Task<IEnumerable<CertificateItem>> GetCertificateVersionsAsync(string certificateName, int regionIndex = 0, CancellationToken cancellationToken = default(CancellationToken))
        {
            Guard.ArgumentNotNullOrWhitespace(certificateName, nameof(certificateName));
            Guard.ArgumentIsValidRegion(regionIndex, nameof(regionIndex));
            Guard.ArgumentInRange(regionIndex, 0, _keyVaultClients.Length - 1, nameof(regionIndex));

            var listResponse = await _keyVaultClients[regionIndex].GetCertificateVersionsAsync(_keyVaultClients[regionIndex].VaultUri, certificateName, Consts.GetCertificateVersionsMaxResults, cancellationToken: cancellationToken).ConfigureAwait(false);
            Dictionary<string, CertificateItem> result = new Dictionary<string, CertificateItem>(StringComparer.InvariantCulture);
            if (listResponse == null) // No certificates in the vault
            {
                return result.Values;
            }
            foreach (CertificateItem ci in listResponse)
            {
                result[ci.Identifier.Identifier] = ci;
            }

            while (!string.IsNullOrEmpty(listResponse.NextPageLink))
            {
                listResponse = await _keyVaultClients[regionIndex].GetCertificateVersionsNextAsync(listResponse.NextPageLink, cancellationToken).ConfigureAwait(false);
                foreach (CertificateItem ci in listResponse)
                {
                    result[ci.Identifier.Identifier] = ci;
                }
            }

            return result.Values;
        }

        #endregion
    }
}