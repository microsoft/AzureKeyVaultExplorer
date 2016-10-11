// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Vault.Library;
using Microsoft.Vault.Core;

namespace Microsoft.Vault.Explorer
{
    [JsonObject]
    public class FavoriteSecret
    {
        [JsonProperty]
        public readonly DateTimeOffset CreationTime;

        public FavoriteSecret()
        {
            CreationTime = DateTimeOffset.UtcNow;
        }

        [JsonConstructor]
        public FavoriteSecret(DateTimeOffset creationTime)
        {
            CreationTime = creationTime;
        }
    }

    [JsonDictionary]
    public class FavoriteSecrets : Dictionary<string, FavoriteSecret>
    {
        public FavoriteSecrets() : base() { }

        [JsonConstructor]
        public FavoriteSecrets(IDictionary<string, FavoriteSecret> dictionary) : base(dictionary, StringComparer.CurrentCultureIgnoreCase)
        {
            foreach (string secretName in Keys)
            {
                if (false == Consts.ValidSecretNameRegex.IsMatch(secretName))
                {
                    throw new ArgumentException($"Invalid secret name {secretName}");
                }
            }
        }
    }

    [JsonDictionary]
    public class FavoriteSecretsDictionary : Dictionary<string, FavoriteSecrets>
    {
        [JsonConstructor]
        public FavoriteSecretsDictionary(IDictionary<string, FavoriteSecrets> dictionary) : base(dictionary, StringComparer.CurrentCultureIgnoreCase)
        {
            foreach (string vaultAlias in Keys)
            {
                Guard.ArgumentNotNullOrWhitespace(vaultAlias, nameof(vaultAlias));
            }
        }
    }

    public static class FavoriteSecretUtil
    {
        public static bool Contains(string vaultAlias, string secretName)
        {
            return Settings.Default.FavoriteSecretsDictionary.ContainsKey(vaultAlias) ? 
                Settings.Default.FavoriteSecretsDictionary[vaultAlias].ContainsKey(secretName) ? true : false : false;
        }

        public static void Add(string vaultAlias, string secretName)
        {
            if (false == Settings.Default.FavoriteSecretsDictionary.ContainsKey(vaultAlias))
            {
                Settings.Default.FavoriteSecretsDictionary.Add(vaultAlias, new FavoriteSecrets());
            }
            var favorites = Settings.Default.FavoriteSecretsDictionary[vaultAlias];
            favorites.Add(secretName, new FavoriteSecret());
        }

        public static void Remove(string vaultAlias, string secretName)
        {
            if (Settings.Default.FavoriteSecretsDictionary.ContainsKey(vaultAlias))
            {
                var favorites = Settings.Default.FavoriteSecretsDictionary[vaultAlias];
                favorites.Remove(secretName);
                if (favorites.Count == 0)
                {
                    Settings.Default.FavoriteSecretsDictionary.Remove(vaultAlias);
                }
            }
        }
    }
}
