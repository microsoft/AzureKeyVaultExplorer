// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Globalization;
using System.Linq;
using Microsoft.Vault.Library;
using Microsoft.Vault.Core;
using Azure.Security.KeyVault.Certificates;

namespace Microsoft.Vault.Explorer
{
    #region ObservableCustomCollection, ExpandableCollectionObjectConverter and ExpandableCollectionEditor
    /// <summary>
    /// Simple wrapper on top of ObservableCollection, so we can enforce some validation logic plus register for:
    /// protected event PropertyChangedEventHandler PropertyChanged;
    /// </summary>
    /// <typeparam name="T">type of the item in the collection</typeparam>
    [TypeConverter(typeof(ExpandableCollectionObjectConverter))]
    public abstract class ObservableCustomCollection<T> : ObservableCollection<T>, ICustomTypeDescriptor where T : class
    {
        private PropertyChangedEventHandler _propertyChanged;
        protected abstract PropertyDescriptor GetPropertyDescriptor(T item);

        public ObservableCustomCollection() : base() { }

        public ObservableCustomCollection(IEnumerable<T> collection) : base(collection) { }

        public void SetPropertyChangedEventHandler(PropertyChangedEventHandler propertyChanged)
        {
            _propertyChanged = propertyChanged;
            PropertyChanged += propertyChanged;
        }

        public PropertyChangedEventHandler GetLastPropertyChangedEventHandler() => _propertyChanged;

        public void AddOrReplace(T item)
        {
            int i = IndexOf(item);
            if (i == -1) Add(item); else SetItem(i, item);
        }

        public void AddOrKeep(T item)
        {
            int i = IndexOf(item);
            if (i == -1) Add(item);
        }

        public T GetOrNull(T item)
        {
            int i = IndexOf(item);
            return (i == -1) ? null : this[i];
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

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return new PropertyDescriptorCollection((from item in this select GetPropertyDescriptor(item)).ToArray());
        }

        #endregion
    }

