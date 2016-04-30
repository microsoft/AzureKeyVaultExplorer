using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VaultExplorer
{
    /// <summary>
    /// User experience operatoin
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
