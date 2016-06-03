using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Microsoft.PS.Common.Vault.Explorer
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
    }

    [JsonArray]
    public class VaultAliases : List<VaultAlias> { }
}
