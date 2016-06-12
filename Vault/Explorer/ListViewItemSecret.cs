using Microsoft.Azure.KeyVault;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.PS.Common.Vault.Explorer
{
    /// <summary>
    /// Secret list view item which also presents itself nicely to PropertyGrid
    /// </summary>
    public class ListViewItemSecret : ListViewItemBase
    {
        public readonly SecretAttributes Attributes;
        public readonly string ContentTypeStr;
        public readonly ContentType ContentType;

        private ListViewItemSecret(VaultAlias vaultAlias, ListViewGroupCollection groups, SecretIdentifier identifier, SecretAttributes attributes, string contentTypeStr, Dictionary<string, string> tags) : 
            base(vaultAlias, groups, ContentTypeEnumConverter.GetValue(contentTypeStr).IsCertificate() ? CertificatesGroup : SecretsGroup, 
                identifier, tags, attributes.Enabled, attributes.Created, attributes.Updated, attributes.NotBefore, attributes.Expires)
        {
            Attributes = attributes;
            ContentTypeStr = contentTypeStr;
            ContentType = ContentTypeEnumConverter.GetValue(contentTypeStr);
        }

        public ListViewItemSecret(VaultAlias vaultAlias, ListViewGroupCollection groups, SecretItem si) : this(vaultAlias, groups, si.Identifier, si.Attributes, si.ContentType, si.Tags) { }

        public ListViewItemSecret(VaultAlias vaultAlias, ListViewGroupCollection groups, Secret s) : this(vaultAlias, groups, s.SecretIdentifier, s.Attributes, s.ContentType, s.Tags) { }

        protected override IEnumerable<PropertyDescriptor> GetCustomProperties()
        {
            yield return new ReadOnlyPropertyDescriptor("Content Type", ContentTypeStr);
        }
    }
}
