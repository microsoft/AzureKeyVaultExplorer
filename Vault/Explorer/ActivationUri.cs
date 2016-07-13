using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace Microsoft.PS.Common.Vault.Explorer
{
    public class ActivationUri : VaultLinkUri
    {
        public static readonly ActivationUri Empty = new ActivationUri("vault:");

        public ActivationUri(string vaultUri) : base(vaultUri) { }

        public new static ActivationUri Parse()
        {
            string vaultUri = (ApplicationDeployment.IsNetworkDeployed) ?
                AppDomain.CurrentDomain.SetupInformation?.ActivationArguments?.ActivationData?.FirstOrDefault() :
                (Environment.GetCommandLineArgs().Length == 2) ? Environment.GetCommandLineArgs()[1] : null;
            if (null == vaultUri) return Empty;
            if (vaultUri.StartsWith("file:", StringComparison.CurrentCultureIgnoreCase)) return Empty;
            return new ActivationUri(vaultUri.TrimEnd('/', '\\'));
        }

        public void CopyToClipboard(Vault vault)
        {
            PropertyObject po = null;
            switch (Collection)
            {
                case VaultUriCollection.Keys:
                    return;
                case VaultUriCollection.Certificates:
                    var cb = vault.GetCertificateAsync(ItemName, Version, CancellationToken.None).GetAwaiter().GetResult();
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
        /// "C:\windows\system32\rundll32.exe" C:\windows\system32\dfshim.dll, ShOpenVerbShortcut C:\Users\elize\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Microsoft\VaultExplorer.appref-ms|%1
        /// </summary>
        public static void RegisterVaultProtocol()
        {
            if (ApplicationDeployment.IsNetworkDeployed && ApplicationDeployment.CurrentDeployment.IsFirstRun)
            {
                try
                {
                    var subKey = Registry.CurrentUser.OpenSubKey(@"Software\Classes\vault", true);
                    if (null != subKey)
                    {
                        subKey.CreateSubKey("DefaultIcon").SetValue("", $"{Application.ExecutablePath},0"); // Update the location to icon 
                        return;
                    }
                    using (var vaultKey = Registry.CurrentUser.CreateSubKey(@"Software\Classes\vault"))
                    {
                        vaultKey.SetValue("", "URL:Vault Protocol");
                        vaultKey.SetValue("URL Protocol", "");
                        vaultKey.CreateSubKey("DefaultIcon").SetValue("", $"{Application.ExecutablePath},0");
                        string system32 = Environment.GetFolderPath(Environment.SpecialFolder.System);
                        string appref_ms = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu), "Programs", "Microsoft", "VaultExplorer.appref-ms");
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
