// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 

namespace Microsoft.Vault.Explorer
{
    public interface ISession
    {
        VaultAlias CurrentVaultAlias { get; }

        Library.Vault CurrentVault { get; }

        ListViewSecrets ListViewSecrets { get; }
    }
}
