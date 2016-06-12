using Microsoft.Azure.KeyVault;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Microsoft.PS.Common.Vault.Explorer
{
    public abstract class ListViewItemBase : ListViewItem, ICustomTypeDescriptor
    {
        public const int FavoritesGroup = 0;
        public const int CertificatesGroup = 1;
        public const int KeyVaultCertificatesGroup = 2;
        public const int SecretsGroup = 3;

        public readonly VaultAlias VaultAlias;
        public readonly ListViewGroupCollection Groups;
        public readonly int GroupIndex;
        public readonly ObjectIdentifier Identifier;
        public readonly IDictionary<string, string> Tags;
        public readonly bool Enabled;
        public readonly DateTime? Created;
        public readonly DateTime? Updated;
        public readonly DateTime? NotBefore;
        public readonly DateTime? Expires;

        protected ListViewItemBase(VaultAlias vaultAlias, ListViewGroupCollection groups, int groupIndex,
            ObjectIdentifier identifier, IDictionary<string, string> tags, bool? enabled,
            DateTime? created, DateTime? updated, DateTime? notBefore, DateTime? expires) : base(identifier.Name)
        {
            VaultAlias = vaultAlias;
            Groups = groups;
            GroupIndex = groupIndex;
            Identifier = identifier;
            Tags = tags;
            Enabled = enabled ?? true;
            Created = created;
            Updated = updated;
            NotBefore = notBefore;
            Expires = expires;

            ImageIndex = Enabled ? 2 * GroupIndex - 1 : 2 * GroupIndex;
            ForeColor = Enabled ? SystemColors.WindowText : SystemColors.GrayText;

            Name = identifier.Name;
            SubItems.Add(new ListViewSubItem(this, Utils.NullableDateTimeToString(updated)) { Tag = updated }); // Add Tag so ListViewItemSorter will sort datetime correctly
            SubItems.Add(ChangedBy);

            ToolTipText = string.Format("Creation time:\t\t{0}\nLast updated time:\t{1}",
                Utils.NullableDateTimeToString(created),
                Utils.NullableDateTimeToString(updated));

            Group = Groups[FavoriteSecretUtil.Contains(VaultAlias.Alias, Name) ? FavoritesGroup : GroupIndex];
        }

        public string Id => Identifier.Identifier;

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
                ImageIndex = value ? 0 : Enabled ? 2 * GroupIndex - 1 : 2 * GroupIndex;
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
                Group = value ? Groups[FavoritesGroup] : Groups[GroupIndex];
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

        protected abstract IEnumerable<PropertyDescriptor> GetCustomProperties();

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
                new ReadOnlyPropertyDescriptor("Creation time", Utils.NullableDateTimeToString(Created)),
                new ReadOnlyPropertyDescriptor("Last updated time", Utils.NullableDateTimeToString(Updated)),
                new ReadOnlyPropertyDescriptor("Enabled", Enabled),
                new ReadOnlyPropertyDescriptor("Valid from time (UTC)", NotBefore),
                new ReadOnlyPropertyDescriptor("Valid until time (UTC)", Expires),
            };
            properties.AddRange(GetCustomProperties());
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
