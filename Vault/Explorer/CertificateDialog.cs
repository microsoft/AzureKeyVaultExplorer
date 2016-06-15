using ICSharpCode.TextEditor;
using Microsoft.Azure.KeyVault;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Forms;

namespace Microsoft.PS.Common.Vault.Explorer
{
    public partial class CertificateDialog : FormTelemetry
    {
        private enum Mode
        {
            NewCertificate,
            EditCertificate
        };

        private readonly ISession _session;
        private readonly Mode _mode;
        private bool _changed;
        public PropertyObjectCertificate CertificateObject { private set; get; }

        private CertificateDialog(ISession session, string title, Mode mode)
        {
            InitializeComponent();
            _session = session;
            Text = title;
            _mode = mode;
            uxTextBoxName.Font = Settings.Default.SecretFont;
        }

        /// <summary>
        /// New certificate from file
        /// </summary>
        public CertificateDialog(ISession session, FileInfo fi) : this(session, "New certificate", Mode.NewCertificate)
        {
            string password = null;
            var pwdDlg = new PasswordDialog();
            if (pwdDlg.ShowDialog() != DialogResult.OK)
            {
                DialogResult = DialogResult.Cancel;
                return;
            }
            password = pwdDlg.Password;
            var cert = new X509Certificate2(fi.FullName, password, X509KeyStorageFlags.UserKeySet | X509KeyStorageFlags.Exportable);
            NewCertificate(cert);
        }

        /// <summary>
        /// New certificate from X509Certificate2
        /// </summary>
        public CertificateDialog(ISession session, X509Certificate2 cert) : this(session, "New certificate", Mode.NewCertificate)
        {
            NewCertificate(cert);
        }

        /// <summary>
        /// Edit certificate
        /// </summary>
        public CertificateDialog(ISession session, CertificateBundle cb, X509Certificate2 certificate) : this(session, $"Edit certificate {cb.Id.Name}", Mode.EditCertificate)
        {
            RefreshCertificateObject(cb, cb.Policy, certificate);
            uxTextBoxName.Text = cb.Id.Name;
            uxTextBoxName.ReadOnly = true;
        }

        private void NewCertificate(X509Certificate2 cert)
        {
            var cp = new CertificatePolicy()
            {
                KeyProperties = new KeyProperties()
                {
                    Exportable = true,
                    KeySize = 2048,
                    Kty = "RSA",
                    ReuseKey = false
                },
                SecretProperties = new SecretProperties()
                {
                    ContentType = CertificateContentType.Pfx
                }
            };
            RefreshCertificateObject(new CertificateBundle(), cp, cert);
            uxTextBoxName.Text = Utils.ConvertToValidSecretName(cert.GetNameInfo(X509NameType.SimpleName, false));
        }

        private void RefreshCertificateObject(CertificateBundle cb, CertificatePolicy cp, X509Certificate2 certificate)
        {
            uxPropertyGridSecret.SelectedObject = CertificateObject = new PropertyObjectCertificate(cb, cp, certificate, SecretObject_PropertyChanged);
        }

        private void uxTextBoxName_TextChanged(object sender, EventArgs e)
        {
            _changed = true;
            CertificateObject.Name = uxTextBoxName.Text;
            uxErrorProvider.SetError(uxTextBoxName, CertificateObject.IsNameValid ? null : $"Certificate name must match the following regex:\n{Consts.ValidSecretNameRegex}");
            InvalidateOkButton();
        }

        private void SecretObject_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _changed = true;
            InvalidateOkButton();
        }

        private void InvalidateOkButton()
        {
            uxButtonOK.Enabled = _changed && CertificateObject.IsNameValid && CertificateObject.IsValueValid;
        }

        private void uxLinkLabelValue_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            switch (_mode)
            {
                case Mode.NewCertificate:
                    return;
                case Mode.EditCertificate:
                    uxMenuVersions.Show(uxLinkLabelValue, 0, uxLinkLabelValue.Height);
                    return;
            }
        }

        private void uxMenuVersions_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            /*
            var sv = (SecretVersion)e.ClickedItem;
            if (sv.Checked) return; // Same item was clicked
            foreach (var item in uxMenuVersions.Items) ((SecretVersion)item).Checked = false;
            var s = await _session.CurrentVault.GetSecretAsync(sv.SecretItem.Identifier.Name, sv.SecretItem.Identifier.Version);
            sv.Checked = true;
            uxLinkLabelValue.Text = sv.ToString();
            uxToolTip.SetToolTip(uxLinkLabelValue, sv.ToolTipText);
            RefreshSecretObject(s);
            AutoDetectSecretKind();
            _changed = (sender != null); // Sender will be NULL for the first time during Edit Dialog ctor
            InvalidateOkButton();
            */
        }
    }
}
