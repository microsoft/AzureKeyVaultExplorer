// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 

using Microsoft.Azure.KeyVault;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Vault.Library;

namespace Microsoft.Vault.Explorer
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
        public string Md5 => Microsoft.Vault.Library.Utils.CalculateMd5(RawValue);

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

        [Browsable(false)]
        public bool IsExpirationValid => ((NotBefore ?? DateTime.MinValue) < (Expires ?? DateTime.MaxValue))
            && ((Expires ?? DateTime.MaxValue) <= (SecretKind.MaxExpiration == TimeSpan.MaxValue ? DateTime.MaxValue : DateTime.UtcNow + SecretKind.MaxExpiration));

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

        public virtual DataObject GetClipboardValue()
        {
            var dataObj = new DataObject("Preferred DropEffect", DragDropEffects.Move); // "Cut" file to clipboard
            if (_contentType.IsCertificate()) // Common logic for .cer and .pfx
            {
                var tempPath = Path.Combine(Path.GetTempPath(), Name + _contentType.ToExtension());
                SaveToFile(tempPath);
                var sc = new StringCollection();
                sc.Add(tempPath);
                dataObj.SetFileDropList(sc);
            }
            return dataObj;
        }

        public abstract void SaveToFile(string fullName);

        protected abstract IEnumerable<TagItem> GetValueBasedCustomTags();

        public abstract void PopulateCustomTags();

        public abstract void AddOrUpdateSecretKind(SecretKind sk);

        public abstract string AreCustomTagsValid();

        public abstract void PopulateExpiration();

        public Dictionary<string, string> ToTagsDictionary()
        {
            var result = new Dictionary<string, string>();
            // Add all user and custom tags
            foreach (var tagItem in Tags)
            {
                result[tagItem.Name] = tagItem.Value;
            }
            // Add all custom tags which are based on the secret value
            foreach (var tagItem in GetValueBasedCustomTags())
            {
                result[tagItem.Name] = tagItem.Value;
            }
            // Note: Md5 and ChangeBy tags are taken care in the Microsoft.Vault.Library
            return result;
        }

        public string GetFileName() => Name + _contentType.ToExtension();

        public void CopyToClipboard(bool showToast)
        {
            var dataObj = GetClipboardValue();
            if (null != dataObj)
            {
                Clipboard.SetDataObject(dataObj, true);
                Utils.ClearCliboard(Settings.Default.CopyToClipboardTimeToLive, Microsoft.Vault.Library.Utils.CalculateMd5(dataObj.GetText()));
                if (showToast)
                {
                    Utils.ShowToast($"{(_contentType.IsCertificate() ? "Certificate" : "Secret")} {Name} copied to clipboard");
                }
            }
        }

        public string GetLinkAsInternetShortcut() => $"[InternetShortcut]\nURL={new VaultHttpsUri(Identifier.Identifier).VaultLink}";
    }
}
