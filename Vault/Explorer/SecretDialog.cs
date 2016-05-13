using Microsoft.Azure.KeyVault;
using System;
using System.ComponentModel;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;
using ICSharpCode.TextEditor;

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

        public SecretDialog(string filename) : this()
        {
            string extension = Path.GetExtension(filename)?.ToLowerInvariant();
            SecretObject.ContentType = ContentTypeUtils.FromExtension(extension);
            uxTextBoxName.Text = Path.GetFileNameWithoutExtension(filename);
            uxTextBoxValue.Text = File.ReadAllText(filename);
        }

        public SecretDialog(X509Certificate cert) : this()
        {
            SecretObject.ContentType = ContentType.Certificate;
            uxTextBoxName.Text = cert.Subject;
            uxTextBoxValue.Text = Convert.ToBase64String(cert.GetRawCertData());
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
