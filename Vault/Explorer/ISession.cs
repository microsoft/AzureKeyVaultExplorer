using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.PS.Common.Vault.Explorer
{
    public interface ISession
    {
        VaultAlias CurrentVaultAlias { get; }
        Vault CurrentVault { get; }
        ListViewSecrets ListViewSecrets { get; }
    }
}
