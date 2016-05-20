using Microsoft.Azure.KeyVault;
using Microsoft.PS.Common.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Microsoft.PS.Common.Vault.Explorer
{
    public static class Utils
    {
        public const string DataFormatSecret = "VaultExplorerSecret";

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

        public static T LoadFromJsonFile<T>(string filename)
        {
            var jsonFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(jsonFile));
        }
    }
}
