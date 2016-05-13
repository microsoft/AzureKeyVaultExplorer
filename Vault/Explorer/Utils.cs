using Microsoft.Azure.KeyVault;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Microsoft.PS.Common.Vault.Explorer
{
    public static class Utils
    {
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
    }
}
