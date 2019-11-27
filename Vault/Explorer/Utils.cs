// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 

using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Deployment.Application;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.Vault.Library;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Microsoft.Vault.Core;

namespace Microsoft.Vault.Explorer
{
    public static class Utils
    {
        public const string ProductName = "VaultExplorer";

        public const string AppName = "Azure Key Vault Explorer";

        /// <summary>
        /// Space with black down triangle char
        /// </summary>
        public const string DropDownSuffix = " \u25BC";

        /// <summary>
        /// Converts DateTime? to LocalTime string
        /// </summary>
        /// <param name="dt">DateTime?</param>
        /// <returns>string</returns>
        public static string NullableDateTimeToString(DateTime? dt) => (dt == null) ? "(none)" : dt.Value.ToLocalTime().ToString();

        public static string NullableIntToString(int? x) => (x == null) ? "(none)" : x.ToString();

        public static string ExpirationToString(DateTime? dt)
        {
            if (dt == null) return "";
            var ts = dt.Value - DateTime.UtcNow;
            return ExpirationToString(ts);
        }

        public static string ExpirationToString(TimeSpan ts)
        {
            if (ts == TimeSpan.MaxValue) return "Never";
            if (ts.TotalDays < 0) return "Expired";
            if (ts.TotalDays >= 2) return $"{ts.TotalDays:N0} days";
            if (ts.TotalDays >= 1) return $"{ts.TotalDays:N0} day and {ts.Hours} hours";
            return $"{ts.Hours} hours";
        }

        public static string ByteArrayToHex(byte[] arr)
        {
            Guard.ArgumentNotNull(arr, nameof(arr));
            string hex = BitConverter.ToString(arr);
            return hex.Replace("-", "");
        }

        public static string FullPathToJsonFile(string filename)
        {
            filename = Environment.ExpandEnvironmentVariables(filename);
            if (Path.IsPathRooted(filename)) return filename;
            filename = Path.Combine(Settings.Default.JsonConfigurationFilesRoot, filename);
            if (Path.IsPathRooted(filename)) return filename;
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);
        }

        public static T LoadFromJsonFile<T>(string filename, bool isOptional = false) where T : new()
        {
            string path = FullPathToJsonFile(filename);
            if (File.Exists(path))
            {
                var x = JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
                return x;
            }
            if (isOptional)
            {
                return new T();
            }
            throw new FileNotFoundException("Mandatory .json configuration file is not found", path);
        }

        public static Cursor LoadCursorFromResource(byte[] buffer)
        {
            using (var ms = new MemoryStream(buffer))
            {
                return new Cursor(ms);
            }
        }

        public static string ConvertToValidSecretName(string name)
        {
            var result = new Regex("[^0-9a-zA-Z-]", RegexOptions.Singleline).Replace(name, "-");
            return string.IsNullOrEmpty(result) ? "unknown" : result;
        }

