using Microsoft.Azure.KeyVault;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

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

        public override async Task<PropertyObject> GetAsync(CancellationToken cancellationToken)
        {
            var s = await Session.CurrentVault.GetSecretAsync(Name, null, cancellationToken);
            return new PropertyObjectSecret(s, null);
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

        public override async Task<IEnumerable<object>> GetVersionsAsync(CancellationToken cancellationToken)
        {
            return await Session.CurrentVault.GetSecretVersionsAsync(Name, 0, cancellationToken);
        }

        public override Form GetEditDialog(string name, IEnumerable<object> versions)
        {
            return new SecretDialog(Session, name, versions.Cast<SecretItem>());
        }

        private static async Task<ListViewItemSecret> NewOrUpdateAsync(ISession session, object originalObject, PropertyObject newObject, CancellationToken cancellationToken)
        {
            Secret sOriginal = (Secret)originalObject;
            PropertyObjectSecret posNew = (PropertyObjectSecret)newObject;
            Secret s = null;
            // New secret, secret rename or new value
            if ((sOriginal == null) || (sOriginal.SecretIdentifier.Name != posNew.Name) || (sOriginal.Value != posNew.RawValue))
            {
                s = await session.CurrentVault.SetSecretAsync(posNew.Name, posNew.RawValue, posNew.ToTagsDictionary(), ContentTypeEnumConverter.GetDescription(posNew.ContentType), posNew.ToSecretAttributes(), cancellationToken);
            }
            else // Same secret name and value
            {
                s = await session.CurrentVault.UpdateSecretAsync(posNew.Name, posNew.ToTagsDictionary(), ContentTypeEnumConverter.GetDescription(posNew.ContentType), posNew.ToSecretAttributes(), cancellationToken);
            }
            string oldSecretName = sOriginal?.SecretIdentifier.Name;
            if ((oldSecretName != null) && (oldSecretName != posNew.Name)) // Delete old secret
            {
                await session.CurrentVault.DeleteSecretAsync(oldSecretName, cancellationToken);
            }
            return new ListViewItemSecret(session, s);
        }

        public override async Task<ListViewItemBase> UpdateAsync(object originalObject, PropertyObject newObject, CancellationToken cancellationToken)
        {
            return await NewOrUpdateAsync(Session, originalObject, newObject, cancellationToken);
        }

        public static Task<ListViewItemSecret> NewAsync(ISession session, PropertyObject newObject, CancellationToken cancellationToken)
        {
            return NewOrUpdateAsync(session, null, newObject, cancellationToken);
        }
    }
}