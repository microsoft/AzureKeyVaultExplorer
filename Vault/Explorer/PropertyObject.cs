using Microsoft.Azure.KeyVault;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.PS.Common.Vault.Explorer
{
    /// <summary>
    /// Base class to edit an object via PropertyGrid
    /// </summary>
    [DefaultProperty("Tags")]
    public abstract class PropertyObject : INotifyPropertyChanged
    {
        protected void NotifyPropertyChanged(string info) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));

        protected ContentType _contentType;

        public ContentType GetContentType() => _contentType;

        public event PropertyChangedEventHandler PropertyChanged;

        public readonly ObjectIdentifier Identifier;

        [DisplayName("Name")]
        [Browsable(false)]
        public string Name { get; set; }

        [Category("General")]
        [DisplayName("Custom Tags")]
        public ObservableTagItemsCollection Tags { get; set; }

        private bool? _enabled;
        [Category("General")]
        [DisplayName("Enabled")]
        public bool? Enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                _enabled = value;
                NotifyPropertyChanged(nameof(Enabled));
            }
        }

        private DateTime? _expires;
        [Category("General")]
        [DisplayName("Valid until time (UTC)")]
        [Editor(typeof(NullableDateTimePickerEditor), typeof(UITypeEditor))]
        public DateTime? Expires
        {
            get
            {
                return _expires;
            }
            set
            {
                _expires = value;
                NotifyPropertyChanged(nameof(Expires));
            }
        }

        private DateTime? _notBefore;
        [Category("General")]
        [DisplayName("Valid from time (UTC)")]
        [Editor(typeof(NullableDateTimePickerEditor), typeof(UITypeEditor))]
        public DateTime? NotBefore
        {
            get
            {
                return _notBefore;
            }
            set
            {
                _notBefore = value;
                NotifyPropertyChanged(nameof(NotBefore));
            }
        }

        /// <summary>
        /// Human readable value of the secret
        /// </summary>
        protected string _value;
        [DisplayName("Value")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        [Browsable(false)]
        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    NotifyPropertyChanged(nameof(Value));
                }
            }
        }

        /// <summary>
        /// Raw value to store in the vault
        /// </summary>
        [Browsable(false)]
        public string RawValue => _contentType.ToRawValue(_value);

        /// <summary>
        /// Md5 of the raw value
        /// </summary>
        [Browsable(false)]
        public string Md5 => Utils.CalculateMd5(RawValue);

        /// <summary>
        /// Current SecretKind for this secret object
        /// Note: NotifyPropertyChanged is NOT called upon set
        /// </summary>
        [Browsable(false)]
        public SecretKind SecretKind { get; set; }

        [Browsable(false)]
        public bool IsNameValid => (Name == null) ? false : SecretKind.NameRegex.IsMatch(Name);

        [Browsable(false)]
        public bool IsValueValid => (Value == null) ? false : SecretKind.ValueRegex.IsMatch(Value);

        protected PropertyObject(ObjectIdentifier identifier, IDictionary<string, string> tags, 
            bool? enabled, DateTime? expires, DateTime? notBefore,
            PropertyChangedEventHandler propertyChanged)
        {
            Identifier = identifier;
            Name = identifier?.Name;

            Tags = new ObservableTagItemsCollection();
            if (null != tags) foreach (var kvp in tags) Tags.Add(new TagItem(kvp));
            Tags.SetPropertyChangedEventHandler(propertyChanged);

            _enabled = enabled;
            _expires = expires;
            _notBefore = notBefore;

            SecretKind = new SecretKind(); // Default - Custom secret kind

            PropertyChanged += propertyChanged;
        }

        public abstract string GetKeyVaultFileExtension();

        public abstract string GetClipboardValue();

        public abstract void SaveToFile(string fullName);

        protected abstract IEnumerable<KeyValuePair<string, string>> GetCustomTags();

        public Dictionary<string, string> ToTagsDictionary()
        {
            var result = new Dictionary<string, string>();
            // Add all user tags
            foreach (var tagItem in Tags)
            {
                result[tagItem.Name] = tagItem.Value;
            }
            // Add all custom tags
            foreach (var kvp in GetCustomTags())
            {
                result[kvp.Key] = kvp.Value;
            }
            // Add Md5 and ChangedBy tags
            result[Consts.Md5Key] = Md5;
            return Utils.AddChangedBy(result);
        }

        public string GetFileName() => Name + _contentType.ToExtension();
    }
}
