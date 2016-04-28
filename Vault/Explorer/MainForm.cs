using Microsoft.PS.Common.Vault;
using System;
using System.Collections.Generic;
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

        public MainForm()
        {
            InitializeComponent();
            uxComboBoxEnv.SelectedIndex = 0;
            uxComboBoxGeo.SelectedIndex = 0;
        }

        private void uxButtonRefresh_Click(object sender, EventArgs e)
        {
            string geo = ((string)uxComboBoxGeo.SelectedItem).Substring(0, 2);
            string env = (string)uxComboBoxEnv.SelectedItem;

            Cursor.Current = Cursors.WaitCursor;
            try
            {
                _vault = new Vault(geo, env, "west,east");
                uxListViewSecrets.BeginUpdate();
                uxListViewSecrets.Items.Clear();
                foreach (var s in _vault.ListSecretsAsync().GetAwaiter().GetResult())
                {
                    var lvi = new ListViewItem(s.Identifier.Name);
                    lvi.SubItems.Add(NullableDateTimeToString(s.Attributes.Created));
                    lvi.SubItems.Add(NullableDateTimeToString(s.Attributes.Updated));
                    uxListViewSecrets.Items.Add(lvi);
                }
                uxListViewSecrets.EndUpdate();
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private static string NullableDateTimeToString(DateTime? dt)
        {
            if (dt == null) return "NA";
            return dt.Value.ToLocalTime().ToString();
        }

        private void uxListViewSecrets_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (uxListViewSecrets.SelectedItems.Count > 0)
            {
                string secretName = uxListViewSecrets.SelectedItems[0].Text;
                var secret = _vault.GetSecretAsync(secretName).GetAwaiter().GetResult();
                uxPropertyGridSecret.SelectedObject = new SecretObject(secret);
            }
        }

        private void uxPropertyGridSecret_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            // TBD - save
        }
    }
}
