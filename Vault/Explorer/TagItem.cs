using System.Collections.Generic;

namespace Microsoft.PS.Common.Vault.Explorer
{
    public class TagItem
    {
        public string Name { get; set; }

        public string Value { get; set; }

        public TagItem()
        {
            Name = "";
            Value = "";
        }

        public TagItem(KeyValuePair<string, string> kvp)
        {
            Name = kvp.Key;
            Value = kvp.Value;
        }
    }
}
