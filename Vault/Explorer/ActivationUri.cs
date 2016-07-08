using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace Microsoft.PS.Common.Vault.Explorer
{
    class Form1 : Form
    {
        ToolTip _ToolTip = new ToolTip();

        public Form1()
        {
            InitializeComponent();

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            BringToFront();
            _ToolTip.Show("Blah blah... Blah blah... Blah blah...", this, Cursor.Position.X + 8, Cursor.Position.Y, 10000);
        }

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 10;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 264);
            this.ControlBox = false;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Opacity = 0;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timer1;

    }

    public enum ActivationAction
    {
        None,

        CopyToClipboard
    }

    [JsonObject]
    public class ActivationUri
    {
        [JsonProperty]
        public readonly ActivationAction Action = ActivationAction.None;

        [JsonProperty]
        public readonly Uri Id;

        public static ActivationUri Parse()
        {
            string queryString = ApplicationDeployment.IsNetworkDeployed ?
                ApplicationDeployment.CurrentDeployment.ActivationUri?.Query :
                (Environment.GetCommandLineArgs().Length == 2) ? Environment.GetCommandLineArgs()[1] : "";
            var nameValueCollection = HttpUtility.ParseQueryString(queryString);
            var json = new JavaScriptSerializer().Serialize(nameValueCollection.AllKeys.ToDictionary(k => k, k => nameValueCollection[k]));
            return JsonConvert.DeserializeObject<ActivationUri>(json);
        }

        [JsonConstructor]
        public ActivationUri(ActivationAction action, Uri id)
        {            
            Action = action;
            if (action != ActivationAction.None)
            {
                Guard.ArgumentNotNull(id, nameof(id));
            }
            Id = id;
        }

        public bool Perform()
        {
            //var f = new Form1();
            //Application.Run(f);
            switch (Action)
            {
                case ActivationAction.None:
                    return false;
                case ActivationAction.CopyToClipboard:
                    CopyToClipboard();
                    return true;
                default:
                    throw new ArgumentOutOfRangeException(nameof(Action), $"Invalid action {Action}");
            }
        }

        private void CopyToClipboard()
        {
            var m = Consts.ValidVaultItemUriRegex.Match(Id.ToString());
            if (false == m.Success)
            {
                throw new ArgumentException($"Invalid vault item URI {Id}, URI must satisfy the following regex: {Consts.ValidVaultItemUriRegex}", nameof(Id));
            }
            string vaultName = m.Groups["VaultName"].Value;
            string endpoint = m.Groups["Endpoint"].Value;
            string name = m.Groups["Name"].Value;
            string version = string.IsNullOrEmpty(m.Groups["Version"].Value) ? null : m.Groups["Version"].Value;
            var vc = new VaultsConfig(new Dictionary<string, VaultAccessType>
            {
                [vaultName] = new VaultAccessType(
                    new VaultAccess[] { new VaultAccessUserInteractive(null) },
                    new VaultAccess[] { new VaultAccessUserInteractive(null) })
            });
            var vault = new Vault(vc, VaultAccessTypeEnum.ReadOnly, vaultName);
            PropertyObject po = null;
            switch (endpoint.ToLowerInvariant())
            {
                case "keys":
                    return;
                case "certificates":
                    var cb = vault.GetCertificateAsync(name, version, CancellationToken.None).GetAwaiter().GetResult();
                    var cert = vault.GetCertificateWithExportableKeysAsync(name, version, CancellationToken.None).GetAwaiter().GetResult();
                    po = new PropertyObjectCertificate(cb, cb.Policy, cert, null);
                    break;
                case "secrets":
                    var s = vault.GetSecretAsync(name, version, CancellationToken.None).GetAwaiter().GetResult();
                    po = new PropertyObjectSecret(s, null);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(endpoint), $"Invalid endpoint {endpoint}");
            }
            string value = po.GetClipboardValue();
            if (null != value)
            {
                Clipboard.SetText(value);
            }
        }
    }
}
