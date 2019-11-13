// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Vault.Library;
using Microsoft.Vault.Core;

namespace Microsoft.Vault.Explorer
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

        [JsonProperty]
        public readonly Dictionary<string,List<TagValues>> ValueList = new Dictionary<string, List<TagValues>>();

        [JsonProperty]
        public readonly List<TagValues> CustomTagValueList = new List<TagValues>();

        [JsonConstructor]
        public CustomTag(string name, string defaultValue, string valueRegex, string[] valueList)
        {
            Guard.ArgumentNotNullOrWhitespace(name, nameof(name));
            if (name.Length > Consts.MaxTagNameLength)
            {
                throw new ArgumentOutOfRangeException("name.Length", $"Tag name '{name}' is too long, name can be up to {Consts.MaxTagNameLength} chars");
            }
            Name = name;
            DefaultValue = defaultValue;
            ValueRegex = new Regex(valueRegex, RegexOptions.Singleline | RegexOptions.Compiled);

            // Convert the array to a list
            if (valueList != null)
            {
                foreach (string v in valueList)
                {
                    CustomTagValueList.Add(new TagValues(v));
                }
                ValueList.Add(name,CustomTagValueList);
            }

        }

        public override string ToString() => Name;

        public TagItem ToTagItem() => new TagItem(Name, DefaultValue, ValueList);

        public string Verify(TagItem tagItem, bool required)
        {
            if (null == tagItem)
            {
                return required ? $"Tag {Name} is required\n" : "";
            }
            var m = ValueRegex.Match(tagItem.Value);
            return m.Success ? "" : $"Tag {Name} value must match the following regex: {ValueRegex}\n";
        }

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

    // Used for storing a list of values for a tag
    public class TagValues
    {
        public String tagvalue;
        public override String ToString()
        {
            return tagvalue;
        }
        public TagValues(string tag) { tagvalue = tag; }
        public TagValues() : base() { }
    }

    

}
