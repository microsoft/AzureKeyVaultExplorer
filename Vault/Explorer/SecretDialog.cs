using Microsoft.Azure.KeyVault;
using System;
using System.ComponentModel;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;
using ICSharpCode.TextEditor;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.PS.Common.Vault.Explorer
{
    public partial class SecretDialog : Form
    {
        private bool _nameValid;
        private bool _valueValid;
        private bool _changed;
        private SecretKind _currentSecretKind;
        private readonly TextEditorControl uxTextBoxValue;
        public readonly SecretObject SecretObject;      

        private SecretDialog(string[] secretKinds, Secret s, string title)
        {
            InitializeComponent();
            Text = title;
            uxPropertyGridSecret.SelectedObject = SecretObject = new SecretObject(s, SecretObject_PropertyChanged);
            uxTextBoxValue = new TextEditorControl()
            {
                Parent = uxSplitContainer.Panel1,
                BorderStyle = BorderStyle.FixedSingle,
                Dock = DockStyle.Fill,
                ShowMatchingBracket = true,
                ConvertTabsToSpaces = true,
                VRulerRow = 120,
                TabIndex = 4
            };
            uxTextBoxValue.TextChanged += uxTextBoxValue_TextChanged;
            uxTextBoxValue.SetHighlighting(SecretObject.ContentType.ToSyntaxHighlightingMode());

            var sk = Utils.LoadFromJsonFile<SecretKinds>("SecretKinds.json");
            foreach (var name in secretKinds)
            {
                uxMenuSecretKind.Items.Add(sk[name]);
            }
            uxMenuSecretKind.Items[0].PerformClick();
            uxTextBoxName.Text = "";
            uxTextBoxValue.Text = "";
        }

        /// <summary>
        /// New empty secret
        /// </summary>
        public SecretDialog(string[] secretKinds) : this(secretKinds, new Secret() { Attributes = new SecretAttributes(), ContentType = ContentTypeEnumConverter.GetDescription(ContentType.Text) }, "New Secret")
        {
            _changed = true;
        }

        /// <summary>
        /// Edit secret
        /// </summary>
        public SecretDialog(string[] secretKinds, Secret s) : this(secretKinds, s, "Edit secret")
        {
            uxTextBoxName.Text = s.SecretIdentifier.Name;
            uxTextBoxValue.Text = SecretObject.Value;
            uxTextBoxValue.IsReadOnly = SecretObject.ContentType.IsCertificate();
            SecretKind autoDetectSecretKind = null;
            foreach (var item in uxMenuSecretKind.Items) // Auto detect 'last' secret kind based on the name
            {
                SecretKind sk = (SecretKind)item;
                autoDetectSecretKind = sk.NameRegex.IsMatch(uxTextBoxName.Text) ? sk : autoDetectSecretKind;
            }
            autoDetectSecretKind?.PerformClick();
            _changed = false;
            InvalidateOkButton();
        }

        /// <summary>
        /// New secret from file
        /// </summary>
        public SecretDialog(string[] secretKinds, FileInfo fi) : this(secretKinds)
        {
            uxTextBoxName.Text = Path.GetFileNameWithoutExtension(fi.Name);

            string extension = fi.Extension?.ToLowerInvariant();
            SecretObject.ContentType = ContentTypeUtils.FromExtension(extension);
            string password = null;
            switch (SecretObject.ContentType)
            {
                case ContentType.Certificate:
                    break;
                case ContentType.Pkcs12:
                case ContentType.Pkcs12Base64:
                    var pwdDlg = new PasswordDialog();
                    if (pwdDlg.ShowDialog() != DialogResult.OK) return;
                    password = pwdDlg.Password;
                    break;
                default:
                    uxTextBoxValue.Text = File.ReadAllText(fi.FullName);
                    return;
            }
            // Certificate flow
            var cvo = new CertificateValueObject(fi, password);
            cvo.FillTags(SecretObject.Tags);
            uxTextBoxValue.Text = cvo.ToString();
            uxTextBoxValue.IsReadOnly = true;
        }

        private void uxTextBoxName_TextChanged(object sender, EventArgs e)
        {
            _changed = true;
            _nameValid = _currentSecretKind.NameRegex.IsMatch(uxTextBoxName.Text);
            uxErrorProvider.SetError(uxTextBoxName, _nameValid ? null : $"Secret name must match the following regex {_currentSecretKind.NameRegex}");
            SecretObject.Name = uxTextBoxName.Text;
            InvalidateOkButton();
        }

        private void uxTextBoxValue_TextChanged(object sender, EventArgs e)
        {
            _changed = true;
            uxTimerValueTypingCompleted.Stop(); // Wait for user to finish the typing in a text box
            uxTimerValueTypingCompleted.Start();
        }

        private void SecretObject_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _changed = true;
            if (e.PropertyName == nameof(SecretObject.ContentType)) // ContentType changed, refresh
            {
                if ((SecretObject.ContentType == ContentType.Pkcs12Base64) && 
                    Consts.ValidBase64Regex.IsMatch(SecretObject.Value)) // Allow first conversion from none to Pkcs12Base64 content type
                {
                    var cvo = CertificateValueObject.FromJson(Encoding.UTF8.GetString(Convert.FromBase64String(SecretObject.Value)));
                    cvo.FillTags(SecretObject.Tags);
                    uxTextBoxValue.Text = cvo.ToString();
                }
                uxTextBoxValue.IsReadOnly = false;
                uxTextBoxValue_TextChanged(sender, null);
                uxTextBoxValue.SetHighlighting(SecretObject.ContentType.ToSyntaxHighlightingMode());
            }
            InvalidateOkButton();
        }

        private void InvalidateOkButton()
        {
            uxButtonOK.Enabled = _changed && _nameValid && _valueValid;
        }

        private void uxTimerValueTypingCompleted_Tick(object sender, EventArgs e)
        {
            uxTimerValueTypingCompleted.Stop();
            _valueValid = _currentSecretKind.ValueRegex.IsMatch(uxTextBoxValue.Text);
            uxErrorProvider.SetError(uxSplitContainer, _valueValid ? null : $"Secret value must match the following regex {_currentSecretKind.ValueRegex}");

            SecretObject.Value = uxTextBoxValue.Text;
            int rawValueLength = SecretObject.RawValue.Length;

            uxLabelBytesLeft.Text = $"{rawValueLength:N0} bytes / {Consts.MaxSecretValueLength - rawValueLength:N0} bytes left";
            if (_valueValid) // Make sure that we are in the 25KB limit
            {
                _valueValid = (rawValueLength >= 1) && (rawValueLength <= Consts.MaxSecretValueLength);
                uxErrorProvider.SetError(uxSplitContainer, _valueValid ? null : $"Secret value length must be in the following range [1..{Consts.MaxSecretValueLength}]");
            }
            InvalidateOkButton();
        }

        private void uxLinkLabelSecretKind_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            uxMenuSecretKind.Show(uxLinkLabelSecretKind, 0, uxLinkLabelSecretKind.Height);
        }

        private void uxMenuSecretKind_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            foreach (var item in uxMenuSecretKind.Items) ((SecretKind)item).Checked = false;
            _currentSecretKind = (SecretKind)e.ClickedItem;
            _currentSecretKind.Checked = true;
            uxLinkLabelSecretKind.Text = _currentSecretKind.Text + " secret name \u25BC"; // Add black down triangle char
            uxToolTip.SetToolTip(uxLinkLabelSecretKind, _currentSecretKind.Description);
            uxTextBoxName_TextChanged(sender, null);
            uxTextBoxValue_TextChanged(sender, null);
        }
    }
}
