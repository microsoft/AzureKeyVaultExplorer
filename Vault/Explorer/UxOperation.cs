using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Azure.KeyVault;
using System;
using System.Diagnostics;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Microsoft.PS.Common.Vault.Explorer
{
    /// <summary>
    /// User experience operation, to be used with using() keyword
    /// </summary>
    public class UxOperation : IDisposable
    {
        public CancellationToken CancellationToken => _cancellationTokenSource.Token;

        private readonly DateTimeOffset _startTime;
        private readonly VaultAlias _currentVaultAlias;
        private readonly ToolStripItem _controlToToggle;
        private readonly ToolStripItem _statusLabel;
        private readonly ToolStripProgressBar _statusProgress;
        private readonly ToolStripItem _cancelButton;
        private readonly CancellationTokenSource _cancellationTokenSource;

        private bool _disposedValue = false; // To detect redundant calls

        public UxOperation(VaultAlias currentVaultAlias, ToolStripItem controlToToggle, ToolStripItem statusLabel, ToolStripProgressBar statusProgress, ToolStripItem cancelButton)
        {
            _startTime = DateTimeOffset.UtcNow;
            _currentVaultAlias = currentVaultAlias;
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
            if (_disposedValue) return;
            if (_cancelButton != null)
            {
                _cancelButton.Click -= uxButtonCancel_Click;
            }

            var eventTelemetry = new EventTelemetry(_controlToToggle.Name) 
            {
                Timestamp = _startTime,
            };
            eventTelemetry.Metrics.Add("Duration", (DateTimeOffset.UtcNow - _startTime).TotalMilliseconds);
            eventTelemetry.Metrics.Add("Cancelled", _cancellationTokenSource.IsCancellationRequested ? 1 : 0);
            Telemetry.Default.TrackEvent(eventTelemetry);

            _cancellationTokenSource.Dispose();
            _controlToToggle.Enabled = true;
            _statusLabel.Text = "Ready";
            ProgressBarVisibility(false);

            Cursor.Current = Cursors.Default;
            _disposedValue = true;
        }

        public async Task Invoke(string actionName, Func<Task> t)
        {
            try
            {
                await t();
            }
            catch (KeyVaultClientException kvce) when (kvce.Status == System.Net.HttpStatusCode.Forbidden)
            {
                ProgressBarVisibility(false);
                MessageBox.Show($"Operation to {actionName} {_currentVaultAlias.Alias} ({string.Join(", ", _currentVaultAlias.VaultNames)}) denied for {Environment.UserDomainName}\\{Environment.UserName}.\n\nYou are probably missing a certificate in CurrentUser\\My or LocalMachine\\My stores, or you are not part of the appropriate security group.",
                    Utils.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
