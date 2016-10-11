// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 

using Microsoft.ApplicationInsights.DataContracts;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Windows.Forms;

namespace Microsoft.Vault.Explorer
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
            AppDomain.CurrentDomain.AssemblyResolve += (s, args) => ResolveMissingAssembly(args);
            // First run install steps
            Utils.ClickOnce_SetAddRemoveProgramsIcon();
            ActivationUri.RegisterVaultProtocol();
            // In case ActivationUri was passed perform the action and exit
            var form = new MainForm(ActivationUri.Parse());
            if (!form.IsDisposed)
            {
                Application.Run(form);
            }
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