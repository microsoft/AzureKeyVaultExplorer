using Microsoft.Azure.KeyVault;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace VaultExplorer
{
    /// <summary>
    /// Secret list view item which also presents itself nicely to PropertyGrid
    /// </summary>
    public class SecretListViewItem : ListViewItem, ICustomTypeDescriptor
    {
        public readonly SecretAttributes Attributes;
        public readonly string ContentType;
        public readonly string Id;
        public readonly SecretIdentifier Identifier;
        public readonly Dictionary<string, string> Tags;

        public SecretListViewItem(SecretItem si) : base(si.Identifier.Name)
        {
            Attributes = si.Attributes;
            ContentType = si.ContentType;
            Id = si.Id;
            Identifier = si.Identifier;
            Tags = si.Tags;

            Name = si.Identifier.Name;
            SubItems.Add(Utils.NullableDateTimeToString(si.Attributes.Updated));
            SubItems.Add(Utils.GetChangedBy(si.Tags));
        }
        public SecretListViewItem(Secret s) : base(s.SecretIdentifier.Name)
        {
            Attributes = s.Attributes;
            ContentType = s.ContentType;
            Id = s.Id;
            Identifier = s.SecretIdentifier;
            Tags = s.Tags;

            Name = s.SecretIdentifier.Name;
            SubItems.Add(Utils.NullableDateTimeToString(s.Attributes.Updated));
            SubItems.Add(Utils.GetChangedBy(s.Tags));
        }

        public void RefreshAndSelect()
        {
            EnsureVisible();
            Focused = Selected = false;
            Focused = Selected = true;
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
                new ReadOnlyPropertyDescriptor("Name", Identifier.Name),
                new ReadOnlyPropertyDescriptor("Identifier", Id),
                new ReadOnlyPropertyDescriptor("Creation time", Utils.NullableDateTimeToString(Attributes.Created)),
                new ReadOnlyPropertyDescriptor("Last updated time", Utils.NullableDateTimeToString(Attributes.Updated)),
                new ReadOnlyPropertyDescriptor("Enabled", Attributes.Enabled),
                new ReadOnlyPropertyDescriptor("Valid from time (UTC)", Attributes.NotBefore),
                new ReadOnlyPropertyDescriptor("Valid until time (UTC)", Attributes.Expires),
                new ReadOnlyPropertyDescriptor("Content Type", ContentType)
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
