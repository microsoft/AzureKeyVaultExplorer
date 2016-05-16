using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Microsoft.PS.Common.Vault.Explorer
{
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
}
