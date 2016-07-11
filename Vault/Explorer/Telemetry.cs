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
    public static class Telemetry
    {
        /// <summary>
        /// Application Insights portal:
        /// https://ms.portal.azure.com/#resource/subscriptions/34f2c5cf-b95b-4922-aad3-cc4c8ad13afb/resourcegroups/Default-ApplicationInsights-CentralUS/providers/microsoft.insights/components/vaultexplorer
        /// Application Insights analytics:
        /// https://analytics.applicationinsights.io/subscriptions/34f2c5cf-b95b-4922-aad3-cc4c8ad13afb/resourcegroups/Default-ApplicationInsights-CentralUS/components/vaultexplorer
        /// </summary>
        public static TelemetryClient Default { get; private set; }

        public static void Init()
        {
            TelemetryConfiguration config = TelemetryConfiguration.CreateDefault();
            config.InstrumentationKey = "ebe1f199-8317-4c4f-913c-5b569f1cba9f";
            config.DisableTelemetry = Settings.Default.DisableTelemetry || CommonUtils.IsDebug;
            Default = new TelemetryClient(config) { InstrumentationKey = config.InstrumentationKey };
            config.TelemetryInitializers.Add(new TelemetryInitializer());
        }
    }

    internal class TelemetryInitializer : ITelemetryInitializer
    {
        private static readonly Guid SessionId = Guid.NewGuid();
        private static readonly string UserDomainName = Environment.UserDomainName;
        private static readonly string UserName = Environment.UserName;
        private static readonly string AppVersion = Utils.GetFileVersionString("", Path.GetFileName(Application.ExecutablePath));
        private static readonly string CurrentUILanguageTag = CultureInfo.CurrentUICulture.IetfLanguageTag;
        private static readonly string OSVersion = Environment.OSVersion.ToString();
        private static readonly string Is64BitOperatingSystem = Environment.Is64BitOperatingSystem ? "true" : "false";
        private static readonly string Is64BitProcess = Environment.Is64BitProcess ? "true" : "false";
        private static readonly string MachineName = Environment.MachineName;
        private static readonly string ProcessorCount = Environment.ProcessorCount.ToString();
        private static readonly string ClrVersion = Environment.Version.ToString();

        public void Initialize(ITelemetry telemetry)
        {
            telemetry.Context.Session.Id = SessionId.ToString();

            telemetry.Context.User.AccountId = UserDomainName;
            telemetry.Context.User.Id = UserName;

            telemetry.Context.Component.Version = AppVersion;

            telemetry.Context.Device.Language = CurrentUILanguageTag;
            telemetry.Context.Device.OperatingSystem = OSVersion;

            telemetry.Context.Properties.Add("64BitOS", Is64BitOperatingSystem);
            telemetry.Context.Properties.Add("64BitProcess", Is64BitProcess);
            telemetry.Context.Properties.Add("MachineName", MachineName);
            telemetry.Context.Properties.Add("ProcessorCount", ProcessorCount);
            telemetry.Context.Properties.Add("ClrVersion", ClrVersion);
        }
    }

    /// <summary>
    /// Base class for forms to track Telemetry data as a PageView event.
    /// Also applies duration to the logged entry
    /// </summary>
    public class FormTelemetry : Form
    {
        private readonly PageViewTelemetry _telemetryData;

        private bool _viewLogged = false;
        private DateTimeOffset _startTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormTelemetry"/> class.
        /// </summary>
        public FormTelemetry()
        {
            _telemetryData = new PageViewTelemetry(!string.IsNullOrWhiteSpace(this.Name) ? this.Name : this.GetType().Name);
        }

        /// <summary>
        /// Raises the <see cref="E:Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            _startTime = DateTimeOffset.UtcNow;
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
                _telemetryData.Timestamp = _startTime;
                _telemetryData.Duration = DateTimeOffset.UtcNow - _startTime;
                Telemetry.Default.TrackPageView(_telemetryData);
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
