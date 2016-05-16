using Microsoft.Azure.KeyVault;
using System;
using System.ComponentModel;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;
using ICSharpCode.TextEditor;
using System.Text;
using Newtonsoft.Json;

namespace Microsoft.PS.Common.Vault.Explorer
{
    public partial class SecretDialog : Form
    {
        private bool _nameValid;
        private bool _valueValid;
        private bool _changed;
        private readonly TextEditorControl uxTextBoxValue;
        public readonly SecretObject SecretObject;

        private SecretDialog(Secret s, string title)
        {
            InitializeComponent();
            uxPropertyGridSecret.SelectedObject = SecretObject = new SecretObject(s, SecretObject_PropertyChanged);
            Text = title;
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
            uxTextBoxName.Text = "";
            uxTextBoxValue.Text = "";
        }

        public SecretDialog() : this(new Secret() { Attributes = new SecretAttributes(), ContentType = ContentTypeEnumConverter.GetDescription(ContentType.Text) }, "New Secret")
        {
            _changed = true;
        }

        public SecretDialog(Secret s) : this(s, "Edit secret")
        {
            uxTextBoxName.Text = s.SecretIdentifier.Name;
            uxTextBoxValue.Text = SecretObject.Value;
            _changed = false;
            InvalidateOkButton();
        }

        public SecretDialog(FileInfo fi) : this()
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
        }

        private void uxTextBoxName_TextChanged(object sender, EventArgs e)
        {
            _changed = true;
            _nameValid = Consts.ValidSecretNameRegex.Match(uxTextBoxName.Text).Success;
            uxErrorProvider.SetError(uxTextBoxName, _nameValid ? null : 
                $"Secret name must match the following regex {Consts.ValidSecretNameRegex}");
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
                    Consts.ValidBase64Regex.Match(SecretObject.Value).Success) // Allow first conversion from none to the right content type
                {
                    var cvo = JsonConvert.DeserializeObject<CertificateValueObject>(Encoding.UTF8.GetString(Convert.FromBase64String(SecretObject.Value)));
                    cvo.FillTags(SecretObject.Tags);
                    uxTextBoxValue.Text = cvo.ToString();
                }
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

            SecretObject.Value = uxTextBoxValue.Text;
            int rawValueLength = SecretObject.RawValue.Length;

            uxLabelBytesLeft.Text = $"{rawValueLength:N0} bytes / {Consts.MaxSecretValueLength - rawValueLength:N0} bytes left";
            _valueValid = (rawValueLength >= 1) && (rawValueLength <= Consts.MaxSecretValueLength);
            uxErrorProvider.SetError(uxSplitContainer, _valueValid ? null :
                $"Secret value length must be in the following range [1..{Consts.MaxSecretValueLength}]");
            InvalidateOkButton();
        }
    }
}
