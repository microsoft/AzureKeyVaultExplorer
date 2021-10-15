// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Vault.Library;

namespace Microsoft.Vault.Explorer
{
    public partial class SettingsDialog : FormTelemetry
    {
        private readonly Settings _currentSettings;

        public SettingsDialog()
        {
            InitializeComponent();
            uxPropertyGrid.SelectedObject = _currentSettings = new Settings();
            _currentSettings.PropertyChanged += (object sender, PropertyChangedEventArgs e) => { uxButtonOK.Enabled = true; };
            uxTextBoxVersions.Text = FetchVersions();

            string licenseTxt = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "License.txt");
            if (File.Exists(licenseTxt))
            {
                uxTextBoxLicense.Text = File.ReadAllText(licenseTxt).Replace("\n", "\r\n");
            }
        }

        private void uxButtonOK_Click(object sender, EventArgs e)
        {
            _currentSettings.Save();
            Settings.Default.Reload();
        }

        private void uxLinkLabelTitle_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://aka.ms/vaultexplorer");
        }

        private void uxLinkLabelSendFeedback_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string body = uxTextBoxVersions.Text;
            body = body.Replace("\n", "%0A").Replace(" ", "%20");
            ProcessStartInfo sInfo = new ProcessStartInfo(string.Format("mailto://Azure Key Vault Explorer Developers <vedev@microsoft.com>?subject={0}&body={1}", Utils.AppName, body));
            Process.Start(sInfo);
        }

        private void uxLinkLabelInstallLocation_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo() { FileName = Path.GetDirectoryName(Application.ExecutablePath), UseShellExecute = true };
            Process.Start(sInfo);
        }

        private void uxLinkLabelUserSettingsLocation_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
            ProcessStartInfo sInfo = new ProcessStartInfo() { FileName = Path.GetDirectoryName(config.FilePath), UseShellExecute = true };
            Process.Start(sInfo);
        }

        private string FetchVersions()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(Utils.GetFileVersionString(string.Format("{0} version: ", Utils.AppName), Path.GetFileName(Application.ExecutablePath), string.Format(" ({0})", Environment.Is64BitProcess ? "x64" : "x86")));
            sb.AppendLine(string.Format(".NET framework version: {0}", Environment.Version));
            sb.AppendLine(Utils.GetFileVersionString("Microsoft.Azure.KeyVault.dll version: ", "Microsoft.Azure.KeyVault.dll"));
            sb.AppendLine(Utils.GetFileVersionString("Microsoft.Azure.Management.KeyVault.dll version: ", "Microsoft.Azure.Management.KeyVault.dll"));
            return sb.ToString();
        }

        private void uxLinkLabelClearTokenCache_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CachePersistence.ClearAllFileTokenCaches();
        }
    }
}
