using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.PS.Common.Vault.Explorer
{
    public class TagItem
    {
        private string _name;
        private string _value;

        [Category("Tag")]
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                Guard.ArgumentNotNullOrEmptyString(value, nameof(value));
                if (value.Length > Consts.MaxTagNameLength)
                {
                    throw new ArgumentOutOfRangeException("Name.Length", $"Tag name '{value}' is too long, name can be up to {Consts.MaxTagNameLength} chars");
                }
                _name = value;
            }
        }

        [Category("Tag")]
        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                Guard.ArgumentNotNull(value, nameof(value));
                if (value.Length > Consts.MaxTagValueLength)
                {
                    throw new ArgumentOutOfRangeException("Value.Length", $"Tag value '{value}' is too long, value can be up to {Consts.MaxTagValueLength} chars");
                }
                _value = value;
            }
        }

        public TagItem() : this("name", "") { }

        public TagItem(KeyValuePair<string, string> kvp) : this(kvp.Key, kvp.Value) { }

        public TagItem(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public override string ToString() => $"{Name}";

        public override bool Equals(object obj) => Equals(obj as TagItem);

        public bool Equals(TagItem ti) => (ti != null) && (0 == string.Compare(ti.Name, Name, true));

        public override int GetHashCode() => Name.GetHashCode();
    }
}
