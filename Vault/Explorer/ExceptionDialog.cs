using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VaultExplorer
{
    public partial class ExceptionDialog : FormTelemetry
    {
        public ExceptionDialog(Exception e)
        {
            InitializeComponent();
            uxRichTextBoxCaption.Rtf = string.Format(@"{{\rtf1\ansi Oops... Unhandled exception of type \b {0} \b0 has occurred: \b {1} \b0 To ignore this error just click Continue, otherwise click Quit.}}", e.GetType(), Utils.GetRtfUnicodeEscapedString(e.Message));
            uxTextBoxExceptionDetails.Text = e.ToString();
            uxTextBoxExceptionDetails.Select(0, 0);
        }

        private void uxButtonQuit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
