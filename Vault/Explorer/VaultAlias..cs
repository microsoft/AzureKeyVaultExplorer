using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.PS.Common.Vault.Explorer
{
    [JsonObject]
    internal class VaultAlias
    {
        [JsonProperty]
        public readonly string Alias;

        [JsonProperty]
        public readonly string[] VaultNames;

        [JsonConstructor]
        public VaultAlias(string alias, params string[] vaultNames)
        {
            Guard.ArgumentNotNullOrEmptyString(alias, nameof(alias));
            Guard.ArgumentCollectionNotEmpty(vaultNames, nameof(vaultNames));
            Alias = alias;
            VaultNames = vaultNames;
        }

        public override string ToString() => Alias;
    }
}
