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

    public partial class MainForm : FormTelemetry, ISession
    {
        private string _clipboardValue;
        private Cursor _moveSecretCursor;
        private Cursor _moveValueCursor;
        private bool _keyDownOccured;
        private ToolStripButton uxButtonCancel;

        #region ISession

        public VaultAlias CurrentVaultAlias { get; private set; }

        public Vault CurrentVault { get; private set; }

        public ListViewSecrets ListViewSecrets => uxListViewSecrets;

        #endregion

        public MainForm()
        {
            InitializeComponent();
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
            uxListViewSecrets.Sorting = UISettings.Default.MainFormSecretsSorting;
            uxListViewSecrets.SortingColumn = UISettings.Default.MainFormSecretsSortingColumn;
            uxButtonCopy.ToolTipText = uxMenuItemCopy.ToolTipText = $"Copy secret value to clipboard for {Settings.Default.CopyToClipboardTimeToLive.TotalSeconds} seconds";
            uxTimerClearClipboard.Interval = (int)Settings.Default.CopyToClipboardTimeToLive.TotalMilliseconds;
        }

        private void SaveSettings()
        {
            UISettings.Default.MainFormLocation = (WindowState == FormWindowState.Normal) ? Location : RestoreBounds.Location;
            UISettings.Default.MainFormWindowSize = (WindowState == FormWindowState.Normal) ? Size : RestoreBounds.Size;
            UISettings.Default.MainFormSecretsSorting = uxListViewSecrets.Sorting;
            UISettings.Default.MainFormSecretsSortingColumn = uxListViewSecrets.SortingColumn;
            UISettings.Default.Save();
            Settings.Default.Save();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSettings();
        }

        private UxOperation NewUxOperationWithProgress(ToolStripItem controlToToggle) => new UxOperation(CurrentVaultAlias, controlToToggle, uxStatusLabel, uxStatusProgressBar, uxButtonCancel);

        private UxOperation NewUxOperation(ToolStripItem controlToToggle) => new UxOperation(CurrentVaultAlias, controlToToggle, uxStatusLabel, null, null);

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
            if (null == uxComboBoxVaultAlias.SelectedItem) return;
            if (CurrentVaultAlias?.Alias == ((VaultAlias)uxComboBoxVaultAlias.SelectedItem).Alias) return;
            CurrentVaultAlias = (VaultAlias)uxComboBoxVaultAlias.SelectedItem;
            bool itemSelected = (null != CurrentVaultAlias);
            uxComboBoxVaultAlias.ToolTipText = itemSelected ? "Vault names: " + string.Join(", ", CurrentVaultAlias.VaultNames) : "";
            uxMenuItemRefresh.Enabled = itemSelected;
            if (itemSelected)
            {
                uxMenuItemRefresh.PerformClick();
            }
        }

        private void RefreshItemsCount()
        {
            uxStatusLabelSecertsCount.Text = string.IsNullOrWhiteSpace(uxTextBoxSearch.Text) ? $"{uxListViewSecrets.Items.Count} items" : $"{uxListViewSecrets.SearchResultsCount} out of {uxListViewSecrets.Items.Count} items";
            uxStatusLabelSecretsSelected.Text = $"{uxListViewSecrets.SelectedItems.Count} selected";
        }

        private delegate void UpdateLabelSecertsCountDelegate(int count);

        private async void uxMenuItemRefresh_Click(object sender, EventArgs e)
        {
            using (var op = NewUxOperationWithProgress(uxMenuItemRefresh)) await op.Invoke("access", async () =>
            {
                try
                {
                    CurrentVault = new Vault(Utils.FullPathToJsonFile(Settings.Default.VaultsJsonFileLocation), VaultAccessTypeEnum.ReadWrite, CurrentVaultAlias.VaultNames);
                    uxListViewSecrets.BeginUpdate();
                    uxListViewSecrets.RemoveAllItems();
                    RefreshItemsCount();
                    UpdateLabelSecertsCountDelegate ulscd = (c) => uxStatusLabelSecertsCount.Text = $"{uxListViewSecrets.Items.Count + c} secrets"; // We use delegate and Invoke() below to execute on the thread that owns the control
                    foreach (var s in await CurrentVault.ListSecretsAsync(0, (c) => { Invoke(ulscd, c); }, cancellationToken: op.CancellationToken))
                    {
                        uxListViewSecrets.Items.Add(new ListViewItemSecret(this, s));
                    }
                    Text = $"{Utils.AppName} ({CurrentVault.AuthenticatedUserName})";
                    // List Key Vault Certificates
                    if (CurrentVaultAlias.KeyVaultCertificates)
                    {
                        foreach (var c in await CurrentVault.ListCertificatesAsync(0, (c) => { Invoke(ulscd, c); }, cancellationToken: op.CancellationToken))
                        {
                            int secretAsKvCertIndex = uxListViewSecrets.Items.IndexOfKey(c.Identifier.Name); // Remove "secret" (in fact this is a certifiacte) which was returned as part of ListSecretsAsync
                            if (secretAsKvCertIndex != -1)
                            {
                                uxListViewSecrets.Items.RemoveAt(secretAsKvCertIndex);
                            }
                            uxListViewSecrets.Items.Add(new ListViewItemCertificate(this, c));
                        }
                    }

                    uxButtonAdd.Enabled = uxMenuItemAdd.Enabled = true;
                    uxImageSearch.Enabled = uxTextBoxSearch.Enabled = true;
                    uxAddKVCert.Visible = CurrentVaultAlias.KeyVaultCertificates;
                    uxListViewSecrets.AllowDrop = true;
                    uxListViewSecrets.RefreshGroupsHeader();
                    RefreshItemsCount();
                    uxTimerSearchTextTypingCompleted_Tick(null, EventArgs.Empty); // Refresh search
                }
                finally
                {
                    uxListViewSecrets.EndUpdate();
                }
            });
        }

        private void uxListViewSecrets_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool singleItemSelected = (uxListViewSecrets.SelectedItems.Count == 1);
            bool manyItemsSelected = (uxListViewSecrets.SelectedItems.Count >= 1);
            ListViewItemBase selectedItem = singleItemSelected ? uxListViewSecrets.SelectedItems[0] as ListViewItemBase : null;
            bool secretEnabled = selectedItem?.Enabled ?? false;
            bool favorite = selectedItem?.Favorite ?? false;
            uxButtonEdit.Enabled = uxButtonShare.Enabled = uxButtonCopy.Enabled = uxButtonSave.Enabled = secretEnabled;
            uxMenuItemEdit.Enabled = uxMenuItemCopy.Enabled = uxMenuItemSave.Enabled = secretEnabled;
            uxButtonDelete.Enabled = uxMenuItemDelete.Enabled = uxButtonFavorite.Enabled = uxMenuItemFavorite.Enabled = manyItemsSelected;
            uxButtonToggle.Enabled = uxMenuItemToggle.Enabled = singleItemSelected;
            uxButtonToggle.Text = uxMenuItemToggle.Text = secretEnabled ? "Disabl&e" : "&Enable";
            uxButtonToggle.ToolTipText = uxMenuItemToggle.ToolTipText = secretEnabled ? "Disable secret" : "Enable secret";
            uxMenuItemToggle.Text = uxButtonToggle.Text + "...";
            uxButtonFavorite.Checked = uxMenuItemFavorite.Checked = favorite;
            uxButtonFavorite.ToolTipText = uxMenuItemFavorite.ToolTipText = favorite ? "Remove item(s) from favorites group" : "Add item(s) to favorites group";
            uxPropertyGridSecret.SelectedObject = singleItemSelected ? uxListViewSecrets.SelectedItems[0] : null;
            RefreshItemsCount();
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
                    foreach (ListViewItemBase item in uxListViewSecrets.Items) item.Selected = true;
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

        private FileInfo GetFileInfo(object sender, EventArgs e)
        {
            FileInfo fi = null;
            if (e is AddFileEventArgs) // File was dropped
            {
                fi = new FileInfo((e as AddFileEventArgs).FileName);
            }
            else
            {
                uxOpenFileDialog.FilterIndex = (sender == uxAddCertFromFile) || (sender == uxAddCertFromFile2) || (sender == uxAddKVCertFromFile) ? ContentType.Pkcs12.ToFilterIndex() : ContentType.None.ToFilterIndex();
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

        private void AddOrReplaceItemInListView(ListViewItemBase newItem, ListViewItemBase oldItem = null)
        {
            try
            {
                uxListViewSecrets.BeginUpdate();
                if (null != oldItem) uxListViewSecrets.Items.Remove(oldItem); // Rename flow
                uxListViewSecrets.Items.RemoveByKey(newItem.Name); // Overwrite flow
                uxListViewSecrets.Items.Add(newItem);
                uxTimerSearchTextTypingCompleted_Tick(null, EventArgs.Empty); // Refresh search
                newItem.RefreshAndSelect();
                uxListViewSecrets.RefreshGroupsHeader();
                RefreshItemsCount();
            }
            finally
            {
                uxListViewSecrets.EndUpdate();
            }
        }

        private async void uxMenuItemAddSecret_Click(object sender, EventArgs e)
        {
            SecretDialog nsDlg = null;
            // Add secret
            using (var dtf = new DeleteTempFileInfo())
            {
                if ((sender == uxAddSecret) || (sender == uxAddSecret2))
                {
                    nsDlg = new SecretDialog(this);
                }
                // Add certificate from file or configuration file
                if ((sender == uxAddCertFromFile) || (sender == uxAddCertFromFile2) || (sender == uxAddFile) || (sender == uxAddFile2))
                {
                    dtf.FileInfoObject = GetFileInfo(sender, e);
                    if (dtf.FileInfoObject == null) return;
                    nsDlg = new SecretDialog(this, dtf.FileInfoObject);
                }
                // Add certificate from store
                if ((sender == uxAddCertFromUserStore) || (sender == uxAddCertFromUserStore2) || (sender == uxAddCertFromMachineStore) || (sender == uxAddCertFromMachineStore2))
                {
                    var cert = Utils.SelectCertFromStore(StoreName.My, (sender == uxAddCertFromUserStore) || (sender == uxAddCertFromUserStore2) ? StoreLocation.CurrentUser : StoreLocation.LocalMachine, CurrentVaultAlias.Alias, Handle);
                    if (cert == null) return;
                    nsDlg = new SecretDialog(this, cert);
                }
                // DialogResult.Cancel is when user clicked cancel during password prompt from the ctor(), if OK was clicked, check for duplication by Name and Md5
                if ((nsDlg != null) && (nsDlg.DialogResult != DialogResult.Cancel) && (nsDlg.ShowDialog() == DialogResult.OK) && ListViewItemBase.VerifyDuplication(this, null, nsDlg.PropertyObject))
                {
                    using (var op = NewUxOperationWithProgress(uxButtonAdd)) await op.Invoke("add secret to", async () =>
                    {
                        AddOrReplaceItemInListView(await ListViewItemSecret.NewAsync(this, nsDlg.PropertyObject, op.CancellationToken));
                    });
                }
            }
        }

        private async void uxMenuItemAddKVCertificate_Click(object sender, EventArgs e)
        {
            CertificateDialog certDlg = null;
            // Add certificate
            using (var dtf = new DeleteTempFileInfo())
            {
                // Add certificate from file
                if (sender == uxAddKVCertFromFile)
                {
                    dtf.FileInfoObject = GetFileInfo(sender, e);
                    if (dtf.FileInfoObject == null) return;
                    certDlg = new CertificateDialog(this, dtf.FileInfoObject);
                }
                // Add certificate from store
                if ((sender == uxAddKVCertFromUserStore) || (sender == uxAddKVCertFromMachineStore))
                {
                    var cert = Utils.SelectCertFromStore(StoreName.My, (sender == uxAddKVCertFromUserStore) ? StoreLocation.CurrentUser : StoreLocation.LocalMachine, CurrentVaultAlias.Alias, Handle);
                    if (cert == null) return;
                    certDlg = new CertificateDialog(this, cert);
                }
                // DialogResult.Cancel is when user clicked cancel during password prompt from the ctor(), if OK was clicked, check for duplication by Name and Md5
                if ((certDlg != null) && (certDlg.DialogResult != DialogResult.Cancel) && (certDlg.ShowDialog() == DialogResult.OK) && ListViewItemBase.VerifyDuplication(this, null, certDlg.PropertyObject))
                {
                    using (var op = NewUxOperationWithProgress(uxButtonAdd)) await op.Invoke("add certificate to", async () =>
                    {
                        AddOrReplaceItemInListView(await ListViewItemCertificate.NewAsync(this, certDlg.PropertyObject, op.CancellationToken));
                    });
                }
            }
        }

        private async void uxButtonEdit_Click(object sender, EventArgs e)
        {
            var item = uxListViewSecrets.FirstSelectedItem;
            if ((null != item) && (item.Enabled))
            {
                IEnumerable<object> versions = null;
                using (var op = NewUxOperationWithProgress(uxButtonEdit)) await op.Invoke($"get {item.Kind} from", async () =>
                {
                    versions = await item.GetVersionsAsync(op.CancellationToken);
                });
                dynamic editDlg = item.GetEditDialog(item.Name, versions);
                // If OK was clicked, check for duplication by Name and Md5
                if ((editDlg.ShowDialog() == DialogResult.OK) && ListViewItemBase.VerifyDuplication(this, item.Name, editDlg.PropertyObject))
                {
                    using (var op = NewUxOperationWithProgress(uxButtonEdit)) await op.Invoke($"update {item.Kind} in", async () =>
                    {
                        var newItem = await item.UpdateAsync(editDlg.OriginalObject, editDlg.PropertyObject, op.CancellationToken);
                        AddOrReplaceItemInListView(newItem, item);
                    });
                }
            }
        }

        private async void uxButtonToggle_Click(object sender, EventArgs e)
        {
            var item = uxListViewSecrets.FirstSelectedItem;
            if (null != item)
            {
                string action = item.Enabled ? "disable" : "enable";
                if (MessageBox.Show($"Are you sure you want to {action} {item.Kind} '{item.Name}'?", Utils.AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    using (var op = NewUxOperationWithProgress(uxButtonToggle)) await op.Invoke($"update {item.Kind} in", async () =>
                    {
                        AddOrReplaceItemInListView(await item.ToggleAsync(op.CancellationToken), item);
                    });
                }
            }
        }

        private async void uxButtonDelete_Click(object sender, EventArgs e)
        {
            if (uxListViewSecrets.SelectedItems.Count > 0)
            {
                string itemNames = string.Join(", ", from item in uxListViewSecrets.SelectedItems.Cast<ListViewItem>() select item.Name);
                if (MessageBox.Show($"Are you sure you want to delete {uxListViewSecrets.SelectedItems.Count} item(s) with the following name(s)?\n{itemNames}\n\nWarning: This operation can not be undone!", Utils.AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    using (var op = NewUxOperationWithProgress(uxButtonDelete)) await op.Invoke("delete item(s) in", async () =>
                    {
                        foreach (ListViewItemBase lvi in uxListViewSecrets.SelectedItems)
                        {
                            uxListViewSecrets.Items.Remove(await lvi.DeleteAsync(op.CancellationToken));
                            RefreshItemsCount();
                        }
                    });
                }
            }
        }

        private void uxTimerSearchTextTypingCompleted_Tick(object sender, EventArgs e)
        {
            uxTimerSearchTextTypingCompleted.Stop();
            uxListViewSecrets.FindItemsWithText(uxTextBoxSearch.Text);
            RefreshItemsCount();
        }

        private void uxTextBoxSearch_TextChanged(object sender, EventArgs e)
        {
            uxTimerSearchTextTypingCompleted.Stop(); // Wait for user to finish the typing in a text box
            uxTimerSearchTextTypingCompleted.Start();
        }

        private async void uxButtonCopy_Click(object sender, EventArgs e)
        {
            var item = uxListViewSecrets.FirstSelectedItem;
            if (null != item)
            {
                using (var op = NewUxOperationWithProgress(uxButtonCopy)) await op.Invoke($"get {item.Kind} from", async () =>
                {
                    var po = await item.GetAsync(op.CancellationToken);
                    _clipboardValue = po.GetClipboardValue();
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
            var item = uxListViewSecrets.FirstSelectedItem;
            if (null != item)
            {
                PropertyObject po = null;
                using (var op = NewUxOperationWithProgress(uxButtonSave)) await op.Invoke($"get {item.Kind} from", async () =>
                {
                    po = await item.GetAsync(op.CancellationToken);
                });
                uxSaveFileDialog.FileName = po.GetFileName();
                uxSaveFileDialog.DefaultExt = po.GetContentType().ToExtension();
                uxSaveFileDialog.FilterIndex = po.GetContentType().ToFilterIndex();
                if (uxSaveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    po.SaveToFile(uxSaveFileDialog.FileName);
                }
            }
        }

        private void uxButtonFavorite_Click(object sender, EventArgs e)
        {
            if (uxListViewSecrets.SelectedItems.Count > 0)
            {
                using (var op = NewUxOperationWithProgress(uxButtonFavorite))
                {
                    uxListViewSecrets.ToggleSelectedItemsToFromFavorites();
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

        #region Drag & Drop

        private bool _ctrlKeyPressed = false; // Flag to indicate if CTRL key was down during start of the drag

        private async void uxListViewSecrets_ItemDrag(object sender, ItemDragEventArgs e)
        {
            using (var op = NewUxOperation(uxButtonSave)) await op.Invoke("get item(s) from", async () =>
            {
                _ctrlKeyPressed = (ModifierKeys & Keys.Control) != 0;
                List<string> filesList = new List<string>();
                foreach (var item in uxListViewSecrets.SelectedItems.Cast<ListViewItemBase>())
                {
                    var po = await item.GetAsync(op.CancellationToken);
                    // Pick .kv-secret or .kv-certificate extension if CTRL is pressed
                    var filename = po.Name + (_ctrlKeyPressed ? po.GetKeyVaultFileExtension() : po.GetContentType().ToExtension());
                    var fullName = Path.Combine(Path.GetTempPath(), filename);
                    po.SaveToFile(fullName);
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
            e.Effect = (CurrentVault != null) && e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Move : DragDropEffects.None;
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
                FileInfo fi = new FileInfo(file);
                switch (ContentTypeUtils.FromExtension(fi.Extension))
                {
                    case ContentType.KeyVaultCertificate:
                        uxMenuItemAddKVCertificate_Click(uxAddKVCertFromFile, new AddFileEventArgs(file));
                        break;
                    case ContentType.KeyVaultSecret:
                    default:
                        uxMenuItemAddSecret_Click(uxAddFile, new AddFileEventArgs(file));
                        break;
                }
            }
        }

        #endregion
    }
}
