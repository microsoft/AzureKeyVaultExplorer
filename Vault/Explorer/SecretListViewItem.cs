using Microsoft.Azure.KeyVault;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.PS.Common.Vault.Explorer
{
    /// <summary>
    /// Secret list view item which also presents itself nicely to PropertyGrid
    /// </summary>
    public class SecretListViewItem : ListViewItem, ICustomTypeDescriptor
    {
        private const int FavoritesGroup = 0;
        private const int OtherGroup = 1;

        public readonly VaultAlias VaultAlias;
        public readonly ListViewGroupCollection Groups;
        public readonly SecretIdentifier Identifier;
        public readonly SecretAttributes Attributes;
        public readonly string ContentTypeStr;
        public readonly ContentType ContentType;
        public readonly string Id;
        public readonly Dictionary<string, string> Tags;

        private readonly int BaseImageIndex;

        private SecretListViewItem(VaultAlias vaultAlias, ListViewGroupCollection groups, SecretIdentifier identifier, SecretAttributes attributes, string contentTypeStr, 
            Dictionary<string, string> tags) : base(identifier.Name)
        {
            VaultAlias = vaultAlias;
            Groups = groups;
            Identifier = identifier;
            Attributes = attributes;
            ContentTypeStr = contentTypeStr;
            ContentType = ContentTypeEnumConverter.GetValue(contentTypeStr);
            Id = identifier.Identifier;
            Tags = tags;

            BaseImageIndex = ContentType.IsCertificate() ? 3 : 1;
            ImageIndex = (Attributes.Enabled ?? true) ? BaseImageIndex : BaseImageIndex + 1;
            ForeColor = (Attributes.Enabled ?? true) ? SystemColors.WindowText : SystemColors.GrayText;

            Name = identifier.Name;
            SubItems.Add(new ListViewSubItem(this, Utils.NullableDateTimeToString(attributes.Updated)) { Tag = attributes.Updated }); // Add Tag so ListViewItemSorter will sort datetime correctly
            SubItems.Add(ChangedBy);

            ToolTipText = string.Format("Creation time:\t\t{0}\nLast updated time:\t{1}",
                Utils.NullableDateTimeToString(Attributes.Created),
                Utils.NullableDateTimeToString(Attributes.Updated));

            Group = Groups[FavoriteSecretUtil.Contains(VaultAlias.Alias, Name) ? FavoritesGroup : OtherGroup];
        }

        public SecretListViewItem(VaultAlias vaultAlias, ListViewGroupCollection groups, SecretItem si) : this(vaultAlias, groups, si.Identifier, si.Attributes, si.ContentType, si.Tags) { }

        public SecretListViewItem(VaultAlias vaultAlias, ListViewGroupCollection groups, Secret s) : this(vaultAlias, groups, s.SecretIdentifier, s.Attributes, s.ContentType, s.Tags) { }

        public string ChangedBy => Utils.GetChangedBy(Tags);

        public string Md5 => Utils.GetMd5(Tags);

        public void RefreshAndSelect()
        {
            ListView.MultiSelect = false;
            EnsureVisible();
            Focused = Selected = false;
            Focused = Selected = true;
            ListView.MultiSelect = true;
        }

        public bool Strikeout
        {
            get
            {
                return (ImageIndex == 0);
            }
            set
            {
                ForeColor = value ? SystemColors.GrayText : SystemColors.WindowText;
                ImageIndex = value ? 0 : (Attributes.Enabled ?? true) ? BaseImageIndex : BaseImageIndex + 1;
            }
        }

        public bool Favorite
        {
            get
            {
                return Group == Groups[FavoritesGroup];
            }
            set
            {
                Group = value ? Groups[FavoritesGroup] : Groups[OtherGroup];
                if (value)
                {
                    FavoriteSecretUtil.Add(VaultAlias.Alias, Name);
                }
                else
                {
                    FavoriteSecretUtil.Remove(VaultAlias.Alias, Name);
                }
            }
        }

        public bool Contains(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return true;
            foreach (var pd in GetProperties(null))
            {
                ReadOnlyPropertyDescriptor ropd = pd as ReadOnlyPropertyDescriptor;
                if ((ropd.Name.IndexOf(text, StringComparison.InvariantCultureIgnoreCase) >= 0) ||
                    (ropd.Value?.ToString().IndexOf(text, StringComparison.InvariantCultureIgnoreCase) >= 0))
                    return true;
            }
            return false;
        }

        #region ICustomTypeDescriptor interface to show properties in PropertyGrid

        public string GetComponentName() => TypeDescriptor.GetComponentName(this, true);

        public EventDescriptor GetDefaultEvent() => TypeDescriptor.GetDefaultEvent(this, true);

        public string GetClassName() => TypeDescriptor.GetClassName(this, true);

        public EventDescriptorCollection GetEvents(Attribute[] attributes) => TypeDescriptor.GetEvents(this, attributes, true);

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents() => TypeDescriptor.GetEvents(this, true);

        public TypeConverter GetConverter() => TypeDescriptor.GetConverter(this, true);

        public object GetPropertyOwner(PropertyDescriptor pd) => Id;

        public AttributeCollection GetAttributes() => TypeDescriptor.GetAttributes(this, true);

        public object GetEditor(Type editorBaseType) => TypeDescriptor.GetEditor(this, editorBaseType, true);

        public PropertyDescriptor GetDefaultProperty() => null;

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties() => ((ICustomTypeDescriptor)this).GetProperties(new Attribute[0]);

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            List<PropertyDescriptor> properties = new List<PropertyDescriptor>()
            {
                new ReadOnlyPropertyDescriptor("Name", Name),
                new ReadOnlyPropertyDescriptor("Identifier", Id),
                new ReadOnlyPropertyDescriptor("Creation time", Utils.NullableDateTimeToString(Attributes.Created)),
                new ReadOnlyPropertyDescriptor("Last updated time", Utils.NullableDateTimeToString(Attributes.Updated)),
                new ReadOnlyPropertyDescriptor("Enabled", Attributes.Enabled),
                new ReadOnlyPropertyDescriptor("Valid from time (UTC)", Attributes.NotBefore),
                new ReadOnlyPropertyDescriptor("Valid until time (UTC)", Attributes.Expires),
                new ReadOnlyPropertyDescriptor("Content Type", ContentTypeStr)
            };
            if (Tags != null)
            {
                foreach (var kvp in Tags)
                {
                    properties.Add(new ReadOnlyPropertyDescriptor(kvp.Key, kvp.Value));
                }
            }
            return new PropertyDescriptorCollection(properties.ToArray());
        }

        #endregion
    }
}
