// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 

using Newtonsoft.Json;

// We keep old namespace for backward compatiblity reasons, to be able to deserialize old Vaults.json
namespace Microsoft.PS.Common.Vault
{
    [JsonObject]
    public class VaultAccessUserInteractive : Microsoft.Vault.Library.VaultAccessUserInteractive
    {
        [JsonConstructor]
        public VaultAccessUserInteractive(string domainHint, string userAlias) : base(domainHint) { }
    }

    [JsonObject]
    public class VaultAccessClientCredential : Microsoft.Vault.Library.VaultAccessClientCredential
    {
        [JsonConstructor]
        public VaultAccessClientCredential(string clientId, string clientSecret) : base(clientId, clientSecret) { }
    }

    [JsonObject]
    public class VaultAccessClientCertificate : Microsoft.Vault.Library.VaultAccessClientCertificate
    {
        [JsonConstructor]
        public VaultAccessClientCertificate(string clientId, string certificateThumbprint) : base(clientId, certificateThumbprint) { }
    }
}
