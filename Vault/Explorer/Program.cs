﻿// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 

using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Vault.Library;
using System;
using System.Drawing;
using System.Linq;
using System.Security.Permissions;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Microsoft.Vault.Explorer
{
    public static class Program
    {
        private static readonly System.Windows.Forms.Timer IdleTimer = new System.Windows.Forms.Timer();
        private static readonly int TimeIntervalForApplicationIdle = (int)TimeSpan.FromHours(1).TotalMilliseconds;
        private static readonly int TimeIntervalForUserInput =  (int)TimeSpan.FromMinutes(1).TotalMilliseconds;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            IdleTimer.Enabled = false;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Telemetry.Init();
            Application.ApplicationExit += (s, e) => Telemetry.Default.Flush();
            Application.ApplicationExit += (s, e) => DeleteTokenCacheOnApplicationExit();
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (s, e) => TrackExceptionAndShowError(e.Exception);
            AppDomain.CurrentDomain.UnhandledException += (s, e) => TrackExceptionAndShowError(e.ExceptionObject as Exception);
            AppDomain.CurrentDomain.AssemblyResolve += (s, args) => ResolveMissingAssembly(args);
            // First run install steps
            Utils.ClickOnce_SetAddRemoveProgramsIcon();
            ActivationUri.RegisterVaultProtocol();
            // In case ActivationUri was passed perform the action and exit
            //Add a message filter to check if application is idle
            LeaveIdleMessageFilter limf = new LeaveIdleMessageFilter(IdleTimer);
            Application.AddMessageFilter(limf);
            Application.Idle += new EventHandler(ApplicationIdle);
            IdleTimer.Interval = TimeIntervalForApplicationIdle;
            IdleTimer.Tick += TimeDone;
            var form = new MainForm(ActivationUri.Parse());
            if (!form.IsDisposed)
            {
                Application.Run(form);
            }
        }

        /// <summary>
        /// Delete Token cache on Application Exit
        /// </summary>
        private static void DeleteTokenCacheOnApplicationExit()
        {
            CachePersistence.ClearAllFileTokenCaches();
        }

        /// <summary>
        /// Microsoft.PS.Common.Vault.dll was renamed to Microsoft.Vault.Library.dll
        /// For backward compatibility reasons, to be able to deserialize old Vaults.json we resolve the missing
        /// assembly and point to our new Microsoft.Vault.Library.dll
        /// </summary>
        /// <seealso cref="BackwardCompatibility.cs"/>
        private static Assembly ResolveMissingAssembly(ResolveEventArgs args)
        {
            if (args.Name == "Microsoft.PS.Common.Vault")
            {
                var vaultLibrary = from a in AppDomain.CurrentDomain.GetAssemblies() where a.GetName().Name == "Microsoft.Vault.Library" select a;
                return vaultLibrary.FirstOrDefault();
            }
            return null;
        }

        /// <summary>
        /// Event Handler for Application Idle. Starts the Idle Timer 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static private void ApplicationIdle(Object sender, EventArgs e)
        {
            if (!IdleTimer.Enabled)
            {
                IdleTimer.Start();
            }
        }

        /// <summary>
        /// Called when Application is idle for the configured time interval
        /// </summary>
        static private void TimeDone(object sender, EventArgs e)
        {
            IdleTimer.Stop();
            const string message = "VaultExplorer is being closed due to inactivity. Do you want to continue working on it?";
            const string caption = "Closing Vault Explorer";
            using (AutoClosingMessageBox autoClosingMessageBox = new AutoClosingMessageBox(TimeIntervalForUserInput))
            {
                var result = autoClosingMessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                if (result == DialogResult.No)
                {
                    Application.Exit();
                }
                else if (result == DialogResult.Yes)
                {
                    IdleTimer.Stop();
                }
            }
        }

        private static void TrackExceptionAndShowError(Exception e)
        {
            if (e is OperationCanceledException)
            {
                object o = CallContext.LogicalGetData($"{nameof(UxOperation) + nameof(CancellationToken)}");
                if (o != null) return; // Do not show any dialog to user
            }
            // TrackException
            Telemetry.Default.TrackException(new ExceptionTelemetry(e)
            {
                HandledAt = ExceptionHandledAt.Unhandled,
                SeverityLevel = SeverityLevel.Critical,
            });
            // Show error
            var ed = new ExceptionDialog(e);
            ed.ShowDialog();
        }
    }

    /// <summary>
    /// This class filters (listens to) all messages for the application and if
    /// a relevant message (such as mouse or keyboard) is received then it resets the timer.
    /// </summary>
    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public class LeaveIdleMessageFilter : IMessageFilter
    {
        const int WM_NCLBUTTONDOWN = 0x00A1;
        const int WM_NCLBUTTONUP = 0x00A2;
        const int WM_NCRBUTTONDOWN = 0x00A4;
        const int WM_NCRBUTTONUP = 0x00A5;
        const int WM_NCMBUTTONDOWN = 0x00A7;
        const int WM_NCMBUTTONUP = 0x00A8;
        const int WM_NCXBUTTONDOWN = 0x00AB;
        const int WM_NCXBUTTONUP = 0x00AC;
        const int WM_KEYDOWN = 0x0100;
        const int WM_KEYUP = 0x0101;
        const int WM_MOUSEMOVE = 0x0200;
        const int WM_LBUTTONDOWN = 0x0201;
        const int WM_LBUTTONUP = 0x0202;
        const int WM_RBUTTONDOWN = 0x0204;
        const int WM_RBUTTONUP = 0x0205;
        const int WM_MBUTTONDOWN = 0x0207;
        const int WM_MBUTTONUP = 0x0208;
        const int WM_XBUTTONDOWN = 0x020B;
        const int WM_XBUTTONUP = 0x020C;

        // The Messages array must be sorted due to use of Array.BinarySearch
        static int[] ActivityMessages = new int[] {WM_NCLBUTTONDOWN,
            WM_NCLBUTTONUP, WM_NCRBUTTONDOWN, WM_NCRBUTTONUP, WM_NCMBUTTONDOWN,
            WM_NCMBUTTONUP, WM_NCXBUTTONDOWN, WM_NCXBUTTONUP, WM_KEYDOWN, WM_KEYUP,
            WM_LBUTTONDOWN, WM_LBUTTONUP, WM_RBUTTONDOWN, WM_RBUTTONUP,
            WM_MBUTTONDOWN, WM_MBUTTONUP, WM_XBUTTONDOWN, WM_XBUTTONUP};

        private readonly System.Windows.Forms.Timer timer;

        public LeaveIdleMessageFilter(System.Windows.Forms.Timer timer)
        {
            if (timer == null)
            {
                throw new ArgumentNullException(nameof(timer));
            }

            this.timer = timer;
        }

        public bool PreFilterMessage(ref Message m)
        {
            // Stop the idle timer if we see user interaction message
            if (this.timer.Enabled && Array.BinarySearch(ActivityMessages, m.Msg) >= 0)
            {
                this.timer.Stop();
            }

            return false;
        }
    }
}

