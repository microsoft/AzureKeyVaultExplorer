// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 

using Microsoft.Azure.KeyVault;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Azure.KeyVault.Models;

namespace Microsoft.Vault.Explorer
{
    /// <summary>
    /// Secret list view item which also presents itself nicely to PropertyGrid
    /// </summary>
    public class ListViewItemSecret : ListViewItemBase
    {
        public readonly SecretAttributes Attributes;
        public readonly string ContentTypeStr;
        public readonly ContentType ContentType;

        private ListViewItemSecret(ISession session, SecretIdentifier identifier, SecretAttributes attributes, string contentTypeStr, IDictionary<string, string> tags) :
            base(session, ContentTypeEnumConverter.GetValue(contentTypeStr).IsCertificate() ? CertificatesGroup : SecretsGroup,
                identifier, tags, attributes.Enabled, attributes.Created, attributes.Updated, attributes.NotBefore, attributes.Expires)
        {
            Attributes = attributes;
            ContentTypeStr = contentTypeStr;
            ContentType = ContentTypeEnumConverter.GetValue(contentTypeStr);
        }

        public ListViewItemSecret(ISession session, SecretItem si) : this(session, si.Identifier, si.Attributes, si.ContentType, si.Tags) { }

        public ListViewItemSecret(ISession session, SecretBundle s) : this(session, s.SecretIdentifier, s.Attributes, s.ContentType, s.Tags) { }

        protected override IEnumerable<PropertyDescriptor> GetCustomProperties()
        {
            yield return new ReadOnlyPropertyDescriptor("Content Type", ContentTypeStr);
        }

        public override ContentType GetContentType() => ContentType;

        public override async Task<PropertyObject> GetAsync(CancellationToken cancellationToken)
        {
            var s = await Session.CurrentVault.GetSecretAsync(Name, null, cancellationToken);
            return new PropertyObjectSecret(s, null);
        }

        public override async Task<ListViewItemBase> ToggleAsync(CancellationToken cancellationToken)
        {
            SecretBundle s = await Session.CurrentVault.UpdateSecretAsync(Name, null, new Dictionary<string, string>(Tags), null, new SecretAttributes() { Enabled = !Attributes.Enabled }, cancellationToken); // Toggle only Enabled attribute
            return new ListViewItemSecret(Session, s);
        }

        public override async Task<ListViewItemBase> ResetExpirationAsync(CancellationToken cancellationToken)
        {
            var sa = new SecretAttributes()
            {
                NotBefore = (this.NotBefore == null) ? (DateTime?)null : DateTime.UtcNow.AddHours(-1),
                Expires = (this.Expires == null) ? (DateTime?)null : DateTime.UtcNow.AddYears(1)
            };
            SecretBundle s = await Session.CurrentVault.UpdateSecretAsync(Name, null, new Dictionary<string, string>(Tags), null, sa, cancellationToken); // Reset only NotBefore and Expires attributes
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
            SecretBundle sOriginal = (SecretBundle)originalObject;
            PropertyObjectSecret posNew = (PropertyObjectSecret)newObject;
            SecretBundle s = null;
            // New secret, secret rename or new value
            if ((sOriginal == null) || (sOriginal.SecretIdentifier.Name != posNew.Name) || (sOriginal.Value != posNew.RawValue))
            {
                s = await session.CurrentVault.SetSecretAsync(posNew.Name, posNew.RawValue, posNew.ToTagsDictionary(), ContentTypeEnumConverter.GetDescription(posNew.ContentType), posNew.ToSecretAttributes(), cancellationToken);
            }
            else // Same secret name and value
            {
                s = await session.CurrentVault.UpdateSecretAsync(posNew.Name, null, posNew.ToTagsDictionary(), ContentTypeEnumConverter.GetDescription(posNew.ContentType), posNew.ToSecretAttributes(), cancellationToken);
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