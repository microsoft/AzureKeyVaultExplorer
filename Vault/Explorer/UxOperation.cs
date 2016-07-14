using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Azure.KeyVault;
using System;
using System.Collections.Generic;
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
        private readonly ToolStripItem _statusLabel;
        private readonly ToolStripProgressBar _statusProgress;
        private readonly ToolStripItem _cancelButton;
        private readonly ToolStripItem[] _controlsToToggle;
        private readonly CancellationTokenSource _cancellationTokenSource;

        private bool _disposedValue = false; // To detect redundant calls

        public UxOperation(VaultAlias currentVaultAlias, ToolStripItem statusLabel, ToolStripProgressBar statusProgress, ToolStripItem cancelButton, params ToolStripItem[] controlsToToggle)
        {
            _startTime = DateTimeOffset.UtcNow;
            _currentVaultAlias = currentVaultAlias;
            _statusLabel = statusLabel;
            _statusProgress = statusProgress;
            _cancelButton = cancelButton;
            _controlsToToggle = controlsToToggle;

            _cancellationTokenSource = new CancellationTokenSource();

            ToggleControls(false, _controlsToToggle);
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

            var eventTelemetry = new EventTelemetry(_controlsToToggle[0].Name)
            {
                Timestamp = _startTime,
            };
            eventTelemetry.Metrics.Add("Duration", (DateTimeOffset.UtcNow - _startTime).TotalMilliseconds);
            eventTelemetry.Metrics.Add("Cancelled", _cancellationTokenSource.IsCancellationRequested ? 1 : 0);
            Telemetry.Default.TrackEvent(eventTelemetry);

            _cancellationTokenSource.Dispose();
            ToggleControls(true, _controlsToToggle);
            _statusLabel.Text = "Ready";
            ProgressBarVisibility(false);

            Cursor.Current = Cursors.Default;
            _disposedValue = true;
        }

        public async Task Invoke(string actionName, params Func<Task>[] tasks)
        {
            var exceptions = new List<Exception>();
            foreach (var t in tasks)
            {
                try
                {
                    await t();
                }
                catch (KeyVaultClientException kvce) when (kvce.Status == System.Net.HttpStatusCode.Forbidden)
                {
                    exceptions.Add(kvce);
                }
            }
            ProgressBarVisibility(false);
            if (exceptions.Count == tasks.Length) // In case all tasks failed with Forbidden, show message box to user
            {
                MessageBox.Show($"Operation to {actionName} {_currentVaultAlias.Alias} ({string.Join(", ", _currentVaultAlias.VaultNames)}) denied for {Environment.UserDomainName}\\{Environment.UserName}.\n\nYou are probably missing a certificate in CurrentUser\\My or LocalMachine\\My stores, or you are not part of the appropriate security group.",
                    Utils.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void ToggleControls(bool enabled, params ToolStripItem[] controlsToToggle)
        {
            foreach (var c in controlsToToggle) c.Enabled = enabled;
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
