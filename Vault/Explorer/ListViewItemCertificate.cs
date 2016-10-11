// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 

using Microsoft.Azure.KeyVault;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Azure.KeyVault.Models;

namespace Microsoft.Vault.Explorer
{
    /// <summary>
    /// Key Vault Certificate list view item which also presents itself nicely to PropertyGrid
    /// </summary>
    public class ListViewItemCertificate : ListViewItemBase
    {
        public readonly CertificateAttributes Attributes;
        public readonly string Thumbprint;

        private ListViewItemCertificate(ISession session, CertificateIdentifier identifier, CertificateAttributes attributes, string thumbprint, IDictionary<string, string> tags) : 
            base(session, KeyVaultCertificatesGroup, identifier, tags, attributes.Enabled, attributes.Created, attributes.Updated, attributes.NotBefore, attributes.Expires)
        {
            Attributes = attributes;
            Thumbprint = thumbprint?.ToLowerInvariant();
        }

        public ListViewItemCertificate(ISession session, CertificateItem c) : this(session, c.Identifier, c.Attributes, Utils.ByteArrayToHex(c.X509Thumbprint), c.Tags) { }

        public ListViewItemCertificate(ISession session, CertificateBundle cb) : this(session, cb.CertificateIdentifier, cb.Attributes, Utils.ByteArrayToHex(cb.X509Thumbprint), cb.Tags) { }

        protected override IEnumerable<PropertyDescriptor> GetCustomProperties()
        {
            yield return new ReadOnlyPropertyDescriptor("Content Type", CertificateContentType.Pfx);
            yield return new ReadOnlyPropertyDescriptor("Thumbprint", Thumbprint);
        }

        public override ContentType GetContentType() => ContentType.Pkcs12;

        public override async Task<PropertyObject> GetAsync(CancellationToken cancellationToken)
        {
            var cb = await Session.CurrentVault.GetCertificateAsync(Name, null, cancellationToken);
            var cert = await Session.CurrentVault.GetCertificateWithExportableKeysAsync(Name, null, cancellationToken);
            return new PropertyObjectCertificate(cb, cb.Policy, cert, null);
        }

        public override async Task<ListViewItemBase> ToggleAsync(CancellationToken cancellationToken)
        {
            CertificateBundle cb = await Session.CurrentVault.UpdateCertificateAsync(Name, null, null, new CertificateAttributes() { Enabled = !Attributes.Enabled }, Tags, cancellationToken); // Toggle only Enabled attribute
            return new ListViewItemCertificate(Session, cb);
        }

        public override async Task<ListViewItemBase> ResetExpirationAsync(CancellationToken cancellationToken)
        {
            var ca = new CertificateAttributes()
            {
                NotBefore = (this.NotBefore == null) ? (DateTime?)null : DateTime.UtcNow.AddHours(-1),
                Expires = (this.Expires == null) ? (DateTime?)null : DateTime.UtcNow.AddYears(1)
            };
            CertificateBundle cb = await Session.CurrentVault.UpdateCertificateAsync(Name, null, null, ca, Tags, cancellationToken); // Reset only NotBefore and Expires attributes
            return new ListViewItemCertificate(Session, cb);
        }

        public override async Task<ListViewItemBase> DeleteAsync(CancellationToken cancellationToken)
        {
            await Session.CurrentVault.DeleteCertificateAsync(Name, cancellationToken);
            return this;
        }

        public override async Task<IEnumerable<object>> GetVersionsAsync(CancellationToken cancellationToken)
        {
            return await Session.CurrentVault.GetCertificateVersionsAsync(Name, 0, cancellationToken);
        }

        public override Form GetEditDialog(string name, IEnumerable<object> versions)
        {
            return new CertificateDialog(Session, name, versions.Cast<CertificateItem>());
        }

        public override async Task<ListViewItemBase> UpdateAsync(object originalObject, PropertyObject newObject, CancellationToken cancellationToken)
        {
            CertificateBundle cb = (CertificateBundle)originalObject;
            PropertyObjectCertificate certNew = (PropertyObjectCertificate)newObject;
            await Session.CurrentVault.UpdateCertificatePolicyAsync(certNew.Name, certNew.CertificatePolicy, cancellationToken);
            cb = await Session.CurrentVault.UpdateCertificateAsync(certNew.Name, null, null, certNew.ToCertificateAttributes(), certNew.ToTagsDictionary(), cancellationToken);
            return new ListViewItemCertificate(Session, cb);
        }

        public static async Task<ListViewItemCertificate> NewAsync(ISession session, PropertyObject newObject, CancellationToken cancellationToken)
        {
            PropertyObjectCertificate certNew = (PropertyObjectCertificate)newObject;
            var certCollection = new X509Certificate2Collection();
            certCollection.Add(certNew.Certificate);
            CertificateBundle cb = await session.CurrentVault.ImportCertificateAsync(certNew.Name, certCollection, certNew.CertificatePolicy, certNew.CertificateBundle.Attributes, certNew.ToTagsDictionary(), cancellationToken);
            return new ListViewItemCertificate(session, cb);
        }
    }
}
