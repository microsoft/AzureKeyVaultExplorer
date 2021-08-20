// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Azure.Security.KeyVault.Secrets;

namespace Microsoft.Vault.Explorer
{
    /// <summary>
    /// Secret list view item which also presents itself nicely to PropertyGrid
    /// </summary>
    public class ListViewItemSecret : ListViewItemBase
    {
        public readonly SecretProperties Attributes;
        public readonly string ContentTypeStr;
        public readonly ContentType ContentType;

        private ListViewItemSecret(ISession session, SecretProperties attributes) :
            base(session, ContentTypeEnumConverter.GetValue(attributes.ContentType).IsCertificate() ? CertificatesGroup : SecretsGroup,
                attributes.Id, attributes.Name, attributes.Tags, attributes.Enabled, attributes.CreatedOn, attributes.UpdatedOn, attributes.NotBefore, attributes.ExpiresOn)
        {
            Attributes = attributes;
            ContentTypeStr = attributes.ContentType;
            ContentType = ContentTypeEnumConverter.GetValue(attributes.ContentType);
        }

        public ListViewItemSecret(ISession session, KeyVaultSecret s) : this(session, s.Properties) { }

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
            SecretProperties secretProperties = new SecretProperties(Name);
            secretProperties.Enabled = !Attributes.Enabled;
            SecretProperties s = await Session.CurrentVault.UpdateSecretAsync(secretProperties, cancellationToken); // Toggle only Enabled attribute
            return new ListViewItemSecret(Session, s);
        }

        public override async Task<ListViewItemBase> ResetExpirationAsync(CancellationToken cancellationToken)
        {
            var sa = new SecretProperties(Name)
            {
                NotBefore = (this.NotBefore == null) ? (DateTime?)null : DateTime.UtcNow.AddHours(-1),
                ExpiresOn = (this.Expires == null) ? (DateTime?)null : DateTime.UtcNow.AddYears(1)
            };
            SecretProperties s = await Session.CurrentVault.UpdateSecretAsync(sa, cancellationToken); // Reset only NotBefore and Expires attributes
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
            return new SecretDialog(Session, name, versions.Cast<SecretProperties>());
        }

        private static async Task<ListViewItemSecret> NewOrUpdateAsync(ISession session, object originalObject, PropertyObject newObject, CancellationToken cancellationToken)
        {
            KeyVaultSecret sOriginal = (KeyVaultSecret)originalObject;
            PropertyObjectSecret posNew = (PropertyObjectSecret)newObject;
            KeyVaultSecret s = null;
            SecretProperties sp = null;
            SecretProperties properties = new SecretProperties(posNew.Name)
            {
                ContentType = ContentTypeEnumConverter.GetDescription(posNew.ContentType),
                Enabled = posNew.Enabled,
                ExpiresOn = posNew.Expires,
                NotBefore = posNew.NotBefore
            };
            // New secret, secret rename or new value
            if ((sOriginal == null) || (sOriginal.Name != posNew.Name) || (sOriginal.Value != posNew.RawValue))
            {
                s = await session.CurrentVault.SetSecretAsync(posNew.Name, posNew.RawValue, properties, cancellationToken);
            }
            else // Same secret name and value
            {
                sp = await session.CurrentVault.UpdateSecretAsync(properties, cancellationToken);
            }
            string oldSecretName = sOriginal?.Name;
            if ((oldSecretName != null) && (oldSecretName != posNew.Name)) // Delete old secret
            {
                await session.CurrentVault.DeleteSecretAsync(oldSecretName, cancellationToken);
            }
            if (s != null)
            {
                return new ListViewItemSecret(session, s);
            }
            return new ListViewItemSecret(session, sp);
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