using Microsoft.Azure.KeyVault;
using Microsoft.PS.Common.Vault;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VaultExplorer
{
    public partial class MainForm : Form
    {
        private Vault _vault;
        private SortOrder _sortOder = SortOrder.Ascending;
        private int _sortColumn = 0;

        public MainForm()
        {
            InitializeComponent();
            uxComboBoxEnv.SelectedIndex = 0;
            uxComboBoxGeo.SelectedIndex = 0;
        }

        private UxOperation NewUxOperation(ToolStripItem controlToToggle) => new UxOperation(controlToToggle, uxStatusLabel);

        private async void uxButtonList_Click(object sender, EventArgs e)
        {
            string geo = ((string)uxComboBoxGeo.SelectedItem).Substring(0, 2);
            string env = (string)uxComboBoxEnv.SelectedItem;

            using (NewUxOperation(uxButtonList))
            {
                _vault = new Vault(geo, env, Utils.GeoRegions[geo]);
                //uxListViewSecrets.BeginUpdate();
                uxListViewSecrets.Items.Clear();
                foreach (var s in await _vault.ListSecretsAsync())
                {
                    uxListViewSecrets.Items.Add(new SecretListViewItem(s));
                }
                //uxListViewSecrets.EndUpdate();
                uxButtonAdd.Enabled = uxMenuItemAddSecret.Enabled = uxMenuItemAddCertificate.Enabled = true;
            }
        }

        private void uxListViewSecrets_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool flag = (uxListViewSecrets.SelectedItems.Count == 1);
            uxButtonEdit.Enabled = uxButtonDelete.Enabled = uxButtonCopy.Enabled = flag;
            uxMenuItemEdit.Enabled = uxMenuItemDelete.Enabled = uxMenuItemCopy.Enabled = flag;
            uxPropertyGridSecret.SelectedObject = flag ? uxListViewSecrets.SelectedItems[0] : null;
        }

        private async Task AddOrUpdateSecret(SecretObject so)
        {
            Secret s = await _vault.SetSecretAsync(so.Name, so.Value, so.TagsToDictionary(), so.ContentType, so.ToSecretAttributes());
            uxListViewSecrets.Items.RemoveByKey(so.Name);
            var lvi = uxListViewSecrets.Items.Add(new SecretListViewItem(s));
            lvi.EnsureVisible();
            lvi.Focused = lvi.Selected = true;
        }

        private async void uxButtonAddSecret_Click(object sender, EventArgs e)
        {
            SecretDialog nsDlg = new SecretDialog();
            if ((nsDlg.ShowDialog() == DialogResult.OK) &&
                (!uxListViewSecrets.Items.ContainsKey(nsDlg.SecretObject.Name) ||
                (uxListViewSecrets.Items.ContainsKey(nsDlg.SecretObject.Name) && 
                (MessageBox.Show($"Are you sure you want to replace secret '{nsDlg.SecretObject.Name}' with new value?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes))))
            {
                using (NewUxOperation(uxButtonAdd))
                {
                    await AddOrUpdateSecret(nsDlg.SecretObject);
                }
            }
        }
        private void uxAddCertificate_Click(object sender, EventArgs e)
        {
            MessageBox.Show("To be implemeneted, stay tuned");
        }

        private async void uxButtonEdit_Click(object sender, EventArgs e)
        {
            if (uxListViewSecrets.SelectedItems.Count == 1)
            {
                string secretName = uxListViewSecrets.SelectedItems[0].Text;
                using (NewUxOperation(uxButtonEdit))
                {
                    var s = await _vault.GetSecretAsync(secretName);
                    SecretDialog nsDlg = new SecretDialog(s);
                    if (nsDlg.ShowDialog() == DialogResult.OK)
                    {
                        await AddOrUpdateSecret(nsDlg.SecretObject);
                    }
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
                    using (NewUxOperation(uxButtonDelete))
                    {
                        await _vault.DeleteSecretAsync(secretName);
                        uxListViewSecrets.Items.RemoveByKey(secretName);
                    }
                }
            }
        }

        private async void uxButtonCopy_Click(object sender, EventArgs e)
        {
            if (uxListViewSecrets.SelectedItems.Count == 1)
            {
                string secretName = uxListViewSecrets.SelectedItems[0].Text;
                using (NewUxOperation(uxButtonCopy))
                {
                    var s = await _vault.GetSecretAsync(secretName);
                    Clipboard.SetText(s.Value);
                }
            }
        }

        private void uxButtonExit_Click(object sender, EventArgs e)
        {
            Close();
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
    }
}
