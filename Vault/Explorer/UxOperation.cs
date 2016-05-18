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
        private readonly ToolStripProgressBar _statusProgress;

        public UxOperation(ToolStripItem controlToToggle, ToolStripItem statusLabel, ToolStripProgressBar statusProgress)
        {
            _controlToToggle = controlToToggle;
            _statusLabel = statusLabel;
            _statusProgress = statusProgress;

            _controlToToggle.Enabled = false;
            _statusLabel.Text = "Busy";
            ProgressBarVisibility(true);
            Cursor.Current = Cursors.WaitCursor;
        }

        public void Dispose()
        {
            _controlToToggle.Enabled = true;
            _statusLabel.Text = "Ready";
            ProgressBarVisibility(false);
            Cursor.Current = Cursors.Default;
        }

        private void ProgressBarVisibility(bool visible)
        {
            if (_statusProgress != null)
            {
                _statusProgress.Visible = visible;
            }
        }
    }
}
