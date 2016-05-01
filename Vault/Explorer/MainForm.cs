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
            bool itemSelected = (uxListViewSecrets.SelectedItems.Count == 1);
            bool secretEnabled = itemSelected ? (uxListViewSecrets.SelectedItems[0] as SecretListViewItem).Attributes.Enabled ?? true : false;
            uxButtonEdit.Enabled = uxButtonCopy.Enabled = uxMenuItemEdit.Enabled = uxMenuItemCopy.Enabled = secretEnabled;
            uxButtonDelete.Enabled = uxMenuItemDelete.Enabled = itemSelected;
            uxButtonToggle.Enabled = uxMenuItemToggle.Enabled = itemSelected;
            uxButtonToggle.Text = secretEnabled ? "Disabl&e" : "&Enable";
            uxMenuItemToggle.Text = uxButtonToggle.Text + "...";
            uxPropertyGridSecret.SelectedObject = itemSelected ? uxListViewSecrets.SelectedItems[0] : null;
        }

        private async Task AddOrUpdateSecret(Secret sOld, SecretObject soNew)
        {
            Secret s = null;
            // New secret, secret rename or new value
            if ((sOld == null) || (sOld.SecretIdentifier.Name != soNew.Name) || (sOld.Value != soNew.Value))
            {
                s = await _vault.SetSecretAsync(soNew.Name, soNew.Value, soNew.TagsToDictionary(), soNew.ContentType, soNew.ToSecretAttributes());
            }
            else // Same secret name and value
            {
                s = await _vault.UpdateSecretAsync(soNew.Name, soNew.TagsToDictionary(), soNew.ContentType, soNew.ToSecretAttributes());
            }
            string oldSecretName = sOld?.SecretIdentifier.Name;
            if ((oldSecretName != null) && (oldSecretName != soNew.Name)) // Delete old key
            {
                await _vault.DeleteSecretAsync(oldSecretName);
                uxListViewSecrets.Items.RemoveByKey(oldSecretName);
            }
            uxListViewSecrets.Items.RemoveByKey(soNew.Name);
            var slvi = new SecretListViewItem(s);
            uxListViewSecrets.Items.Add(slvi);
            slvi.RefreshAndSelect();
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
                    await AddOrUpdateSecret(null, nsDlg.SecretObject);
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
                var slvi = uxListViewSecrets.SelectedItems[0] as SecretListViewItem;
                if (slvi.Attributes.Enabled ?? true)
                {
                    using (NewUxOperation(uxButtonEdit))
                    {
                        var s = await _vault.GetSecretAsync(slvi.Name);
                        SecretDialog nsDlg = new SecretDialog(s);
                        if (nsDlg.ShowDialog() == DialogResult.OK)
                        {
                            await AddOrUpdateSecret(s, nsDlg.SecretObject);
                        }
                    }
                }
            }
        }

        private async void uxButtonToggle_Click(object sender, EventArgs e)
        {
            if (uxListViewSecrets.SelectedItems.Count == 1)
            {
                var slvi = uxListViewSecrets.SelectedItems[0] as SecretListViewItem;
                string action = (slvi.Attributes.Enabled ?? true) ? "disable" : "enable";
                if (MessageBox.Show($"Are you sure you want to {action} secret '{slvi.Name}'?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    using (NewUxOperation(uxButtonToggle))
                    {
                        Secret s = await _vault.UpdateSecretAsync(slvi.Name, Utils.AddChangedBy(slvi.Tags), null, new SecretAttributes() { Enabled = !slvi.Attributes.Enabled }); // Toggle only Enabled attribute
                        slvi = new SecretListViewItem(s);
                        uxListViewSecrets.Items.RemoveByKey(slvi.Name);
                        uxListViewSecrets.Items.Add(slvi);
                        slvi.RefreshAndSelect();
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
