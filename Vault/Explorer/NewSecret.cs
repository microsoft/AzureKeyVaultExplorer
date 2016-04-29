using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VaultExplorer
{
    public partial class NewSecret : Form
    {
        private const int SecretValueMaxLength = 10240; // 10 KB
        private static Regex s_secretNameRegex = new Regex("^[0-9a-zA-Z-]{1,127}$", RegexOptions.Singleline | RegexOptions.Compiled);
        private bool _nameValid;
        private bool _valueValid;

        public NewSecret()
        {
            InitializeComponent();
            uxErrorProvider.SetIconAlignment(uxTextBoxName, ErrorIconAlignment.MiddleLeft);
            uxErrorProvider.SetIconAlignment(uxTextBoxValue, ErrorIconAlignment.TopLeft);
            uxTextBoxName_TextChanged(this, EventArgs.Empty);
            uxTextBoxValue_TextChanged(this, EventArgs.Empty);
        }

        public string SecretName => uxTextBoxName.Text;

        public string SecretValue => uxTextBoxValue.Text;

        private void uxTextBoxName_TextChanged(object sender, EventArgs e)
        {
            _nameValid = s_secretNameRegex.Match(uxTextBoxName.Text).Success;
            uxErrorProvider.SetError(uxTextBoxName, _nameValid ? null :
                $"Secret name must match the following regex {s_secretNameRegex}");
            InvalidateOkButton();
        }

        private void uxTextBoxValue_TextChanged(object sender, EventArgs e)
        {
            uxLabelValue.Text = $"Value: ({uxTextBoxValue.Text.Length} chars)";
            _valueValid = (uxTextBoxValue.Text.Length >= 1) && (uxTextBoxValue.Text.Length <= SecretValueMaxLength);
            uxErrorProvider.SetError(uxTextBoxValue, _valueValid ? null :
                $"Secret value length must be in the following range [1..{SecretValueMaxLength}]");
            InvalidateOkButton();
        }

        private void InvalidateOkButton()
        {
            uxButtonOK.Enabled = _nameValid && _valueValid;
        }
    }
}
