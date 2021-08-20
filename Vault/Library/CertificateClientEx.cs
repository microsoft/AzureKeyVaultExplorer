// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 

using Azure.Security.KeyVault.Certificates;
using Azure.Security.KeyVault.Secrets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Vault.Library
{
    /// <summary>
    /// Simple wrapper around KeyVaultClient
    /// </summary> 
    internal class CertificateClientEx : CertificateClient
    {
        public readonly string VaultName;
        public new readonly string VaultUri;

        public CertificateClientEx(string vaultName)
        {
            Utils.GuardVaultName(vaultName);
            VaultName = vaultName;
            VaultUri = string.Format(Consts.AzureVaultUriFormat, VaultName);
        }

        private string ToIdentifier(string endpoint, string name, string version) => $"{VaultUri}/{endpoint}/{name}" + (string.IsNullOrEmpty(version) ? "" : $"/{version}");

        public string ToSecretIdentifier(string secretName, string version = null) => ToIdentifier(Consts.SecretsEndpoint, secretName, version);

        public string ToCertificateIdentifier(string certificateName, string version = null) => ToIdentifier(Consts.CertificatesEndpoint, certificateName, version);

        public override string ToString() => VaultUri;
    }
}