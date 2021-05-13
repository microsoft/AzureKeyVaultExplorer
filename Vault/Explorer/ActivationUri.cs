// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 

namespace Microsoft.Vault.Explorer
{
    using Microsoft.Win32;
    using System;
    using System.Collections.Generic;
    using System.Deployment.Application;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Windows.Forms;
    using Microsoft.Vault.Library;

    public class ActivationUri : VaultLinkUri
    {
        private static readonly string OnlineActivationUri = $"https://elize.blob.core.windows.net/{Utils.ProductName}/{Utils.ProductName}.application";
        public static readonly ActivationUri Empty = new ActivationUri("vault:");

        public ActivationUri(string vaultUri) : base(vaultUri) { }

        public new static ActivationUri Parse()
        {
            string vaultUri = (ApplicationDeployment.IsNetworkDeployed) ?
                AppDomain.CurrentDomain.SetupInformation?.ActivationArguments?.ActivationData?.FirstOrDefault() :
                (Environment.GetCommandLineArgs().Length == 2) ? Environment.GetCommandLineArgs()[1] : null;
            // Arguments were not passed at all or activation happened via Application Reference (.appref-ms)
            if (string.IsNullOrEmpty(vaultUri)) return Empty;
            if (vaultUri.StartsWith("file:", StringComparison.CurrentCultureIgnoreCase)) return Empty;
            // Online activation
            if (vaultUri.StartsWith(OnlineActivationUri, StringComparison.CurrentCultureIgnoreCase))
            {
                vaultUri = vaultUri.Substring(OnlineActivationUri.Length).TrimStart('?');
            }
            if (string.IsNullOrEmpty(vaultUri)) return Empty;
            return new ActivationUri(vaultUri.TrimEnd('/', '\\'));
        }

        public void PerformAction(Vault vault)
        {
            switch (Action)
            {
                case Microsoft.Vault.Library.Action.Default:
                    CopyToClipboard(vault);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(Action), $"Invalid action {Action}");
            }
        }

        private void CopyToClipboard(Vault vault)
        {
            PropertyObject po = null;
            switch (Collection)
            {
                case VaultUriCollection.Keys:
                    return;
                case VaultUriCollection.Certificates:
                    var cb = vault.GetCertificateAsync(ItemName, CancellationToken.None).GetAwaiter().GetResult();
                    var cert = vault.GetCertificateWithExportableKeysAsync(ItemName, Version, CancellationToken.None).GetAwaiter().GetResult();
                    po = new PropertyObjectCertificate(cb, cb.Policy, cert, null);
                    break;
                case VaultUriCollection.Secrets:
                    var s = vault.GetSecretAsync(ItemName, Version, CancellationToken.None).GetAwaiter().GetResult();
                    po = new PropertyObjectSecret(s, null);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(Collection), $"Invalid endpoint {Collection}");
            }
            po.CopyToClipboard(true);
        }

        /// <summary>
        /// Register vault: protocol for current user, pretty much will set the following regkey
        /// HKEY_CURRENT_USER\SOFTWARE\Classes\vault\shell\open\command
        /// "C:\windows\system32\rundll32.exe" C:\windows\system32\dfshim.dll, ShOpenVerbShortcut C:\Users\elize\AppData\Roaming\Microsoft Corporation\Windows\Start Menu\Programs\Microsoft\VaultExplorer.appref-ms|%1
        /// </summary>
        public static void RegisterVaultProtocol()
        {
            if (ApplicationDeployment.IsNetworkDeployed && ApplicationDeployment.CurrentDeployment.IsFirstRun)
            {
                try
                {
                    using (var vaultKey = Registry.CurrentUser.CreateSubKey(@"Software\Classes\vault"))
                    {
                        vaultKey.SetValue("", "URL:Vault Protocol");
                        vaultKey.SetValue("URL Protocol", "");
                        vaultKey.CreateSubKey("DefaultIcon").SetValue("", $"{Application.ExecutablePath},0");
                        string system32 = Environment.GetFolderPath(Environment.SpecialFolder.System);
                        string appref_ms = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu), "Programs", "Microsoft Corporation", "VaultExplorer.appref-ms");
                        vaultKey.CreateSubKey("shell").CreateSubKey("open").CreateSubKey("command").SetValue("",
                                $"\"{system32}\\rundll32.exe\" {system32}\\dfshim.dll, ShOpenVerbShortcut {appref_ms}|%1");
                    }
                    // Enable trust to vault: protocol handler for different Office version(s)
                    // https://support.microsoft.com/en-us/kb/982301
                    for (int officeVersion = 14; officeVersion < 30; officeVersion++)
                    {
                        string version = $"{officeVersion}.0";
                        if (null != Registry.CurrentUser.OpenSubKey($@"Software\Policies\Microsoft\Office\{version}"))
                        {
                            Registry.CurrentUser.CreateSubKey($@"Software\Policies\Microsoft\Office\{version}\common\security\trusted protocols\all applications\vault:");
                        }
                    }
                }
                catch { }
            }
        }
    }
}
