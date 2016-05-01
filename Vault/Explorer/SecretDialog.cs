using Microsoft.Azure.KeyVault;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VaultExplorer
{
    public partial class SecretDialog : Form
    {
        private static Regex s_secretNameRegex = new Regex("^[0-9a-zA-Z-]{1,127}$", RegexOptions.Singleline | RegexOptions.Compiled);
        private bool _nameValid;
        private bool _valueValid;
        private bool _changed;
        public readonly SecretObject SecretObject;

        private SecretDialog(Secret s, string title)
        {
            InitializeComponent();
            Text = title;
            uxTextBoxValue.MaxLength = Utils.MaxSecretValueLength;
            uxErrorProvider.SetIconAlignment(uxTextBoxName, ErrorIconAlignment.MiddleLeft);
            uxErrorProvider.SetIconAlignment(uxSplitContainer, ErrorIconAlignment.TopLeft);
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
            _nameValid = s_secretNameRegex.Match(uxTextBoxName.Text).Success;
            uxErrorProvider.SetError(uxTextBoxName, _nameValid ? null :
                $"Secret name must match the following regex {s_secretNameRegex}");
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
