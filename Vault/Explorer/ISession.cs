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
