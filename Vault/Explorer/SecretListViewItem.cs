using Microsoft.Azure.KeyVault;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VaultExplorer
{
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
            SubItems.Add(Utils.NullableDateTimeToString(si.Attributes.Created));
            SubItems.Add(Utils.NullableDateTimeToString(si.Attributes.Updated));
            SubItems.Add(si.ContentType);
        }
        public SecretListViewItem(Secret s) : base(s.SecretIdentifier.Name)
        {
            Attributes = s.Attributes;
            ContentType = s.ContentType;
            Id = s.Id;
            Identifier = s.SecretIdentifier;
            Tags = s.Tags;

            Name = s.SecretIdentifier.Name;
            SubItems.Add(Utils.NullableDateTimeToString(s.Attributes.Created));
            SubItems.Add(Utils.NullableDateTimeToString(s.Attributes.Updated));
            SubItems.Add(s.ContentType);
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

        PropertyDescriptorCollection
            System.ComponentModel.ICustomTypeDescriptor.GetProperties()
        {
            return ((ICustomTypeDescriptor)this).GetProperties(new Attribute[0]);
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            List<PropertyDescriptor> properties = new List<PropertyDescriptor>()
            {
                new ReadOnlyPropertyDescriptor("Name", Identifier.Name),
                new ReadOnlyPropertyDescriptor("Identifier", Id),
                new ReadOnlyPropertyDescriptor("Creation time (UTC)", Attributes.Created),
                new ReadOnlyPropertyDescriptor("Last updated time (UTC)", Attributes.Updated),
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
