using Microsoft.Azure.KeyVault;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.PS.Common.Vault.Explorer
{
    public class SecretComboBoxItem
    {
        public readonly SecretItem SecretItem;

        public SecretComboBoxItem(SecretItem secretItem)
        {
            Guard.ArgumentNotNull(secretItem, nameof(secretItem));
            SecretItem = secretItem;
        }

        public override string ToString()
        {
            return $"{Utils.NullableDateTimeToString(SecretItem.Attributes.Created)}";
        }
    }
}
