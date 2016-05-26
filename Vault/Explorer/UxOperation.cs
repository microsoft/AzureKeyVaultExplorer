using System;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Windows.Forms;

namespace Microsoft.PS.Common.Vault.Explorer
{
    /// <summary>
    /// User experience operation, to be used with using() keyword
    /// </summary>
    public class UxOperation : IDisposable
    {
        public CancellationToken CancellationToken => _cancellationTokenSource.Token;

        private readonly ToolStripItem _controlToToggle;
        private readonly ToolStripItem _statusLabel;
        private readonly ToolStripProgressBar _statusProgress;
        private readonly ToolStripItem _cancelButton;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public UxOperation(ToolStripItem controlToToggle, ToolStripItem statusLabel, ToolStripProgressBar statusProgress, ToolStripItem cancelButton)
        {
            _controlToToggle = controlToToggle;
            _statusLabel = statusLabel;
            _statusProgress = statusProgress;
            _cancelButton = cancelButton;

            _cancellationTokenSource = new CancellationTokenSource();

            _controlToToggle.Enabled = false;
            _statusLabel.Text = "Busy";
            ProgressBarVisibility(true);
            if (_cancelButton != null)
            {
                _cancelButton.Click += uxButtonCancel_Click;
            }
            Cursor.Current = Cursors.WaitCursor;
        }

        public void Dispose()
        {
            if (_cancelButton != null)
            {
                _cancelButton.Click -= uxButtonCancel_Click;
            }
            _cancellationTokenSource.Dispose();
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
            if (_cancelButton != null)
            {
                _cancelButton.Visible = visible;
            }
        }

        private void uxButtonCancel_Click(object sender, EventArgs e)
        {
            CallContext.LogicalSetData($"{nameof(UxOperation) + nameof(CancellationToken)}", CancellationToken);
            _cancellationTokenSource.Cancel();
        }
    }
}
