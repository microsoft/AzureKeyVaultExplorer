using Microsoft.Azure.KeyVault;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;

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

        private ListViewItemSecret(ISession session, SecretIdentifier identifier, SecretAttributes attributes, string contentTypeStr, Dictionary<string, string> tags) : 
            base(session, ContentTypeEnumConverter.GetValue(contentTypeStr).IsCertificate() ? CertificatesGroup : SecretsGroup, 
                identifier, tags, attributes.Enabled, attributes.Created, attributes.Updated, attributes.NotBefore, attributes.Expires)
        {
            Attributes = attributes;
            ContentTypeStr = contentTypeStr;
            ContentType = ContentTypeEnumConverter.GetValue(contentTypeStr);
        }

        public ListViewItemSecret(ISession session, SecretItem si) : this(session, si.Identifier, si.Attributes, si.ContentType, si.Tags) { }

        public ListViewItemSecret(ISession session, Secret s) : this(session, s.SecretIdentifier, s.Attributes, s.ContentType, s.Tags) { }

        protected override IEnumerable<PropertyDescriptor> GetCustomProperties()
        {
            yield return new ReadOnlyPropertyDescriptor("Content Type", ContentTypeStr);
        }

        public override async Task<ListViewItemBase> ToggleAsync(CancellationToken cancellationToken)
        {
            Secret s = await Session.CurrentVault.UpdateSecretAsync(Name, Utils.AddChangedBy(Tags), null, new SecretAttributes() { Enabled = !Attributes.Enabled }, cancellationToken); // Toggle only Enabled attribute
            return new ListViewItemSecret(Session, s);
        }

        public override async Task<ListViewItemBase> DeleteAsync(CancellationToken cancellationToken)
        {
            await Session.CurrentVault.DeleteSecretAsync(Name, cancellationToken);
            return this;
        }
    }
}
