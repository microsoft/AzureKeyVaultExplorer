using Microsoft.Azure.KeyVault;
using Microsoft.PS.Common.Vault;
using Microsoft.PS.Common.Vault.Explorer.Properties;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Microsoft.PS.Common.Vault.Explorer
{
    using UISettings = Properties.Settings;

    public partial class MainForm : FormTelemetry
    {
        private VaultAlias _currentVaultAlias;
        private Vault _vault;
        private ListViewItemSorter _listViewItemSorter;
        private int _strikedoutSecrets;
        private string _clipboardValue;
        private Cursor _moveSecretCursor;
        private Cursor _moveValueCursor;
        private bool _keyDownOccured;
        private ToolStripButton uxButtonCancel;

        public MainForm()
        {
            InitializeComponent();
            Text = $"{Utils.AppName} ({Environment.UserDomainName}\\{Environment.UserName})";
            uxListViewSecrets.ListViewItemSorter = _listViewItemSorter = new ListViewItemSorter();

            ApplySettings();

            _moveSecretCursor = Utils.LoadCursorFromResource(Resources.move_secret);
            _moveValueCursor = Utils.LoadCursorFromResource(Resources.move_value);

            uxButtonCancel = new ToolStripButton("", Resources.cancel)
            {
                Margin = new Padding(0, 0, 20, 0),
                Size = new Size(uxStatusProgressBar.Width, uxStatusProgressBar.Width),
                ToolTipText = "Cancel operation",
                Visible = false
            };
            uxStatusStrip.Items.Insert(3, uxButtonCancel);
        }

        private void ApplySettings()
        {
            Size = UISettings.Default.MainFormWindowSize;
            _listViewItemSorter.SortOrder = UISettings.Default.MainFormSecretsSorting;
            uxButtonCopy.ToolTipText = uxMenuItemCopy.ToolTipText = $"Copy secret value to clipboard for {Settings.Default.CopyToClipboardTimeToLive.TotalSeconds} seconds";
            uxTimerClearClipboard.Interval = (int)Settings.Default.CopyToClipboardTimeToLive.TotalMilliseconds;
        }

        private void SaveSettings()
        {
            UISettings.Default.MainFormLocation = (WindowState == FormWindowState.Normal) ? Location : RestoreBounds.Location;
            UISettings.Default.MainFormWindowSize = (WindowState == FormWindowState.Normal) ? Size : RestoreBounds.Size;
            UISettings.Default.MainFormSecretsSorting = _listViewItemSorter.SortOrder;
            UISettings.Default.Save();
            Settings.Default.Save();
        }

        private UxOperation NewUxOperationWithProgress(ToolStripItem controlToToggle) => new UxOperation(_currentVaultAlias, controlToToggle, uxStatusLabel, uxStatusProgressBar, uxButtonCancel);

        private UxOperation NewUxOperation(ToolStripItem controlToToggle) => new UxOperation(_currentVaultAlias, controlToToggle, uxStatusLabel, null, null);

        private void uxComboBoxVaultAlias_DropDown(object sender, EventArgs e)
        {
            int prevSelectedIndex = uxComboBoxVaultAlias.SelectedIndex;
            uxComboBoxVaultAlias.Items.Clear();
            uxComboBoxVaultAlias.Items.AddRange(Utils.LoadFromJsonFile<VaultAliases>(Settings.Default.VaultAliasesJsonFileLocation).ToArray());
            if (prevSelectedIndex < uxComboBoxVaultAlias.Items.Count)
            {
                uxComboBoxVaultAlias.SelectedIndex = prevSelectedIndex;
            }
        }

        private void uxComboBoxVaultAlias_DropDownClosed(object sender, EventArgs e)
        {
            _currentVaultAlias = (VaultAlias)uxComboBoxVaultAlias.SelectedItem;
            bool itemSelected = (null != _currentVaultAlias);
            uxComboBoxVaultAlias.ToolTipText = itemSelected ? "Vault names: " + string.Join(", ", _currentVaultAlias.VaultNames) : "";
            uxMenuItemRefresh.Enabled = itemSelected;
            if (itemSelected)
            {
                uxMenuItemRefresh.PerformClick();
            }
        }

        private void RefreshSecertsCount()
        {
            uxStatusLabelSecertsCount.Text = (_strikedoutSecrets == 0) ? $"{uxListViewSecrets.Items.Count} secrets" : $"{uxListViewSecrets.Items.Count - _strikedoutSecrets} out of {uxListViewSecrets.Items.Count} secrets";
            uxStatusLabelSecretsSelected.Text = $"{uxListViewSecrets.SelectedItems.Count} selected";
        }

        private delegate void UpdateLabelSecertsCountDelegate(int count);

        private async void uxMenuItemRefresh_Click(object sender, EventArgs e)
        {
            using (var op = NewUxOperationWithProgress(uxMenuItemRefresh)) await op.Invoke("access", async () =>
            {
                _vault = new Vault(Utils.FullPathToJsonFile(Settings.Default.VaultsJsonFileLocation), VaultAccessTypeEnum.ReadWrite, _currentVaultAlias.VaultNames);
                uxListViewSecrets.Items.Clear();
                _strikedoutSecrets = 0;
                RefreshSecertsCount();
                UpdateLabelSecertsCountDelegate ulscd = (c) => uxStatusLabelSecertsCount.Text = $"{c} secrets"; // We use delegate and Invoke() below to execute on the thread that owns the control
                foreach (var s in await _vault.ListSecretsAsync(0, (c) => { Invoke(ulscd, c); }, cancellationToken: op.CancellationToken))
                {
                    uxListViewSecrets.Items.Add(new SecretListViewItem(_currentVaultAlias, uxListViewSecrets.Groups, s));
                }
                uxButtonAdd.Enabled = uxMenuItemAdd.Enabled = true;
                uxImageSearch.Enabled = uxTextBoxSearch.Enabled = true;
                uxListViewSecrets.AllowDrop = true;
                RefreshSecertsCount();
                uxTimerSearchTextTypingCompleted_Tick(null, EventArgs.Empty); // Refresh search
            });
        }

        private void uxListViewSecrets_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool singleItemSelected = (uxListViewSecrets.SelectedItems.Count == 1);
            bool manyItemsSelected = (uxListViewSecrets.SelectedItems.Count >= 1);
            bool secretEnabled = singleItemSelected ? (uxListViewSecrets.SelectedItems[0] as SecretListViewItem).Attributes.Enabled ?? true : false;
            bool favorite = manyItemsSelected ? (uxListViewSecrets.SelectedItems[0] as SecretListViewItem).Favorite : false;
            uxButtonEdit.Enabled = uxButtonCopy.Enabled = uxButtonSave.Enabled = secretEnabled;
            uxMenuItemEdit.Enabled = uxMenuItemCopy.Enabled = uxMenuItemSave.Enabled = secretEnabled;
            uxButtonDelete.Enabled = uxMenuItemDelete.Enabled = uxButtonFavorite.Enabled = uxMenuItemFavorite.Enabled = manyItemsSelected;
            uxButtonToggle.Enabled = uxMenuItemToggle.Enabled = singleItemSelected;
            uxButtonToggle.Text = uxMenuItemToggle.Text = secretEnabled ? "Disabl&e" : "&Enable";
            uxButtonToggle.ToolTipText = uxMenuItemToggle.ToolTipText = secretEnabled ? "Disable secret" : "Enable secret";
            uxMenuItemToggle.Text = uxButtonToggle.Text + "...";
            uxButtonFavorite.Checked = uxMenuItemFavorite.Checked = favorite;
            uxButtonFavorite.ToolTipText = uxMenuItemFavorite.ToolTipText = favorite ? "Remove secret(s) from favorites group" : "Add secret(s) to favorites group";
            uxPropertyGridSecret.SelectedObject = singleItemSelected ? uxListViewSecrets.SelectedItems[0] : null;
            RefreshSecertsCount();
        }
        private void uxListViewSecrets_KeyDown(object sender, KeyEventArgs e)
        {
            _keyDownOccured = true; // Prevents from 'global' KeyUp event, basically key down happened in the other app
        }

        private void uxListViewSecrets_KeyUp(object sender, KeyEventArgs e)
        {
            if (!_keyDownOccured) return;
            _keyDownOccured = false;
            switch (e.KeyCode)
            {
                case Keys.F1:
                    uxButtonHelp.PerformClick();
                    return;
                case Keys.F5:
                    uxMenuItemRefresh.PerformClick();
                    return;
                case Keys.Insert:
                    uxButtonAdd.PerformClick();
                    return;
                case Keys.Delete:
                    uxButtonDelete.PerformClick();
                    return;
                case Keys.Enter:
                    uxButtonEdit.PerformClick();
                    return;
            }
            if (!e.Control) return;
            switch (e.KeyCode)
            {
                case Keys.A:
                    foreach (SecretListViewItem item in uxListViewSecrets.Items) item.Selected = !item.Strikeout;
                    break;
                case Keys.C:
                    uxButtonCopy.PerformClick();
                    return;
                case Keys.E:
                    uxButtonEdit.PerformClick();
                    return;
                case Keys.F:
                    uxButtonFavorite.PerformClick();
                    return;
                case Keys.R:
                    uxMenuItemRefresh.PerformClick();
                    return;
                case Keys.S:
                    uxButtonSave.PerformClick();
                    return;
            }
        }

        private void uxButtonAdd_Click(object sender, EventArgs e)
        {
            (sender as ToolStripDropDownItem)?.ShowDropDown();
        }

        private bool VerifyDuplication(Secret sOld, SecretObject soNew)
        {
            string oldName = sOld?.SecretIdentifier.Name ?? "";
            string newMd5 = soNew.Md5;

            // Check if we already have *another* secret with the same name
            if ((oldName != soNew.Name) && (uxListViewSecrets.Items.ContainsKey(soNew.Name) &&
                (MessageBox.Show($"Are you sure you want to replace secret '{soNew.Name}' with new value?", Utils.AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)))
            {
                return false;
            }

            // Detect dups by Md5
            var sameSecretsList = from slvi in uxListViewSecrets.Items.Cast<SecretListViewItem>() where (slvi.Md5 == newMd5) && (slvi.Name != oldName) && (slvi.Name != soNew.Name) select slvi.Name;
            if ((sameSecretsList.Count() > 0) &&
                (MessageBox.Show($"There are {sameSecretsList.Count()} other secret(s) in the vault which has the same Md5: {newMd5}.\nHere the name(s) of the other secrets:\n{string.Join(", ", sameSecretsList)}\nAre you sure you want to add or update secret {soNew.Name} and have a duplication of secrets?", Utils.AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != DialogResult.Yes))
            {
                return false;
            }
            return true;
        }

        private async Task AddOrUpdateSecret(UxOperation op, Secret sOld, SecretObject soNew)
        {
            Secret s = null;
            // Check for duplication by Name and Md5
            if (false == VerifyDuplication(sOld, soNew)) return;
            // New secret, secret rename or new value
            if ((sOld == null) || (sOld.SecretIdentifier.Name != soNew.Name) || (sOld.Value != soNew.RawValue))
            {
                s = await _vault.SetSecretAsync(soNew.Name, soNew.RawValue, soNew.TagsToDictionary(), ContentTypeEnumConverter.GetDescription(soNew.ContentType), soNew.ToSecretAttributes(), op.CancellationToken);
            }
            else // Same secret name and value
            {
                s = await _vault.UpdateSecretAsync(soNew.Name, soNew.TagsToDictionary(), ContentTypeEnumConverter.GetDescription(soNew.ContentType), soNew.ToSecretAttributes(), op.CancellationToken);
            }
            string oldSecretName = sOld?.SecretIdentifier.Name;
            if ((oldSecretName != null) && (oldSecretName != soNew.Name)) // Delete old secret
            {
                await _vault.DeleteSecretAsync(oldSecretName, op.CancellationToken);
                uxListViewSecrets.Items.RemoveByKey(oldSecretName);
            }
            uxListViewSecrets.Items.RemoveByKey(soNew.Name);
            var slvi = new SecretListViewItem(_currentVaultAlias, uxListViewSecrets.Groups, s);
            uxListViewSecrets.Items.Add(slvi);
            uxTimerSearchTextTypingCompleted_Tick(null, EventArgs.Empty); // Refresh search
            slvi.RefreshAndSelect();
            RefreshSecertsCount();
        }

        private FileInfo GetFileInfo(object sender, EventArgs e)
        {
            FileInfo fi = null;
            if (e is AddFileEventArgs) // File was dropped
            {
                fi = new FileInfo((e as AddFileEventArgs).FileName);
            }
            else
            {
                uxOpenFileDialog.FilterIndex = (sender == uxAddCertFromFile) || (sender == uxAddCertFromFile2) ? ContentType.Pkcs12.ToFilterIndex() : ContentType.None.ToFilterIndex();
                if (uxOpenFileDialog.ShowDialog() != DialogResult.OK) return null;
                fi = new FileInfo(uxOpenFileDialog.FileName);
            }
            if (fi.Length > CommonConsts.MB)
            {
                MessageBox.Show($"File {fi.FullName} size is {fi.Length:N0} bytes. Maximum file size allowed for secret value (before compression) is 1 MB.", Utils.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            return fi;
        }

        private async void uxMenuItemAdd_Click(object sender, EventArgs e)
        {
            SecretDialog nsDlg = null;
            // Add secret
            using (var dtf = new DeleteTempFileInfo())
            {
                if ((sender == uxAddSecret) || (sender == uxAddSecret2))
                {
                    nsDlg = new SecretDialog(_currentVaultAlias.SecretKinds);
                }
                // Add certificate from file or configuration file
                if ((sender == uxAddCertFromFile) || (sender == uxAddCertFromFile2) || (sender == uxAddFile) || (sender == uxAddFile2))
                {
                    dtf.FileInfoObject = GetFileInfo(sender, e);
                    if (dtf.FileInfoObject == null) return;
                    nsDlg = new SecretDialog(_currentVaultAlias.SecretKinds, dtf.FileInfoObject);
                }
                // Add certificate from store
                if ((sender == uxAddCertFromUserStore) || (sender == uxAddCertFromUserStore2) || (sender == uxAddCertFromMachineStore) || (sender == uxAddCertFromMachineStore2))
                {
                    var cert = Utils.SelectCertFromStore(StoreName.My, (sender == uxAddCertFromUserStore) || (sender == uxAddCertFromUserStore2) ? StoreLocation.CurrentUser : StoreLocation.LocalMachine, _currentVaultAlias.Alias, Handle);
                    if (cert == null) return;
                    nsDlg = new SecretDialog(_currentVaultAlias.SecretKinds, cert);
                }
                if ((nsDlg != null) && (nsDlg.DialogResult != DialogResult.Cancel) && (nsDlg.ShowDialog() == DialogResult.OK)) // DialogResult.Cancel is when user clicked cancel during password prompt
                {
                    using (var op = NewUxOperationWithProgress(uxButtonAdd)) await op.Invoke("add or update secret in", async () =>
                    {
                        await AddOrUpdateSecret(op, null, nsDlg.SecretObject);
                    });
                }
            }
        }

        private async void uxButtonEdit_Click(object sender, EventArgs e)
        {
            if (uxListViewSecrets.SelectedItems.Count == 1)
            {
                var slvi = uxListViewSecrets.SelectedItems[0] as SecretListViewItem;
                if (slvi.Attributes.Enabled ?? true)
                {
                    Secret s = null;
                    IEnumerable<SecretItem> versions = null;
                    using (var op = NewUxOperationWithProgress(uxButtonEdit)) await op.Invoke("get secret or secret versions from", async () =>
                    {
                        s = await _vault.GetSecretAsync(slvi.Name, null, op.CancellationToken);
                        versions = await _vault.GetSecretVersionsAsync(slvi.Name, 0, op.CancellationToken);
                    });
                    SecretDialog nsDlg = new SecretDialog(_vault, _currentVaultAlias.SecretKinds, s, versions);
                    if (nsDlg.ShowDialog() == DialogResult.OK)
                    {
                        using (var op = NewUxOperationWithProgress(uxButtonEdit)) await op.Invoke("add or update secret in", async () =>
                        {
                            await AddOrUpdateSecret(op, s, nsDlg.SecretObject);
                        });
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
                if (MessageBox.Show($"Are you sure you want to {action} secret '{slvi.Name}'?", Utils.AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    using (var op = NewUxOperationWithProgress(uxButtonToggle)) await op.Invoke("update secret in", async () =>
                    {
                        Secret s = await _vault.UpdateSecretAsync(slvi.Name, Utils.AddChangedBy(slvi.Tags), null, new SecretAttributes() { Enabled = !slvi.Attributes.Enabled }, op.CancellationToken); // Toggle only Enabled attribute
                        slvi = new SecretListViewItem(_currentVaultAlias, uxListViewSecrets.Groups, s);
                        uxListViewSecrets.Items.RemoveByKey(slvi.Name);
                        uxListViewSecrets.Items.Add(slvi);
                        slvi.RefreshAndSelect();
                    });
                }
            }
        }

        private async void uxButtonDelete_Click(object sender, EventArgs e)
        {
            if (uxListViewSecrets.SelectedItems.Count > 0)
            {
                string secretNames = string.Join(", ", from item in uxListViewSecrets.SelectedItems.Cast<ListViewItem>() select item.Name);
                if (MessageBox.Show($"Are you sure you want to delete {uxListViewSecrets.SelectedItems.Count} secret(s) with the following names?\n{secretNames}\n\nWarning: This operation can not be undone!", Utils.AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    using (var op = NewUxOperationWithProgress(uxButtonDelete)) await op.Invoke("delete secret(s) in", async () =>
                    {
                        foreach (ListViewItem lvi in uxListViewSecrets.SelectedItems)
                        {
                            await _vault.DeleteSecretAsync(lvi.Name, op.CancellationToken);
                            uxListViewSecrets.Items.RemoveByKey(lvi.Name);
                            RefreshSecertsCount();
                        }
                    });
                }
            }
        }

        private void uxTimerSearchTextTypingCompleted_Tick(object sender, EventArgs e)
        {
            uxTimerSearchTextTypingCompleted.Stop();

            _strikedoutSecrets = 0;
            SecretListViewItem selectItem = null;
            uxListViewSecrets.BeginUpdate();
            foreach (SecretListViewItem slvi in uxListViewSecrets.Items)
            {
                bool contains = slvi.Contains(uxTextBoxSearch.Text);
                slvi.Strikeout = !contains;
                _strikedoutSecrets += !contains ? 1 : 0;
                if ((selectItem == null) && contains)
                {
                    selectItem = slvi;
                }
            }
            uxListViewSecrets.Sort();
            selectItem?.RefreshAndSelect();
            uxListViewSecrets.EndUpdate();
            RefreshSecertsCount();
        }

        private void uxTextBoxSearch_TextChanged(object sender, EventArgs e)
        {
            uxTimerSearchTextTypingCompleted.Stop(); // Wait for user to finish the typing in a text box
            uxTimerSearchTextTypingCompleted.Start();
        }

        private async void uxButtonCopy_Click(object sender, EventArgs e)
        {
            if (uxListViewSecrets.SelectedItems.Count == 1)
            {
                string secretName = uxListViewSecrets.SelectedItems[0].Name;
                using (var op = NewUxOperationWithProgress(uxButtonCopy)) await op.Invoke("get secret from", async () =>
                {
                    var so = new SecretObject(await _vault.GetSecretAsync(secretName, null, op.CancellationToken), null);
                    _clipboardValue = so.GetClipboardValue();
                    Clipboard.SetText(_clipboardValue);
                    uxTimerClearClipboard.Start();
                });
            }
        }

        private void uxTimerClearClipboard_Tick(object sender, EventArgs e)
        {
            uxTimerClearClipboard.Stop();
            if (_clipboardValue == Clipboard.GetText()) // We don't want to override other clipboard value
            {
                Clipboard.Clear();
            }
        }

        private async void uxButtonSave_Click(object sender, EventArgs e)
        {
            if (uxListViewSecrets.SelectedItems.Count == 1)
            {
                string secretName = uxListViewSecrets.SelectedItems[0].Name;
                SecretObject so = null;
                using (var op = NewUxOperationWithProgress(uxButtonSave)) await op.Invoke("get secret from", async () =>
                {
                    so = new SecretObject(await _vault.GetSecretAsync(secretName, null, op.CancellationToken), null);
                });
                uxSaveFileDialog.FileName = so.GetFileName();
                uxSaveFileDialog.DefaultExt = so.ContentType.ToExtension();
                uxSaveFileDialog.FilterIndex = so.ContentType.ToFilterIndex();
                if (uxSaveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    so.SaveToFile(uxSaveFileDialog.FileName);
                }
            }
        }

        private void uxButtonFavorite_Click(object sender, EventArgs e)
        {
            if (uxListViewSecrets.SelectedItems.Count > 0)
            {
                uxButtonFavorite.Checked = uxMenuItemFavorite.Checked = !uxButtonFavorite.Checked;
                using (var op = NewUxOperationWithProgress(uxButtonFavorite))
                {
                    uxListViewSecrets.BeginUpdate();
                    foreach (SecretListViewItem slvi in uxListViewSecrets.SelectedItems)
                    {
                        slvi.Favorite = !slvi.Favorite;
                    }
                    uxListViewSecrets.Sort();
                    uxListViewSecrets.EndUpdate();
                    SaveSettings();
                }
                uxListViewSecrets_SelectedIndexChanged(null, EventArgs.Empty);
            }
        }

        private void uxButtonSettings_Click(object sender, EventArgs e)
        {
            var dlg = new SettingsDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                ApplySettings();
            }
        }

        private void uxButtonHelp_Click(object sender, EventArgs e)
        {
            Process.Start("http://aka.ms/vaultexplorer");
        }

        private void uxListViewSecrets_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (_listViewItemSorter.Column == e.Column)
            {
                _listViewItemSorter.SwapSortOder();
            }
            else
            {
                _listViewItemSorter.Column = e.Column;
                _listViewItemSorter.SortOrder = SortOrder.Ascending;
            }
            uxListViewSecrets.Sort();
        }

        #region Drag & Drop

        private bool _ctrlKeyPressed = false; // Flag to indicate if CTRL key was down during start of the drag

        private async void uxListViewSecrets_ItemDrag(object sender, ItemDragEventArgs e)
        {
            using (var op = NewUxOperation(uxButtonSave)) await op.Invoke("get secret from", async () =>
            {
                _ctrlKeyPressed = (ModifierKeys & Keys.Control) != 0;
                List<string> filesList = new List<string>();
                foreach (var slvi in uxListViewSecrets.SelectedItems.Cast<SecretListViewItem>())
                {
                    var so = new SecretObject(await _vault.GetSecretAsync(slvi.Name), null);
                    // Replace extension to .secret if CTRL is pressed
                    var filename = so.Name + (_ctrlKeyPressed ? ContentType.Secret.ToExtension() : so.ContentType.ToExtension());
                    var fullName = Path.Combine(Path.GetTempPath(), filename);
                    so.SaveToFile(fullName);
                    filesList.Add(fullName);
                }
                var dataObject = new DataObject(DataFormats.FileDrop, filesList.ToArray());
                uxListViewSecrets.DoDragDrop(dataObject, DragDropEffects.Move);
            });
        }

        private void uxListViewSecrets_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            e.UseDefaultCursors = false;  
            Cursor.Current = _ctrlKeyPressed ? _moveSecretCursor : _moveValueCursor;
        }

        private void uxListViewSecrets_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = (_vault != null) && e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Move : DragDropEffects.None;
        }

        private void uxListViewSecrets_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                // Since we are about to show modal dialog(s), we release the caller (other Vault Explorer instance) by calling ourself via BeginInvoke
                BeginInvoke(new ProcessDropedFilesDelegate(ProcessDropedFiles), string.Join("|", files));
            }
        }

        private delegate void ProcessDropedFilesDelegate(string files);

        private void ProcessDropedFiles(string files)
        {
            foreach (string file in files.Split('|'))
            {
                uxMenuItemAdd_Click(uxAddFile, new AddFileEventArgs(file));
            }
        }

        #endregion

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSettings();
        }
    }
}
