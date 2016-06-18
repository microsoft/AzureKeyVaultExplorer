using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Microsoft.PS.Common.Vault.Explorer
{
    public enum ItemDialogBaseMode
    {
        New,
        Edit
    };

    public partial class ItemDialogBase<T> : FormTelemetry where T : PropertyObject
    {
        protected readonly ISession _session;
        protected readonly ItemDialogBaseMode _mode;
        protected bool _changed;
        public T PropertyObject { get; protected set; }

        public ItemDialogBase() { }

        public ItemDialogBase(ISession session, string title, ItemDialogBaseMode mode)
        {
            InitializeComponent();
            _session = session;
            Text = title;
            _mode = mode;
            uxTextBoxName.Font = Settings.Default.SecretFont;
        }

        protected virtual void InvalidateOkButton()
        {
            uxButtonOK.Enabled = _changed && PropertyObject.IsNameValid && PropertyObject.IsValueValid;
        }

        protected virtual void uxTextBoxName_TextChanged(object sender, EventArgs e)
        {
            _changed = true;
        }

        protected virtual void uxLinkLabelSecretKind_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            uxMenuSecretKind.Show(uxLinkLabelSecretKind, 0, uxLinkLabelSecretKind.Height);
        }

        protected virtual void uxMenuSecretKind_ItemClicked(object sender, ToolStripItemClickedEventArgs e) { }

        protected virtual void OnVersionChange(CustomVersion cv) { }

        protected void uxMenuVersions_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            var cv = (CustomVersion)e.ClickedItem;
            if (cv.Checked) return; // Same item was clicked
            foreach (var item in uxMenuVersions.Items) ((CustomVersion)item).Checked = false;

            OnVersionChange(cv);

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
