using Microsoft.Azure.KeyVault;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaultExplorer
{
    public class TagItem
    {
        public string Key { get; set; }

        public string Value { get; set; }

        public TagItem()
        {
            Key = "";
            Value = "";
        }

        public TagItem(KeyValuePair<string, string> kvp)
        {
            Key = kvp.Key;
            Value = kvp.Value;
        }
    }

    [DefaultProperty("Name")]
    public class SecretObject
    {
        [Description("The secret Name")]
        public string Name { get; set; }

        [Description("The secret Value")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string Value { get; set; }

        [Description("The secret Id")]
        public string Id { get; set; }

        [Description("The content type of the secret")]
        public string ContentType { get; set; }

        [Description("The tags for the secret")]
        public List<TagItem> Tags { get; set; }

        [Description("Creation time in UTC")]
        public DateTime? Created { get; }

        [Description("Determines whether the key is enabled")]
        public bool? Enabled { get; set; }

        [Description("Expiry date in UTC")]
        public DateTime? Expires { get; set; }

        [Description("Not before date in UTC")]
        public DateTime? NotBefore { get; set; }

        [Description("Last updated time in UTC")]
        public DateTime? Updated { get; }

        public SecretIdentifier SecretIdentifier { get; }

        public SecretObject(Secret secret)
        {
            Name = secret.SecretIdentifier.Name;
            Value = secret.Value;
            Id = secret.Id;
            ContentType = secret.ContentType;
            Tags = new List<TagItem>();
            if (null != secret.Tags)
            {
                secret.Tags.Select((kvp) => { Tags.Add(new TagItem(kvp)); return kvp; });
            }
            Created = secret.Attributes.Created;
            Enabled = secret.Attributes.Enabled;
            Expires = secret.Attributes.Expires;
            NotBefore = secret.Attributes.NotBefore;
            Updated = secret.Attributes.Updated;
        }
    }
}
