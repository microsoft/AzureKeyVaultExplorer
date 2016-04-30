using Microsoft.Azure.KeyVault;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace VaultExplorer
{
    public class TagItem
    {
        public string Key { get; set; }

        public string Value { get; set; }

        public TagItem()
        {
            Key = "";
            Value = "";
        }

        public TagItem(KeyValuePair<string, string> kvp)
        {
            Key = kvp.Key;
            Value = kvp.Value;
        }
    }

    public class NullableDateTimePickerEditor : UITypeEditor
    {
        IWindowsFormsEditorService editorService;
        DateTimePicker picker = new DateTimePicker();

        public NullableDateTimePickerEditor()
        {
            picker.Format = DateTimePickerFormat.Long;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider != null)
            {
                this.editorService = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
            }

            if (this.editorService != null)
            {
                if (value != null)
                {
                    picker.Value = Convert.ToDateTime(value);
                }
                this.editorService.DropDownControl(picker);
                value = new Nullable<DateTime>(picker.Value);
            }

            return value;
        }
    }

    [DefaultProperty("Name")]
    public class SecretObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));

        [DisplayName("Name")]
        [Browsable(false)]
        public string Name { get; set; }

        private ObservableCollection<TagItem> _tags;
        [DisplayName("Custom Tags")]
        public ObservableCollection<TagItem> Tags
        {
            get
            {
                return _tags;
            }
            set
            {
                _tags = value;
                NotifyPropertyChanged(nameof(Tags));
            }
        }

        private bool? _enabled;
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

        private string _contentType;
        [DisplayName("Content Type")]
        public string ContentType
        {
            get
            {
                return _contentType;
            }
            set
            {
                _contentType = value;
                NotifyPropertyChanged(nameof(ContentType));
            }
        }

        private string _value;
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
                _value = value;
                NotifyPropertyChanged(nameof(Value));
            }
        }

        public SecretObject(Secret secret, PropertyChangedEventHandler propertyChanged)
        {
            // get and set
            _tags = new ObservableCollection<TagItem>();
            if (null != secret.Tags)
            {
                foreach (var kvp in secret.Tags)
                {
                    _tags.Add(new TagItem(kvp));
                }
            }
            _tags.SetPropertyChangedEventHandler(propertyChanged);
            _enabled = secret.Attributes.Enabled;
            _expires = secret.Attributes.Expires;
            _notBefore = secret.Attributes.NotBefore;
            _contentType = secret.ContentType;
            _value = secret.Value;

            PropertyChanged += propertyChanged;
        }

        public Dictionary<string, string> TagsToDictionary()
        {
            if (_tags.Count == 0)
            {
                return null;
            }
            var result = new Dictionary<string, string>();
            foreach (var tagItem in _tags)
            {
                result.Add(tagItem.Key, tagItem.Value);
            }
            return result;
        }

        public SecretAttributes ToSecretAttributes() => new SecretAttributes()
        {
            Enabled = _enabled,
            Expires = _expires,
            NotBefore = _notBefore
        };
    }
}
