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

        [JsonIgnore]
        public bool IsCertificate => !string.IsNullOrEmpty(CertificateFormat);

        public SecretKind() : base("Custom")
        {
            Alias = "Custom";
            ToolTipText = Description = "The name must be a string 1-127 characters in length containing only 0-9, a-z, A-Z, and -.";
            NameRegex = Consts.ValidSecretNameRegex;
            ValueRegex = new Regex("^.{0,1048576}$", RegexOptions.Singleline | RegexOptions.Compiled);
            CertificateFormat = null;
        }

        [JsonConstructor]
        public SecretKind(string alias, string description, string nameRegex, string valueRegex, string certificateFormat) : base(alias)
        {
            Alias = alias;
            ToolTipText = Description = description;
            NameRegex = new Regex(nameRegex, RegexOptions.Singleline | RegexOptions.Compiled);
            ValueRegex = new Regex(valueRegex, RegexOptions.Singleline | RegexOptions.Compiled);
            CertificateFormat = certificateFormat;
        }

        public override string ToString() => Text + " secret name" + Utils.DropDownSuffix;
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
