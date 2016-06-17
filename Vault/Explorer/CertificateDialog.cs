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
        public CertificateDialog(ISession session, CertificateBundle cb, X509Certificate2 certificate, IEnumerable<ListCertificateResponseMessage> versions) : this(session, $"Edit certificate {cb.Id.Name}", Mode.EditCertificate)
        {
            uxTextBoxName.ReadOnly = true;
            int i = 0;
            uxMenuVersions.Items.AddRange((from v in versions orderby v.Attributes.Created descending select new CertificateVersion(i++, v)).ToArray());
            uxMenuVersions_ItemClicked(null, new ToolStripItemClickedEventArgs(uxMenuVersions.Items[0])); // Pass sender as NULL so _changed will be set to false
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
            uxTextBoxName.Text = CertificateObject.Name;
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

        private async void uxMenuVersions_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            var cv = (CertificateVersion)e.ClickedItem;
            if (cv.Checked) return; // Same item was clicked
            foreach (var item in uxMenuVersions.Items) ((CertificateVersion)item).Checked = false;

            var cb = await _session.CurrentVault.GetCertificateAsync(cv.Id.Name, (cv.Index == 0) ? null : cv.Id.Version);
            var cert = await _session.CurrentVault.GetCertificateWithPrivateKeyAsync(cv.Id.Name, cv.Id.Version);

            cv.Checked = true;
            uxLinkLabelValue.Text = cv.ToString();
            uxToolTip.SetToolTip(uxLinkLabelValue, cv.ToolTipText);
            RefreshCertificateObject(cb, cb.Policy ?? new CertificatePolicy(), cert); // CertificatePolicy in some cases NULL ??? WHY!!!
            _changed = (sender != null); // Sender will be NULL for the first time during Edit Dialog ctor
            InvalidateOkButton();
        }
    }
}
