// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 

namespace Microsoft.Vault.Library
{
    using Core;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Security.Cryptography.X509Certificates;

    [JsonObject]
    public abstract class VaultAccess
    {
        [JsonProperty]
        public readonly string ClientId; // Also known as ApplicationId, see Get-AzureRmADApplication

        [JsonIgnore]
        public readonly int Order;

        public VaultAccess(string clientId, int order)
        {
            Guid r;
            if (!Guid.TryParseExact(clientId, "D", out r))
            {
                throw new ArgumentException($"{clientId} must be a valid GUID in the following format: 00000000-0000-0000-0000-000000000000", nameof(clientId));
            }
            ClientId = clientId;
            Order = order;
        }

        protected abstract AuthenticationResult AcquireTokenInternal(AuthenticationContext authenticationContext, string resource, string userAlias = "");

        public AuthenticationResult AcquireToken(AuthenticationContext authenticationContext, string resource, string userAlias="")
        {
            // first, try to get a token silently
            if (authenticationContext.TokenCache != null)
            {
                try
                {
                    return authenticationContext.AcquireTokenSilentAsync(resource, ClientId).GetAwaiter().GetResult();
                }
                catch (AdalException ex)
                {
                    // There is no token in the cache; fallback -> prompt the user to sign-in.
                    if (ex.ErrorCode != "failed_to_acquire_token_silently")
                    {
                        // An unexpected error occurred.
                        throw;
                    }
                }
            }
            return AcquireTokenInternal(authenticationContext, resource, userAlias);
        }
    }

    [JsonObject]
    public class VaultAccessUserInteractive : VaultAccess
    {
        internal const string PowerShellApplicationId = "1950a258-227b-4e31-a9cf-717495945fc2";

        [JsonProperty]
        public readonly string DomainHint;

        [JsonProperty]
        public readonly string UserAliasType;

        public VaultAccessUserInteractive(string domainHint) : base(PowerShellApplicationId, 2)
        {
            DomainHint = string.IsNullOrEmpty(domainHint) ? "microsoft.com" : domainHint;
        }

        [JsonConstructor]
        public VaultAccessUserInteractive(string domainHint, string UserAlias) : base(PowerShellApplicationId, 2)
        {
            DomainHint = string.IsNullOrEmpty(domainHint) ? "microsoft.com" : domainHint;
            UserAliasType = string.IsNullOrEmpty(UserAlias) ? Environment.UserName : UserAlias;
        }

        protected override AuthenticationResult AcquireTokenInternal(AuthenticationContext authenticationContext, string resource, string userAlias)
        {
            if (false == Environment.UserInteractive)
            {
                throw new VaultAccessException($@"Current process PID: {Process.GetCurrentProcess().Id} is running in non user interactive mode. Username: {Environment.UserDomainName}\{Environment.UserName} Machine name: {Environment.MachineName}");
            }
            // Attempt to login with provided user alias.
            else if(!string.IsNullOrEmpty(userAlias))
            {
                return authenticationContext.AcquireTokenAsync(resource, ClientId, new Uri("urn:ietf:wg:oauth:2.0:oob"), new PlatformParameters(PromptBehavior.Auto), UserIdentifier.AnyUser, $"login_hint={userAlias}@{DomainHint}&domain_hint={DomainHint}").GetAwaiter().GetResult();
            }
            // No alias provided so force login prompt.
            else
            {
                return authenticationContext.AcquireTokenAsync(resource, ClientId, new Uri("urn:ietf:wg:oauth:2.0:oob"), new PlatformParameters(PromptBehavior.Always), UserIdentifier.AnyUser).GetAwaiter().GetResult();
            }
        }

        public override string ToString() => $"{nameof(VaultAccessUserInteractive)}";
    }

    [JsonObject]
    public class VaultAccessClientCredential : VaultAccess
    {
        [JsonProperty]
        public readonly string ClientSecret;

        [JsonConstructor]
        public VaultAccessClientCredential(string clientId, string clientSecret) : base(clientId, 1)
        {
            Guard.ArgumentNotNull(clientSecret, nameof(clientSecret));
            ClientSecret = clientSecret;
        }

        protected override AuthenticationResult AcquireTokenInternal(AuthenticationContext authenticationContext, string resource, string userAlias = "")
        {
            return authenticationContext.AcquireTokenAsync(resource, new ClientCredential(ClientId, ClientSecret)).GetAwaiter().GetResult();
        }

        public override string ToString() => $"{nameof(VaultAccessClientCredential)}";
    }

    [JsonObject]
    public class VaultAccessClientCertificate : VaultAccess
    {
        [JsonProperty]
        public readonly string CertificateThumbprint;

        private X509Certificate2 _certificate;

        [JsonConstructor]
        public VaultAccessClientCertificate(string clientId, string certificateThumbprint) : base(clientId, 0)
        {
            Guard.ArgumentIsSha1(certificateThumbprint, nameof(certificateThumbprint));
            CertificateThumbprint = certificateThumbprint;
            _certificate = null;
        }

        private IEnumerable<X509Store> EnumerateX509Stores()
        {
            yield return new X509Store(StoreName.My, StoreLocation.CurrentUser);
            yield return new X509Store(StoreName.My, StoreLocation.LocalMachine);
        }

        private void FindCertificate()
        {
            if (null != _certificate)
            {
                return;
            }

            foreach (var store in EnumerateX509Stores())
            {
                try
                {
                    store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

                    var storeCerts = store.Certificates.Find(X509FindType.FindByThumbprint, CertificateThumbprint, false);
                    if ((storeCerts != null) && (storeCerts.Count > 0))
                    {
                        _certificate = storeCerts[0];
                        break;
                    }
                }
                finally
                {
                    store.Close();
                }
            }
        }

        [JsonIgnore]
        public X509Certificate2 Certificate
        {
            get
            {
                FindCertificate();
                if (_certificate == null)
                {
                    throw new CertificateNotFoundException($@"Certificate {CertificateThumbprint} is not installed in CurrentUser\My or in LocalMachine\My stores. Username: {Environment.UserDomainName}\{Environment.UserName} Machine name: {Environment.MachineName}");
                }

                return _certificate;
            }
        }

        protected override AuthenticationResult AcquireTokenInternal(AuthenticationContext authenticationContext, string resource, string userAlias = "")
        {
            return authenticationContext.AcquireTokenAsync(resource, new ClientAssertionCertificate(ClientId, Certificate)).GetAwaiter().GetResult();
        }

        public override string ToString() => $"{nameof(VaultAccessClientCertificate)}";
    }
}
