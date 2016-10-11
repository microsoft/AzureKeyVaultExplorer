// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 

using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Microsoft.Vault.Explorer
{
    public class NullableDateTimePickerEditor : UITypeEditor
    {
        IWindowsFormsEditorService editorService;
        ToolTip expirationToolTip = new ToolTip();
        DateTimePicker picker = new DateTimePicker();

        public NullableDateTimePickerEditor()
        {
            expirationToolTip.ShowAlways = true;
            picker.Format = DateTimePickerFormat.Long;
            picker.ValueChanged += Picker_ValueChanged;
        }

        private void Picker_ValueChanged(object sender, EventArgs e)
        {
            TimeSpan ts = picker.Value - DateTime.UtcNow;
            expirationToolTip.SetToolTip(picker, Utils.ExpirationToString(ts));
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) => UITypeEditorEditStyle.DropDown;

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
                value = new DateTime?(picker.Value);
            }

            return value;
        }
    }
}
