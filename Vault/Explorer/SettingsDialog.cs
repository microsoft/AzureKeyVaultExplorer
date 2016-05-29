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
        private readonly Settings _currentSettings;

        public SettingsDialog()
        {
            InitializeComponent();
            _currentSettings = new Settings();
            _currentSettings.PropertyChanged += CurrentSettings_PropertyChanged;
            uxPropertyGrid.SelectedObject = _currentSettings;
        }

        private void CurrentSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            uxButtonOK.Enabled = true;
        }

        private void uxButtonOK_Click(object sender, EventArgs e)
        {
            _currentSettings.Save();
            Settings.Default.Reload();
        }
    }
}
