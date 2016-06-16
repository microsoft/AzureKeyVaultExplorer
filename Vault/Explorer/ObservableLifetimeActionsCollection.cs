using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.PS.Common.Vault.Explorer
{
    [DefaultProperty("Type")]
    [Description("Action and its trigger that will be performed by Key Vault over the lifetime of a certificate.")]
    public class LifetimeActionItem
    {
        [Category("Action")]
        public string Type { get; set; }

        [Category("Trigger")]
        public int? DaysBeforeExpiry { get; set; }

        [Category("Trigger")]
        public int? LifetimePercentage { get; set; }

        public override string ToString() => Type;
    }

    /// <summary>
    /// Simple wrapper on top of ObservableCollection, so we can enforce some validation logic plus register for:
    /// protected event PropertyChangedEventHandler PropertyChanged;
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [TypeConverter(typeof(ExpandableCollectionObjectConverter))]
    [Editor(typeof(LifetimeActionsCollectionEditor), typeof(UITypeEditor))]
    public class ObservableLifetimeActionsCollection : ObservableCollection<LifetimeActionItem>, ICustomTypeDescriptor
    {
        public ObservableLifetimeActionsCollection() : base() { }

        public ObservableLifetimeActionsCollection(IEnumerable<LifetimeActionItem> collection) : base(collection) { }

        public void SetPropertyChangedEventHandler(PropertyChangedEventHandler propertyChanged)
        {
            PropertyChanged += propertyChanged;
        }

        #region ICustomTypeDescriptor interface to show properties in PropertyGrid

        public string GetComponentName() => TypeDescriptor.GetComponentName(this, true);

        public EventDescriptor GetDefaultEvent() => TypeDescriptor.GetDefaultEvent(this, true);

        public string GetClassName() => TypeDescriptor.GetClassName(this, true);

        public EventDescriptorCollection GetEvents(Attribute[] attributes) => TypeDescriptor.GetEvents(this, attributes, true);

        public EventDescriptorCollection GetEvents() => TypeDescriptor.GetEvents(this, true);

        public TypeConverter GetConverter() => TypeDescriptor.GetConverter(this, true);

        public object GetPropertyOwner(PropertyDescriptor pd) => this;

        public AttributeCollection GetAttributes() => TypeDescriptor.GetAttributes(this, true);

        public object GetEditor(Type editorBaseType) => TypeDescriptor.GetEditor(this, editorBaseType, true);

        public PropertyDescriptor GetDefaultProperty() => null;

        public PropertyDescriptorCollection GetProperties() => GetProperties(new Attribute[0]);

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes) =>
            new PropertyDescriptorCollection((from lai in this select new ReadOnlyPropertyDescriptor(lai.Type, $"DaysBeforeExpiry={Utils.NullableIntToString(lai.DaysBeforeExpiry)}, LifetimePercentage={Utils.NullableIntToString(lai.LifetimePercentage)}")).ToArray());

        #endregion
    }

    public class LifetimeActionsCollectionEditor : CollectionEditor
    {
        public LifetimeActionsCollectionEditor(Type type) : base(type) { }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            ObservableLifetimeActionsCollection oc = value as ObservableLifetimeActionsCollection;
            bool changed = false;
            oc.SetPropertyChangedEventHandler((s, e) => { changed = true; });
            var collection = base.EditValue(context, provider, value);
            // If something was changed in the collection we always return a new value (copy ctor), to force refresh the expandable read only properties
            return (changed) ? new ObservableLifetimeActionsCollection((IEnumerable<LifetimeActionItem>)collection) : collection;
        }
    }
}

