// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 

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
