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

        public override Form GetEditDialog(ISession session, string name, IEnumerable<object> versions)
        {
            return new SecretDialog(session, name, versions.Cast<SecretItem>());
        }

        private bool VerifyDuplication(ISession session, Secret sOld, PropertyObjectSecret soNew)
        {
            string oldName = sOld?.SecretIdentifier.Name ?? "";
            string newMd5 = soNew.Md5;

            // Check if we already have *another* secret with the same name
            if ((oldName != soNew.Name) && (session.ListViewSecrets.Items.ContainsKey(soNew.Name) &&
                (MessageBox.Show($"Are you sure you want to replace secret '{soNew.Name}' with new value?", Utils.AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)))
            {
                return false;
            }

            // Detect dups by Md5
            var sameSecretsList = from slvi in session.ListViewSecrets.Items.Cast<ListViewItemBase>() where (slvi.Md5 == newMd5) && (slvi.Name != oldName) && (slvi.Name != soNew.Name) select slvi.Name;
            if ((sameSecretsList.Count() > 0) &&
                (MessageBox.Show($"There are {sameSecretsList.Count()} other secret(s) in the vault which has the same Md5: {newMd5}.\nHere the name(s) of the other secrets:\n{string.Join(", ", sameSecretsList)}\nAre you sure you want to add or update secret {soNew.Name} and have a duplication of secrets?", Utils.AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != DialogResult.Yes))
            {
                return false;
            }
            return true;
        }

        public override async Task<ListViewItemBase> AddOrUpdate(ISession session, ItemDialogBaseMode mode, object originalObject, PropertyObject newObject, CancellationToken cancellationToken)
        {
            Secret sOriginal = (Secret)originalObject;
            PropertyObjectSecret posNew = (PropertyObjectSecret)newObject;
            Secret s = null;
            // Check for duplication by Name and Md5
            if (false == VerifyDuplication(session, sOriginal, posNew)) return null;
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
    }
}