using System.Collections.Generic;

namespace Microsoft.PS.Common.Vault.Explorer
{
    public class TagItem
    {
        public string Name { get; set; }

        public string Value { get; set; }

        public TagItem() : this("", "") { }

        public TagItem(KeyValuePair<string, string> kvp) : this(kvp.Key, kvp.Value) { }

        public TagItem(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}
