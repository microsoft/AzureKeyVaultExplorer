using Microsoft.Azure.KeyVault;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Microsoft.PS.Common.Vault.Explorer
{
    public class ListViewItemCertificate : ListViewItemBase
    {
        public readonly CertificateAttributes Attributes;
        public readonly string Thumbprint;

        private ListViewItemCertificate(VaultAlias vaultAlias, ListViewGroupCollection groups, CertificateIdentifier identifier, CertificateAttributes attributes, string thumbprint, IDictionary<string, string> tags) : 
            base(vaultAlias, groups, KeyVaultCertificatesGroup,
                identifier, tags, attributes.Enabled, attributes.Created, attributes.Updated, attributes.NotBefore, attributes.Expires)
        {
            Attributes = attributes;
            Thumbprint = thumbprint;
            Group = Groups[FavoriteSecretUtil.Contains(VaultAlias.Alias, Name) ? FavoritesGroup : KeyVaultCertificatesGroup];
        }

        public ListViewItemCertificate(VaultAlias vaultAlias, ListViewGroupCollection groups, ListCertificateResponseMessage c) : this(vaultAlias, groups, c.Identifier, c.Attributes, c.X5T, c.Tags) { }

        public ListViewItemCertificate(VaultAlias vaultAlias, ListViewGroupCollection groups, CertificateBundle cb) : this(vaultAlias, groups, cb.Id, cb.Attributes, cb.X5T, cb.Tags) { }

        protected override IEnumerable<PropertyDescriptor> GetCustomProperties()
        {
            yield return new ReadOnlyPropertyDescriptor("Content Type", CertificateContentType.Pfx);
            yield return new ReadOnlyPropertyDescriptor("Thumbprint", Thumbprint);
        }
    }
}
