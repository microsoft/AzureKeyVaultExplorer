// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 

using VaultLibrary;

namespace VaultExplorer
{
    public interface ISession
    {
        VaultAlias CurrentVaultAlias { get; }

        Vault CurrentVault { get; }

        ListViewSecrets ListViewSecrets { get; }
    }
}
