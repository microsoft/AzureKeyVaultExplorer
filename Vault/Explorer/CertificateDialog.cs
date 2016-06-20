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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Microsoft.PS.Common.Vault.Explorer
{
    public partial class CertificateDialog : ItemDialogBase<PropertyObjectCertificate, CertificateBundle>
    {
        private CertificatePolicy _certificatePolicy; // There is one policy and multiple versions of kv certificate. A policy is a recipe to create a next version of the kv certificate.

        private CertificateDialog(ISession session, string title, ItemDialogBaseMode mode) : base(session, title, mode)
        {
            InitializeComponent();
        }

        /// <summary>
        /// New certificate from file
        /// </summary>
        public CertificateDialog(ISession session, FileInfo fi) : this(session, "New certificate", ItemDialogBaseMode.New)
        {
            X509Certificate2 cert = null;
            switch (ContentTypeUtils.FromExtension(fi.Extension))
            {
                case ContentType.Certificate:
                    cert = new X509Certificate2(fi.FullName);
                    break;
                case ContentType.Pkcs12:
                    string password = null;
                    var pwdDlg = new PasswordDialog();
                    if (pwdDlg.ShowDialog() != DialogResult.OK)
                    {
                        DialogResult = DialogResult.Cancel;
                        return;
                    }
                    password = pwdDlg.Password;
                    cert = new X509Certificate2(fi.FullName, password, X509KeyStorageFlags.UserKeySet | X509KeyStorageFlags.Exportable);
                    break;
                case ContentType.KeyVaultCertificate:
                    var kvcf = Utils.LoadFromJsonFile<KeyVaultCertificateFile>(fi.FullName);
                    CertificateBundle s = kvcf.Deserialize();
                    break;
            }
            NewCertificate(cert);
        }

        /// <summary>
        /// New certificate from X509Certificate2
        /// </summary>
        public CertificateDialog(ISession session, X509Certificate2 cert) : this(session, "New certificate", ItemDialogBaseMode.New)
        {
            NewCertificate(cert);
        }

        /// <summary>
        /// Edit certificate
        /// </summary>
        public CertificateDialog(ISession session, string name, IEnumerable<ListCertificateResponseMessage> versions) : this(session, $"Edit certificate {name}", ItemDialogBaseMode.Edit)
        {
            uxTextBoxName.ReadOnly = true;
            int i = 0;
            uxMenuVersions.Items.AddRange((from v in versions orderby v.Attributes.Created descending select new CertificateVersion(i++, v)).ToArray());
            uxMenuVersions_ItemClicked(null, new ToolStripItemClickedEventArgs(uxMenuVersions.Items[0])); // Pass sender as NULL so _changed will be set to false
        }

        private void NewCertificate(X509Certificate2 cert)
        {
            _certificatePolicy = new CertificatePolicy()
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
            RefreshCertificateObject(new CertificateBundle(), _certificatePolicy, cert);
            uxTextBoxName.Text = Utils.ConvertToValidSecretName(cert.GetNameInfo(X509NameType.SimpleName, false));
        }

        private void RefreshCertificateObject(CertificateBundle cb, CertificatePolicy cp, X509Certificate2 certificate)
        {
            uxPropertyGridSecret.SelectedObject = PropertyObject = new PropertyObjectCertificate(cb, cp, certificate, SecretObject_PropertyChanged);
            uxTextBoxName.Text = PropertyObject.Name;
            uxToolTip.SetToolTip(uxLinkLabelSecretKind, PropertyObject.SecretKind.Description);
        }

        protected override void uxTextBoxName_TextChanged(object sender, EventArgs e)
        {
            PropertyObject.Name = uxTextBoxName.Text;
            uxErrorProvider.SetError(uxTextBoxName, PropertyObject.IsNameValid ? null : $"Certificate name must match the following regex:\n{Consts.ValidSecretNameRegex}");
            base.uxTextBoxName_TextChanged(sender, e);
            InvalidateOkButton();
        }

        private void SecretObject_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _changed = true;
            InvalidateOkButton();
        }

        protected override async Task<CertificateBundle> OnVersionChangeAsync(CustomVersion cv)
        {
            var cb = await _session.CurrentVault.GetCertificateAsync(cv.Id.Name, (cv.Index == 0) ? null : cv.Id.Version); // Pass NULL as a version to fetch current CertificatePolicy
            var cert = await _session.CurrentVault.GetCertificateWithPrivateKeyAsync(cv.Id.Name, cv.Id.Version);
            if ((_certificatePolicy == null) && (cb.Policy != null)) // cb.Policy will be NULL when version is not current
            {
                _certificatePolicy = cb.Policy;
            }
            RefreshCertificateObject(cb, _certificatePolicy, cert);
            return cb;
        }
    }
}
