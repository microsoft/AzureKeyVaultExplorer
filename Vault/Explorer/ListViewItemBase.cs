using Microsoft.Azure.KeyVault;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Microsoft.PS.Common.Vault.Explorer
{
    /// <summary>
    /// Base list view item which also presents itself nicely to PropertyGrid
    /// </summary>
    public abstract class ListViewItemBase : ListViewItem, ICustomTypeDescriptor
    {
        public const int SearchResultsGroup = 0;
        public const int FavoritesGroup = 1;
        public const int CertificatesGroup = 2;
        public const int KeyVaultCertificatesGroup = 3;
        public const int SecretsGroup = 4;

        public readonly ISession Session;
        public readonly int GroupIndex;
        public readonly VaultHttpsUri VaultHttpsUri;
        public readonly IDictionary<string, string> Tags;
        public readonly bool Enabled;
        public readonly DateTime? Created;
        public readonly DateTime? Updated;
        public readonly DateTime? NotBefore;
        public readonly DateTime? Expires;

        protected ListViewItemBase(ISession session, int groupIndex,
            ObjectIdentifier identifier, IDictionary<string, string> tags, bool? enabled,
            DateTime? created, DateTime? updated, DateTime? notBefore, DateTime? expires) : base(identifier.Name)
        {
            Session = session;
            GroupIndex = groupIndex;
            VaultHttpsUri = new VaultHttpsUri(identifier.Identifier);
            Tags = tags;
            Enabled = enabled ?? true;
            Created = created;
            Updated = updated;
            NotBefore = notBefore;
            Expires = expires;

            ImageIndex = Enabled ? 2 * GroupIndex - 3 : 2 * GroupIndex - 2;
            ForeColor = Enabled ? SystemColors.WindowText : SystemColors.GrayText;
            Font = Active ? Font : new Font(this.Font, FontStyle.Strikeout);

            Name = identifier.Name;
            SubItems.Add(new ListViewSubItem(this, Utils.NullableDateTimeToString(updated)) { Tag = updated }); // Add Tag so ListViewItemSorter will sort datetime correctly
            SubItems.Add(ChangedBy);

            string status = (Enabled ? "Enabled" : "Disabled") + (Active ? ", Active" : ", Expired");
            ToolTipText += string.Format("Status:\t\t\t{0}\nCreation time:\t\t{1}\nLast updated time:\t{2}",
                status,
                Utils.NullableDateTimeToString(created),
                Utils.NullableDateTimeToString(updated));

            _favorite = FavoriteSecretUtil.Contains(Session.CurrentVaultAlias.Alias, Name);
            _searchResult = false;
            SetGroup();
        }

        public ListViewGroupCollection Groups => Session.ListViewSecrets.Groups;

        public string Id => VaultHttpsUri.ToString();

        public string ChangedBy => Common.Vault.Utils.GetChangedBy(Tags);

        public string Md5 => Utils.GetMd5(Tags);

        public string Link => $"https://aka.ms/ve?{VaultHttpsUri.VaultLink}";

        /// <summary>
        /// True only if current time is within the below range, or range is NULL
        /// [NotBefore] Valid from time (UTC) 
        /// [Expires] Valid until time (UTC) 
        /// </summary>
        public bool Active => (DateTime.UtcNow >= (NotBefore ?? DateTime.MinValue)) && (DateTime.UtcNow <= (Expires ?? DateTime.MaxValue));

        private static string[] GroupIndexToName = new string[] { "s", "f", "certificate", "key vault certificate", "secret" };
        public string Kind => GroupIndexToName[GroupIndex];

        public void RefreshAndSelect()
        {
            Session.ListViewSecrets.MultiSelect = false;
            EnsureVisible();
            Focused = Selected = false;
            Focused = Selected = true;
            Session.ListViewSecrets.MultiSelect = true;
        }

        private void SetGroup()
        {
            Group = _searchResult ? Groups[SearchResultsGroup] : _favorite ? Groups[FavoritesGroup] : Groups[GroupIndex];
        }

        private bool _searchResult;
        public bool SearchResult
        {
            get
            {
                return _searchResult;
            }
            set
            {
                _searchResult = value;
                SetGroup();
            }
        }

        private bool _favorite;
        public bool Favorite
        {
            get
            {
                return _favorite;
            }
            set
            {
                _favorite = value;
                SetGroup();
                if (_favorite)
                {
                    FavoriteSecretUtil.Add(Session.CurrentVaultAlias.Alias, Name);
                }
                else
                {
                    FavoriteSecretUtil.Remove(Session.CurrentVaultAlias.Alias, Name);
                }
            }
        }

        public bool Contains(Regex regexPattern)
        {
            if (string.IsNullOrWhiteSpace(regexPattern.ToString()))
                return false;
            foreach (ReadOnlyPropertyDescriptor ropd in GetProperties(null))
            {
                if (regexPattern.Match($"{ropd.Name}={ropd.Value}").Success)
                    return true;
            }
            return false;
        }

        public static bool VerifyDuplication(ISession session, string oldName, PropertyObject soNew)
        {
            string newMd5 = soNew.Md5;

            // Check if we already have *another* secret with the same name
            if ((oldName != soNew.Name) && (session.ListViewSecrets.Items.ContainsKey(soNew.Name) &&
                (MessageBox.Show($"Are you sure you want to replace existing item '{soNew.Name}' with new value?", Utils.AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)))
            {
                return false;
            }

            // Detect dups by Md5
            var sameSecretsList = from slvi in session.ListViewSecrets.Items.Cast<ListViewItemBase>() where (slvi.Md5 == newMd5) && (slvi.Name != oldName) && (slvi.Name != soNew.Name) select slvi.Name;
            if ((sameSecretsList.Count() > 0) &&
                (MessageBox.Show($"There are {sameSecretsList.Count()} other item(s) in the vault which has the same Md5: {newMd5}.\nHere the name(s) of the other items:\n{string.Join(", ", sameSecretsList)}\nAre you sure you want to add or update item {soNew.Name} and have a duplication of secrets?", Utils.AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != DialogResult.Yes))
            {
                return false;
            }
            return true;
        }

        protected abstract IEnumerable<PropertyDescriptor> GetCustomProperties();

        public abstract Task<PropertyObject> GetAsync(CancellationToken cancellationToken);

        public abstract Task<ListViewItemBase> ToggleAsync(CancellationToken cancellationToken);

        public abstract Task<ListViewItemBase> ResetExpirationAsync(CancellationToken cancellationToken);

        public abstract Task<ListViewItemBase> DeleteAsync(CancellationToken cancellationToken);

        public abstract Task<IEnumerable<object>> GetVersionsAsync(CancellationToken cancellationToken);

        public abstract Form GetEditDialog(string name, IEnumerable<object> versions);

        public abstract Task<ListViewItemBase> UpdateAsync(object originalObject, PropertyObject newObject, CancellationToken cancellationToken);

        #region ICustomTypeDescriptor interface to show properties in PropertyGrid

        public string GetComponentName() => TypeDescriptor.GetComponentName(this, true);

        public EventDescriptor GetDefaultEvent() => TypeDescriptor.GetDefaultEvent(this, true);

        public string GetClassName() => TypeDescriptor.GetClassName(this, true);

        public EventDescriptorCollection GetEvents(Attribute[] attributes) => TypeDescriptor.GetEvents(this, attributes, true);

        public EventDescriptorCollection GetEvents() => TypeDescriptor.GetEvents(this, true);

        public TypeConverter GetConverter() => TypeDescriptor.GetConverter(this, true);

        public object GetPropertyOwner(PropertyDescriptor pd) => Id;

        public AttributeCollection GetAttributes() => TypeDescriptor.GetAttributes(this, true);

        public object GetEditor(Type editorBaseType) => TypeDescriptor.GetEditor(this, editorBaseType, true);

        public PropertyDescriptor GetDefaultProperty() => null;

        public PropertyDescriptorCollection GetProperties() => GetProperties(new Attribute[0]);

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            List<PropertyDescriptor> properties = new List<PropertyDescriptor>()
            {
                new ReadOnlyPropertyDescriptor("Name", Name),
                new ReadOnlyPropertyDescriptor("Link", Link),
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