        public static string ConvertToValidTagValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            if (value.Length <= Consts.MaxTagNameLength) return value;
            return value.Substring(0, Consts.MaxTagNameLength - 3) + "...";
        }

        public static string GetRtfUnicodeEscapedString(string s)
        {
            var sb = new StringBuilder();
            foreach (var c in s)
            {
                if (c == '\\' || c == '{' || c == '}')
                    sb.Append(@"\" + c);
                else if (c <= 0x7f)
                    sb.Append(c);
                else
                    sb.Append("\\u" + Convert.ToUInt32(c) + "?");
            }
            return sb.ToString();
        }

        public static string GetFileVersionString(string title, string peFilename, string optionalPrefix = "")
        {
            var filepath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), peFilename);
            string version = "Unknown";
            try
            {
                var verInfo = FileVersionInfo.GetVersionInfo(filepath);
                version = string.Format("{0}.{1}.{2}.{3}", verInfo.FileMajorPart, verInfo.FileMinorPart, verInfo.FileBuildPart, verInfo.FilePrivatePart);
            }
            catch { }
            return string.Format(string.Format("{0}{1}{2}", title, version, optionalPrefix));
        }

        public static string NewSecurePassword()
        {
            var UpperCharsSet = Enumerable.Range(65, 26).Select(i => (byte)i).ToArray(); // A..Z
            var LowerCharsSet = Enumerable.Range(97, 26).Select(i => (byte)i).ToArray(); // a..z
            var NumbersSet = Enumerable.Range(48, 10).Select(i => (byte)i).ToArray(); // 0..9
            var SpecialCharsSet = new byte[] { 33, 35, 40, 41, 64 }; // !, #, (, ), @
            var All = UpperCharsSet.Concat(LowerCharsSet).Concat(NumbersSet).Concat(SpecialCharsSet).ToArray();

            using (var r = new CryptoRandomGenerator())
            {
                int length = r.Next(32, 41);
                var u = Enumerable.Range(0, 5).Select(i => UpperCharsSet[r.Next(0, UpperCharsSet.Length)]);
                var l = Enumerable.Range(0, 1).Select(i => LowerCharsSet[r.Next(0, LowerCharsSet.Length)]);
                var n = Enumerable.Range(0, 1).Select(i => NumbersSet[r.Next(0, NumbersSet.Length)]);
                var s = Enumerable.Range(0, 4).Select(i => SpecialCharsSet[r.Next(0, SpecialCharsSet.Length)]);
                var a = Enumerable.Range(0, length - 11).Select(i => All[r.Next(0, All.Length)]);
                return Encoding.ASCII.GetString(u.Concat(l).Concat(n).Concat(s).Concat(a).Shuffle().ToArray());
            }
        }

        public static string NewApiKey(int length = 64)
        {
            // Using Crypto Random Generator to fetch bytes.
            var r = new RNGCryptoServiceProvider();
            byte[] buff = new byte[length];
            r.GetBytes(buff);
            r.Dispose();
            return Convert.ToBase64String(buff);
        }

        public static void ClickOnce_SetAddRemoveProgramsIcon()
        {
            if (ApplicationDeployment.IsNetworkDeployed && ApplicationDeployment.CurrentDeployment.IsFirstRun)
            {
                try
                {
                    using (RegistryKey myUninstallKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall"))
                    {
                        foreach (var subKeyName in myUninstallKey.GetSubKeyNames())
                        {
                            using (RegistryKey myKey = myUninstallKey.OpenSubKey(subKeyName, true))
                            {
                                object myValue = myKey.GetValue("DisplayName");
                                if (myValue != null && myValue.ToString() == Utils.ProductName)
                                {
                                    myKey.SetValue("DisplayIcon", Application.ExecutablePath);
                                    break;
                                }
                            }
                        }
                    }
                }
                catch { }
            }
        }

        public static X509Certificate2 SelectCertFromStore(StoreName name, StoreLocation location, string vaultAlias, IntPtr hwndParent)
        {
            X509Store store = new X509Store(name, location);
            store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
            X509Certificate2Collection notExpiredAndSortedCerts = new X509Certificate2Collection(
                (from cert in store.Certificates.Find(X509FindType.FindByTimeValid, DateTime.Now, false).Cast<X509Certificate2>()
                 orderby string.IsNullOrEmpty(cert.FriendlyName) ? cert.GetNameInfo(X509NameType.SimpleName, false) : cert.FriendlyName descending
                 select cert).ToArray());
            X509Certificate2Collection selected = X509Certificate2UI.SelectFromCollection(notExpiredAndSortedCerts, Utils.AppName,
                $"Select a certificate from the {location}\\{name} store that you would like to add to {vaultAlias}", X509SelectionFlag.SingleSelection, hwndParent);
            return (1 == selected.Count) ? selected[0] : null;
        }

        /// <summary>
        /// Set specified hyperlink as HTML, Text formats and .URL file in the Clipboard
        /// </summary>
        /// <param name="link">Hyperlink to set</param>
        /// <param name="name">Name of the link</param>
        public static void ClipboardSetHyperlink(string link, string name)
        {
            // HTML format which works fine with any Office product
            const string html = @"Version:0.9
                StartHTML:<<<<<<<1
                EndHTML:<<<<<<<2
                StartFragment:<<<<<<<3
                EndFragment:<<<<<<<4
                SourceURL: {0}
                <html>
                <body>
                <!--StartFragment-->
                <a href='{0}'>{1}</a>
                <!--EndFragment-->
                </body>
                </html>";
            var dataObj = new DataObject("Preferred DropEffect", DragDropEffects.Move); // "Cut" file to clipboard
            dataObj.SetData(DataFormats.Text, link);
            dataObj.SetData(DataFormats.UnicodeText, link);
            // Add HTML format and .URL as a file
            dataObj.SetData(DataFormats.Html, string.Format(html, link, name));
            var tempPath = Path.Combine(Path.GetTempPath(), name + ContentType.KeyVaultLink.ToExtension());
            File.WriteAllText(tempPath, $"[InternetShortcut]\r\nURL={link}\r\nIconIndex=47\r\nIconFile=%SystemRoot%\\system32\\SHELL32.dll");
            var sc = new StringCollection();
            sc.Add(tempPath);
            dataObj.SetFileDropList(sc);
            Clipboard.SetDataObject(dataObj, true);
        }

        public static void ClearCliboard(TimeSpan interval, string md5)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo(Path.Combine(Application.StartupPath, "ClearClipboard.exe"), $"{interval} {md5}")
            {
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false,
                LoadUserProfile = false
            };
            Process.Start(sInfo);
        }

        public static void LaunchPowerShell(string vaultsJsonFile, string firstVaultName, string secondVaultName)
        {
            string vaultPs1 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Vault.ps1");
            ProcessStartInfo sInfo = new ProcessStartInfo("powershell.exe",
                $"-NoExit -NoProfile -NoLogo -ExecutionPolicy Unrestricted -File \"{vaultPs1}\" \"{vaultsJsonFile}\" \"{firstVaultName}\" \"{secondVaultName}\"")
            {
                UseShellExecute = true,
                LoadUserProfile = true
            };
            Process.Start(sInfo);
        }

        public static void ShowToast(string body)
        {
            // Get a toast XML template
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastImageAndText02);

            // Fill in the text elements
            XmlNodeList stringElements = toastXml.GetElementsByTagName("text");
            stringElements[0].AppendChild(toastXml.CreateTextNode(Utils.AppName));
            stringElements[1].AppendChild(toastXml.CreateTextNode(body));

            // Absolute path to an image
            var imagePath = "file:///" + Path.ChangeExtension(Application.ExecutablePath, ".png");
            XmlNodeList imageElements = toastXml.GetElementsByTagName("image");
            imageElements[0].Attributes.GetNamedItem("src").NodeValue = imagePath;

            // Create and show the toast
            ToastNotification toast = new ToastNotification(toastXml);
            ToastNotificationManager.CreateToastNotifier(Utils.AppName).Show(toast);
        }
    }
}
