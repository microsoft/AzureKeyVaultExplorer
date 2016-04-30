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
    }
}
