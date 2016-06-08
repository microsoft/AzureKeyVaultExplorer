using Microsoft.Azure.KeyVault;
using System.Windows.Forms;

namespace Microsoft.PS.Common.Vault.Explorer
{
    public class SecretVersion : ToolStripMenuItem
    {
        public readonly int Index;
        public readonly SecretItem SecretItem;

        public SecretVersion(int index, SecretItem secretItem) : base($"{Utils.NullableDateTimeToString(secretItem.Attributes.Created)}")
        {
            Index = index;
            SecretItem = secretItem;
            ToolTipText = string.Format("Creation time:\t\t{0}\nLast updated time:\t{1}\nChanged by:\t\t{2}\nVersion:\t        {3}",
                Utils.NullableDateTimeToString(SecretItem.Attributes.Created),
                Utils.NullableDateTimeToString(SecretItem.Attributes.Updated),
                Utils.GetChangedBy(SecretItem.Tags),
                SecretItem.Identifier.Version);
        }

        public override string ToString() => ((0 == Index) ? "Current value" : $"Value from {Text}") + Utils.DropDownSuffix;
    }
}
