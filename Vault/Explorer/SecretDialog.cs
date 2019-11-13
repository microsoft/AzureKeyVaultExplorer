// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 
using ScintillaNET;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
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
using Microsoft.Vault.Library;

namespace Microsoft.Vault.Explorer
{
    public partial class SecretDialog : ItemDialogBase<PropertyObjectSecret, SecretBundle>
    {
        private CertificateValueObject _certificateObj;
        private Scintilla uxTextBoxValue;

        private SecretDialog(ISession session, string title, ItemDialogBaseMode mode) : base(session, title, mode)
        {
            InitializeComponent();
            uxErrorProvider.SetIconAlignment(uxSplitContainer, ErrorIconAlignment.TopLeft);
            uxErrorProvider.SetIconAlignment(uxPropertyGridSecret, ErrorIconAlignment.TopLeft);
            uxErrorProvider.SetIconPadding(uxPropertyGridSecret, -16);

            SetUpTextBoxValue();
            List<string> unknownSk;
            List<SecretKind> secretKinds = LoadSecretKinds(_session.CurrentVaultAlias, out unknownSk);

            if (unknownSk.Count > 0)
            {
                MessageBox.Show(this,
                    $"Secret kinds '{string.Join(",", unknownSk)}' in vault alias '{_session.CurrentVaultAlias.Alias}' are being ignored because they are not found in {Settings.Default.SecretKindsJsonFileLocation}",
                    "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            uxMenuSecretKind.Items.AddRange(secretKinds.ToArray());
            uxSplitContainer_Panel1_SizeChanged(null, EventArgs.Empty);
            ActiveControl = uxTextBoxName;
        }

        /// <summary>
        /// New empty secret
        /// </summary>
        public SecretDialog(ISession session) : this(session, "New secret", ItemDialogBaseMode.New)
        {
            _changed = true;
            var s = new SecretBundle() { Attributes = new SecretAttributes(), ContentType = ContentTypeEnumConverter.GetDescription(ContentType.Text) };
            RefreshSecretObject(s);
            SecretKind defaultSK = TryGetDefaultSecretKind();
            int defaultIndex = uxMenuSecretKind.Items.IndexOf(defaultSK);
            uxMenuSecretKind.Items[defaultIndex].PerformClick();            
        }

        /// <summary>
        /// New secret from file
        /// </summary>
        public SecretDialog(ISession session, FileInfo fi) : this(session)
        {
            uxTextBoxName.Text = Utils.ConvertToValidSecretName(Path.GetFileNameWithoutExtension(fi.Name));
            PropertyObject.ContentType = ContentTypeUtils.FromExtension(fi.Extension);
            string password = null;
            switch (PropertyObject.ContentType)
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
                case ContentType.KeyVaultSecret:
                    var kvsf = Utils.LoadFromJsonFile<KeyVaultSecretFile>(fi.FullName);
                    SecretBundle s = kvsf.Deserialize();
                    uxPropertyGridSecret.SelectedObject = PropertyObject = new PropertyObjectSecret(s, SecretObject_PropertyChanged);
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

            PropertyObject.ContentType = hasExportablePrivateKey ? ContentType.Pkcs12 : ContentType.Certificate;
            uxTextBoxName.Text = Utils.ConvertToValidSecretName(certificate.GetNameInfo(X509NameType.SimpleName, false));
            string password = hasExportablePrivateKey ? Utils.NewSecurePassword() : null;
            byte[] data = hasExportablePrivateKey ? certificate.Export(X509ContentType.Pkcs12, password) : certificate.Export(X509ContentType.Cert);
            RefreshCertificate(new CertificateValueObject(Convert.ToBase64String(data), password));
            AutoDetectSecretKind();
        }

        /// <summary>
        /// Edit or Copy secret
        /// </summary>
        public SecretDialog(ISession session, string name, IEnumerable<SecretItem> versions) : this(session, "Edit secret", ItemDialogBaseMode.Edit)
        {
            Text += $" {name}";
            int i = 0;
            uxMenuVersions.Items.AddRange((from v in versions orderby v.Attributes.Created descending select new SecretVersion(i++, v)).ToArray());
            uxMenuVersions_ItemClicked(null, new ToolStripItemClickedEventArgs(uxMenuVersions.Items[0])); // Pass sender as NULL so _changed will be set to false
        }

        private static List<SecretKind> LoadSecretKinds(VaultAlias vaultAlias, out List<string> unknownSk)
        {
            SecretKinds allSecretKinds = Utils.LoadFromJsonFile<SecretKinds>(Settings.Default.SecretKindsJsonFileLocation);
            List<SecretKind> validatedSecretKinds = new List<SecretKind>(allSecretKinds.Count) ?? new List<SecretKind>(vaultAlias.SecretKinds.Length);
            unknownSk = new List<string>();

            // If there are no SecretKinds in the VaultAliases.json for a vault OR if it's a vault Not in VaultAliases, return ALL SecretKinds.
            if (vaultAlias.SecretKinds == null || (vaultAlias.SecretKinds.Length == 1 && (string)vaultAlias.SecretKinds.GetValue(0) == "Custom"))
            {
                foreach (var key in allSecretKinds.Keys)
                {
                    SecretKind sk;
                    allSecretKinds.TryGetValue(key, out sk);
                    validatedSecretKinds.Add(sk);
                }
            }
            // Otherwise, return just the specified SecretKinds
            else
            {
                foreach (var secretKind in vaultAlias.SecretKinds)
                {

                    SecretKind sk;
                    if (allSecretKinds.TryGetValue(secretKind, out sk))
                    {
                        validatedSecretKinds.Add(sk);
                    }
                    else
                    {
                        unknownSk.Add(secretKind);
                    }
                }
            }

            // Sort the Secret Kinds
            List<SecretKind> orderedValidatedSecretKinds = validatedSecretKinds.OrderBy(o => o.Alias).ToList();

            return orderedValidatedSecretKinds;
        }

        private void RefreshSecretObject(SecretBundle s)
        {
            PropertyObject = new PropertyObjectSecret(s, SecretObject_PropertyChanged);
            uxPropertyGridSecret.SelectedObject = PropertyObject;
            uxTextBoxName.Text = PropertyObject.Name;
            uxTextBoxValue.Text = PropertyObject.Value;

            // Handle Scintilla framework bug where text is not updated.
            if(uxTextBoxValue.Text != PropertyObject.Value)
            {
                // Remove and create new textbox with value
                uxSplitContainer.Panel1.Controls.Remove(uxTextBoxValue);
                SetUpTextBoxValue();
                uxTextBoxValue.Text = PropertyObject.Value;
            }

            ToggleCertificateMode(PropertyObject.ContentType.IsCertificate());
            uxTextBoxValue.Refresh();
        }

        private void SetUpTextBoxValue()
        {
            uxTextBoxValue = new Scintilla();
            uxSplitContainer.Panel1.Controls.Add(uxTextBoxValue);

            // basic config
            uxTextBoxValue.Dock = System.Windows.Forms.DockStyle.Fill;
            uxTextBoxValue.TextChanged += uxTextBoxValue_TextChanged;

            //initial view config
            uxTextBoxValue.WrapMode = WrapMode.None;
            uxTextBoxValue.IndentationGuides = IndentView.LookBoth;
        }

        private void AutoDetectSecretKind()
        {
            SecretKind defaultSecretKind = TryGetDefaultSecretKind(); // Default is the first one which is always Custom
            SecretKind autoDetectSecretKind = new SecretKind(defaultSecretKind.Alias); 
            TagItem currentSKTag = PropertyObject.Tags.GetOrNull(new TagItem(Consts.SecretKindKey, ""));
            bool shouldAddNew = true;

            // Read the CustomTags and determine the SecretKind
            foreach (SecretKind sk in uxMenuSecretKind.Items) // Auto detect 'last' secret kind based on the name only
            {

                if (currentSKTag == null)
                {
                    autoDetectSecretKind = defaultSecretKind;
                    shouldAddNew = false;
                    break;
                }

                // If the current Secret Kind is in the list of menu items,
                if (currentSKTag.Value == sk.Alias)
                {
                    autoDetectSecretKind = sk;
                    shouldAddNew = false;
                    break;
                }
            }
            if (shouldAddNew)
            {
                autoDetectSecretKind = new SecretKind(currentSKTag.Value);
                uxMenuSecretKind.Items.Add(autoDetectSecretKind);
            }

            // Apply last found secret kind, only when both Content Type and SecretKind are certificate or both not, otherwise fallback to Custom (the first one)
            if ((!PropertyObject.ContentType.IsCertificate() || !autoDetectSecretKind.IsCertificate) &&
                (PropertyObject.ContentType.IsCertificate() || autoDetectSecretKind.IsCertificate))
            {
                autoDetectSecretKind = TryGetDefaultSecretKind();
            }
            _certificateObj = PropertyObject.ContentType.IsCertificate() ? CertificateValueObject.FromValue(uxTextBoxValue.Text) : null;
            autoDetectSecretKind?.PerformClick();
        }

        private SecretKind TryGetDefaultSecretKind(string alias = "Custom")
        {
            foreach (SecretKind sk in uxMenuSecretKind.Items)
            {
                if (sk.Alias == alias)
                {
                    return sk;
                }
            }
            return (SecretKind)uxMenuSecretKind.Items[0];
        }

        private void ToggleCertificateMode(bool enable)
        {
            uxTextBoxValue.ReadOnly = enable;
            uxLinkLabelViewCertificate.Visible = enable;
        }

        private void RefreshCertificate(CertificateValueObject cvo)
        {
            _certificateObj = cvo;
            if (_certificateObj != null)
            {
                _certificateObj.FillTagsAndExpiration(PropertyObject);
                uxTextBoxValue.Text = _certificateObj.ToValue(PropertyObject.SecretKind.CertificateFormat);
                uxTextBoxValue.Refresh();
            }
            ToggleCertificateMode(_certificateObj != null);
        }

        private void uxTextBoxValue_TextChanged(object sender, EventArgs e)
        {
            _changed = true;
            PropertyObject.Value = uxTextBoxValue.Text;
            uxTimerValueTypingCompleted.Stop(); // Wait for user to finish the typing in a text box
            uxTimerValueTypingCompleted.Start();
        }

        private void SecretObject_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _changed = true;
            if (e.PropertyName == nameof(PropertyObject.ContentType)) // ContentType changed, refresh
            {
                AutoDetectSecretKind();
                RefreshCertificate(_certificateObj);
                uxTextBoxValue_TextChanged(sender, null);
            }

            string tagsExpirationError = PropertyObject.AreCustomTagsValid();
            if (false == PropertyObject.IsExpirationValid)
            {
                tagsExpirationError += $"Expiration values are invalid: 'Valid from time' must be less then 'Valid until time' and expiration period must be less or equal to {Utils.ExpirationToString(PropertyObject.SecretKind.MaxExpiration)}";
            }
            uxErrorProvider.SetError(uxPropertyGridSecret, string.IsNullOrEmpty(tagsExpirationError) ? null : tagsExpirationError);

            InvalidateOkButton();
        }

        private void uxTimerValueTypingCompleted_Tick(object sender, EventArgs e)
        {
            uxTimerValueTypingCompleted.Stop();
            bool valueValid = PropertyObject.IsValueValid;
            uxErrorProvider.SetError(uxSplitContainer, valueValid ? null : $"Secret value must match the following regex:\n{PropertyObject.SecretKind.ValueRegex}");

            int rawValueLength = PropertyObject.RawValue.Length;
            uxLabelBytesLeft.Text = $"{rawValueLength:N0} bytes / {Consts.MaxSecretValueLength - rawValueLength:N0} bytes left";
            if (valueValid) // Make sure that we are in the 25KB limit
            {
                valueValid = (rawValueLength >= 1) && (rawValueLength <= Consts.MaxSecretValueLength);
                uxErrorProvider.SetError(uxSplitContainer, valueValid ? null : $"Secret value length must be in the following range [1..{Consts.MaxSecretValueLength}]");
            }
            InvalidateOkButton();
        }

        protected override void uxMenuSecretKind_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            var sk = (SecretKind)e.ClickedItem;
            if (sk.Checked) return; // Same item was clicked
            foreach (var item in uxMenuSecretKind.Items) ((SecretKind)item).Checked = false;

            PropertyObject.AddOrUpdateSecretKind(sk);
            PropertyObject.SecretKind = sk;
            PropertyObject.PopulateCustomTags();
            // Populate default expiration and value template in case this is a new secret
            if (_mode == ItemDialogBaseMode.New)
            {
                PropertyObject.PopulateExpiration();
                uxTextBoxValue.Text = sk.ValueTemplate;
            }
            sk.Checked = true;
            uxLinkLabelSecretKind.Text = sk.ToString();
            uxToolTip.SetToolTip(uxLinkLabelSecretKind, sk.Description);
            RefreshCertificate(_certificateObj);
            uxTextBoxName_TextChanged(sender, null);
            uxTextBoxValue_TextChanged(sender, null);
            uxPropertyGridSecret.Refresh();
        }

        protected override async Task<SecretBundle> OnVersionChangeAsync(CustomVersion cv)
        {
            SecretVersion sv = (SecretVersion)cv;
            var s = await _session.CurrentVault.GetSecretAsync(sv.SecretItem.Identifier.Name, sv.SecretItem.Identifier.Version);            
            RefreshSecretObject(s);
            AutoDetectSecretKind();
            return s;
        }

        protected override ContextMenuStrip GetNewValueMenu() => uxMenuNewValue;

        private void uxMenuItemNewPassword_Click(object sender, EventArgs e)
        {
            if (uxTextBoxValue.ReadOnly) return;
            uxTextBoxValue.Text = Utils.NewSecurePassword();
            uxTextBoxValue.Refresh();
        }

        private void uxMenuItemNewGuid_Click(object sender, EventArgs e)
        {
            if (uxTextBoxValue.ReadOnly) return;
            uxTextBoxValue.Text = Guid.NewGuid().ToString("D");
            uxTextBoxValue.Refresh();
        }

        private void uxMenuItemNewApiKey_Click(object sender, EventArgs e)
        {
            if (uxTextBoxValue.ReadOnly) return;
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
