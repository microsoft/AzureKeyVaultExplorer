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

namespace Microsoft.PS.Common.Vault.Explorer
{
    /// <summary>
    /// Key Vault Certificate list view item which also presents itself nicely to PropertyGrid
    /// </summary>
    public class ListViewItemCertificate : ListViewItemBase
    {
        public readonly CertificateAttributes Attributes;
        public readonly string Thumbprint;

        private ListViewItemCertificate(ISession session, CertificateIdentifier identifier, CertificateAttributes attributes, string thumbprint, IDictionary<string, string> tags) : 
            base(session, KeyVaultCertificatesGroup,
                identifier, tags, attributes.Enabled, attributes.Created, attributes.Updated, attributes.NotBefore, attributes.Expires)
        {
            Attributes = attributes;
            Thumbprint = thumbprint?.ToLowerInvariant();
        }

        public ListViewItemCertificate(ISession session, ListCertificateResponseMessage c) : this(session, c.Identifier, c.Attributes, c.X5T, c.Tags) { }

        public ListViewItemCertificate(ISession session, CertificateBundle cb) : this(session, cb.Id, cb.Attributes, cb.X5T, cb.Tags) { }

        protected override IEnumerable<PropertyDescriptor> GetCustomProperties()
        {
            yield return new ReadOnlyPropertyDescriptor("Content Type", CertificateContentType.Pfx);
            yield return new ReadOnlyPropertyDescriptor("Thumbprint", Thumbprint);
        }

        public override async Task<PropertyObject> GetAsync(CancellationToken cancellationToken)
        {
            var cb = await Session.CurrentVault.GetCertificateAsync(Name, null, cancellationToken);
            var cert = await Session.CurrentVault.GetCertificateWithPrivateKeyAsync(Name, null, cancellationToken);
            return new PropertyObjectCertificate(cb, cb.Policy, cert, null);
        }

        public override async Task<ListViewItemBase> ToggleAsync(CancellationToken cancellationToken)
        {
            CertificateBundle cb = await Session.CurrentVault.UpdateCertificateAsync(Name, new CertificateAttributes() { Enabled = !Attributes.Enabled }, Utils.AddChangedBy(Tags), cancellationToken); // Toggle only Enabled attribute
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
            return new CertificateDialog(Session, name, versions.Cast<ListCertificateResponseMessage>());
        }

        public override async Task<ListViewItemBase> UpdateAsync(object originalObject, PropertyObject newObject, CancellationToken cancellationToken)
        {
            CertificateBundle cb = (CertificateBundle)originalObject;
            PropertyObjectCertificate certNew = (PropertyObjectCertificate)newObject;
            await Session.CurrentVault.UpdateCertificatePolicyAsync(certNew.Name, certNew.CertificatePolicy, cancellationToken);
            cb = await Session.CurrentVault.UpdateCertificateAsync(certNew.Name, certNew.ToCertificateAttributes(), certNew.ToTagsDictionary(), cancellationToken);
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
