// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 

using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
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
        public readonly CertificateBundle CertificateBundle;
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
        public CertificateIdentifier Id => CertificateBundle.CertificateIdentifier;

        [Category("Identifiers")]
        [DisplayName("Key")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public KeyIdentifier KeyId => CertificateBundle.KeyIdentifier;

        [Category("Identifiers")]
        [DisplayName("Secret")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public SecretIdentifier SecretId => CertificateBundle.SecretIdentifier;

        [Category("Policy")]
        [DisplayName("Attributes")]
        [ReadOnly(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public CertificateAttributes PolicyAttributes => CertificatePolicy.Attributes;

        [Category("Policy")]
        [DisplayName("Certificate properties")]
        [ReadOnly(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public X509CertificateProperties X509CertificateProperties => CertificatePolicy.X509CertificateProperties;

        [Category("Policy")]
        [DisplayName("Key properties")]
        [ReadOnly(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public KeyProperties KeyProperties => CertificatePolicy.KeyProperties;

        [Category("Policy")]
        [DisplayName("Secret properties")]
        [ReadOnly(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public SecretProperties SecretProperties => CertificatePolicy.SecretProperties;

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
                    CertificatePolicy.LifetimeActions = LifetimeActionsToList();
                }
            }
        }

        [Category("Policy")]
        [DisplayName("Issuer parameters")]
        [ReadOnly(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public IssuerParameters IssuerReference => CertificatePolicy.IssuerParameters;

        public PropertyObjectCertificate(CertificateBundle certificateBundle, CertificatePolicy policy, X509Certificate2 certificate, PropertyChangedEventHandler propertyChanged) :
            base(certificateBundle.CertificateIdentifier, certificateBundle.Tags, certificateBundle.Attributes.Enabled, certificateBundle.Attributes.Expires, certificateBundle.Attributes.NotBefore, propertyChanged)
        {
            CertificateBundle = certificateBundle;
            CertificatePolicy = policy;
            Certificate = certificate;
            _contentType = ContentType.Pkcs12;
            _value = certificate.Thumbprint.ToLowerInvariant();
            var olac = new ObservableLifetimeActionsCollection();
            if (null != CertificatePolicy?.LifetimeActions)
            {
                foreach (var la in CertificatePolicy.LifetimeActions)
                {
                    olac.Add(new LifetimeActionItem() { Type = la.Action.ActionType, DaysBeforeExpiry = la.Trigger.DaysBeforeExpiry, LifetimePercentage = la.Trigger.LifetimePercentage });
                }
            }
            LifetimeActions = olac;
            LifetimeActions.SetPropertyChangedEventHandler(propertyChanged);
        }

        public CertificateAttributes ToCertificateAttributes() => new CertificateAttributes() { Enabled = Enabled, Expires = Expires, NotBefore = NotBefore };

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
                    File.WriteAllText(fullName, new KeyVaultCertificateFile(CertificateBundle).Serialize());
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
            (from lai in LifetimeActions select new LifetimeAction(new Trigger(lai.LifetimePercentage, lai.DaysBeforeExpiry), new Microsoft.Azure.KeyVault.Models.Action(lai.Type))).ToList();
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
