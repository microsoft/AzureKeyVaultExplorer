namespace Microsoft.Vault.Explorer
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.SplitContainer splitContainer1;
            System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("", System.Windows.Forms.HorizontalAlignment.Center);
            System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("", System.Windows.Forms.HorizontalAlignment.Center);
            System.Windows.Forms.ListViewGroup listViewGroup3 = new System.Windows.Forms.ListViewGroup("", System.Windows.Forms.HorizontalAlignment.Center);
            System.Windows.Forms.ListViewGroup listViewGroup4 = new System.Windows.Forms.ListViewGroup("", System.Windows.Forms.HorizontalAlignment.Center);
            System.Windows.Forms.ListViewGroup listViewGroup5 = new System.Windows.Forms.ListViewGroup("", System.Windows.Forms.HorizontalAlignment.Center);
            System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
            System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
            System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
            System.Windows.Forms.ToolStripLabel toolStripLabel1;
            System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
            System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
            System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
            System.Windows.Forms.ToolStripStatusLabel usStatusLabelSpring;
            System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.uxListViewSecrets = new Microsoft.Vault.Explorer.ListViewSecrets();
            this.uxContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.uxMenuItemAdd = new System.Windows.Forms.ToolStripMenuItem();
            this.uxAddSecret2 = new System.Windows.Forms.ToolStripMenuItem();
            this.uxAddCert2 = new System.Windows.Forms.ToolStripMenuItem();
            this.uxAddCertFromFile2 = new System.Windows.Forms.ToolStripMenuItem();
            this.uxAddCertFromUserStore2 = new System.Windows.Forms.ToolStripMenuItem();
            this.uxAddCertFromMachineStore2 = new System.Windows.Forms.ToolStripMenuItem();
            this.uxAddKVCert2 = new System.Windows.Forms.ToolStripMenuItem();
            this.uxAddKVCertFromFile2 = new System.Windows.Forms.ToolStripMenuItem();
            this.uxAddKVCertFromUserStore2 = new System.Windows.Forms.ToolStripMenuItem();
            this.uxAddKVCertFromMachineStore2 = new System.Windows.Forms.ToolStripMenuItem();
            this.uxAddFile2 = new System.Windows.Forms.ToolStripMenuItem();
            this.uxMenuItemEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.uxMenuItemToggle = new System.Windows.Forms.ToolStripMenuItem();
            this.uxMenuItemDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.uxMenuItemShare = new System.Windows.Forms.ToolStripMenuItem();
            this.uxMenuItemCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.uxMenuItemCopyLink = new System.Windows.Forms.ToolStripMenuItem();
            this.uxMenuItemSave = new System.Windows.Forms.ToolStripMenuItem();
            this.uxMenuItemExportToTsv = new System.Windows.Forms.ToolStripMenuItem();
            this.uxMenuItemFavorite = new System.Windows.Forms.ToolStripMenuItem();
            this.uxMenuItemRefresh = new System.Windows.Forms.ToolStripMenuItem();
            this.uxPropertyGridSecret = new System.Windows.Forms.PropertyGrid();
            this.uxToolStrip = new System.Windows.Forms.ToolStrip();
            this.uxComboBoxVaultAlias = new System.Windows.Forms.ToolStripComboBox();
            this.uxButtonAdd = new System.Windows.Forms.ToolStripSplitButton();
            this.uxAddSecret = new System.Windows.Forms.ToolStripMenuItem();
            this.uxAddCert = new System.Windows.Forms.ToolStripMenuItem();
            this.uxAddCertFromFile = new System.Windows.Forms.ToolStripMenuItem();
            this.uxAddCertFromUserStore = new System.Windows.Forms.ToolStripMenuItem();
            this.uxAddCertFromMachineStore = new System.Windows.Forms.ToolStripMenuItem();
            this.uxAddKVCert = new System.Windows.Forms.ToolStripMenuItem();
            this.uxAddKVCertFromFile = new System.Windows.Forms.ToolStripMenuItem();
            this.uxAddKVCertFromUserStore = new System.Windows.Forms.ToolStripMenuItem();
            this.uxAddKVCertFromMachineStore = new System.Windows.Forms.ToolStripMenuItem();
            this.uxAddFile = new System.Windows.Forms.ToolStripMenuItem();
            this.uxButtonEdit = new System.Windows.Forms.ToolStripButton();
            this.uxButtonToggle = new System.Windows.Forms.ToolStripButton();
            this.uxButtonDelete = new System.Windows.Forms.ToolStripButton();
            this.uxImageSearch = new System.Windows.Forms.ToolStripLabel();
            this.uxTextBoxSearch = new System.Windows.Forms.ToolStripTextBox();
            this.uxButtonShare = new System.Windows.Forms.ToolStripDropDownButton();
            this.uxButtonCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.uxButtonCopyLink = new System.Windows.Forms.ToolStripMenuItem();
            this.uxButtonSave = new System.Windows.Forms.ToolStripMenuItem();
            this.uxButtonExportToTsv = new System.Windows.Forms.ToolStripMenuItem();
            this.uxButtonFavorite = new System.Windows.Forms.ToolStripButton();
            this.uxButtonPowershell = new System.Windows.Forms.ToolStripButton();
            this.uxButtonSettings = new System.Windows.Forms.ToolStripButton();
            this.uxButtonHelp = new System.Windows.Forms.ToolStripButton();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.uxStatusStrip = new System.Windows.Forms.StatusStrip();
            this.uxStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.uxStatusProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.uxStatusLabelSecertsCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.uxStatusLabelSecretsSelected = new System.Windows.Forms.ToolStripStatusLabel();
            this.uxOpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.uxTimerSearchTextTypingCompleted = new System.Windows.Forms.Timer(this.components);
            this.uxSaveFileDialog = new System.Windows.Forms.SaveFileDialog();
            splitContainer1 = new System.Windows.Forms.SplitContainer();
            toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            usStatusLabelSpring = new System.Windows.Forms.ToolStripStatusLabel();
            toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            ((System.ComponentModel.ISupportInitialize)(splitContainer1)).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            this.uxContextMenuStrip.SuspendLayout();
            this.uxToolStrip.SuspendLayout();
            this.toolStripContainer1.BottomToolStripPanel.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.uxStatusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.DataBindings.Add(new System.Windows.Forms.Binding("SplitterDistance", global::Microsoft.Vault.Explorer.Properties.Settings.Default, "MainFormSplitterDistance", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer1.Location = new System.Drawing.Point(0, 0);
            splitContainer1.Margin = new System.Windows.Forms.Padding(2);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(this.uxListViewSecrets);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(this.uxPropertyGridSecret);
            splitContainer1.Size = new System.Drawing.Size(1403, 562);
            splitContainer1.SplitterDistance = global::Microsoft.Vault.Explorer.Properties.Settings.Default.MainFormSplitterDistance;
            splitContainer1.SplitterWidth = 6;
            splitContainer1.TabIndex = 8;
            // 
            // uxListViewSecrets
            // 
            this.uxListViewSecrets.ContextMenuStrip = this.uxContextMenuStrip;
            this.uxListViewSecrets.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uxListViewSecrets.Location = new System.Drawing.Point(0, 0);
            this.uxListViewSecrets.Margin = new System.Windows.Forms.Padding(2);
            this.uxListViewSecrets.Name = "uxListViewSecrets";
            this.uxListViewSecrets.Size = new System.Drawing.Size(1403, 260);
            this.uxListViewSecrets.TabIndex = 0;
            this.uxListViewSecrets.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.uxListViewSecrets_ItemDrag);
            this.uxListViewSecrets.SelectedIndexChanged += new System.EventHandler(this.uxListViewSecrets_SelectedIndexChanged);
            this.uxListViewSecrets.DragDrop += new System.Windows.Forms.DragEventHandler(this.uxListViewSecrets_DragDrop);
            this.uxListViewSecrets.DragEnter += new System.Windows.Forms.DragEventHandler(this.uxListViewSecrets_DragEnter);
            this.uxListViewSecrets.GiveFeedback += new System.Windows.Forms.GiveFeedbackEventHandler(this.uxListViewSecrets_GiveFeedback);
            this.uxListViewSecrets.DoubleClick += new System.EventHandler(this.uxButtonEdit_Click);
            this.uxListViewSecrets.KeyDown += new System.Windows.Forms.KeyEventHandler(this.uxListViewSecrets_KeyDown);
            this.uxListViewSecrets.KeyUp += new System.Windows.Forms.KeyEventHandler(this.uxListViewSecrets_KeyUp);
            // 
            // uxContextMenuStrip
            // 
            this.uxContextMenuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.uxContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.uxMenuItemAdd,
            this.uxMenuItemEdit,
            this.uxMenuItemToggle,
            this.uxMenuItemDelete,
            toolStripSeparator5,
            this.uxMenuItemShare,
            this.uxMenuItemFavorite,
            toolStripSeparator4,
            this.uxMenuItemRefresh});
            this.uxContextMenuStrip.Name = "uxContextMenuStrip";
            this.uxContextMenuStrip.Size = new System.Drawing.Size(190, 198);
            // 
            // uxMenuItemAdd
            // 
            this.uxMenuItemAdd.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.uxAddSecret2,
            this.uxAddCert2,
            this.uxAddKVCert2,
            this.uxAddFile2});
            this.uxMenuItemAdd.Enabled = false;
            this.uxMenuItemAdd.Image = global::Microsoft.Vault.Explorer.Properties.Resources.lock_add;
            this.uxMenuItemAdd.Name = "uxMenuItemAdd";
            this.uxMenuItemAdd.Size = new System.Drawing.Size(189, 26);
            this.uxMenuItemAdd.Text = "&Add";
            this.uxMenuItemAdd.ToolTipText = "Add item";
            this.uxMenuItemAdd.Click += new System.EventHandler(this.uxButtonAdd_Click);
            // 
            // uxAddSecret2
            // 
            this.uxAddSecret2.Image = global::Microsoft.Vault.Explorer.Properties.Resources.key;
            this.uxAddSecret2.Name = "uxAddSecret2";
            this.uxAddSecret2.Size = new System.Drawing.Size(217, 26);
            this.uxAddSecret2.Text = "&Secret...";
            this.uxAddSecret2.Click += new System.EventHandler(this.uxMenuItemAddSecret_Click);
            // 
            // uxAddCert2
            // 
            this.uxAddCert2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.uxAddCertFromFile2,
            this.uxAddCertFromUserStore2,
            this.uxAddCertFromMachineStore2});
            this.uxAddCert2.Image = global::Microsoft.Vault.Explorer.Properties.Resources.certificate;
            this.uxAddCert2.Name = "uxAddCert2";
            this.uxAddCert2.Size = new System.Drawing.Size(217, 26);
            this.uxAddCert2.Text = "Certificate...";
            this.uxAddCert2.Click += new System.EventHandler(this.uxMenuItemAddSecret_Click);
            // 
            // uxAddCertFromFile2
            // 
            this.uxAddCertFromFile2.Name = "uxAddCertFromFile2";
            this.uxAddCertFromFile2.Size = new System.Drawing.Size(224, 26);
            this.uxAddCertFromFile2.Text = "From &file...";
            this.uxAddCertFromFile2.Click += new System.EventHandler(this.uxMenuItemAddSecret_Click);
            // 
            // uxAddCertFromUserStore2
            // 
            this.uxAddCertFromUserStore2.Name = "uxAddCertFromUserStore2";
            this.uxAddCertFromUserStore2.Size = new System.Drawing.Size(224, 26);
            this.uxAddCertFromUserStore2.Text = "From &user store...";
            this.uxAddCertFromUserStore2.Click += new System.EventHandler(this.uxMenuItemAddSecret_Click);
            // 
            // uxAddCertFromMachineStore2
            // 
            this.uxAddCertFromMachineStore2.Name = "uxAddCertFromMachineStore2";
            this.uxAddCertFromMachineStore2.Size = new System.Drawing.Size(224, 26);
            this.uxAddCertFromMachineStore2.Text = "From &machine store...";
            this.uxAddCertFromMachineStore2.Click += new System.EventHandler(this.uxMenuItemAddSecret_Click);
            // 
            // uxAddKVCert2
            // 
            this.uxAddKVCert2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.uxAddKVCertFromFile2,
            this.uxAddKVCertFromUserStore2,
            this.uxAddKVCertFromMachineStore2});
            this.uxAddKVCert2.Image = global::Microsoft.Vault.Explorer.Properties.Resources.certificate2;
            this.uxAddKVCert2.Name = "uxAddKVCert2";
            this.uxAddKVCert2.Size = new System.Drawing.Size(217, 26);
            this.uxAddKVCert2.Text = "Key Vault Certificate";
            // 
            // uxAddKVCertFromFile2
            // 
            this.uxAddKVCertFromFile2.Name = "uxAddKVCertFromFile2";
            this.uxAddKVCertFromFile2.Size = new System.Drawing.Size(224, 26);
            this.uxAddKVCertFromFile2.Text = "From &file...";
            this.uxAddKVCertFromFile2.Click += new System.EventHandler(this.uxMenuItemAddKVCertificate_Click);
            // 
            // uxAddKVCertFromUserStore2
            // 
            this.uxAddKVCertFromUserStore2.Name = "uxAddKVCertFromUserStore2";
            this.uxAddKVCertFromUserStore2.Size = new System.Drawing.Size(224, 26);
            this.uxAddKVCertFromUserStore2.Text = "From &user store...";
            this.uxAddKVCertFromUserStore2.Click += new System.EventHandler(this.uxMenuItemAddKVCertificate_Click);
            // 
            // uxAddKVCertFromMachineStore2
            // 
            this.uxAddKVCertFromMachineStore2.Name = "uxAddKVCertFromMachineStore2";
            this.uxAddKVCertFromMachineStore2.Size = new System.Drawing.Size(224, 26);
            this.uxAddKVCertFromMachineStore2.Text = "From &machine store...";
            this.uxAddKVCertFromMachineStore2.Click += new System.EventHandler(this.uxMenuItemAddKVCertificate_Click);
            // 
            // uxAddFile2
            // 
            this.uxAddFile2.Image = global::Microsoft.Vault.Explorer.Properties.Resources.folder_key;
            this.uxAddFile2.Name = "uxAddFile2";
            this.uxAddFile2.Size = new System.Drawing.Size(217, 26);
            this.uxAddFile2.Text = "&File...";
            this.uxAddFile2.Click += new System.EventHandler(this.uxMenuItemAddSecret_Click);
            // 
            // uxMenuItemEdit
            // 
            this.uxMenuItemEdit.Enabled = false;
            this.uxMenuItemEdit.Image = global::Microsoft.Vault.Explorer.Properties.Resources.lock_edit;
            this.uxMenuItemEdit.Name = "uxMenuItemEdit";
            this.uxMenuItemEdit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
            this.uxMenuItemEdit.Size = new System.Drawing.Size(189, 26);
            this.uxMenuItemEdit.Text = "&Edit...";
            this.uxMenuItemEdit.ToolTipText = "Edit item";
            this.uxMenuItemEdit.Click += new System.EventHandler(this.uxButtonEdit_Click);
            // 
            // uxMenuItemToggle
            // 
            this.uxMenuItemToggle.Enabled = false;
            this.uxMenuItemToggle.Image = global::Microsoft.Vault.Explorer.Properties.Resources.lock_break;
            this.uxMenuItemToggle.Name = "uxMenuItemToggle";
            this.uxMenuItemToggle.Size = new System.Drawing.Size(189, 26);
            this.uxMenuItemToggle.Text = "Disabl&e...";
            this.uxMenuItemToggle.ToolTipText = "Disable item";
            this.uxMenuItemToggle.Click += new System.EventHandler(this.uxButtonToggle_Click);
            // 
            // uxMenuItemDelete
            // 
            this.uxMenuItemDelete.Enabled = false;
            this.uxMenuItemDelete.Image = global::Microsoft.Vault.Explorer.Properties.Resources.lock_delete;
            this.uxMenuItemDelete.Name = "uxMenuItemDelete";
            this.uxMenuItemDelete.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.uxMenuItemDelete.Size = new System.Drawing.Size(189, 26);
            this.uxMenuItemDelete.Text = "&Delete...";
            this.uxMenuItemDelete.ToolTipText = "Delete item";
            this.uxMenuItemDelete.Click += new System.EventHandler(this.uxButtonDelete_Click);
            // 
            // toolStripSeparator5
            // 
            toolStripSeparator5.Name = "toolStripSeparator5";
            toolStripSeparator5.Size = new System.Drawing.Size(186, 6);
            // 
            // uxMenuItemShare
            // 
            this.uxMenuItemShare.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.uxMenuItemCopy,
            this.uxMenuItemCopyLink,
            this.uxMenuItemSave,
            toolStripMenuItem2,
            this.uxMenuItemExportToTsv});
            this.uxMenuItemShare.Enabled = false;
            this.uxMenuItemShare.Image = global::Microsoft.Vault.Explorer.Properties.Resources.group;
            this.uxMenuItemShare.Name = "uxMenuItemShare";
            this.uxMenuItemShare.Size = new System.Drawing.Size(189, 26);
            this.uxMenuItemShare.Text = "&Share";
            // 
            // uxMenuItemCopy
            // 
            this.uxMenuItemCopy.Image = global::Microsoft.Vault.Explorer.Properties.Resources.page_copy;
            this.uxMenuItemCopy.Name = "uxMenuItemCopy";
            this.uxMenuItemCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.uxMenuItemCopy.Size = new System.Drawing.Size(236, 26);
            this.uxMenuItemCopy.Text = "Copy";
            this.uxMenuItemCopy.Click += new System.EventHandler(this.uxButtonCopy_Click);
            // 
            // uxMenuItemCopyLink
            // 
            this.uxMenuItemCopyLink.Image = global::Microsoft.Vault.Explorer.Properties.Resources.link;
            this.uxMenuItemCopyLink.Name = "uxMenuItemCopyLink";
            this.uxMenuItemCopyLink.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.C)));
            this.uxMenuItemCopyLink.Size = new System.Drawing.Size(236, 26);
            this.uxMenuItemCopyLink.Text = "Copy &link";
            this.uxMenuItemCopyLink.ToolTipText = "Copy link to clipboard";
            this.uxMenuItemCopyLink.Click += new System.EventHandler(this.uxButtonCopyLink_Click);
            // 
            // uxMenuItemSave
            // 
            this.uxMenuItemSave.Image = global::Microsoft.Vault.Explorer.Properties.Resources.disk;
            this.uxMenuItemSave.Name = "uxMenuItemSave";
            this.uxMenuItemSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.uxMenuItemSave.Size = new System.Drawing.Size(236, 26);
            this.uxMenuItemSave.Text = "&Save...";
            this.uxMenuItemSave.ToolTipText = "Save item to file";
            this.uxMenuItemSave.Click += new System.EventHandler(this.uxButtonSave_Click);
            // 
            // toolStripMenuItem2
            // 
            toolStripMenuItem2.Name = "toolStripMenuItem2";
            toolStripMenuItem2.Size = new System.Drawing.Size(233, 6);
            // 
            // exportToTsvToolStripMenuItem
            // 
            this.uxMenuItemExportToTsv.Image = global::Microsoft.Vault.Explorer.Properties.Resources.table;
            this.uxMenuItemExportToTsv.Name = "exportToTsvToolStripMenuItem";
            this.uxMenuItemExportToTsv.Size = new System.Drawing.Size(236, 26);
            this.uxMenuItemExportToTsv.Text = "&Export to Tsv...";
            this.uxMenuItemExportToTsv.ToolTipText = "Export all or selected items to .tsv file";
            this.uxMenuItemExportToTsv.Click += new System.EventHandler(this.uxButtonExportToTsv_Click);
            // 
            // uxMenuItemFavorite
            // 
            this.uxMenuItemFavorite.Enabled = false;
            this.uxMenuItemFavorite.Image = global::Microsoft.Vault.Explorer.Properties.Resources.star;
            this.uxMenuItemFavorite.Name = "uxMenuItemFavorite";
            this.uxMenuItemFavorite.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this.uxMenuItemFavorite.Size = new System.Drawing.Size(189, 26);
            this.uxMenuItemFavorite.Text = "Favorite";
            this.uxMenuItemFavorite.ToolTipText = "Add item(s) to favorites group";
            this.uxMenuItemFavorite.Click += new System.EventHandler(this.uxButtonFavorite_Click);
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new System.Drawing.Size(186, 6);
            // 
            // uxMenuItemRefresh
            // 
            this.uxMenuItemRefresh.Enabled = false;
            this.uxMenuItemRefresh.Image = global::Microsoft.Vault.Explorer.Properties.Resources.lock_go;
            this.uxMenuItemRefresh.Name = "uxMenuItemRefresh";
            this.uxMenuItemRefresh.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.uxMenuItemRefresh.Size = new System.Drawing.Size(189, 26);
            this.uxMenuItemRefresh.Text = "&Refresh";
            this.uxMenuItemRefresh.ToolTipText = "Refresh all items";
            this.uxMenuItemRefresh.Click += new System.EventHandler(this.uxMenuItemRefresh_Click);
            // 
            // uxPropertyGridSecret
            // 
            this.uxPropertyGridSecret.DisabledItemForeColor = System.Drawing.SystemColors.WindowText;
            this.uxPropertyGridSecret.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uxPropertyGridSecret.HelpVisible = false;
            this.uxPropertyGridSecret.Location = new System.Drawing.Point(0, 0);
            this.uxPropertyGridSecret.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.uxPropertyGridSecret.Name = "uxPropertyGridSecret";
            this.uxPropertyGridSecret.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.uxPropertyGridSecret.Size = new System.Drawing.Size(1403, 296);
            this.uxPropertyGridSecret.TabIndex = 0;
            this.uxPropertyGridSecret.ToolbarVisible = false;
            // 
            // toolStripLabel1
            // 
            toolStripLabel1.Name = "toolStripLabel1";
            toolStripLabel1.Size = new System.Drawing.Size(42, 25);
            toolStripLabel1.Text = "Vault";
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(6, 28);
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new System.Drawing.Size(6, 28);
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new System.Drawing.Size(6, 28);
            // 
            // usStatusLabelSpring
            // 
            usStatusLabelSpring.Name = "usStatusLabelSpring";
            usStatusLabelSpring.Size = new System.Drawing.Size(1196, 20);
            usStatusLabelSpring.Spring = true;
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new System.Drawing.Size(175, 6);
            // 
            // uxToolStrip
            // 
            this.uxToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.uxToolStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.uxToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            toolStripLabel1,
            this.uxComboBoxVaultAlias,
            toolStripSeparator1,
            this.uxButtonAdd,
            this.uxButtonEdit,
            this.uxButtonToggle,
            this.uxButtonDelete,
            toolStripSeparator2,
            this.uxImageSearch,
            this.uxTextBoxSearch,
            this.uxButtonShare,
            this.uxButtonFavorite,
            toolStripSeparator3,
            this.uxButtonPowershell,
            this.uxButtonSettings,
            this.uxButtonHelp});
            this.uxToolStrip.Location = new System.Drawing.Point(3, 0);
            this.uxToolStrip.Name = "uxToolStrip";
            this.uxToolStrip.Size = new System.Drawing.Size(1301, 28);
            this.uxToolStrip.TabIndex = 0;
            // 
            // uxComboBoxVaultAlias
            // 
            this.uxComboBoxVaultAlias.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
            this.uxComboBoxVaultAlias.MaxDropDownItems = 16;
            this.uxComboBoxVaultAlias.Name = "uxComboBoxVaultAlias";
            this.uxComboBoxVaultAlias.Size = new System.Drawing.Size(250, 28);
            this.uxComboBoxVaultAlias.DropDown += new System.EventHandler(this.uxComboBoxVaultAlias_DropDown);
            this.uxComboBoxVaultAlias.DropDownClosed += new System.EventHandler(this.uxComboBoxVaultAlias_DropDownClosed);
            this.uxComboBoxVaultAlias.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            // 
            // uxButtonAdd
            // 
            this.uxButtonAdd.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.uxAddSecret,
            this.uxAddCert,
            this.uxAddKVCert,
            this.uxAddFile});
            this.uxButtonAdd.Enabled = false;
            this.uxButtonAdd.Image = global::Microsoft.Vault.Explorer.Properties.Resources.lock_add;
            this.uxButtonAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.uxButtonAdd.Name = "uxButtonAdd";
            this.uxButtonAdd.Size = new System.Drawing.Size(76, 25);
            this.uxButtonAdd.Text = "&Add";
            this.uxButtonAdd.ToolTipText = "Add item";
            this.uxButtonAdd.Click += new System.EventHandler(this.uxButtonAdd_Click);
            // 
            // uxAddSecret
            // 
            this.uxAddSecret.Image = global::Microsoft.Vault.Explorer.Properties.Resources.key;
            this.uxAddSecret.Name = "uxAddSecret";
            this.uxAddSecret.Size = new System.Drawing.Size(217, 26);
            this.uxAddSecret.Text = "&Secret...";
            this.uxAddSecret.Click += new System.EventHandler(this.uxMenuItemAddSecret_Click);
            // 
            // uxAddCert
            // 
            this.uxAddCert.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.uxAddCertFromFile,
            this.uxAddCertFromUserStore,
            this.uxAddCertFromMachineStore});
            this.uxAddCert.Image = global::Microsoft.Vault.Explorer.Properties.Resources.certificate;
            this.uxAddCert.Name = "uxAddCert";
            this.uxAddCert.Size = new System.Drawing.Size(217, 26);
            this.uxAddCert.Text = "Ce&rtificate";
            // 
            // uxAddCertFromFile
            // 
            this.uxAddCertFromFile.Name = "uxAddCertFromFile";
            this.uxAddCertFromFile.Size = new System.Drawing.Size(224, 26);
            this.uxAddCertFromFile.Text = "From &file...";
            this.uxAddCertFromFile.Click += new System.EventHandler(this.uxMenuItemAddSecret_Click);
            // 
            // uxAddCertFromUserStore
            // 
            this.uxAddCertFromUserStore.Name = "uxAddCertFromUserStore";
            this.uxAddCertFromUserStore.Size = new System.Drawing.Size(224, 26);
            this.uxAddCertFromUserStore.Text = "From &user store...";
            this.uxAddCertFromUserStore.Click += new System.EventHandler(this.uxMenuItemAddSecret_Click);
            // 
            // uxAddCertFromMachineStore
            // 
            this.uxAddCertFromMachineStore.Name = "uxAddCertFromMachineStore";
            this.uxAddCertFromMachineStore.Size = new System.Drawing.Size(224, 26);
            this.uxAddCertFromMachineStore.Text = "From &machine store...";
            this.uxAddCertFromMachineStore.Click += new System.EventHandler(this.uxMenuItemAddSecret_Click);
            // 
            // uxAddKVCert
            // 
            this.uxAddKVCert.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.uxAddKVCertFromFile,
            this.uxAddKVCertFromUserStore,
            this.uxAddKVCertFromMachineStore});
            this.uxAddKVCert.Image = global::Microsoft.Vault.Explorer.Properties.Resources.certificate2;
            this.uxAddKVCert.Name = "uxAddKVCert";
            this.uxAddKVCert.Size = new System.Drawing.Size(217, 26);
            this.uxAddKVCert.Text = "Key Vault &Certificate";
            // 
            // uxAddKVCertFromFile
            // 
            this.uxAddKVCertFromFile.Name = "uxAddKVCertFromFile";
            this.uxAddKVCertFromFile.Size = new System.Drawing.Size(224, 26);
            this.uxAddKVCertFromFile.Text = "From &file...";
            this.uxAddKVCertFromFile.Click += new System.EventHandler(this.uxMenuItemAddKVCertificate_Click);
            // 
            // uxAddKVCertFromUserStore
            // 
            this.uxAddKVCertFromUserStore.Name = "uxAddKVCertFromUserStore";
            this.uxAddKVCertFromUserStore.Size = new System.Drawing.Size(224, 26);
            this.uxAddKVCertFromUserStore.Text = "From &user store...";
            this.uxAddKVCertFromUserStore.Click += new System.EventHandler(this.uxMenuItemAddKVCertificate_Click);
            // 
            // uxAddKVCertFromMachineStore
            // 
            this.uxAddKVCertFromMachineStore.Name = "uxAddKVCertFromMachineStore";
            this.uxAddKVCertFromMachineStore.Size = new System.Drawing.Size(224, 26);
            this.uxAddKVCertFromMachineStore.Text = "From &machine store...";
            this.uxAddKVCertFromMachineStore.Click += new System.EventHandler(this.uxMenuItemAddKVCertificate_Click);
            // 
            // uxAddFile
            // 
            this.uxAddFile.Image = global::Microsoft.Vault.Explorer.Properties.Resources.folder_key;
            this.uxAddFile.Name = "uxAddFile";
            this.uxAddFile.Size = new System.Drawing.Size(217, 26);
            this.uxAddFile.Text = "&File...";
            this.uxAddFile.Click += new System.EventHandler(this.uxMenuItemAddSecret_Click);
            // 
            // uxButtonEdit
            // 
            this.uxButtonEdit.Enabled = false;
            this.uxButtonEdit.Image = global::Microsoft.Vault.Explorer.Properties.Resources.lock_edit;
            this.uxButtonEdit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.uxButtonEdit.Name = "uxButtonEdit";
            this.uxButtonEdit.Size = new System.Drawing.Size(59, 25);
            this.uxButtonEdit.Text = "&Edit";
            this.uxButtonEdit.ToolTipText = "Edit item";
            this.uxButtonEdit.Click += new System.EventHandler(this.uxButtonEdit_Click);
            // 
            // uxButtonToggle
            // 
            this.uxButtonToggle.Enabled = false;
            this.uxButtonToggle.Image = global::Microsoft.Vault.Explorer.Properties.Resources.lock_break;
            this.uxButtonToggle.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.uxButtonToggle.Name = "uxButtonToggle";
            this.uxButtonToggle.Size = new System.Drawing.Size(83, 25);
            this.uxButtonToggle.Text = "Disabl&e";
            this.uxButtonToggle.ToolTipText = "Disable item";
            this.uxButtonToggle.Click += new System.EventHandler(this.uxButtonToggle_Click);
            // 
            // uxButtonDelete
            // 
            this.uxButtonDelete.Enabled = false;
            this.uxButtonDelete.Image = global::Microsoft.Vault.Explorer.Properties.Resources.lock_delete;
            this.uxButtonDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.uxButtonDelete.Name = "uxButtonDelete";
            this.uxButtonDelete.Size = new System.Drawing.Size(77, 25);
            this.uxButtonDelete.Text = "&Delete";
            this.uxButtonDelete.ToolTipText = "Delete item";
            this.uxButtonDelete.Click += new System.EventHandler(this.uxButtonDelete_Click);
            // 
            // uxImageSearch
            // 
            this.uxImageSearch.Enabled = false;
            this.uxImageSearch.Image = global::Microsoft.Vault.Explorer.Properties.Resources.magnifier;
            this.uxImageSearch.Name = "uxImageSearch";
            this.uxImageSearch.Size = new System.Drawing.Size(20, 25);
            this.uxImageSearch.ToolTipText = "Regular expression search\nExamples: elize, enabled=false";
            // 
            // uxTextBoxSearch
            // 
            this.uxTextBoxSearch.Enabled = false;
            this.uxTextBoxSearch.Name = "uxTextBoxSearch";
            this.uxTextBoxSearch.Size = new System.Drawing.Size(200, 28);
            this.uxTextBoxSearch.ToolTipText = "Regular expression search\nExamples: elize, enabled=false";
            this.uxTextBoxSearch.TextChanged += new System.EventHandler(this.uxTextBoxSearch_TextChanged);
            // 
            // uxButtonShare
            // 
            this.uxButtonShare.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.uxButtonCopy,
            this.uxButtonCopyLink,
            this.uxButtonSave,
            toolStripMenuItem1,
            this.uxButtonExportToTsv});
            this.uxButtonShare.Enabled = false;
            this.uxButtonShare.Image = global::Microsoft.Vault.Explorer.Properties.Resources.group;
            this.uxButtonShare.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.uxButtonShare.Name = "uxButtonShare";
            this.uxButtonShare.Size = new System.Drawing.Size(80, 25);
            this.uxButtonShare.Text = "Share";
            this.uxButtonShare.ToolTipText = "Share item";
            // 
            // uxButtonCopy
            // 
            this.uxButtonCopy.Image = global::Microsoft.Vault.Explorer.Properties.Resources.page_copy;
            this.uxButtonCopy.Name = "uxButtonCopy";
            this.uxButtonCopy.Size = new System.Drawing.Size(178, 26);
            this.uxButtonCopy.Text = "&Copy";
            this.uxButtonCopy.ToolTipText = "Copy value to clipboard";
            this.uxButtonCopy.Click += new System.EventHandler(this.uxButtonCopy_Click);
            // 
            // uxButtonCopyLink
            // 
            this.uxButtonCopyLink.Image = global::Microsoft.Vault.Explorer.Properties.Resources.link;
            this.uxButtonCopyLink.Name = "uxButtonCopyLink";
            this.uxButtonCopyLink.Size = new System.Drawing.Size(178, 26);
            this.uxButtonCopyLink.Text = "Copy &link";
            this.uxButtonCopyLink.ToolTipText = "Copy link to clipboard";
            this.uxButtonCopyLink.Click += new System.EventHandler(this.uxButtonCopyLink_Click);
            // 
            // uxButtonSave
            // 
            this.uxButtonSave.Image = global::Microsoft.Vault.Explorer.Properties.Resources.disk;
            this.uxButtonSave.Name = "uxButtonSave";
            this.uxButtonSave.Size = new System.Drawing.Size(178, 26);
            this.uxButtonSave.Text = "&Save...";
            this.uxButtonSave.ToolTipText = "Save item to file";
            this.uxButtonSave.Click += new System.EventHandler(this.uxButtonSave_Click);
            // 
            // uxButtonExportToTsv
            // 
            this.uxButtonExportToTsv.Image = global::Microsoft.Vault.Explorer.Properties.Resources.table;
            this.uxButtonExportToTsv.Name = "uxButtonExportToTsv";
            this.uxButtonExportToTsv.Size = new System.Drawing.Size(178, 26);
            this.uxButtonExportToTsv.Text = "&Export to Tsv...";
            this.uxButtonExportToTsv.ToolTipText = "Export all or selected items to .tsv file";
            this.uxButtonExportToTsv.Click += new System.EventHandler(this.uxButtonExportToTsv_Click);
            // 
            // uxButtonFavorite
            // 
            this.uxButtonFavorite.Enabled = false;
            this.uxButtonFavorite.Image = global::Microsoft.Vault.Explorer.Properties.Resources.star;
            this.uxButtonFavorite.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.uxButtonFavorite.Name = "uxButtonFavorite";
            this.uxButtonFavorite.Size = new System.Drawing.Size(85, 25);
            this.uxButtonFavorite.Text = "&Favorite";
            this.uxButtonFavorite.ToolTipText = "Add item(s) to favorites group";
            this.uxButtonFavorite.Click += new System.EventHandler(this.uxButtonFavorite_Click);
            // 
            // uxButtonPowershell
            // 
            this.uxButtonPowershell.Enabled = false;
            this.uxButtonPowershell.Image = global::Microsoft.Vault.Explorer.Properties.Resources.powershell;
            this.uxButtonPowershell.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.uxButtonPowershell.Name = "uxButtonPowershell";
            this.uxButtonPowershell.Size = new System.Drawing.Size(105, 25);
            this.uxButtonPowershell.Text = "&PowerShell";
            this.uxButtonPowershell.ToolTipText = "Open PowerShell session with current vault(s)";
            this.uxButtonPowershell.Click += new System.EventHandler(this.uxButtonPowershell_Click);
            // 
            // uxButtonSettings
            // 
            this.uxButtonSettings.Image = global::Microsoft.Vault.Explorer.Properties.Resources.wrench;
            this.uxButtonSettings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.uxButtonSettings.Name = "uxButtonSettings";
            this.uxButtonSettings.Size = new System.Drawing.Size(86, 25);
            this.uxButtonSettings.Text = "&Settings";
            this.uxButtonSettings.ToolTipText = "Options and About Vault Explorer";
            this.uxButtonSettings.Click += new System.EventHandler(this.uxButtonSettings_Click);
            // 
            // uxButtonHelp
            // 
            this.uxButtonHelp.Image = ((System.Drawing.Image)(resources.GetObject("uxButtonHelp.Image")));
            this.uxButtonHelp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.uxButtonHelp.Name = "uxButtonHelp";
            this.uxButtonHelp.Size = new System.Drawing.Size(65, 25);
            this.uxButtonHelp.Text = "&Help";
            this.uxButtonHelp.ToolTipText = resources.GetString("uxButtonHelp.ToolTipText");
            this.uxButtonHelp.Click += new System.EventHandler(this.uxButtonHelp_Click);
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.BottomToolStripPanel
            // 
            this.toolStripContainer1.BottomToolStripPanel.Controls.Add(this.uxStatusStrip);
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(splitContainer1);
            this.toolStripContainer1.ContentPanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.TabIndex = 8;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.uxToolStrip);
            // 
            // uxStatusStrip
            // 
            this.uxStatusStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.uxStatusStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.uxStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.uxStatusLabel,
            usStatusLabelSpring,
            this.uxStatusProgressBar,
            this.uxStatusLabelSecertsCount,
            this.uxStatusLabelSecretsSelected});
            this.uxStatusStrip.Location = new System.Drawing.Point(0, 0);
            this.uxStatusStrip.Name = "uxStatusStrip";
            this.uxStatusStrip.ShowItemToolTips = true;
            this.uxStatusStrip.Size = new System.Drawing.Size(1403, 25);
            this.uxStatusStrip.TabIndex = 0;
            // 
            // uxStatusLabel
            // 
            this.uxStatusLabel.Name = "uxStatusLabel";
            this.uxStatusLabel.Size = new System.Drawing.Size(50, 20);
            this.uxStatusLabel.Text = "Ready";
            this.uxStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // uxStatusProgressBar
            // 
            this.uxStatusProgressBar.AutoSize = false;
            this.uxStatusProgressBar.Margin = new System.Windows.Forms.Padding(1, 3, 0, 3);
            this.uxStatusProgressBar.Name = "uxStatusProgressBar";
            this.uxStatusProgressBar.Size = new System.Drawing.Size(200, 19);
            this.uxStatusProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.uxStatusProgressBar.Visible = false;
            // 
            // uxStatusLabelSecertsCount
            // 
            this.uxStatusLabelSecertsCount.Name = "uxStatusLabelSecertsCount";
            this.uxStatusLabelSecertsCount.Size = new System.Drawing.Size(66, 20);
            this.uxStatusLabelSecertsCount.Text = "0 secrets";
            this.uxStatusLabelSecertsCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // uxStatusLabelSecretsSelected
            // 
            this.uxStatusLabelSecretsSelected.Name = "uxStatusLabelSecretsSelected";
            this.uxStatusLabelSecretsSelected.Size = new System.Drawing.Size(76, 20);
            this.uxStatusLabelSecretsSelected.Text = "0 selected";
            // 
            // uxOpenFileDialog
            // 
            this.uxOpenFileDialog.Filter = resources.GetString("uxOpenFileDialog.Filter");
            this.uxOpenFileDialog.Title = "Open file";
            // 
            // uxTimerSearchTextTypingCompleted
            // 
            this.uxTimerSearchTextTypingCompleted.Interval = 250;
            this.uxTimerSearchTextTypingCompleted.Tick += new System.EventHandler(this.uxTimerSearchTextTypingCompleted_Tick);
            // 
            // uxSaveFileDialog
            // 
            this.uxSaveFileDialog.Filter = resources.GetString("uxSaveFileDialog.Filter");
            this.uxSaveFileDialog.Title = "Save As";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1700, 600);
            this.Controls.Add(this.toolStripContainer1);
            this.DataBindings.Add(new System.Windows.Forms.Binding("Location", global::Microsoft.Vault.Explorer.Properties.Settings.Default, "MainFormLocation", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = global::Microsoft.Vault.Explorer.Properties.Settings.Default.MainFormLocation;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MinimumSize = new System.Drawing.Size(700, 500);
            this.Name = "MainForm";
            this.Text = "Azure Key Vault Explorer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(splitContainer1)).EndInit();
            splitContainer1.ResumeLayout(false);
            this.uxContextMenuStrip.ResumeLayout(false);
            this.uxToolStrip.ResumeLayout(false);
            this.uxToolStrip.PerformLayout();
            this.toolStripContainer1.BottomToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.BottomToolStripPanel.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.uxStatusStrip.ResumeLayout(false);
            this.uxStatusStrip.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.StatusStrip uxStatusStrip;
        private System.Windows.Forms.ToolStripStatusLabel uxStatusLabel;
        private ListViewSecrets uxListViewSecrets;
        private System.Windows.Forms.PropertyGrid uxPropertyGridSecret;
        private System.Windows.Forms.ToolStripComboBox uxComboBoxVaultAlias;
        private System.Windows.Forms.ToolStripMenuItem uxButtonCopy;
        private System.Windows.Forms.ToolStripButton uxButtonHelp;
        private System.Windows.Forms.ToolStripButton uxButtonEdit;
        private System.Windows.Forms.ToolStripButton uxButtonDelete;
        private System.Windows.Forms.ToolStripSplitButton uxButtonAdd;
        private System.Windows.Forms.ToolStripMenuItem uxAddSecret;
        private System.Windows.Forms.ToolStripMenuItem uxAddCert;
        private System.Windows.Forms.ToolStripButton uxButtonSettings;
        private System.Windows.Forms.ContextMenuStrip uxContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem uxMenuItemAdd;
        private System.Windows.Forms.ToolStripMenuItem uxMenuItemEdit;
        private System.Windows.Forms.ToolStripMenuItem uxMenuItemDelete;
        private System.Windows.Forms.ToolStripMenuItem uxMenuItemRefresh;
        private System.Windows.Forms.ToolStripButton uxButtonToggle;
        private System.Windows.Forms.ToolStripMenuItem uxMenuItemToggle;
        private System.Windows.Forms.ToolStripMenuItem uxAddSecret2;
        private System.Windows.Forms.ToolStripMenuItem uxAddCert2;
        private System.Windows.Forms.ToolStripMenuItem uxAddFile2;
        private System.Windows.Forms.ToolStripMenuItem uxAddFile;
        private System.Windows.Forms.OpenFileDialog uxOpenFileDialog;
        private System.Windows.Forms.ToolStripTextBox uxTextBoxSearch;
        private System.Windows.Forms.ToolStripLabel uxImageSearch;
        private System.Windows.Forms.Timer uxTimerSearchTextTypingCompleted;
        private System.Windows.Forms.ToolStrip uxToolStrip;
        private System.Windows.Forms.ToolStripStatusLabel uxStatusLabelSecertsCount;
        private System.Windows.Forms.ToolStripMenuItem uxButtonSave;
        private System.Windows.Forms.SaveFileDialog uxSaveFileDialog;
        private System.Windows.Forms.ToolStripProgressBar uxStatusProgressBar;
        private System.Windows.Forms.ToolStripStatusLabel uxStatusLabelSecretsSelected;
        private System.Windows.Forms.ToolStripMenuItem uxAddCertFromFile;
        private System.Windows.Forms.ToolStripMenuItem uxAddCertFromUserStore;
        private System.Windows.Forms.ToolStripMenuItem uxAddCertFromMachineStore;
        private System.Windows.Forms.ToolStripMenuItem uxAddCertFromFile2;
        private System.Windows.Forms.ToolStripMenuItem uxAddCertFromUserStore2;
        private System.Windows.Forms.ToolStripMenuItem uxAddCertFromMachineStore2;
        private System.Windows.Forms.ToolStripButton uxButtonFavorite;
        private System.Windows.Forms.ToolStripMenuItem uxMenuItemFavorite;
        private System.Windows.Forms.ToolStripMenuItem uxAddKVCert;
        private System.Windows.Forms.ToolStripMenuItem uxAddKVCertFromFile;
        private System.Windows.Forms.ToolStripMenuItem uxAddKVCertFromUserStore;
        private System.Windows.Forms.ToolStripMenuItem uxAddKVCertFromMachineStore;
        private System.Windows.Forms.ToolStripDropDownButton uxButtonShare;
        private System.Windows.Forms.ToolStripMenuItem uxButtonCopyLink;
        private System.Windows.Forms.ToolStripMenuItem uxAddKVCert2;
        private System.Windows.Forms.ToolStripMenuItem uxAddKVCertFromFile2;
        private System.Windows.Forms.ToolStripMenuItem uxAddKVCertFromUserStore2;
        private System.Windows.Forms.ToolStripMenuItem uxAddKVCertFromMachineStore2;
        private System.Windows.Forms.ToolStripMenuItem uxMenuItemShare;
        private System.Windows.Forms.ToolStripMenuItem uxMenuItemCopy;
        private System.Windows.Forms.ToolStripMenuItem uxMenuItemCopyLink;
        private System.Windows.Forms.ToolStripMenuItem uxMenuItemSave;
        private System.Windows.Forms.ToolStripMenuItem uxButtonExportToTsv;
        private System.Windows.Forms.ToolStripMenuItem uxMenuItemExportToTsv;
        private System.Windows.Forms.ToolStripButton uxButtonPowershell;
    }
}

