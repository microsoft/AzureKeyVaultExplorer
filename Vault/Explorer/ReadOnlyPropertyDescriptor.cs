using System;
using System.ComponentModel;

namespace VaultExplorer
{
    public class ReadOnlyPropertyDescriptor : PropertyDescriptor
    {
        private readonly string _name;
        private readonly object _value;

        internal ReadOnlyPropertyDescriptor(string name, object value) : base(name, null)
        {
            _name = name;
            _value = value;
        }

        public override Type PropertyType => (_value == null) ? typeof(string) : _value.GetType();

        public override void SetValue(object component, object value)
        {
            throw new InvalidOperationException();
        }

        public override object GetValue(object component) => _value;

        public override bool IsReadOnly => true;

        public override Type ComponentType => null;

        public override bool CanResetValue(object component) => false;

        public override void ResetValue(object component) { }

        public override bool ShouldSerializeValue(object component) => false;
    }
}
