using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.PS.Common.Vault.Explorer
{
    [JsonArray]
    public class FavoriteSecrets : List<string> { }

    [JsonDictionary]
    public class FavoriteSecretsDictionary : Dictionary<string, FavoriteSecrets>
    {
        [JsonConstructor]
        public FavoriteSecretsDictionary(IDictionary<string, FavoriteSecrets> favoriteSecretsDictionary) : base(favoriteSecretsDictionary, StringComparer.CurrentCultureIgnoreCase)
        {
            foreach (string vaultAlias in Keys)
            {
                Guard.ArgumentNotNullOrWhitespace(vaultAlias, nameof(vaultAlias));
            }
        }
    }
}
