using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Deployment.Application;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Microsoft.PS.Common.Vault.Explorer
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
        public static string NullableDateTimeToString(DateTime? dt)
        {
            if (dt == null) return "Unknown";
            return dt.Value.ToLocalTime().ToString();
        }

        public static string CalculateMd5(string value)
        {
            byte[] buff = Encoding.UTF8.GetBytes(value);
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(buff);
                return BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();
            }
        }

        public static Dictionary<string, string> AddChangedBy(Dictionary<string, string> tags)
        {
            if (tags == null)
            {
                tags = new Dictionary<string, string>();
            }
            tags[Consts.ChangedByKey] = $"{Environment.UserDomainName}\\{Environment.UserName}";
            return tags;
        }

        public static string GetChangedBy(Dictionary<string, string> tags)
        {
            if ((tags == null) || (!tags.ContainsKey(Consts.ChangedByKey)))
            {
                return "";
            }
            return tags[Consts.ChangedByKey];
        }

        public static string GetMd5(Dictionary<string, string> tags)
        {
            if ((tags == null) || (!tags.ContainsKey(Consts.Md5Key)))
            {
                return "";
            }
            return tags[Consts.Md5Key];
        }

        public static string FullPathToJsonFile(string filename)
        {
            filename = Environment.ExpandEnvironmentVariables(filename);
            if (Path.IsPathRooted(filename)) return filename;
            filename = Path.Combine(Settings.Default.JsonConfigurationFilesRoot, filename);
            if (Path.IsPathRooted(filename)) return filename;
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);
        }

        public static T LoadFromJsonFile<T>(string filename)
        {
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(FullPathToJsonFile(filename)));
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

            var r = new Random();
            int length = r.Next(32, 41);

            var u = Enumerable.Range(0, 5).Select(i => UpperCharsSet[r.Next(0, UpperCharsSet.Length)]);
            var l = Enumerable.Range(0, 1).Select(i => LowerCharsSet[r.Next(0, LowerCharsSet.Length)]);
            var n = Enumerable.Range(0, 1).Select(i => NumbersSet[r.Next(0, NumbersSet.Length)]);
            var s = Enumerable.Range(0, 4).Select(i => SpecialCharsSet[r.Next(0, SpecialCharsSet.Length)]);
            var a = Enumerable.Range(0, length - 11).Select(i => All[r.Next(0, All.Length)]);

            return Encoding.ASCII.GetString(u.Concat(l).Concat(n).Concat(s).Concat(a).Shuffle().ToArray());
        }

        public static string NewApiKey(int length = 64)
        {
            var r = new Random();
            byte[] buff = new byte[length];
            r.NextBytes(buff);
            return Convert.ToBase64String(buff);
        }

        public static void ClickOnce_SetAddRemoveProgramsIcon()
        {
            if (ApplicationDeployment.IsNetworkDeployed && ApplicationDeployment.CurrentDeployment.IsFirstRun)
            {
                try
                {
                    RegistryKey myUninstallKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall");
                    foreach (var subKeyName in myUninstallKey.GetSubKeyNames())
                    {
                        RegistryKey myKey = myUninstallKey.OpenSubKey(subKeyName, true);
                        object myValue = myKey.GetValue("DisplayName");
                        if (myValue != null && myValue.ToString() == Utils.ProductName)
                        {
                            myKey.SetValue("DisplayIcon", Application.ExecutablePath);
                            break;
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
    }
}
