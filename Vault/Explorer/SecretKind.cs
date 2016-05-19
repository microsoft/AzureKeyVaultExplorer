using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Microsoft.PS.Common.Vault.Explorer
{
    [JsonObject]
    public class SecretKind : ToolStripMenuItem
    {
        [JsonProperty]
        public readonly string Alias;

        [JsonProperty]
        public readonly string Description;

        [JsonProperty]
        public readonly Regex NameRegex;

        [JsonProperty]
        public readonly Regex ValueRegex;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public readonly string CertificateFormat;

        [JsonConstructor]
        public SecretKind(string alias, string description, string nameRegex, string valueRegex, string certificateFormat) : base(alias)
        {
            Alias = alias;
            Description = description;
            NameRegex = new Regex(nameRegex, RegexOptions.Singleline | RegexOptions.Compiled);
            ValueRegex = new Regex(valueRegex, RegexOptions.Singleline | RegexOptions.Compiled);
            CertificateFormat = certificateFormat;
        }
    }

    [JsonDictionary]
    public class SecretKinds : Dictionary<string, SecretKind>
    {
        [JsonConstructor]
        public SecretKinds(IDictionary<string, SecretKind> secretKinds) : base(secretKinds, StringComparer.CurrentCultureIgnoreCase)
        {
            foreach (string secretKindName in Keys)
            {
                Guard.ArgumentNotNullOrWhitespace(secretKindName, nameof(secretKindName));
            }
        }
    }
}
