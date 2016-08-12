using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Microsoft.PS.Common.Vault.Explorer
{
    [JsonObject]
    public class CustomTag
    {
        [JsonProperty]
        public readonly string Name;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public readonly string DefaultValue = "";

        [JsonProperty]
        public readonly Regex ValueRegex;

        [JsonConstructor]
        public CustomTag(string name, string defaultValue, string valueRegex)
        {
            Guard.ArgumentNotNullOrWhitespace(name, nameof(name));
            if (name.Length > Consts.MaxTagNameLength)
            {
                throw new ArgumentOutOfRangeException("name.Length", $"Tag name '{name}' is too long, name can be up to {Consts.MaxTagNameLength} chars");
            }
            Name = name;
            DefaultValue = defaultValue;
            ValueRegex = new Regex(valueRegex, RegexOptions.Singleline | RegexOptions.Compiled);
        }

        public override string ToString() => Name;
    }

    [JsonDictionary]
    public class CustomTags : Dictionary<string, CustomTag>
    {
        public CustomTags() : base() { }

        [JsonConstructor]
        public CustomTags(IDictionary<string, CustomTag> customTags) : base(customTags, StringComparer.CurrentCultureIgnoreCase)
        {
            foreach (string customTagKey in Keys)
            {
                Guard.ArgumentNotNullOrWhitespace(customTagKey, nameof(customTagKey));
            }
        }
    }
}
