// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 

using Microsoft.ApplicationInsights.DataContracts;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Microsoft.Vault.Explorer
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

        /// <summary>
        /// Invoke specified vault releated tasks in parallel, in case all tasks failed with Forbidden code
        /// show access denied message box. If at least one task finished successfully, no error is showed to user
        /// </summary>
        /// <param name="actionName">Nice name of the action to show in the message box</param>
        /// <param name="tasks"></param>
        /// <returns></returns>
        public async Task Invoke(string actionName, params Func<Task>[] tasks)
        {
            var tasksList = new List<Task>();
            var exceptions = new ConcurrentQueue<Exception>();

            foreach (var t in tasks)
            {
                tasksList.Add(Task.Run(async () =>
                {
                    try
                    {
                        await t();
                    }
                    catch (Exception ce)
                    {
                        exceptions.Enqueue(ce);
                    }
                }));
            }
            await Task.WhenAll(tasksList);
            ProgressBarVisibility(false);
            if (exceptions.Count == tasks.Length) // In case all tasks failed with Forbidden, show message box to user
            {
                MessageBox.Show($"Operation to {actionName} {_currentVaultAlias.Alias} ({string.Join(", ", _currentVaultAlias.VaultNames)}) denied.\n\nYou are probably missing a certificate in CurrentUser\\My or LocalMachine\\My stores, or you are not part of the appropriate security group.",
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
