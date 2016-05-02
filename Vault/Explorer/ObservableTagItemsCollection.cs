using System;
using System.ComponentModel;

namespace VaultExplorer
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
            if (this.Count >= Utils.MaxNumberOfTags)
            {
                throw new ArgumentOutOfRangeException("Tags.Count", $"Too many tags, maximum number of tags for secret is only {Utils.MaxNumberOfTags}");
            }
            if (item.Name.Length > Utils.MaxTagNameLength)
            {
                throw new ArgumentOutOfRangeException("Name.Length", $"Tag name '{item.Name}' is too long, name can be up to {Utils.MaxTagNameLength} chars");
            }
            if (item.Value.Length > Utils.MaxTagValueLength)
            {
                throw new ArgumentOutOfRangeException("Value.Length", $"Tag value '{item.Value}' is too long, value can be up to {Utils.MaxTagValueLength} chars");
            }
            base.InsertItem(index, item);
        }
    }
}
