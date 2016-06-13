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
    public partial class SecretDialog : FormTelemetry
    {
        private enum Mode
        {
            NewSecret,
            EditSecret
        };

        private readonly ISession _session;
        private readonly Mode _mode;
        private bool _changed;
        private CertificateValueObject _certificateObj;
        private readonly TextEditorControl uxTextBoxValue;
        public PropertyObject SecretObject { private set; get; }

        private SecretDialog(ISession session, string title, Mode mode)
        {
            InitializeComponent();
            _session = session;
            Text = title;
            _mode = mode;
            uxTextBoxName.Font = Settings.Default.SecretFont;
            uxTextBoxValue = new TextEditorControl()
            {
                Parent = uxSplitContainer.Panel1,
                BorderStyle = BorderStyle.FixedSingle,
                Dock = DockStyle.Fill,
                ShowMatchingBracket = true,
                ConvertTabsToSpaces = Settings.Default.ConvertTabsToSpaces,
                TabIndent = Settings.Default.TabIndent,
                VRulerRow = 120,
                TabIndex = 4,
                AllowDrop = false,
                Font = Settings.Default.SecretFont,
                ShowLineNumbers = Settings.Default.ShowLineNumbers,
                ContextMenuStrip = uxMenuNewValue
            };
            uxTextBoxValue.TextChanged += uxTextBoxValue_TextChanged;
            var sk = Utils.LoadFromJsonFile<SecretKinds>(Settings.Default.SecretKindsJsonFileLocation);
            uxMenuSecretKind.Items.AddRange((from name in _session.CurrentVaultAlias.SecretKinds select sk[name]).ToArray());
            uxSplitContainer_Panel1_SizeChanged(null, EventArgs.Empty);
        }

        /// <summary>
        /// New empty secret
        /// </summary>
        public SecretDialog(ISession session) : this(session, "New secret", Mode.NewSecret)
        {
            _changed = true;
            var s = new Secret() { Attributes = new SecretAttributes(), ContentType = ContentTypeEnumConverter.GetDescription(ContentType.Text) };
            RefreshSecretObject(s);
            uxMenuSecretKind.Items[0].PerformClick();
        }

        /// <summary>
        /// New secret from file
        /// </summary>
        public SecretDialog(ISession session, FileInfo fi) : this(session)
        {
            uxTextBoxName.Text = Utils.ConvertToValidSecretName(Path.GetFileNameWithoutExtension(fi.Name));

            SecretObject.ContentType = ContentTypeUtils.FromExtension(fi.Extension);
            string password = null;
            switch (SecretObject.ContentType)
            {
                case ContentType.Certificate:
                    break;
                case ContentType.Pkcs12:
                case ContentType.Pkcs12Base64:
                    var pwdDlg = new PasswordDialog();
                    if (pwdDlg.ShowDialog() != DialogResult.OK)
                    {
                        DialogResult = DialogResult.Cancel;
                        return;
                    }
                    password = pwdDlg.Password;
                    break;
                case ContentType.Secret:
                    SecretFile sf = Utils.LoadFromJsonFile<SecretFile>(fi.FullName);
                    Secret s = sf.Deserialize();
                    uxPropertyGridSecret.SelectedObject = SecretObject = new PropertyObjectSecret(s, SecretObject_PropertyChanged);
                    uxTextBoxName.Text = s.SecretIdentifier?.Name;
                    uxTextBoxValue.Text = s.Value;
                    return;
                default:
                    uxTextBoxValue.Text = File.ReadAllText(fi.FullName);
                    return;
            }
            // Certificate flow
            RefreshCertificate(new CertificateValueObject(fi, password));
            AutoDetectSecretKind();
        }

        /// <summary>
        /// New secret from certificate
        /// </summary>
        public SecretDialog(ISession session, X509Certificate2 certificate) : this(session)
        {
            bool hasExportablePrivateKey = certificate.HasPrivateKey && (
                ((certificate.PrivateKey as RSACryptoServiceProvider)?.CspKeyContainerInfo.Exportable ?? false) ||
                ((certificate.PrivateKey as DSACryptoServiceProvider)?.CspKeyContainerInfo.Exportable ?? false));

            SecretObject.ContentType = hasExportablePrivateKey ? ContentType.Pkcs12 : ContentType.Certificate;
            uxTextBoxName.Text = Utils.ConvertToValidSecretName(certificate.GetNameInfo(X509NameType.SimpleName, false));
            string password = hasExportablePrivateKey ? Utils.NewSecurePassword() : null;
            byte[] data = hasExportablePrivateKey ? certificate.Export(X509ContentType.Pkcs12, password) : certificate.Export(X509ContentType.Cert);
            RefreshCertificate(new CertificateValueObject(Convert.ToBase64String(data), password));
            AutoDetectSecretKind();
        }

        /// <summary>
        /// Edit or Copy secret
        /// </summary>
        public SecretDialog(ISession session, Secret s, IEnumerable<SecretItem> versions) : this(session, "Edit secret", Mode.EditSecret)
        {
            Text += $" {s.SecretIdentifier.Name}";
            int i = 0;
            uxMenuVersions.Items.AddRange((from v in versions orderby v.Attributes.Created descending select new SecretVersion(i++, v)).ToArray());
            uxMenuVersions_ItemClicked(null, new ToolStripItemClickedEventArgs(uxMenuVersions.Items[0])); // Pass sender as NULL so _changed will be set to false
        }

        public SecretDialog(ISession session, CertificateBundle cb) : this(session, "Edit certificate", Mode.EditSecret)
        {
            Text += $" {cb.Id.Name}";
            uxPropertyGridSecret.SelectedObject = SecretObject = new PropertyObjectCertificate(cb, SecretObject_PropertyChanged);
        }

        private void RefreshSecretObject(Secret s)
        {
            SecretObject = new PropertyObjectSecret(s, SecretObject_PropertyChanged);
            uxPropertyGridSecret.SelectedObject = SecretObject;
            uxTextBoxValue.SetHighlighting(SecretObject.ContentType.ToSyntaxHighlightingMode());
            uxTextBoxName.Text = SecretObject.Name;
            ToggleCertificateMode(SecretObject.ContentType.IsCertificate());
            uxTextBoxValue.Text = SecretObject.Value;
            uxTextBoxValue.Refresh();
        }

        private void AutoDetectSecretKind()
        {
            SecretKind autoDetectSecretKind = (SecretKind)uxMenuSecretKind.Items[0]; // Default is the first one which is always Custom
            foreach (var item in uxMenuSecretKind.Items) // Auto detect 'last' secret kind based on the name only
            {
                SecretKind sk = (SecretKind)item;
                autoDetectSecretKind = sk.NameRegex.IsMatch(uxTextBoxName.Text) ? sk : autoDetectSecretKind;
            }
            // Apply last found secret kind, only when both Content Type and SecretKind are certificate or both not, otherwise fallback to Custom (the first one)
            if ((!SecretObject.ContentType.IsCertificate() || !autoDetectSecretKind.IsCertificate) &&
                (SecretObject.ContentType.IsCertificate() || autoDetectSecretKind.IsCertificate))
            {
                autoDetectSecretKind = (SecretKind)uxMenuSecretKind.Items[0];
            }
            _certificateObj = (SecretObject.ContentType.IsCertificate() && autoDetectSecretKind.IsCertificate) ? CertificateValueObject.FromValue(uxTextBoxValue.Text) : null;
            autoDetectSecretKind?.PerformClick();
        }

        private void ToggleCertificateMode(bool enable)
        {
            uxTextBoxValue.IsReadOnly = enable;
            uxLinkLabelViewCertificate.Visible = enable;
        }

        private void RefreshCertificate(CertificateValueObject cvo)
        {
            _certificateObj = cvo;
            if (_certificateObj != null)
            {
                _certificateObj.FillTags(SecretObject.Tags);
                uxTextBoxValue.Text = _certificateObj.ToValue(SecretObject.SecretKind.CertificateFormat);
                uxTextBoxValue.Refresh();
            }
            ToggleCertificateMode(_certificateObj != null);
        }

        private void uxTextBoxName_TextChanged(object sender, EventArgs e)
        {
            _changed = true;
            SecretObject.Name = uxTextBoxName.Text;
            uxErrorProvider.SetError(uxTextBoxName, SecretObject.IsNameValid ? null : $"Secret name must match the following regex:\n{SecretObject.SecretKind.NameRegex}");
            InvalidateOkButton();
        }

        private void uxTextBoxValue_TextChanged(object sender, EventArgs e)
        {
            _changed = true;
            SecretObject.Value = uxTextBoxValue.Text;
            uxTimerValueTypingCompleted.Stop(); // Wait for user to finish the typing in a text box
            uxTimerValueTypingCompleted.Start();
        }

        private void SecretObject_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _changed = true;
            if (e.PropertyName == nameof(SecretObject.ContentType)) // ContentType changed, refresh
            {
                AutoDetectSecretKind();
                RefreshCertificate(_certificateObj);
                uxTextBoxValue_TextChanged(sender, null);
                uxTextBoxValue.SetHighlighting(SecretObject.ContentType.ToSyntaxHighlightingMode());
            }
            InvalidateOkButton();
        }

        private void InvalidateOkButton()
        {
            uxButtonOK.Enabled = _changed && SecretObject.IsNameValid && SecretObject.IsValueValid;
        }

        private void uxTimerValueTypingCompleted_Tick(object sender, EventArgs e)
        {
            uxTimerValueTypingCompleted.Stop();

            bool valueValid = SecretObject.IsValueValid;
            uxErrorProvider.SetError(uxSplitContainer, valueValid ? null : $"Secret value must match the following regex:\n{SecretObject.SecretKind.ValueRegex}");

            int rawValueLength = SecretObject.RawValue.Length;
            uxLabelBytesLeft.Text = $"{rawValueLength:N0} bytes / {Consts.MaxSecretValueLength - rawValueLength:N0} bytes left";
            if (valueValid) // Make sure that we are in the 25KB limit
            {
                valueValid = (rawValueLength >= 1) && (rawValueLength <= Consts.MaxSecretValueLength);
                uxErrorProvider.SetError(uxSplitContainer, valueValid ? null : $"Secret value length must be in the following range [1..{Consts.MaxSecretValueLength}]");
            }
            InvalidateOkButton();
        }

        private void uxLinkLabelSecretKind_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            uxMenuSecretKind.Show(uxLinkLabelSecretKind, 0, uxLinkLabelSecretKind.Height);
        }

        private void uxMenuSecretKind_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            var sk = (SecretKind)e.ClickedItem;
            if (sk.Checked) return; // Same item was clicked
            foreach (var item in uxMenuSecretKind.Items) ((SecretKind)item).Checked = false;
            SecretObject.SecretKind = sk;
            sk.Checked = true;
            uxLinkLabelSecretKind.Text = sk.ToString();
            uxToolTip.SetToolTip(uxLinkLabelSecretKind, sk.Description);
            RefreshCertificate(_certificateObj);
            uxTextBoxName_TextChanged(sender, null);
            uxTextBoxValue_TextChanged(sender, null);
        }

        private void uxLinkLabelValue_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            switch (_mode)
            {
                case Mode.NewSecret:
                    uxMenuNewValue.Show(uxLinkLabelValue, 0, uxLinkLabelValue.Height);
                    return;
                case Mode.EditSecret:
                    uxMenuVersions.Show(uxLinkLabelValue, 0, uxLinkLabelValue.Height);
                    return;
            }
        }

        private async void uxMenuVersions_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
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
        }

        private void uxMenuItemNewPassword_Click(object sender, EventArgs e)
        {
            if (uxTextBoxValue.IsReadOnly) return;
            uxTextBoxValue.Text = Utils.NewSecurePassword();
            uxTextBoxValue.Refresh();
        }

        private void uxMenuItemNewGuid_Click(object sender, EventArgs e)
        {
            if (uxTextBoxValue.IsReadOnly) return;
            uxTextBoxValue.Text = Guid.NewGuid().ToString("D");
            uxTextBoxValue.Refresh();
        }

        private void uxMenuItemNewApiKey_Click(object sender, EventArgs e)
        {
            if (uxTextBoxValue.IsReadOnly) return;
            uxTextBoxValue.Text = Utils.NewApiKey();
            uxTextBoxValue.Refresh();
        }

        private void uxSplitContainer_Panel1_SizeChanged(object sender, EventArgs e)
        {
            uxLinkLabelViewCertificate.Left = (uxSplitContainer.Panel1.Width - uxLinkLabelViewCertificate.Width) / 2;
            uxLinkLabelViewCertificate.Top = (uxSplitContainer.Panel1.Height - uxLinkLabelViewCertificate.Height) / 2;
        }

        private void uxLinkLabelViewCertificate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            X509Certificate2UI.DisplayCertificate(_certificateObj.Certificate, Handle);
        }
    }
}
