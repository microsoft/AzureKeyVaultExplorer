using Microsoft.Azure.KeyVault;
using Microsoft.PS.Common.Vault;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VaultExplorer
{
    public partial class MainForm : Form
    {
        private Vault _vault;
        private SortOrder _sortOder = SortOrder.Ascending;
        private int _sortColumn = 0;

        #region static

        private static StringDictionary s_geoRegions = new StringDictionary()
        {
            { "us", "west,east" },
            { "eu", "north,west" },
            { "as", "east,southeast" },
            { "jp", "east,west" },
            { "au", "east,southeast" },
            { "in", "south,west" },
        };

        private static ListViewItem SecretToListViewItem(string name, SecretAttributes sa)
        {
            var lvi = new ListViewItem(name);
            lvi.Name = name;
            lvi.SubItems.Add(NullableDateTimeToString(sa.Created));
            lvi.SubItems.Add(NullableDateTimeToString(sa.Updated));
            return lvi;
        }

        private static string NullableDateTimeToString(DateTime? dt)
        {
            if (dt == null) return "Unknown";
            return dt.Value.ToLocalTime().ToString();
        }

        #endregion

        public MainForm()
        {
            InitializeComponent();
            uxComboBoxEnv.SelectedIndex = 0;
            uxComboBoxGeo.SelectedIndex = 0;
        }

        private async void uxButtonList_Click(object sender, EventArgs e)
        {
            uxButtonList.Enabled = false;
            string geo = ((string)uxComboBoxGeo.SelectedItem).Substring(0, 2);
            string env = (string)uxComboBoxEnv.SelectedItem;

            try
            {
                Cursor.Current = Cursors.WaitCursor;

                _vault = new Vault(geo, env, s_geoRegions[geo]);
                uxListViewSecrets.BeginUpdate();
                uxListViewSecrets.Items.Clear();

                foreach (var s in await _vault.ListSecretsAsync())
                {
                    uxListViewSecrets.Items.Add(SecretToListViewItem(s.Identifier.Name, s.Attributes));
                }

                uxButtonAdd.Enabled = true;
            }
            finally
            {
                uxListViewSecrets.EndUpdate();
                uxButtonList.Enabled = true;
                Cursor.Current = Cursors.Default;
            }
        }

        private async void uxListViewSecrets_SelectedIndexChanged(object sender, EventArgs e)
        {
            uxButtonDelete.Enabled = uxButtonRefresh.Enabled = uxButtonCopy.Enabled = (uxListViewSecrets.SelectedItems.Count == 1);
            if (uxListViewSecrets.SelectedItems.Count > 0)
            {
                try
                {
                    uxButtonSave.Enabled = false;
                    Cursor.Current = Cursors.WaitCursor;
                    string secretName = uxListViewSecrets.SelectedItems[0].Text;
                    var secret = await _vault.GetSecretAsync(secretName);
                    var secretObject = new SecretObject(secret, SecretObject_PropertyChanged);
                    uxPropertyGridSecret.SelectedObject = secretObject;
                }
                finally
                {
                    Cursor.Current = Cursors.Default;
                }
            }
        }

        private void SecretObject_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            uxButtonSave.Enabled = true;
        }

        private async void uxButtonAdd_Click(object sender, EventArgs e)
        {
            NewSecret nsDlg = new NewSecret();
            if ((nsDlg.ShowDialog() == DialogResult.OK) &&
                (!uxListViewSecrets.Items.ContainsKey(nsDlg.SecretName) ||
                (uxListViewSecrets.Items.ContainsKey(nsDlg.SecretName) && 
                (MessageBox.Show($"Are you sure you want to replace secret '{nsDlg.SecretName}' with new value?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes))))
            {
                try
                {
                    Cursor.Current = Cursors.WaitCursor;
                    Secret s = await _vault.SetSecretAsync(nsDlg.SecretName, nsDlg.SecretValue);
                    uxListViewSecrets.Items.RemoveByKey(nsDlg.SecretName);
                    var lvi = uxListViewSecrets.Items.Add(SecretToListViewItem(s.SecretIdentifier.Name, s.Attributes));
                    lvi.EnsureVisible();
                    lvi.Selected = true;
                }
                finally
                {
                    Cursor.Current = Cursors.Default;
                }
            }
        }

        private async void uxButtonDelete_Click(object sender, EventArgs e)
        {
            if (uxListViewSecrets.SelectedItems.Count == 1)
            {
                string secretName = uxListViewSecrets.SelectedItems[0].Text;
                if (MessageBox.Show($"Are you sure you want to delete secret '{secretName}'?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        Cursor.Current = Cursors.WaitCursor;
                        await _vault.DeleteSecretAsync(secretName);
                        uxListViewSecrets.Items.RemoveByKey(secretName);
                    }
                    finally
                    {
                        Cursor.Current = Cursors.Default;
                    }
                }
            }
        }

        private void uxListViewSecrets_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (_sortColumn == e.Column)
            {
                _sortOder = (_sortOder == SortOrder.Ascending) ? SortOrder.Descending : SortOrder.Ascending;
            }
            _sortColumn = e.Column;
            uxListViewSecrets.ListViewItemSorter = new ListViewItemComparer(e.Column, _sortOder);
        }

        private void uxButtonCopy_Click(object sender, EventArgs e)
        {
            SecretObject so = uxPropertyGridSecret.SelectedObject as SecretObject;
            if (so != null)
            {
                Clipboard.SetText(so.Value);
            }
        }

        private async void uxButtonSave_Click(object sender, EventArgs e)
        {
            uxButtonSave.Enabled = false;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                SecretObject so = uxPropertyGridSecret.SelectedObject as SecretObject;
                if (so != null)
                {
                    await _vault.SetSecretAsync(so.Name, so.Value, so.TagsToDictionary(), so.ContentType, so.ToSecretAttributes());
                }
            }
            finally
            {
                uxButtonSave.Enabled = true;
                Cursor.Current = Cursors.Default;
            }
        }

        private void uxButtonExit_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
