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

        public VaultAlias CurrentVaultAlias { get; set; }

        public Vault CurrentVault { get; set; }

        public ListViewSecrets ListViewSecrets => uxListViewSecrets;

        #endregion

        public MainForm()
        {
            InitializeComponent();
            Text = $"{Utils.AppName} ({Environment.UserDomainName}\\{Environment.UserName})";

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
            uxStatusLabelSecertsCount.Text = (uxListViewSecrets.StrikedoutSecrets == 0) ? $"{uxListViewSecrets.Items.Count} items" : $"{uxListViewSecrets.Items.Count - uxListViewSecrets.StrikedoutSecrets} out of {uxListViewSecrets.Items.Count} items";
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
            uxButtonEdit.Enabled = uxButtonCopy.Enabled = uxButtonSave.Enabled = secretEnabled;
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
                    foreach (ListViewItemBase item in uxListViewSecrets.Items) item.Selected = !item.Strikeout;
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



        private async Task AddOrUpdateSecret(UxOperation op, Secret sOld, PropertyObjectSecret soNew)
        {
            Secret s = null;
            // Check for duplication by Name and Md5
            //if (false == VerifyDuplication(sOld, soNew)) return;
            // New secret, secret rename or new value
            if ((sOld == null) || (sOld.SecretIdentifier.Name != soNew.Name) || (sOld.Value != soNew.RawValue))
            {
                s = await CurrentVault.SetSecretAsync(soNew.Name, soNew.RawValue, soNew.ToTagsDictionary(), ContentTypeEnumConverter.GetDescription(soNew.ContentType), soNew.ToSecretAttributes(), op.CancellationToken);
            }
            else // Same secret name and value
            {
                s = await CurrentVault.UpdateSecretAsync(soNew.Name, soNew.ToTagsDictionary(), ContentTypeEnumConverter.GetDescription(soNew.ContentType), soNew.ToSecretAttributes(), op.CancellationToken);
            }
            string oldSecretName = sOld?.SecretIdentifier.Name;
            if ((oldSecretName != null) && (oldSecretName != soNew.Name)) // Delete old secret
            {
                await CurrentVault.DeleteSecretAsync(oldSecretName, op.CancellationToken);
                uxListViewSecrets.Items.RemoveByKey(oldSecretName);
            }
            uxListViewSecrets.Items.RemoveByKey(soNew.Name);
            var slvi = new ListViewItemSecret(this, s);
            uxListViewSecrets.Items.Add(slvi);
            uxTimerSearchTextTypingCompleted_Tick(null, EventArgs.Empty); // Refresh search
            slvi.RefreshAndSelect();
            RefreshItemsCount();
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
                if ((nsDlg != null) && (nsDlg.DialogResult != DialogResult.Cancel) && (nsDlg.ShowDialog() == DialogResult.OK)) // DialogResult.Cancel is when user clicked cancel during password prompt
                {
                    using (var op = NewUxOperationWithProgress(uxButtonAdd)) await op.Invoke("add secret to", async () =>
                    {
                        await AddOrUpdateSecret(op, null, nsDlg.PropertyObject);
                    });
                }
            }
        }

        private async Task AddOrUpdateCertificate(UxOperation op, ItemDialogBaseMode mode, PropertyObjectCertificate certNew)
        {
            CertificateBundle cb = null;
            switch (mode)
            {
                case ItemDialogBaseMode.New:
                    var certCollection = new X509Certificate2Collection();
                    certCollection.Add(certNew.Certificate);
                    cb = await CurrentVault.ImportCertificateAsync(certNew.Name, certCollection, certNew.CertificatePolicy, certNew.CertificateBundle.Attributes, certNew.ToTagsDictionary(), op.CancellationToken);
                    break;
                case ItemDialogBaseMode.Edit:
                    await CurrentVault.UpdateCertificatePolicyAsync(certNew.Name, certNew.CertificatePolicy, op.CancellationToken);
                    cb = await CurrentVault.UpdateCertificateAsync(certNew.Name, certNew.ToCertificateAttributes(), certNew.ToTagsDictionary(), op.CancellationToken);
                    break;
            }
            uxListViewSecrets.Items.RemoveByKey(certNew.Name);
            var clvi = new ListViewItemCertificate(this, cb);
            uxListViewSecrets.Items.Add(clvi);
            uxTimerSearchTextTypingCompleted_Tick(null, EventArgs.Empty); // Refresh search
            clvi.RefreshAndSelect();
            RefreshItemsCount();
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
                if ((certDlg != null) && (certDlg.DialogResult != DialogResult.Cancel) && (certDlg.ShowDialog() == DialogResult.OK)) // DialogResult.Cancel is when user clicked cancel during password prompt
                {
                    using (var op = NewUxOperationWithProgress(uxButtonAdd)) await op.Invoke("add certificate to", async () =>
                    {
                        await AddOrUpdateCertificate(op, ItemDialogBaseMode.New, certDlg.PropertyObject);
                    });
                }
            }
        }

        private async void uxButtonEdit_Click(object sender, EventArgs e)
        {
            if (uxListViewSecrets.SelectedItems.Count == 1)
            {
                var item = uxListViewSecrets.SelectedItems[0] as ListViewItemBase;
                if (item.Enabled)
                {
                    IEnumerable<object> versions = null;
                    using (var op = NewUxOperationWithProgress(uxButtonEdit)) await op.Invoke($"get {item.Kind} from", async () =>
                    {
                        versions = await item.GetVersionsAsync(op.CancellationToken);
                    });
                    dynamic editDlg = item.GetEditDialog(this, item.Name, versions);
                    if (editDlg.ShowDialog() == DialogResult.OK)
                    {
                        using (var op = NewUxOperationWithProgress(uxButtonEdit)) await op.Invoke($"update {item.Kind} in", async () =>
                        {
                            var lvi = await item.AddOrUpdate(this, ItemDialogBaseMode.Edit, editDlg.OriginalObject, editDlg.PropertyObject, op.CancellationToken);
                            if (null == lvi) return; // Operation was cancelled
                            uxListViewSecrets.Items.Remove(item);
                            uxListViewSecrets.Items.Add(lvi);
                            uxTimerSearchTextTypingCompleted_Tick(null, EventArgs.Empty); // Refresh search
                            lvi.RefreshAndSelect();
                            RefreshItemsCount();
                        });
                    }
                }
            }
        }

        private async void uxButtonToggle_Click(object sender, EventArgs e)
        {
            if (null != uxListViewSecrets.FirstSelectedItem)
            {
                var slvi = uxListViewSecrets.FirstSelectedItem;
                string action = slvi.Enabled ? "disable" : "enable";
                if (MessageBox.Show($"Are you sure you want to {action} {slvi.Kind} '{slvi.Name}'?", Utils.AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    using (var op = NewUxOperationWithProgress(uxButtonToggle)) await op.Invoke($"update {slvi.Kind} in", async () =>
                    {
                        uxListViewSecrets.Replace(slvi, await slvi.ToggleAsync(op.CancellationToken));
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
            if (uxListViewSecrets.SelectedItems.Count == 1)
            {
                string secretName = uxListViewSecrets.SelectedItems[0].Name;
                using (var op = NewUxOperationWithProgress(uxButtonCopy)) await op.Invoke("get secret from", async () =>
                {
                    var so = new PropertyObjectSecret(await CurrentVault.GetSecretAsync(secretName, null, op.CancellationToken), null);
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
                PropertyObjectSecret so = null;
                using (var op = NewUxOperationWithProgress(uxButtonSave)) await op.Invoke("get secret from", async () =>
                {
                    so = new PropertyObjectSecret(await CurrentVault.GetSecretAsync(secretName, null, op.CancellationToken), null);
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
            using (var op = NewUxOperation(uxButtonSave)) await op.Invoke("get secret from", async () =>
            {
                _ctrlKeyPressed = (ModifierKeys & Keys.Control) != 0;
                List<string> filesList = new List<string>();
                foreach (var slvi in uxListViewSecrets.SelectedItems.Cast<ListViewItemSecret>())
                {
                    var so = new PropertyObjectSecret(await CurrentVault.GetSecretAsync(slvi.Name), null);
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
                uxMenuItemAddSecret_Click(uxAddFile, new AddFileEventArgs(file));
            }
        }

        #endregion

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSettings();
        }
    }
}
