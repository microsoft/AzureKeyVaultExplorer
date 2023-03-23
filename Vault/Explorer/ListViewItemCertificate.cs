// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Security.Cryptography.X509Certificates;
using Azure.Security.KeyVault.Certificates;

namespace Microsoft.Vault.Explorer
{
    /// <summary>
    /// Key Vault Certificate list view item which also presents itself nicely to PropertyGrid
    /// </summary>
    public class ListViewItemCertificate : ListViewItemBase
    {
        public readonly CertificateProperties CertificateProperties;
        public readonly string Thumbprint;

        public ListViewItemCertificate(ISession session, CertificateProperties certificateProperties) :
            base(session, KeyVaultCertificatesGroup, certificateProperties.Id, certificateProperties.Name, certificateProperties.Tags, certificateProperties.Enabled, certificateProperties.CreatedOn, certificateProperties.UpdatedOn, certificateProperties.NotBefore, certificateProperties.ExpiresOn)
        {
            CertificateProperties = certificateProperties;
            Thumbprint = Utils.ByteArrayToHex(certificateProperties.X509Thumbprint)?.ToLowerInvariant();
        }

        public ListViewItemCertificate(ISession session, KeyVaultCertificate c) : this(session, c.Properties) { }

        public ListViewItemCertificate(ISession session, KeyVaultCertificateWithPolicy cb) : this(session, cb.Properties) { }

        protected override IEnumerable<PropertyDescriptor> GetCustomProperties()
        {
            yield return new ReadOnlyPropertyDescriptor("Content Type", CertificateContentType.Pkcs12);
            yield return new ReadOnlyPropertyDescriptor("Thumbprint", Thumbprint);
        }

        public override ContentType GetContentType() => ContentType.Pkcs12;

        public override async Task<PropertyObject> GetAsync(CancellationToken cancellationToken)
        {
            var cb = await Session.CurrentVault.GetCertificateAsync(Name, cancellationToken);
            var cert = await Session.CurrentVault.GetCertificateWithExportableKeysAsync(Name, null, cancellationToken);
            return new PropertyObjectCertificate(cb, cb.Policy, cert, null);
        }

        public override async Task<ListViewItemBase> ToggleAsync(CancellationToken cancellationToken)
        {
            CertificateProperties certificateProperties = new CertificateProperties(Name);
            certificateProperties.Enabled = !CertificateProperties.Enabled;
            KeyVaultCertificate cb = await Session.CurrentVault.UpdateCertificateAsync(certificateProperties, cancellationToken); // Toggle only Enabled attribute
            return new ListViewItemCertificate(Session, cb);
        }

        public override async Task<ListViewItemBase> ResetExpirationAsync(CancellationToken cancellationToken)
        {
            var ca = new CertificateProperties(Name);
            KeyVaultCertificate cb = await Session.CurrentVault.UpdateCertificateAsync(ca, cancellationToken); // Reset only NotBefore and Expires CertificateProperties
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
            return new CertificateDialog(Session, name, versions.Cast<CertificateProperties>());
        }

        public override async Task<ListViewItemBase> UpdateAsync(object originalObject, PropertyObject newObject, CancellationToken cancellationToken)
        {
            KeyVaultCertificate cb = (KeyVaultCertificate)originalObject;
            PropertyObjectCertificate certNew = (PropertyObjectCertificate)newObject;
            CertificateProperties properties = new CertificateProperties(certNew.Name)
            {
                Enabled = certNew.Enabled,
            };
            await Session.CurrentVault.UpdateCertificatePolicyAsync(certNew.Name, certNew.CertificatePolicy, cancellationToken);
            cb = await Session.CurrentVault.UpdateCertificateAsync(properties, cancellationToken);
            return new ListViewItemCertificate(Session, cb);
        }

        public static async Task<ListViewItemCertificate> NewAsync(ISession session, PropertyObject newObject, CancellationToken cancellationToken)
        {
            PropertyObjectCertificate certNew = (PropertyObjectCertificate)newObject;
            var certCollection = new X509Certificate2Collection();
            certCollection.Add(certNew.Certificate);
            ImportCertificateOptions importCertificateOptions = new ImportCertificateOptions(certNew.Name, certNew.Certificate.RawData);
            importCertificateOptions.Policy = certNew.CertificatePolicy;
            KeyVaultCertificateWithPolicy cb = await session.CurrentVault.ImportCertificateAsync(importCertificateOptions, cancellationToken);
            return new ListViewItemCertificate(session, cb);
        }
    }
}
