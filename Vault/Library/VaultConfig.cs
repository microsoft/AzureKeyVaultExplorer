using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace VaultLibrary
{
    [JsonDictionary]
    public class VaultsConfig : Dictionary<string, VaultAccessType>
    {
        [JsonConstructor]
        public VaultsConfig(IDictionary<string, VaultAccessType> vaults) : base(vaults, StringComparer.CurrentCultureIgnoreCase)
        {
            foreach (string vaultName in Keys)
            {
                Utils.GuardVaultName(vaultName);
            }
        }
    }
}
