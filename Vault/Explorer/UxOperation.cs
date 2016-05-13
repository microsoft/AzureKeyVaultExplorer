using System;
using System.Windows.Forms;

namespace Microsoft.PS.Common.Vault.Explorer
{
    /// <summary>
    /// User experience operation, to be used with using() keyword
    /// </summary>
    public class UxOperation : IDisposable
    {
        private readonly ToolStripItem _controlToToggle;
        private readonly ToolStripItem _statusLabel;

        public UxOperation(ToolStripItem controlToToggle, ToolStripItem statusLabel)
        {
            _controlToToggle = controlToToggle;
            _statusLabel = statusLabel;

            _controlToToggle.Enabled = false;
            _statusLabel.Text = "Busy";
            Cursor.Current = Cursors.WaitCursor;
        }
        public void Dispose()
        {
            _controlToToggle.Enabled = true;
            _statusLabel.Text = "Ready";
            Cursor.Current = Cursors.Default;
        }
    }
}
