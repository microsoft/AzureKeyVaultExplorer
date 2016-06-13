using Microsoft.Azure.KeyVault;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.PS.Common.Vault.Explorer
{
    /// <summary>
    /// Certificate object to edit via PropertyGrid
    /// </summary>
    public class PropertyObjectCertificate : PropertyObject
    {
        CertificateBundle _certificate;

        [DisplayName("Thumbprint")]
        [Browsable(true)]
        public string Thumbprint => _certificate.X5T;

        [DisplayName("Policy attributes")]
        [Browsable(true)]
        [ReadOnly(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public CertificatePolicyAttributes PolicyAttributes => _certificate.Policy.Attributes;

        [DisplayName("Issuer reference")]
        [Browsable(true)]
        [ReadOnly(true)]
        public string IssuerReference => _certificate.Policy.IssuerReference?.Name;

        [DisplayName("Key properties")]
        [Browsable(true)]
        [ReadOnly(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public KeyProperties KeyProperties => _certificate.Policy.KeyProperties;

        [DisplayName("Life time actions")]
        [Browsable(true)]
        [ReadOnly(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public LifetimeAction[] LifetimeActions => _certificate.Policy.LifetimeActions.ToArray();

        [DisplayName("Certificate properties")]
        [Browsable(true)]
        [ReadOnly(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public X509CertificateProperties X509CertificateProperties => _certificate.Policy.X509CertificateProperties;

        public PropertyObjectCertificate(CertificateBundle certificate, PropertyChangedEventHandler propertyChanged) :
            base(certificate.Id, certificate.Tags, certificate.Attributes.Enabled, certificate.Attributes.Expires, certificate.Attributes.NotBefore, propertyChanged)
        {
            _certificate = certificate;
            _contentType = ContentType.Pkcs12;
        }

        protected override IEnumerable<KeyValuePair<string, string>> GetCustomTags()
        {
            yield break;
        }
    }
}
