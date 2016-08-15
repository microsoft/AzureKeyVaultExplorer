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

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public readonly string[] RequiredCustomTags;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public readonly string[] OptionalCustomTags;

        public SecretKind() : base("Custom")
        {
            Alias = "Custom";
            ToolTipText = Description = "The name must be a string 1-127 characters in length containing only 0-9, a-z, A-Z, and -.";
            NameRegex = Consts.ValidSecretNameRegex;
            ValueRegex = new Regex("^.{0,1048576}$", RegexOptions.Singleline | RegexOptions.Compiled);
            CertificateFormat = null;
            RequiredCustomTags = new string[0];
            OptionalCustomTags = new string[0];
        }

        [JsonConstructor]
        public SecretKind(string alias, string description, string nameRegex, string valueRegex, string certificateFormat, string[] requiredCustomTags, string[] optionalCustomTags) : base(alias)
        {
            Alias = alias;
            ToolTipText = Description = description;
            NameRegex = new Regex(nameRegex, RegexOptions.Singleline | RegexOptions.Compiled);
            ValueRegex = new Regex(valueRegex, RegexOptions.Singleline | RegexOptions.Compiled);
            CertificateFormat = certificateFormat;
            RequiredCustomTags = requiredCustomTags ?? new string[0];
            OptionalCustomTags = optionalCustomTags ?? new string[0];
            if (RequiredCustomTags.Length + OptionalCustomTags.Length > Consts.MaxNumberOfTags)
            {
                throw new ArgumentOutOfRangeException("Total CustomTags.Length", $"Too many custom tags for secret kind {alias}, maximum number of tags for secret is only {Consts.MaxNumberOfTags}");
            }
        }

        public override string ToString() => Text + " secret name" + Utils.DropDownSuffix;
    }

    [JsonDictionary]
    public class SecretKinds : Dictionary<string, SecretKind>
    {
        public SecretKinds() : base() { }

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
