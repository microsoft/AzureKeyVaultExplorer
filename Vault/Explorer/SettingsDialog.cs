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
    public partial class SettingsDialog : Form
    {
        public SettingsDialog()
        {
            InitializeComponent();
            Settings.Default.PropertyChanged += CurrentSettings_PropertyChanged;
            uxPropertyGrid.SelectedObject = Settings.Default;
        }

        private void CurrentSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            uxButtonOK.Enabled = true;
        }

        private void SettingsDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            Settings.Default.PropertyChanged -= CurrentSettings_PropertyChanged;
        }

        private void uxButtonOK_Click(object sender, EventArgs e)
        {
            Settings.Default.Save();
        }
    }
}
