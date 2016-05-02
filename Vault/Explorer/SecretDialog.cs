using Microsoft.Azure.KeyVault;
using System;
using System.ComponentModel;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;

namespace VaultExplorer
{
    public partial class SecretDialog : Form
    {
        private bool _nameValid;
        private bool _valueValid;
        private bool _changed;
        public readonly SecretObject SecretObject;

        private SecretDialog(Secret s, string title)
        {
            InitializeComponent();
            Text = title;
            uxTextBoxValue.MaxLength = Utils.MaxSecretValueLength;
            uxTextBoxName_TextChanged(this, EventArgs.Empty);
            uxTextBoxValue_TextChanged(this, EventArgs.Empty);
            SecretObject = new SecretObject(s, SecretObject_PropertyChanged);
            uxPropertyGridSecret.SelectedObject = SecretObject;
        }

        public SecretDialog() : this(new Secret() { Attributes = new SecretAttributes() }, "New Secret")
        {
            _changed = true;
        }

        public SecretDialog(Secret s) : this(s, "Edit secret")
        {
            uxTextBoxName.Text = s.SecretIdentifier.Name;
            uxTextBoxValue.Text = s.Value;
            _changed = false;
            InvalidateOkButton();
        }

        public SecretDialog(string filename) : this()
        {
            uxTextBoxName.Text = Path.GetFileNameWithoutExtension(filename);
            uxTextBoxValue.Text = File.ReadAllText(filename);
        }

        public SecretDialog(X509Certificate cert) : this()
        {
            uxTextBoxName.Text = cert.Subject;
            uxTextBoxValue.Text = Convert.ToBase64String(cert.GetRawCertData());
        }

        private void uxTextBoxName_TextChanged(object sender, EventArgs e)
        {
            _changed = true;
            _nameValid = Utils.ValidSecretNameRegex.Match(uxTextBoxName.Text).Success;
            uxErrorProvider.SetError(uxTextBoxName, _nameValid ? null :
                $"Secret name must match the following regex {Utils.ValidSecretNameRegex}");
            InvalidateOkButton();
        }

        private void uxTextBoxValue_TextChanged(object sender, EventArgs e)
        {
            _changed = true;
            uxLabelBytesLeft.Text = $"{uxTextBoxValue.MaxLength - uxTextBoxValue.Text.Length} bytes left";
            _valueValid = (uxTextBoxValue.Text.Length >= 1) && (uxTextBoxValue.Text.Length <= uxTextBoxValue.MaxLength);
            uxErrorProvider.SetError(uxSplitContainer, _valueValid ? null :
                $"Secret value length must be in the following range [1..{uxTextBoxValue.MaxLength}]");
            InvalidateOkButton();
        }

        private void SecretObject_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _changed = true;
            InvalidateOkButton();
        }

        private void InvalidateOkButton()
        {
            uxButtonOK.Enabled = _changed && _nameValid && _valueValid;
        }

        private void uxButtonOK_Click(object sender, EventArgs e)
        {
            SecretObject.Name = uxTextBoxName.Text;
            SecretObject.Value = uxTextBoxValue.Text;
        }
    }
}
