// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 

using Azure.Security.KeyVault.Certificates;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Microsoft.Vault.Explorer
{
    /// <summary>
    /// Certificate object to edit via PropertyGrid
    /// </summary>
    [DefaultProperty("Certificate")]
    public class PropertyObjectCertificate : PropertyObject
    {
        public readonly KeyVaultCertificateWithPolicy KeyVaultCertificate;
        public readonly CertificatePolicy CertificatePolicy;

        [Category("General")]
        [DisplayName("Certificate")]
        [Description("Displays a system dialog that contains the properties of an X.509 certificate and its associated certificate chain. One can also install the cetificate locally by clicking on Install Certificate button in the dialog.")]
        [Editor(typeof(CertificateUIEditor), typeof(UITypeEditor))]
        public X509Certificate2 Certificate { get; }

        [Category("General")]
        [DisplayName("Thumbprint")]
        public string Thumbprint => Certificate.Thumbprint?.ToLowerInvariant();

        [Category("Identifiers")]
        [DisplayName("Certificate")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Uri Id => KeyVaultCertificate.Id;

        [Category("Identifiers")]
        [DisplayName("Key")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Uri KeyId => KeyVaultCertificate.KeyId;

        [Category("Identifiers")]
        [DisplayName("Secret")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Uri SecretId => KeyVaultCertificate.SecretId;

        [Category("Policy")]
        [DisplayName("CertificatePolicy")]
        [ReadOnly(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public CertificatePolicy PolicyAttributes => CertificatePolicy;

        private ObservableLifetimeActionsCollection _lifetimeActions;
        [Category("Policy")]
        [DisplayName("Life time actions")]
        [Description("Actions that will be performed by Key Vault over the lifetime of a certificate.")]
        [TypeConverter(typeof(ExpandableCollectionObjectConverter))]
        public ObservableLifetimeActionsCollection LifetimeActions
        {
            get
            {
                return _lifetimeActions;
            }
            set
            {
                _lifetimeActions = value;
                if (null != CertificatePolicy)
                {
                    CertificatePolicy.LifetimeActions.Clear();
                    foreach (var lifetimeaction in LifetimeActionsToList())
                    {
                        CertificatePolicy.LifetimeActions.Add(lifetimeaction);
                    }
                }
            }
        }

        [Category("Policy")]
        [DisplayName("Issuer parameters")]
        [ReadOnly(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public string IssuerReference => CertificatePolicy.IssuerName;

        public PropertyObjectCertificate(KeyVaultCertificateWithPolicy keyVaultCertificate, CertificatePolicy policy, X509Certificate2 certificate, PropertyChangedEventHandler propertyChanged) :
            base(keyVaultCertificate.Id, keyVaultCertificate.Name, keyVaultCertificate.Properties.Tags, keyVaultCertificate.Properties.Enabled, keyVaultCertificate.Properties.ExpiresOn, keyVaultCertificate.Properties.NotBefore, propertyChanged)
        {
            KeyVaultCertificate = keyVaultCertificate;
            CertificatePolicy = policy;
            Certificate = certificate;
            _contentType = ContentType.Pkcs12;
            _value = certificate.Thumbprint.ToLowerInvariant();
            var olac = new ObservableLifetimeActionsCollection();
            if (null != CertificatePolicy?.LifetimeActions)
            {
                foreach (var la in CertificatePolicy.LifetimeActions)
                {
                    olac.Add(new LifetimeActionItem() { Type = la.Action, DaysBeforeExpiry = la.DaysBeforeExpiry, LifetimePercentage = la.LifetimePercentage });
                }
            }
            LifetimeActions = olac;
            LifetimeActions.SetPropertyChangedEventHandler(propertyChanged);
        }

        public override string GetKeyVaultFileExtension() => ContentType.KeyVaultCertificate.ToExtension();

        public override DataObject GetClipboardValue()
        {
            var dataObj = base.GetClipboardValue();
            dataObj.SetData(DataFormats.UnicodeText, Certificate.ToString());
            return dataObj;
        }

        public override void SaveToFile(string fullName)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullName));
            switch (ContentTypeUtils.FromExtension(Path.GetExtension(fullName)))
            {
                case ContentType.KeyVaultSecret:
                    throw new InvalidOperationException("One can't save key vault certificate as key vault secret");
                case ContentType.KeyVaultCertificate: // Serialize the entire secret as encrypted JSON for current user
                    File.WriteAllText(fullName, new KeyVaultCertificateFile(KeyVaultCertificate).Serialize());
                    break;
                case ContentType.KeyVaultLink:
                    File.WriteAllText(fullName, GetLinkAsInternetShortcut());
                    break;
                case ContentType.Certificate:
                    File.WriteAllBytes(fullName, Certificate.Export(X509ContentType.Cert));
                    break;
                case ContentType.Pkcs12:
                    string password = null;
                    var pwdDlg = new PasswordDialog();
                    pwdDlg.ShowDialog();
                    password = pwdDlg.Password;
                    File.WriteAllBytes(fullName, Certificate.Export(X509ContentType.Pkcs12, password));
                    break;
                default:                    
                    File.WriteAllText(fullName, Certificate.ToString());
                    break;
            }
        }

        protected override IEnumerable<TagItem> GetValueBasedCustomTags()
        {
            yield break;
        }

        public override void PopulateCustomTags() { }

        public override void AddOrUpdateSecretKind(SecretKind sk) { }

        public override void PopulateExpiration() { }

        public override string AreCustomTagsValid() => ""; // Return always valid

        private IList<LifetimeAction> LifetimeActionsToList() =>
            (from lai in LifetimeActions
             select new LifetimeAction(lai.Type)
             {
                 DaysBeforeExpiry = lai.DaysBeforeExpiry,
                 LifetimePercentage = lai.LifetimePercentage
             }
            ).ToList();
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
