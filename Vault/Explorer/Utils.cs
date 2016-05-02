using Microsoft.Azure.KeyVault;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VaultExplorer
{
    public static class Utils
    {
        public const int MaxSecretValueLength = 25 * 1024; // 25 KB

        public const int MaxNumberOfTags = 15;

        public const int MaxTagNameLength = 256;

        public const int MaxTagValueLength = 256;

        public static readonly StringDictionary GeoRegions = new StringDictionary()
        {
            { "us", "west,east" },
            { "eu", "north,west" },
            { "as", "east,southeast" },
            { "jp", "east,west" },
            { "au", "east,southeast" },
            { "in", "south,west" },
        };

        public static string NullableDateTimeToString(DateTime? dt)
        {
            if (dt == null) return "Unknown";
            return dt.Value.ToLocalTime().ToString();
        }

        private const string ChangedByKey = "ChangedBy";

        public static Dictionary<string, string> AddChangedBy(Dictionary<string, string> tags)
        {
            if (tags == null)
            {
                tags = new Dictionary<string, string>();
            }
            tags[ChangedByKey] = $"{Environment.UserDomainName}\\{Environment.UserName}";
            return tags;
        }

        public static string GetChangedBy(Dictionary<string, string> tags)
        {
            if ((tags == null) || (!tags.ContainsKey(ChangedByKey)))
            {
                return "";
            }
            return tags[ChangedByKey];
        }
    }
}
