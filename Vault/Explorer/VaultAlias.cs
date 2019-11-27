// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Vault.Library;
using Microsoft.Vault.Core;

namespace Microsoft.Vault.Explorer
{
    [JsonObject]
    public class VaultAlias
    {
        [JsonProperty]
        public readonly string Alias;

        [JsonProperty]
        public readonly string[] VaultNames;

        [JsonProperty]
        public readonly string[] SecretKinds;

        public bool SecretsCollectionEnabled;

        public bool CertificatesCollectionEnabled;

        public string DomainHint;

        public string UserAlias;

        [JsonConstructor]
        public VaultAlias(string alias, string[] vaultNames, string[] secretKinds)
        {
            Guard.ArgumentNotNullOrEmptyString(alias, nameof(alias));
            Guard.ArgumentCollectionNotEmpty(vaultNames, nameof(vaultNames));
            Guard.ArgumentInRange(vaultNames.Length, 1, 2, nameof(vaultNames));
            Alias = alias;
            VaultNames = vaultNames;
            SecretKinds = secretKinds;
        }

        public override string ToString() => Alias;

        public override bool Equals(object obj) => Equals((VaultAlias)obj);

        public bool Equals(VaultAlias va) => (Alias == va?.Alias);

        public override int GetHashCode() => Alias?.GetHashCode() ?? 0;
    }

    [JsonArray]
    public class VaultAliases : List<VaultAlias> { }
}
