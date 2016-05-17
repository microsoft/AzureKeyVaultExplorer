using System;
using System.ComponentModel;

namespace Microsoft.PS.Common.Vault.Explorer
{
    /// <summary>
    /// Simple wrapper on top of ObservableCollection, so we can enforce some validation logic plus register for:
    /// protected event PropertyChangedEventHandler PropertyChanged;
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObservableTagItemsCollection : System.Collections.ObjectModel.ObservableCollection<TagItem>
    {
        public ObservableTagItemsCollection() : base() { }

        public void SetPropertyChangedEventHandler(PropertyChangedEventHandler propertyChanged)
        {
            PropertyChanged += propertyChanged;
        }

        protected override void InsertItem(int index, TagItem item)
        {
            if (this.Count >= Consts.MaxNumberOfTags)
            {
                throw new ArgumentOutOfRangeException("Tags.Count", $"Too many tags, maximum number of tags for secret is only {Consts.MaxNumberOfTags}");
            }
            base.InsertItem(index, item);
        }

        public void AddOrReplace(TagItem item)
        {
            int i = IndexOf(item);
            if (i == -1)
            {
                Add(item);
            }
            else
            {
                SetItem(i, item);
            }
        }
    }
}