    /// <summary>
    /// Shows number of items in the collection in the PropertyGrid item
    /// </summary>
    public class ExpandableCollectionObjectConverter : ExpandableObjectConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) =>
            (destinationType == typeof(string) && value is ICollection) ? $"{(value as ICollection).Count} item(s)" : base.ConvertTo(context, culture, value, destinationType);
    }

    /// <summary>
    /// Our collection editor, that will force refresh the expandable properties in case collection was changed
    /// </summary>
    /// <typeparam name="T">type of the collection</typeparam>
    /// <typeparam name="U">type of the item in the collection</typeparam>
    public class ExpandableCollectionEditor<T, U> : CollectionEditor where T : ObservableCustomCollection<U> where U : class
    {
        public ExpandableCollectionEditor(Type type) : base(type) { }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            T oldCollection = value as T;
            bool changed = false;
            var lastHandler = oldCollection.GetLastPropertyChangedEventHandler();
            oldCollection.SetPropertyChangedEventHandler((s, e) => { changed = true; });
            var collection = base.EditValue(context, provider, value);
            // If something was changed in the collection we always return a new value (copy ctor), to force refresh the expandable properties and force PropertyChanged chain
            if (changed)
            {
                T newCollection = (T)Activator.CreateInstance(typeof(T), (IEnumerable<U>)collection);
                newCollection.SetPropertyChangedEventHandler(lastHandler);
                lastHandler.Invoke(newCollection, new PropertyChangedEventArgs("Count"));
                return newCollection;
            }
            else
            {
                return collection;
            }
        }
    }

    #endregion

    #region TagItems

    [Editor(typeof(ExpandableCollectionEditor<ObservableTagItemsCollection, TagItem>), typeof(UITypeEditor))]
    public class ObservableTagItemsCollection : ObservableCustomCollection<TagItem>
    {
        public ObservableTagItemsCollection() : base() { }

        public ObservableTagItemsCollection(IEnumerable<TagItem> collection) : base(collection) { }

        protected override PropertyDescriptor GetPropertyDescriptor(TagItem item) => new ReadOnlyPropertyDescriptor(item.Name, item.Value);

        protected override void InsertItem(int index, TagItem item)
        {
            if (this.Count >= Consts.MaxNumberOfTags)
            {
                throw new ArgumentOutOfRangeException("Tags.Count", $"Too many tags, maximum number of tags for secret is only {Consts.MaxNumberOfTags}");
            }
            base.InsertItem(index, item);
        }
    }

    public partial class TagItem
    {
        private string _name;
        private string _value;
        private List<TagValueListItems> _valueList = new List<TagValueListItems>();
        private static Dictionary<string, List<TagValueListItems>> _valueListDictionary = new Dictionary<string, List<TagValueListItems>>();
        private bool _isList = false;
        
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

        [Category("Value")]
        [TypeConverter(typeof(TagValueListItemConverter))]
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

       
        [Category("ValueList")]
        [Browsable(false)]
        public List<TagValueListItems> ValueList
        {
            get
            {
                return _valueList;
            }
            set
            {
                _valueList = value;
            }
        }

        [Browsable(false)]
        public Dictionary<string, List<TagValueListItems>> ValueListDictionary
        {
            get
            {
                return _valueListDictionary;
            }
            set
            {
                //if (value != null) { _isList = true; }
                _valueListDictionary = value;
            }
        }

        [Category("ValueList")]
        [Browsable(false)]
        public bool IsList
        {
            get
            {
                return _isList;
            }
            set
            {
                _isList = value;
            }

        }
        
        

        public TagItem() : this("name", "") { }

        public TagItem(KeyValuePair<string, string> kvp) : this(kvp.Key, kvp.Value) { }

        public TagItem(string name, string value) : this(name, value, new Dictionary<string, List<TagValues>>()) { }

        public TagItem(string name, string value, Dictionary<string, List<TagValues>> valueList)
        {
            Name = name;
            Value = Utils.ConvertToValidTagValue(value);
            
            //return _valueArray;
            if (valueList.Count > 0)
            {
                List<TagValues> tagValues;
                List<TagValueListItems> tagValueListItems = new List<TagValueListItems>();
                if (valueList.TryGetValue(name, out tagValues))
                {
                    if (!_valueListDictionary.ContainsKey(name))
                    {
                        foreach (var x in tagValues)
                        {
                            tagValueListItems.Add(new TagValueListItems(x.ToString()));
                        }
                        // Sort the value list before adding
                        _valueListDictionary.Add(name, tagValueListItems.OrderBy(o => o.Value).ToList());
                        ValueListDictionary = _valueListDictionary;
                    }
                    _isList = true;
                }

                /*
                foreach (var s in valueList)
                {
                    
                    if (ValueList.Count() < valueList.Count())
                    {
                        //ValueList.Add(new TagValueListItems(s));
                    }
                    _isList = true;
                }
                */
            }

            //ValueList = valueList;

            
    }


        public override string ToString() => $"{Name}";

        public override bool Equals(object obj) => Equals(obj as TagItem);

        public bool Equals(TagItem ti) => (ti != null) && (0 == string.Compare(ti.Name, Name, true));

        public override int GetHashCode() => Name.GetHashCode();

        public class TagValueListItems
        {
            public string Value;
            public override String ToString() => Value;

            public TagValueListItems(string value) { Value = value; }
            public TagValueListItems() : base() { }
        }
        public static List<TagValues> taglist = new List<TagValues>();
        public class TagValueListItemConverter : TypeConverter
        {
            public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
            {
                var x = context.Instance.ToString();
                if (_valueListDictionary.ContainsKey(context.Instance.ToString()))
                {
                    return true;
                }
                return false;
            }

            public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
            {
                return false;
            }

            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                return new StandardValuesCollection(_valueListDictionary[context.Instance.ToString()]);
            }
            
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                if (sourceType == typeof(string))
                {
                    return true;
                }
                return base.CanConvertFrom(context, sourceType);
            }

            public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
            {
                if (value is string)
                {
                    //List<TagValueListItems> tvli; = _valueListDictionary[context.Instance.ToString()];
                    List<TagValueListItems> tvli; 
                    _valueListDictionary.TryGetValue(context.Instance.ToString(), out tvli);

                    // If the value is not in the list dictionary, just return the string value
                    if (tvli == null) { return value; }

                    // If the dictionary has the right key, try to get the value selected from the dictionary
                    foreach (TagValueListItems tv in tvli)
                    {
                        if (tv.Value == (string)value)
                        {
                            return tv.Value;
                        }
                    }
                    
                    // If the value was not in the dictionary, return the value anyway.
                    return value;
                    
                }
                return base.ConvertFrom(context, culture, value);
            }
            /*
            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                if (destinationType == typeof(string))
                {
                    return true;
                }
                return base.CanConvertFrom(context, destinationType);
            }

            public override bool IsValid(ITypeDescriptorContext context, object value)
            {
                return true;
            }
            */
        }
    }

    #endregion

    #region LifetimeActionItems

    [Editor(typeof(ExpandableCollectionEditor<ObservableLifetimeActionsCollection, LifetimeActionItem>), typeof(UITypeEditor))]
    public class ObservableLifetimeActionsCollection : ObservableCustomCollection<LifetimeActionItem>
    {
        public ObservableLifetimeActionsCollection() : base() { }

        public ObservableLifetimeActionsCollection(IEnumerable<LifetimeActionItem> collection) : base(collection) { }

        protected override PropertyDescriptor GetPropertyDescriptor(LifetimeActionItem item) =>
            new ReadOnlyPropertyDescriptor(item.ToString(), $"DaysBeforeExpiry={Utils.NullableIntToString(item.DaysBeforeExpiry)}, LifetimePercentage={Utils.NullableIntToString(item.LifetimePercentage)}");
    }

    public class LifetimeActionTypeEnumConverter : CustomEnumTypeConverter<CertificatePolicyAction> { }

    [DefaultProperty("Type")]
    [Description("Action and its trigger that will be performed by Key Vault over the lifetime of a certificate.")]
    public class LifetimeActionItem
    {
        [Category("Action")]
        public CertificatePolicyAction? Type { get; set; }

        [Category("Trigger")]
        public int? DaysBeforeExpiry { get; set; }

        [Category("Trigger")]
        public int? LifetimePercentage { get; set; }

        public override string ToString() => Type?.ToString();
    }

    #endregion
}
