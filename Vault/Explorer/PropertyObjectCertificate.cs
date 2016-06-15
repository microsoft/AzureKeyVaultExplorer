using Microsoft.Azure.KeyVault;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.PS.Common.Vault.Explorer
{
    /// <summary>
    /// Certificate object to edit via PropertyGrid
    /// </summary>
    [DefaultProperty("Certificate")]
    public class PropertyObjectCertificate : PropertyObject
    {
        public readonly CertificateBundle CertificateBundle;
        public readonly CertificatePolicy CertificatePolicy;

        [Category("General")]
        [DisplayName("Certificate")]
        [Description("Displays a system dialog that contains the properties of an X.509 certificate and its associated certificate chain. One can also install the cetificate locally by clicking on Install Certificate button in the dialog.")]
        [Browsable(true)]
        [Editor(typeof(CertificateUIEditor), typeof(UITypeEditor))]
        public X509Certificate2 Certificate { get; private set; }

        [Category("General")]
        [DisplayName("Thumbprint")]
        [Browsable(true)]
        public string Thumbprint => Certificate.Thumbprint;

        [Category("Identifiers")]
        [DisplayName("Certificate")]
        [Browsable(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public CertificateIdentifier Id => CertificateBundle.Id;

        [Category("Identifiers")]
        [DisplayName("Key")]
        [Browsable(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public KeyIdentifier KeyId => CertificateBundle.KeyId;

        [Category("Identifiers")]
        [DisplayName("Secret")]
        [Browsable(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public SecretIdentifier SecretId => CertificateBundle.SecretId;

        [Category("Policy")]
        [DisplayName("Attributes")]
        [Browsable(true)]
        [ReadOnly(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public CertificatePolicyAttributes PolicyAttributes => CertificatePolicy.Attributes;

        [Category("Policy")]
        [DisplayName("Certificate properties")]
        [Browsable(true)]
        [ReadOnly(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public X509CertificateProperties X509CertificateProperties => CertificatePolicy.X509CertificateProperties;

        [Category("Policy")]
        [DisplayName("Key properties")]
        [Browsable(true)]
        [ReadOnly(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public KeyProperties KeyProperties => CertificatePolicy.KeyProperties;

        [Category("Policy")]
        [DisplayName("Secret properties")]
        [Browsable(true)]
        [ReadOnly(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public SecretProperties SecretProperties => CertificatePolicy.SecretProperties;

        [Category("Policy")]
        [DisplayName("Life time actions")]
        [Browsable(true)]
        [ReadOnly(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public LifetimeAction[] LifetimeActions => CertificatePolicy.LifetimeActions?.ToArray();

        [Category("Policy")]
        [DisplayName("Issuer reference")]
        [Browsable(true)]
        [ReadOnly(true)]
        public string IssuerReference => CertificatePolicy.IssuerReference?.Name;

        [Category("Other")]
        [DisplayName("Pending Reference")]
        [Browsable(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public PendingReference PendingReference => CertificateBundle.PendingReference;

        public PropertyObjectCertificate(CertificateBundle certificateBundle, CertificatePolicy policy, X509Certificate2 certificate, PropertyChangedEventHandler propertyChanged) :
            base(certificateBundle.Id, certificateBundle.Tags, certificateBundle.Attributes.Enabled, certificateBundle.Attributes.Expires, certificateBundle.Attributes.NotBefore, propertyChanged)
        {
            CertificateBundle = certificateBundle;
            CertificatePolicy = policy;
            Certificate = certificate;
            _contentType = ContentType.Pkcs12;
            _value = "Click on the link below to view or install the certificate";
        }

        protected override IEnumerable<KeyValuePair<string, string>> GetCustomTags()
        {
            yield break;
        }
    }

    public class CertificateUIEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) => UITypeEditorEditStyle.Modal;

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            X509Certificate2UI.DisplayCertificate((X509Certificate2)value);
            return value;
        }
    }
}
