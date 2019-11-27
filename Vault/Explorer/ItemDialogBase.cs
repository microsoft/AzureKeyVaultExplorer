// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Microsoft.Vault.Explorer
{
    public enum ItemDialogBaseMode
    {
        New,
        Edit
    };

    public partial class ItemDialogBase<T, U> : FormTelemetry where T : PropertyObject where U : class
    {
        protected readonly ISession _session;
        protected readonly ItemDialogBaseMode _mode;
        protected bool _changed;
        public U OriginalObject; //  Will be NULL in New mode and current value in case of Edit mode
        public T PropertyObject { get; protected set; }

        public ItemDialogBase() { }

        public ItemDialogBase(ISession session, string title, ItemDialogBaseMode mode)
        {
            InitializeComponent();
            _session = session;
            Text = title;
            _mode = mode;
        }

        protected virtual void InvalidateOkButton()
        {
            string tagsError = PropertyObject.AreCustomTagsValid();
            uxButtonOK.Enabled = _changed && PropertyObject.IsNameValid && PropertyObject.IsValueValid && 
                PropertyObject.IsExpirationValid && string.IsNullOrEmpty(tagsError);
        }

        protected virtual void uxTextBoxName_TextChanged(object sender, EventArgs e)
        {
            PropertyObject.Name = uxTextBoxName.Text;
            _changed = true;
            uxErrorProvider.SetError(uxTextBoxName, PropertyObject.IsNameValid ? null : $"Name must match the following regex:\n{PropertyObject.SecretKind.NameRegex}");
            InvalidateOkButton();
        }

        protected virtual void uxLinkLabelSecretKind_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            uxMenuSecretKind.Show(uxLinkLabelSecretKind, 0, uxLinkLabelSecretKind.Height);
        }

        protected virtual void uxMenuSecretKind_ItemClicked(object sender, ToolStripItemClickedEventArgs e) { }

        protected virtual Task<U> OnVersionChangeAsync(CustomVersion cv) => null;

        protected async void uxMenuVersions_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            var cv = (CustomVersion)e.ClickedItem;
            if (cv.Checked) return; // Same item was clicked
            foreach (var item in uxMenuVersions.Items) ((CustomVersion)item).Checked = false;

            var u = await OnVersionChangeAsync(cv);
            OriginalObject = (null == OriginalObject) ? u : OriginalObject;

            cv.Checked = true;
            uxLinkLabelValue.Text = cv.ToString();
            uxToolTip.SetToolTip(uxLinkLabelValue, cv.ToolTipText);
            _changed = (sender != null); // Sender will be NULL for the first time during Edit mode ctor
            InvalidateOkButton();
        }

        protected virtual ContextMenuStrip GetNewValueMenu() => null;

        private void uxLinkLabelValue_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            switch (_mode)
            {
                case ItemDialogBaseMode.New:
                    GetNewValueMenu()?.Show(uxLinkLabelValue, 0, uxLinkLabelValue.Height);
                    return;
                case ItemDialogBaseMode.Edit:
                    uxMenuVersions.Show(uxLinkLabelValue, 0, uxLinkLabelValue.Height);
                    return;
            }
        }
    }
}
