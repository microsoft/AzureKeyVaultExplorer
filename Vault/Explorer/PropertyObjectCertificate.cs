﻿using Microsoft.Azure.KeyVault;
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
        [Editor(typeof(CertificateUIEditor), typeof(UITypeEditor))]
        public X509Certificate2 Certificate { get; }

        [Category("General")]
        [DisplayName("Thumbprint")]
        public string Thumbprint => Certificate.Thumbprint;

        [Category("Identifiers")]
        [DisplayName("Certificate")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public CertificateIdentifier Id => CertificateBundle.Id;

        [Category("Identifiers")]
        [DisplayName("Key")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public KeyIdentifier KeyId => CertificateBundle.KeyId;

        [Category("Identifiers")]
        [DisplayName("Secret")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public SecretIdentifier SecretId => CertificateBundle.SecretId;

        [Category("Policy")]
        [DisplayName("Attributes")]
        [ReadOnly(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public CertificatePolicyAttributes PolicyAttributes => CertificatePolicy.Attributes;

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
                CertificatePolicy.LifetimeActions = LifetimeActionsToEnumerable();
            }
        }

        [Category("Policy")]
        [DisplayName("Issuer reference")]
        [ReadOnly(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public IssuerReference IssuerReference => CertificatePolicy.IssuerReference;

        [Category("Other")]
        [DisplayName("Pending Reference")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public PendingReference PendingReference => CertificateBundle.PendingReference;

        public PropertyObjectCertificate(CertificateBundle certificateBundle, CertificatePolicy policy, X509Certificate2 certificate, PropertyChangedEventHandler propertyChanged) :
            base(certificateBundle.Id, certificateBundle.Tags, certificateBundle.Attributes.Enabled, certificateBundle.Attributes.Expires, certificateBundle.Attributes.NotBefore, propertyChanged)
        {
            CertificateBundle = certificateBundle;
            CertificatePolicy = policy;
            Certificate = certificate;
            _contentType = ContentType.Pkcs12;
            _value = certificate.Thumbprint;
            var olac = new ObservableLifetimeActionsCollection();
            if (null != CertificatePolicy?.LifetimeActions)
            {
                foreach (var la in CertificatePolicy.LifetimeActions)
                {
                    olac.Add(new LifetimeActionItem() { Type = LifetimeActionTypeEnumConverter.GetValue(la.Action.Type), DaysBeforeExpiry = la.Trigger.DaysBeforeExpiry, LifetimePercentage = la.Trigger.LifetimePercentage });
                }
            }
            LifetimeActions = olac;
            LifetimeActions.SetPropertyChangedEventHandler(propertyChanged);
        }

        public CertificateAttributes ToCertificateAttributes() => new CertificateAttributes() { Enabled = Enabled, Expires = Expires, NotBefore = NotBefore };

        protected override IEnumerable<KeyValuePair<string, string>> GetCustomTags()
        {
            yield break;
        }

        private IEnumerable<LifetimeAction> LifetimeActionsToEnumerable() =>
            from lai in LifetimeActions select new LifetimeAction() { Action = new Azure.KeyVault.Action() { Type = lai.Type.ToString() }, Trigger = new Trigger() { DaysBeforeExpiry = lai.DaysBeforeExpiry, LifetimePercentage = lai.LifetimePercentage } };
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