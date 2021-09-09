// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 
using Azure.Security.KeyVault.Certificates;
using Azure.Security.KeyVault.Secrets;
namespace Microsoft.Vault.Explorer
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Security.Cryptography.X509Certificates;
    using System.Linq;
    using System.Windows.Forms;
    using Microsoft.Vault.Explorer.Properties;
    using UISettings = Properties.Settings;
    using Action = System.Action;
    using Microsoft.Vault.Library;
    using Core;

    public partial class MainForm : FormTelemetry, ISession
    {
        private readonly ActivationUri _activationUri;
        private Cursor _moveSecretCursor;
        private Cursor _moveValueCursor;
        private Cursor _moveLinkCursor;
        private bool _keyDownOccured;
        private ToolStripButton uxButtonCancel;
        private readonly Dictionary<string, VaultAlias> _tempVaultAliases; // Temporary picked VaultAliases via SubscriptionsManager
        private const string AddNewVaultText = "How to add new vault here...";
        private const string PickVaultText = "Pick vault from subscription...";

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
            _moveLinkCursor = Utils.LoadCursorFromResource(Resources.move_link);

            uxButtonCancel = new ToolStripButton("", Resources.cancel)
            {
                Margin = new Padding(0, 0, 20, 0),
                Size = new Size(uxStatusProgressBar.Width, uxStatusProgressBar.Width),
                ToolTipText = "Cancel operation",
                Visible = false
            };
            uxStatusStrip.Items.Insert(3, uxButtonCancel);
            _tempVaultAliases = new Dictionary<string, VaultAlias>();
        }

        public MainForm(ActivationUri activationUri) : this()
        {
            Guard.ArgumentNotNull(activationUri, nameof(activationUri));
            _activationUri = activationUri;
            // ActivaionUri is Empty, nothing special to do
            if (_activationUri == ActivationUri.Empty) return;
            // Activation by vault://name
            uxComboBoxVaultAlias_DropDown(this, EventArgs.Empty);
            uxComboBoxVaultAlias.SelectedIndex = 0;
            SetCurrentVaultAlias();
            if (!string.IsNullOrEmpty(_activationUri.VaultName) && string.IsNullOrEmpty(_activationUri.ItemName))
            {
                uxMenuItemRefresh.PerformClick(); // Refresh list
                return;
            }
            // Activation by vault://name/collection/itemName
            SetCurrentVault();
            _activationUri.PerformAction(CurrentVault);
            Close();
        }

        private void ApplySettings()
        {
            Size = UISettings.Default.MainFormWindowSize;
            uxListViewSecrets.Sorting = UISettings.Default.MainFormSecretsSorting;
            uxListViewSecrets.SortingColumn = UISettings.Default.MainFormSecretsSortingColumn;
            uxButtonCopy.ToolTipText = uxMenuItemCopy.ToolTipText = $"Copy secret value to clipboard for {Settings.Default.CopyToClipboardTimeToLive.TotalSeconds} seconds";
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

        private UxOperation NewUxOperationWithProgress(params ToolStripItem[] controlsToToggle) => new UxOperation(CurrentVaultAlias, uxStatusLabel, uxStatusProgressBar, uxButtonCancel, controlsToToggle);

        private UxOperation NewUxOperation(params ToolStripItem[] controlsToToggle) => new UxOperation(CurrentVaultAlias, uxStatusLabel, null, null, controlsToToggle);

        private void Invoke(Action action) => base.Invoke(action); // Small helper to avoid casting delegate to Action

        private void uxComboBoxVaultAlias_DropDown(object sender, EventArgs e)
        {
            object prevSelectedItem = uxComboBoxVaultAlias.SelectedItem;
            uxComboBoxVaultAlias.Items.Clear();
            IEnumerable<VaultAlias> va = Utils.LoadFromJsonFile<VaultAliases>(Settings.Default.VaultAliasesJsonFileLocation);
            if (!string.IsNullOrEmpty(_activationUri.VaultName)) // In case vault name was provided during activation, search for it, if not found let us add it on the fly
            {
                va = from v in va where v.VaultNames.Contains(_activationUri.VaultName, StringComparer.CurrentCultureIgnoreCase) select v;
                if (0 == va.Count()) // Not found, let add new vault alias == vault name, with Custom secret kind
                {
                    va = Enumerable.Repeat(new VaultAlias(_activationUri.VaultName, new string[] { _activationUri.VaultName }, new string[] { "Custom" }), 1);
                }
            }
            uxComboBoxVaultAlias.Items.AddRange(va.ToArray());
            uxComboBoxVaultAlias.Items.AddRange(_tempVaultAliases.Values.ToArray());
            uxComboBoxVaultAlias.Items.Add(AddNewVaultText);
            uxComboBoxVaultAlias.Items.Add(PickVaultText);
            uxComboBoxVaultAlias.SelectedItem = prevSelectedItem;
        }

        private void uxComboBoxVaultAlias_DropDownClosed(object sender, EventArgs e)
        {
            if (SetCurrentVaultAlias())
            {
                uxMenuItemRefresh.PerformClick();
            }
        }

        private void RefreshItemsCount()
        {
            uxStatusLabelSecertsCount.Text = string.IsNullOrWhiteSpace(uxTextBoxSearch.Text) ? $"{uxListViewSecrets.Items.Count} items" : $"{uxListViewSecrets.SearchResultsCount} out of {uxListViewSecrets.Items.Count} items";
            uxStatusLabelSecretsSelected.Text = $"{uxListViewSecrets.SelectedItems.Count} selected";
        }

        private bool SetCurrentVaultAlias()
        {
            if (null == uxComboBoxVaultAlias.SelectedItem) return false;
            // Ignore selection of the same vault alias, only when list view is not empty
            if ((CurrentVaultAlias?.Alias == uxComboBoxVaultAlias.SelectedItem.ToString()) && (uxListViewSecrets.Items.Count > 0)) return false;
            if (uxComboBoxVaultAlias.SelectedItem is string)
            {
                switch (uxComboBoxVaultAlias.SelectedItem.ToString())
                {
                    case AddNewVaultText:
                        uxButtonHelp.PerformClick();
                        uxComboBoxVaultAlias.SelectedItem = CurrentVaultAlias;
                        return false;
                    case PickVaultText:
                        var smd = new SubscriptionsManagerDialog();
                        if (smd.ShowDialog() != DialogResult.OK)
                        {
                            uxComboBoxVaultAlias.SelectedItem = CurrentVaultAlias;
                            return false;
                        }
                        _tempVaultAliases[smd.CurrentVaultAlias.Alias] = smd.CurrentVaultAlias;
                        uxComboBoxVaultAlias.Items.Insert(uxComboBoxVaultAlias.Items.Count - 1, smd.CurrentVaultAlias);
                        uxComboBoxVaultAlias.SelectedItem = smd.CurrentVaultAlias;

                        // Set user alias and domain hint manually as they are not set from the assignment
                        ((VaultAlias)uxComboBoxVaultAlias.SelectedItem).UserAlias = smd.CurrentVaultAlias.UserAlias;
                        ((VaultAlias)uxComboBoxVaultAlias.SelectedItem).DomainHint = smd.CurrentVaultAlias.DomainHint;
                        break;
                }
            }
            CurrentVaultAlias = (VaultAlias)uxComboBoxVaultAlias.SelectedItem;
            bool itemSelected = (null != CurrentVaultAlias);
            uxComboBoxVaultAlias.SelectedText = CurrentVaultAlias.Alias;
            // In some cases, the combobox will be blank. Setting the text on a blank combobox will null the selected item. So, always ensure the selecteditem is set when setting the selected text.
            uxComboBoxVaultAlias.SelectedItem = CurrentVaultAlias;            
            uxComboBoxVaultAlias.ToolTipText = itemSelected ? "Vault names: " + string.Join(", ", CurrentVaultAlias.VaultNames) : "";
            uxMenuItemRefresh.Enabled = itemSelected;
            return itemSelected;
        }

        private void SetCurrentVault()
        {
            CurrentVault = new Vault(Utils.FullPathToJsonFile(Settings.Default.VaultsJsonFileLocation), VaultAccessTypeEnum.ReadWrite, CurrentVaultAlias.VaultNames);
            // In case that subscription is chosen by the dialog, overwrite permissions taken from vaults.json
            if (CurrentVaultAlias.UserAlias!=null)
            {
                CurrentVault.VaultsConfig[CurrentVaultAlias.VaultNames[0]]= new VaultAccessType(
                    new VaultAccess[] { new VaultAccessUserInteractive(CurrentVaultAlias.DomainHint, CurrentVaultAlias.UserAlias) },
                    new VaultAccess[] { new VaultAccessUserInteractive(CurrentVaultAlias.DomainHint, CurrentVaultAlias.UserAlias) });
            }
        }

        private async void uxMenuItemRefresh_Click(object sender, EventArgs e)
        {
            using (var op = NewUxOperationWithProgress(uxMenuItemRefresh, uxComboBoxVaultAlias, uxButtonAdd, uxMenuItemAdd, 
                uxButtonEdit, uxMenuItemEdit, uxButtonToggle, uxMenuItemToggle, uxButtonDelete, uxMenuItemDelete, uxImageSearch, uxTextBoxSearch, 
                uxButtonShare, uxMenuItemShare, uxButtonFavorite, uxMenuItemFavorite, uxButtonPowershell)) 
            {
                try
                {
                    Text = Utils.AppName;
                    SetCurrentVault();
                    uxPropertyGridSecret.SelectedObjects = null;
                    uxListViewSecrets.AllowDrop = false;
                    uxListViewSecrets.RemoveAllItems();
                    uxListViewSecrets.Refresh();
                    RefreshItemsCount();
                    uxListViewSecrets.BeginUpdate();
                    int s = 0, c = 0;
                    Action updateCount = () => uxStatusLabelSecertsCount.Text = $"{s + c} secrets"; // We use delegate and Invoke() below to execute on the thread that owns the control
                    IEnumerable<SecretProperties> secrets = Enumerable.Empty<SecretProperties>();
                    IEnumerable<CertificateProperties> certificates = Enumerable.Empty<CertificateProperties>();
                    await op.Invoke("access",
                        async () => // List Secrets
                        {
                            CurrentVaultAlias.SecretsCollectionEnabled = false;
                            secrets = await CurrentVault.ListSecretsAsync(0, (p) => { s = p;  Invoke(updateCount); }, cancellationToken: op.CancellationToken);
                            CurrentVaultAlias.SecretsCollectionEnabled = true;
                        },
                        async () => // List Key Vault Certificates
                        {
                            CurrentVaultAlias.CertificatesCollectionEnabled = false;
                            certificates = await CurrentVault.ListCertificatesAsync(0, (p) => { c = p;  Invoke(updateCount); }, cancellationToken: op.CancellationToken);
                            CurrentVaultAlias.CertificatesCollectionEnabled = true;
                        }
                    );
                    foreach (var secret in secrets)
                    {
                        uxListViewSecrets.AddOrReplaceItem(new ListViewItemSecret(this, secret));
                    }
                    foreach (var cert in certificates)
                    {
                        // Remove "secret" (in fact this is a certifiacte) which was returned as part of ListSecretsAsync
                        uxListViewSecrets.AddOrReplaceItem(new ListViewItemCertificate(this, cert));
                    }
                }
                catch (OperationCanceledException) // User cancelled one of the list operations
                {
                    uxListViewSecrets.RemoveAllItems();
                    CurrentVaultAlias.SecretsCollectionEnabled = false;
                    CurrentVaultAlias.CertificatesCollectionEnabled = false;
                }
                catch
                {
                    uxListViewSecrets.RemoveAllItems();
                    throw; // Propogate the error and show error to user
                }
                finally
                {
                    // We failed to list from all collections, disable controls
                    if (!CurrentVaultAlias.SecretsCollectionEnabled && !CurrentVaultAlias.CertificatesCollectionEnabled)
                    {
                        UxOperation.ToggleControls(false, uxButtonAdd, uxMenuItemAdd,
                            uxButtonEdit, uxMenuItemEdit, uxButtonToggle, uxMenuItemToggle, uxButtonDelete, uxMenuItemDelete, uxImageSearch, uxTextBoxSearch,
                            uxButtonShare, uxMenuItemShare, uxButtonFavorite, uxMenuItemFavorite, uxButtonPowershell);
                    }
                    else // We were able to list from one or from both collections
                    {
                        Text += $" ({CurrentVault.AuthenticatedUserName})";
                        uxAddSecret.Visible = uxAddSecret2.Visible = uxAddCert.Visible = uxAddCert2.Visible = uxAddFile.Visible = uxAddFile2.Visible = CurrentVaultAlias.SecretsCollectionEnabled;
                        uxAddKVCert.Visible = uxAddKVCert2.Visible = CurrentVaultAlias.CertificatesCollectionEnabled;
                        uxListViewSecrets.AllowDrop = true;
                        uxListViewSecrets.RefreshGroupsHeader();
                    }
                    uxListViewSecrets.EndUpdate();
                    uxTimerSearchTextTypingCompleted_Tick(null, EventArgs.Empty); // Refresh search and items count
                }
            }
        }

        private void uxListViewSecrets_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool singleItemSelected = (uxListViewSecrets.SelectedItems.Count == 1);
            bool manyItemsSelected = (uxListViewSecrets.SelectedItems.Count >= 1);
            bool itemEnabled = uxListViewSecrets.FirstSelectedItem?.Enabled ?? false;
            bool favorite = uxListViewSecrets.FirstSelectedItem?.Favorite ?? false;
            uxButtonEdit.Enabled = uxButtonShare.Enabled = itemEnabled;
            uxMenuItemEdit.Enabled = uxMenuItemShare.Enabled = itemEnabled;
            uxButtonDelete.Enabled = uxMenuItemDelete.Enabled = uxButtonFavorite.Enabled = uxMenuItemFavorite.Enabled = manyItemsSelected;
            uxButtonToggle.Enabled = uxMenuItemToggle.Enabled = singleItemSelected;
            uxButtonToggle.Text = uxMenuItemToggle.Text = itemEnabled ? "Disabl&e" : "&Enable";
            uxButtonToggle.ToolTipText = uxMenuItemToggle.ToolTipText = itemEnabled ? "Disable item" : "Enable item";
            uxMenuItemToggle.Text = uxButtonToggle.Text + "...";
            uxButtonFavorite.Checked = uxMenuItemFavorite.Checked = favorite;
            uxButtonFavorite.ToolTipText = uxMenuItemFavorite.ToolTipText = favorite ? "Remove item(s) from favorites group" : "Add item(s) to favorites group";
            uxPropertyGridSecret.SelectedObjects = uxListViewSecrets.SelectedItems?.Cast<ListViewItemBase>().ToArray();
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
                case Keys.Enter:
                    uxButtonEdit.PerformClick();
                    return;
            }
            if (!e.Control) return;
            switch (e.KeyCode)
            {
                case Keys.A:
                    foreach (ListViewItemBase item in uxListViewSecrets.Items) item.Selected = true;
                    return;
                case Keys.F:
                    uxTextBoxSearch.Focus();
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
                uxOpenFileDialog.FilterIndex = (sender == uxAddCertFromFile) || (sender == uxAddCertFromFile2) || (sender == uxAddKVCertFromFile) || (sender == uxAddKVCertFromFile2)  ? ContentType.Pkcs12.ToFilterIndex() : ContentType.None.ToFilterIndex();
                if (uxOpenFileDialog.ShowDialog() != DialogResult.OK) return null;
                fi = new FileInfo(uxOpenFileDialog.FileName);
            }
            if (fi.Length > Consts.MB)
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
                uxListViewSecrets.AddOrReplaceItem(newItem); // Overwrite flow
                uxTimerSearchTextTypingCompleted_Tick(null, EventArgs.Empty); // Refresh search
                newItem?.RefreshAndSelect();
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
                    using (var op = NewUxOperationWithProgress(uxButtonAdd, uxMenuItemAdd))
                    {
                        ListViewItemSecret lvis = null;
                        await op.Invoke("add secret to", async () => lvis = await ListViewItemSecret.NewAsync(this, nsDlg.PropertyObject, op.CancellationToken));
                        AddOrReplaceItemInListView(lvis);
                    }
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
                if ((sender == uxAddKVCertFromFile) || (sender == uxAddKVCertFromFile2))
                {
                    dtf.FileInfoObject = GetFileInfo(sender, e);
                    if (dtf.FileInfoObject == null) return;
                    certDlg = new CertificateDialog(this, dtf.FileInfoObject);
                }
                // Add certificate from store
                if ((sender == uxAddKVCertFromUserStore) || (sender == uxAddKVCertFromMachineStore) || (sender == uxAddKVCertFromUserStore2) || (sender == uxAddKVCertFromMachineStore2))
                {
                    var cert = Utils.SelectCertFromStore(StoreName.My, (sender == uxAddKVCertFromUserStore) || (sender == uxAddKVCertFromUserStore2) ? StoreLocation.CurrentUser : StoreLocation.LocalMachine, CurrentVaultAlias.Alias, Handle);
                    if (cert == null) return;
                    certDlg = new CertificateDialog(this, cert);
                }
                // DialogResult.Cancel is when user clicked cancel during password prompt from the ctor(), if OK was clicked, check for duplication by Name and Md5
                if ((certDlg != null) && (certDlg.DialogResult != DialogResult.Cancel) && (certDlg.ShowDialog() == DialogResult.OK) && ListViewItemBase.VerifyDuplication(this, null, certDlg.PropertyObject))
                {
                    using (var op = NewUxOperationWithProgress(uxButtonAdd, uxMenuItemAdd))
                    {
                        ListViewItemCertificate lvic = null;
                        await op.Invoke("add certificate to", async () => lvic = await ListViewItemCertificate.NewAsync(this, certDlg.PropertyObject, op.CancellationToken));
                        AddOrReplaceItemInListView(lvic);
                    }
                }
            }
        }
      
        private async void uxButtonEdit_Click(object sender, EventArgs e)
        {
            var item = uxListViewSecrets.FirstSelectedItem;
            if (item == null) return;
            if (!item.Active && MessageBox.Show($"'{item.Name}' {item.Kind} is not active or expired. In order to view or edit {item.Kind}, {Utils.AppName} must change the expiration times of '{item.Name}'. Are you sure you want to change Valid from time (UTC): '{Utils.NullableDateTimeToString(item.NotBefore)}' and Valid until time (UTC): '{Utils.NullableDateTimeToString(item.Expires)}' to one year from now?\n\nNote: You will be able to change back the expiration times in the Edit dialog if needed.",
                Utils.AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                using (var op = NewUxOperationWithProgress(uxButtonEdit, uxMenuItemEdit))
                {
                    ListViewItemBase newItem = null;
                    await op.Invoke($"update {item.Kind} in", async () => newItem = await item.ResetExpirationAsync(op.CancellationToken));
                    AddOrReplaceItemInListView(newItem, item);
                    item = newItem;
                };
            }
            if (item.Enabled && item.Active)
            {
                IEnumerable<object> versions = null;
                using (var op = NewUxOperationWithProgress(uxButtonEdit, uxMenuItemEdit)) await op.Invoke($"get {item.Kind} from", async () =>
                {
                    versions = await item.GetVersionsAsync(op.CancellationToken);
                });
                dynamic editDlg = item.GetEditDialog(item.Name, versions);
                // If OK was clicked, check for duplication by Name and Md5
                if ((editDlg.ShowDialog() == DialogResult.OK) && ListViewItemBase.VerifyDuplication(this, item.Name, editDlg.PropertyObject))
                {
                    using (var op = NewUxOperationWithProgress(uxButtonEdit, uxMenuItemEdit))
                    {
                        ListViewItemBase newItem = null;
                        await op.Invoke($"update {item.Kind} in", async () => newItem = await item.UpdateAsync(editDlg.OriginalObject, editDlg.PropertyObject, op.CancellationToken));
                        AddOrReplaceItemInListView(newItem, item);
                    };
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
                    using (var op = NewUxOperationWithProgress(uxButtonToggle, uxMenuItemToggle))
                    {
                        ListViewItemBase lvib = null;
                        await op.Invoke($"update {item.Kind} in", async () => lvib = await item.ToggleAsync(op.CancellationToken));
                        AddOrReplaceItemInListView(lvib, item);
                    }
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
                    using (var op = NewUxOperationWithProgress(uxButtonDelete, uxMenuItemDelete))
                    {
                        foreach (ListViewItemBase lvi in uxListViewSecrets.SelectedItems)
                        {
                            await op.Invoke("delete item in", async () => await lvi.DeleteAsync(op.CancellationToken));
                            AddOrReplaceItemInListView(null, lvi);
                        }
                    }
                }
            }
        }

        private void uxTimerSearchTextTypingCompleted_Tick(object sender, EventArgs e)
        {
            uxTimerSearchTextTypingCompleted.Stop();
            var ex = uxListViewSecrets.FindItemsWithText(uxTextBoxSearch.Text);
            uxTextBoxSearch.ForeColor = (ex == null) ? DefaultForeColor : Color.Red;
            uxTextBoxSearch.ToolTipText = (ex == null) ? uxImageSearch.ToolTipText : $"Regular expression error during {ex.Message}";
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
                using (var op = NewUxOperationWithProgress(uxButtonCopy, uxMenuItemCopy))
                {
                    PropertyObject po = null;
                    await op.Invoke($"get {item.Kind} from", async () => po = await item.GetAsync(op.CancellationToken));
                    po.CopyToClipboard(false); // Always execute on single thread apartment (STA) - UI thread, because of OLE limitations
                }
            }
        }

        private void uxButtonCopyLink_Click(object sender, EventArgs e)
        {
            var item = uxListViewSecrets.FirstSelectedItem;
            if (null != item)
            {
                using (var op = NewUxOperation(uxButtonCopyLink, uxMenuItemCopyLink))
                {
                    Utils.ClipboardSetHyperlink(item.Link, item.Name);
                }
            }
        }

        private async void uxButtonSave_Click(object sender, EventArgs e)
        {
            var item = uxListViewSecrets.FirstSelectedItem;
            if (null != item)
            {
                PropertyObject po = null;
                using (var op = NewUxOperationWithProgress(uxButtonSave, uxMenuItemSave)) await op.Invoke($"get {item.Kind} from", async () =>
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

        private void uxButtonExportToTsv_Click(object sender, EventArgs e)
        {
            using (var op = NewUxOperation(uxButtonExportToTsv))
            {
                uxSaveFileDialog.FileName = $"{CurrentVaultAlias.Alias}_{DateTime.Now.ToString("yyyy-MM-dd")}";
                uxSaveFileDialog.DefaultExt = ".tsv";
                uxSaveFileDialog.FilterIndex = ContentType.Tsv.ToFilterIndex();
                if (uxSaveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    uxListViewSecrets.ExportToTsv(uxSaveFileDialog.FileName);
                }
            }
        }

        private void uxButtonFavorite_Click(object sender, EventArgs e)
        {
            if (uxListViewSecrets.SelectedItems.Count > 0)
            {
                using (var op = NewUxOperationWithProgress(uxButtonFavorite, uxMenuItemFavorite))
                {
                    uxListViewSecrets.ToggleSelectedItemsToFromFavorites();
                    SaveSettings();
                }
                uxListViewSecrets_SelectedIndexChanged(null, EventArgs.Empty);
            }
        }

        private void uxButtonPowershell_Click(object sender, EventArgs e)
        {
            if (CurrentVault != null)
            {
                using (var op = NewUxOperation(uxButtonPowershell))
                {
                    string firstVaultName = (CurrentVault.VaultNames.Length > 0) ? CurrentVault.VaultNames[0] : "";
                    string secondVaultName = (CurrentVault.VaultNames.Length > 1) ? CurrentVault.VaultNames[1] : "";
                    Utils.LaunchPowerShell(CurrentVault.VaultsConfigFile, firstVaultName, secondVaultName);
                }
            }
        }

        private void uxButtonSettings_Click(object sender, EventArgs e)
        {
            using (var op = NewUxOperation(uxButtonSettings))
            {
                var dlg = new SettingsDialog();
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    ApplySettings();
                }
            }
        }

        private void uxButtonHelp_Click(object sender, EventArgs e)
        {
            using (var op = NewUxOperation(uxButtonHelp))
            {
                Process.Start("http://aka.ms/vaultexplorer");
            }
        }

        #region Drag & Drop

        // Flags to indicate if CTRL and SHIFT keys were down during start of the drag
        private bool _ctrlKeyPressed = false;
        private bool _shiftKeyPressed = false;

        private async void uxListViewSecrets_ItemDrag(object sender, ItemDragEventArgs e)
        {
            using (var op = NewUxOperation(uxButtonSave, uxMenuItemSave))
            {
                _ctrlKeyPressed = (ModifierKeys & Keys.Control) != 0;
                _shiftKeyPressed = (ModifierKeys & Keys.Shift) != 0;
                List<string> filesList = new List<string>();
                foreach (var item in uxListViewSecrets.SelectedItems.Cast<ListViewItemBase>())
                {
                    PropertyObject po = null;
                    await op.Invoke("get item from", async () => po = await item.GetAsync(op.CancellationToken));
                    // Pick .kv-secret or .kv-certificate or .url extension if CTRL and SHIFT are pressed
                    var filename = po.Name + (_ctrlKeyPressed & _shiftKeyPressed ? ContentType.KeyVaultLink.ToExtension() : _ctrlKeyPressed ? po.GetKeyVaultFileExtension() : po.GetContentType().ToExtension());
                    var fullName = Path.Combine(Path.GetTempPath(), filename);
                    po.SaveToFile(fullName);
                    filesList.Add(fullName);
                }
                var dataObject = new DataObject(DataFormats.FileDrop, filesList.ToArray());
                uxListViewSecrets.DoDragDrop(dataObject, DragDropEffects.Move);
            }
        }

        private void uxListViewSecrets_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            e.UseDefaultCursors = false;
            Cursor.Current = _ctrlKeyPressed & _shiftKeyPressed ? _moveLinkCursor : _ctrlKeyPressed ? _moveSecretCursor : _moveValueCursor;
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
