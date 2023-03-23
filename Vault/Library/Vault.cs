// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 
using Azure.Security.KeyVault.Certificates;
using Azure.Security.KeyVault.Secrets;
using Azure.Identity;
namespace Microsoft.Vault.Library
{
    using Core;
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
        private readonly SecretClient[] _secretClients;
        private readonly CertificateClient[] _certificateClients;
        private bool Secondary => (_secretClients.Length == 2);

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
                    _secretClients = new SecretClient[1]
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
                    _secretClients = new SecretClient[2]
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

        private SecretClient CreateKeyVaultClientEx(VaultAccessTypeEnum accessType, string vaultName)
        {
            return new SecretClient(new Uri(vaultName), new DefaultAzureCredential());
        }

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
        /// <returns>KeyVaultSecret</returns>
        public async Task<KeyVaultSecret> GetSecretAsync(string secretName, string secretVersion = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            Queue<Exception> exceptions = new Queue<Exception>();
            string vaults = "";
            secretVersion = secretVersion ?? string.Empty;
            foreach (var kv in _secretClients)
            {
                try
                {
                    return await kv.GetSecretAsync(secretName, secretVersion, cancellationToken).ConfigureAwait(false);
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
        public async Task<KeyVaultSecret> SetSecretAsync(string secretName, string value, SecretProperties secretProperties, CancellationToken cancellationToken = default(CancellationToken))
        {
            //tags = Utils.AddMd5ChangedBy(tags, value, AuthenticatedUserName);
            KeyVaultSecret keyVaultSecret = new KeyVaultSecret(secretName, value);
            keyVaultSecret.Properties.Enabled = secretProperties.Enabled;
            keyVaultSecret.Properties.ExpiresOn = secretProperties.ExpiresOn;
            keyVaultSecret.Properties.NotBefore = secretProperties.NotBefore;
            keyVaultSecret.Properties.ContentType = secretProperties.ContentType;
            var t0 = _secretClients[0].SetSecretAsync(keyVaultSecret, cancellationToken);
            var t1 = Secondary ? _secretClients[1].SetSecretAsync(keyVaultSecret, cancellationToken) : CompletedTask;
            await Task.WhenAll(t0, t1).ContinueWith((t) =>
            {
                if (t0.IsFaulted && t1.IsFaulted)
                {
                    throw new SecretException($"Failed to set secret {secretName} in both vaults {_secretClients[0]} and {_secretClients[1]}", t0.Exception, t1.Exception);
                }
                if (t0.IsFaulted)
                {
                    throw new SecretException($"Failed to set secret {secretName} in vault {_secretClients[0]}", t0.Exception);
                }
                if (t1.IsFaulted)
                {
                    throw new SecretException($"Failed to set secret {secretName} in vault {_secretClients[1]}", t1.Exception);
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
        public async Task<SecretProperties> UpdateSecretAsync(SecretProperties secretProperties, CancellationToken cancellationToken = default(CancellationToken))
        {
            var t0 = _secretClients[0].UpdateSecretPropertiesAsync(secretProperties, cancellationToken);
            var t1 = Secondary ? _secretClients[1].UpdateSecretPropertiesAsync(secretProperties, cancellationToken) : CompletedTask;
            await Task.WhenAll(t0, t1).ContinueWith((t) =>
            {
                if (t0.IsFaulted && t1.IsFaulted)
                {
                    throw new SecretException($"Failed to update secret {secretProperties.Name} in both vaults {_secretClients[0]} and {_secretClients[1]}", t0.Exception, t1.Exception);
                }
                if (t0.IsFaulted)
                {
                    throw new SecretException($"Failed to update secret {secretProperties.Name} in vault {_secretClients[0]}", t0.Exception);
                }
                if (t1.IsFaulted)
                {
                    throw new SecretException($"Failed to update secret {secretProperties.Name} in vault {_secretClients[1]}", t1.Exception);
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
        public async Task<IEnumerable<SecretProperties>> ListSecretsAsync(int regionIndex = 0, ListOperationProgressUpdate listSecretsProgressUpdate = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            Guard.ArgumentIsValidRegion(regionIndex, nameof(regionIndex));
            Guard.ArgumentInRange(regionIndex, 0, _secretClients.Length - 1, nameof(regionIndex));
            var result = _secretClients[regionIndex].GetPropertiesOfSecrets(cancellationToken: cancellationToken);

            return await Task.FromResult(result);
        }


        /// <summary>
        /// List all the versions of a specified secret
        /// This function will only look in single specified Azure Key Vault. It will not fallback to other region.
        /// </summary>
        /// <param name="secretName">The name of the secret in the given vault</param>
        /// <param name="regionIndex">0 - current region, 1 - other region</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns></returns>
        public async Task<IEnumerable<SecretProperties>> GetSecretVersionsAsync(string secretName, int regionIndex = 0, CancellationToken cancellationToken = default(CancellationToken))
        {
            Guard.ArgumentNotNullOrWhitespace(secretName, nameof(secretName));
            Guard.ArgumentIsValidRegion(regionIndex, nameof(regionIndex));
            Guard.ArgumentInRange(regionIndex, 0, _secretClients.Length - 1, nameof(regionIndex));

            var result = _secretClients[regionIndex].GetPropertiesOfSecretVersions(secretName, cancellationToken: cancellationToken);

            return await Task.FromResult(result);
        }

        /// <summary>
        /// Deletes a secret from both vaults
        /// </summary>
        /// <param name="secretName">The name of the secret in the given vault</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns>The deleted secret</returns>
        public async Task<DeleteSecretOperation> DeleteSecretAsync(string secretName, CancellationToken cancellationToken = default(CancellationToken))
        {
            var t0 = _secretClients[0].StartDeleteSecretAsync(secretName, cancellationToken);
            var t1 = Secondary ? _secretClients[1].StartDeleteSecretAsync(secretName, cancellationToken) : CompletedTask;
            await Task.WhenAll(t0, t1).ContinueWith((t) =>
            {
                if (t0.IsFaulted && t1.IsFaulted)
                {
                    throw new SecretException($"Failed to delete secret {secretName} from both vaults {_secretClients[0]} and {_secretClients[1]}", t0.Exception, t1.Exception);
                }
                if (t0.IsFaulted)
                {
                    throw new SecretException($"Failed to delete secret {secretName} from vault {_secretClients[0]}", t0.Exception);
                }
                if (t1.IsFaulted)
                {
                    throw new SecretException($"Failed to delete secret {secretName} from vault {_secretClients[1]}", t1.Exception);
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
            KeyVaultSecret s = await GetSecretAsync(certificateName, certificateVersion, cancellationToken);
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
        public async Task<KeyVaultCertificateWithPolicy> GetCertificateAsync(string certificateName, CancellationToken cancellationToken = default(CancellationToken))
        {
            Queue<Exception> exceptions = new Queue<Exception>();
            string vaults = "";
            foreach (var kv in _certificateClients)
            {
                try
                {
                    return await kv.GetCertificateAsync(certificateName, cancellationToken).ConfigureAwait(false);
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
        /// <returns>IEnumerable of CertificateProperties</returns>
        public async Task<IEnumerable<CertificateProperties>> ListCertificatesAsync(int regionIndex = 0, ListOperationProgressUpdate listCertificatesProgressUpdate = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            Guard.ArgumentIsValidRegion(regionIndex, nameof(regionIndex));
            Guard.ArgumentInRange(regionIndex, 0, _certificateClients.Length - 1, nameof(regionIndex));
            var result = _certificateClients[regionIndex].GetPropertiesOfCertificates(cancellationToken: cancellationToken);

            return await Task.FromResult(result);
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
        public async Task<KeyVaultCertificateWithPolicy> ImportCertificateAsync(ImportCertificateOptions importCertificateOptions, CancellationToken cancellationToken = default(CancellationToken))
        {
            var t0 = _certificateClients[0].ImportCertificateAsync(importCertificateOptions, cancellationToken);
            var t1 = Secondary ? _certificateClients[1].ImportCertificateAsync(importCertificateOptions, cancellationToken) : CompletedTask;
            await Task.WhenAll(t0, t1).ContinueWith((t) =>
            {
                if (t0.IsFaulted && t1.IsFaulted)
                {
                    throw new SecretException($"Failed to import certificate {importCertificateOptions.Name} to both vaults {_certificateClients[0]} and {_certificateClients[1]}", t0.Exception, t1.Exception);
                }
                if (t0.IsFaulted)
                {
                    throw new SecretException($"Failed to import certificate {importCertificateOptions.Name} to vault {_certificateClients[0]}", t0.Exception);
                }
                if (t1.IsFaulted)
                {
                    throw new SecretException($"Failed to import certificate {importCertificateOptions.Name} to vault {_certificateClients[1]}", t1.Exception);
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
        public async Task<DeleteCertificateOperation> DeleteCertificateAsync(string certificateName, CancellationToken cancellationToken = default(CancellationToken))
        {
            var t0 = _certificateClients[0].StartDeleteCertificateAsync(certificateName, cancellationToken);
            var t1 = Secondary ? _certificateClients[1].StartDeleteCertificateAsync(certificateName, cancellationToken) : CompletedTask;
            await Task.WhenAll(t0, t1).ContinueWith((t) =>
            {
                if (t0.IsFaulted && t1.IsFaulted)
                {
                    throw new SecretException($"Failed to delete certificate {certificateName} from both vaults {_certificateClients[0]} and {_certificateClients[1]}", t0.Exception, t1.Exception);
                }
                if (t0.IsFaulted)
                {
                    throw new SecretException($"Failed to delete certificate {certificateName} from vault {_certificateClients[0]}", t0.Exception);
                }
                if (t1.IsFaulted)
                {
                    throw new SecretException($"Failed to delete certificate {certificateName} from vault {_certificateClients[1]}", t1.Exception);
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
        public async Task<KeyVaultCertificate> UpdateCertificateAsync(CertificateProperties certificateProperties, CancellationToken cancellationToken = default(CancellationToken))
        {
            var t0 = _certificateClients[0].UpdateCertificatePropertiesAsync(certificateProperties, cancellationToken);
            var t1 = Secondary ? _certificateClients[1].UpdateCertificatePropertiesAsync(certificateProperties, cancellationToken) : CompletedTask;
            await Task.WhenAll(t0, t1).ContinueWith((t) =>
            {
                if (t0.IsFaulted && t1.IsFaulted)
                {
                    throw new SecretException($"Failed to update certificate {certificateProperties.Name} in both vaults {_certificateClients[0]} and {_certificateClients[1]}", t0.Exception, t1.Exception);
                }
                if (t0.IsFaulted)
                {
                    throw new SecretException($"Failed to update certificate {certificateProperties.Name} in vault {_certificateClients[0]}", t0.Exception);
                }
                if (t1.IsFaulted)
                {
                    throw new SecretException($"Failed to update certificate {certificateProperties.Name} in vault {_certificateClients[1]}", t1.Exception);
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
            var t0 = _certificateClients[0].UpdateCertificatePolicyAsync(certificateName, certificatePolicy, cancellationToken);
            var t1 = Secondary ? _certificateClients[1].UpdateCertificatePolicyAsync(certificateName, certificatePolicy, cancellationToken) : CompletedTask;
            await Task.WhenAll(t0, t1).ContinueWith((t) =>
            {
                if (t0.IsFaulted && t1.IsFaulted)
                {
                    throw new SecretException($"Failed to update certificate policy for {certificateName} in both vaults {_certificateClients[0]} and {_certificateClients[1]}", t0.Exception, t1.Exception);
                }
                if (t0.IsFaulted)
                {
                    throw new SecretException($"Failed to update certificate policy for {certificateName} in vault {_certificateClients[0]}", t0.Exception);
                }
                if (t1.IsFaulted)
                {
                    throw new SecretException($"Failed to update certificate policy for {certificateName} in vault {_certificateClients[1]}", t1.Exception);
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
        /// <returns>IEnumerable of CertificateProperties</returns>
        public async Task<IEnumerable<CertificateProperties>> GetCertificateVersionsAsync(string certificateName, int regionIndex = 0, CancellationToken cancellationToken = default(CancellationToken))
        {
            Guard.ArgumentNotNullOrWhitespace(certificateName, nameof(certificateName));
            Guard.ArgumentIsValidRegion(regionIndex, nameof(regionIndex));
            Guard.ArgumentInRange(regionIndex, 0, _secretClients.Length - 1, nameof(regionIndex));

            var result = _certificateClients[regionIndex].GetPropertiesOfCertificateVersions(certificateName, cancellationToken: cancellationToken);

            return await Task.FromResult(result);
        }

        #endregion
    }
}