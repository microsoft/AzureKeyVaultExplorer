using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.ApplicationInsights.Channel;
using System.Diagnostics;

namespace Microsoft.PS.Common.Vault.Explorer
{
    public static class VaultExplorerTelemetryClient
    {
        /// <summary>
        /// Application Insights
        /// https://ms.portal.azure.com/#resource/subscriptions/34f2c5cf-b95b-4922-aad3-cc4c8ad13afb/resourcegroups/Default-ApplicationInsights-CentralUS/providers/microsoft.insights/components/vaultexplorer
        /// </summary>
        public const string TelemetryClientName = "vaultexplorer";

        public static TelemetryClient Default { get; private set; }

        public static void Init()
        {
            TelemetryConfiguration config = TelemetryConfiguration.CreateDefault();
            config.InstrumentationKey = "ebe1f199-8317-4c4f-913c-5b569f1cba9f";
            Default = new TelemetryClient(config) { InstrumentationKey = config.InstrumentationKey };
            config.TelemetryInitializers.Add(new TelemetryInitializer());
        }
    }

    internal class TelemetryInitializer : ITelemetryInitializer
    {
        public void Initialize(ITelemetry telemetry)
        {
            telemetry.Context.User.AccountId = Environment.UserDomainName;
            telemetry.Context.User.Id = Environment.UserName;

            telemetry.Context.Session.Id = Guid.NewGuid().ToString();

            telemetry.Context.Component.Version = Utils.GetFileVersionString("", Path.GetFileName(Application.ExecutablePath));

            telemetry.Context.Device.Language = CultureInfo.CurrentUICulture.IetfLanguageTag;
            telemetry.Context.Device.OperatingSystem = Environment.OSVersion.ToString();

            telemetry.Context.Properties.Add("64BitOS", Environment.Is64BitOperatingSystem.ToString());
            telemetry.Context.Properties.Add("64BitProcess", Environment.Is64BitProcess.ToString());
            telemetry.Context.Properties.Add("Machine name", Environment.MachineName);
            telemetry.Context.Properties.Add("ProcessorCount", Environment.ProcessorCount.ToString());
            telemetry.Context.Properties.Add("ClrVersion", Environment.Version.ToString());

            telemetry.Context.Session.IsFirst = true;
        }
    }

    /// <summary>
    /// Base class for forms to track Telemetry data as a PageView event.
    /// Also applies duration to the logged entry
    /// </summary>
    public class TelemetryForm : Form
    {
        private readonly PageViewTelemetry _telemetryData;

        private bool _viewLogged = false;
        private Stopwatch _stopwatch = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="TelemetryForm"/> class.
        /// </summary>
        public TelemetryForm()
        {
            _telemetryData = new PageViewTelemetry(!string.IsNullOrWhiteSpace(this.Name) ? this.Name : this.GetType().Name);
        }

        /// <summary>
        /// Raises the <see cref="E:Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            _stopwatch = Stopwatch.StartNew();
            base.OnLoad(e);
        }

        /// <summary>
        /// Raises the <see cref="E:Closed" /> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected override void OnClosed(EventArgs e)
        {
            LogPageView();
            base.OnClosed(e);
        }

        private void LogPageView()
        {
            if (!this.DesignMode && !_viewLogged)
            {
                _telemetryData.Duration = _stopwatch.Elapsed;
                VaultExplorerTelemetryClient.Default.TrackPageView(_telemetryData);
                _viewLogged = true;
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            LogPageView();
            base.Dispose(disposing);
        }
    }
}
