using Microsoft.Azure.KeyVault;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.PS.Common.Vault.Explorer
{
    /// <summary>
    /// Certificate object to edit via PropertyGrid
    /// </summary>
    public class PropertyObjectCertificate : PropertyObject
    {
        public readonly CertificateBundle CertificateBundle;
        public readonly X509Certificate2 Certificate;

        [DisplayName("Thumbprint")]
        [Browsable(true)]
        public string Thumbprint => CertificateBundle.X5T;

        [DisplayName("Policy attributes")]
        [Browsable(true)]
        [ReadOnly(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public CertificatePolicyAttributes PolicyAttributes => CertificateBundle.Policy.Attributes;

        [DisplayName("Issuer reference")]
        [Browsable(true)]
        [ReadOnly(true)]
        public string IssuerReference => CertificateBundle.Policy.IssuerReference?.Name;

        [DisplayName("Key properties")]
        [Browsable(true)]
        [ReadOnly(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public KeyProperties KeyProperties => CertificateBundle.Policy.KeyProperties;

        [DisplayName("Life time actions")]
        [Browsable(true)]
        [ReadOnly(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public LifetimeAction[] LifetimeActions => CertificateBundle.Policy.LifetimeActions.ToArray();

        [DisplayName("Certificate properties")]
        [Browsable(true)]
        [ReadOnly(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public X509CertificateProperties X509CertificateProperties => CertificateBundle.Policy.X509CertificateProperties;

        public PropertyObjectCertificate(CertificateBundle certificateBundle, X509Certificate2 certificate, PropertyChangedEventHandler propertyChanged) :
            base(certificateBundle.Id, certificateBundle.Tags, certificateBundle.Attributes.Enabled, certificateBundle.Attributes.Expires, certificateBundle.Attributes.NotBefore, propertyChanged)
        {
            CertificateBundle = certificateBundle;
            Certificate = certificate;
            _contentType = ContentType.Pkcs12;
            _value = "Click on the link below to view or install the certificate";
        }

        protected override IEnumerable<KeyValuePair<string, string>> GetCustomTags()
        {
            yield break;
        }
    }
}
