using Microsoft.ApplicationInsights.DataContracts;
using System;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Windows.Forms;

namespace Microsoft.PS.Common.Vault.Explorer
{
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Telemetry.Init();
            Application.ApplicationExit += (s, e) => Telemetry.Default.Flush();
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (s, e) => TrackExceptionAndShowError(e.Exception);
            AppDomain.CurrentDomain.UnhandledException += (s, e) => TrackExceptionAndShowError(e.ExceptionObject as Exception);
            // First run install steps
            Utils.ClickOnce_SetAddRemoveProgramsIcon();
            ActivationUri.RegisterVaultProtocol();

            //Application.Run(new SubscriptionsManagerDialog());
            //return;

            // In case ActivationUri was passed perform the action and exit
            var form = new MainForm(ActivationUri.Parse());
            if (!form.IsDisposed)
            {
                Application.Run(form);
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
}