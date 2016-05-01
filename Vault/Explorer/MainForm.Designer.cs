namespace VaultExplorer
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
            System.Windows.Forms.ColumnHeader columnHeader1;
            System.Windows.Forms.ColumnHeader columnHeader2;
            System.Windows.Forms.ColumnHeader columnHeader3;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
            System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
            System.Windows.Forms.ToolStrip uxToolStrip;
            System.Windows.Forms.ToolStripLabel toolStripLabel1;
            System.Windows.Forms.ToolStripLabel toolStripLabel2;
            System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
            System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
            System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
            this.uxListViewSecrets = new System.Windows.Forms.ListView();
            this.uxContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.uxMenuItemAddSecret = new System.Windows.Forms.ToolStripMenuItem();
            this.uxMenuItemAddCertificate = new System.Windows.Forms.ToolStripMenuItem();
            this.uxMenuItemEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.uxMenuItemToggle = new System.Windows.Forms.ToolStripMenuItem();
            this.uxMenuItemDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.uxMenuItemCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.uxMenuItemRefresh = new System.Windows.Forms.ToolStripMenuItem();
            this.uxPropertyGridSecret = new System.Windows.Forms.PropertyGrid();
            this.uxComboBoxEnv = new System.Windows.Forms.ToolStripComboBox();
            this.uxComboBoxGeo = new System.Windows.Forms.ToolStripComboBox();
            this.uxButtonList = new System.Windows.Forms.ToolStripButton();
            this.uxButtonAdd = new System.Windows.Forms.ToolStripSplitButton();
            this.uxAddSecret = new System.Windows.Forms.ToolStripMenuItem();
            this.uxAddCertificate = new System.Windows.Forms.ToolStripMenuItem();
            this.uxButtonEdit = new System.Windows.Forms.ToolStripButton();
            this.uxButtonToggle = new System.Windows.Forms.ToolStripButton();
            this.uxButtonDelete = new System.Windows.Forms.ToolStripButton();
            this.uxButtonCopy = new System.Windows.Forms.ToolStripButton();
            this.uxButtonHelp = new System.Windows.Forms.ToolStripButton();
            this.uxButtonExit = new System.Windows.Forms.ToolStripButton();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.uxStatusStrip = new System.Windows.Forms.StatusStrip();
            this.uxStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            splitContainer1 = new System.Windows.Forms.SplitContainer();
            columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            uxToolStrip = new System.Windows.Forms.ToolStrip();
            toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            ((System.ComponentModel.ISupportInitialize)(splitContainer1)).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            this.uxContextMenuStrip.SuspendLayout();
            uxToolStrip.SuspendLayout();
            this.toolStripContainer1.BottomToolStripPanel.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.uxStatusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer1.Location = new System.Drawing.Point(0, 0);
            splitContainer1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
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
            splitContainer1.Size = new System.Drawing.Size(1154, 562);
            splitContainer1.SplitterDistance = 291;
            splitContainer1.SplitterWidth = 6;
            splitContainer1.TabIndex = 8;
            // 
            // uxListViewSecrets
            // 
            this.uxListViewSecrets.AllowColumnReorder = true;
            this.uxListViewSecrets.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            columnHeader1,
            columnHeader2,
            columnHeader3});
            this.uxListViewSecrets.ContextMenuStrip = this.uxContextMenuStrip;
            this.uxListViewSecrets.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uxListViewSecrets.FullRowSelect = true;
            this.uxListViewSecrets.HideSelection = false;
            this.uxListViewSecrets.Location = new System.Drawing.Point(0, 0);
            this.uxListViewSecrets.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.uxListViewSecrets.MultiSelect = false;
            this.uxListViewSecrets.Name = "uxListViewSecrets";
            this.uxListViewSecrets.Size = new System.Drawing.Size(1154, 291);
            this.uxListViewSecrets.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.uxListViewSecrets.TabIndex = 0;
            this.uxListViewSecrets.UseCompatibleStateImageBehavior = false;
            this.uxListViewSecrets.View = System.Windows.Forms.View.Details;
            this.uxListViewSecrets.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.uxListViewSecrets_ColumnClick);
            this.uxListViewSecrets.SelectedIndexChanged += new System.EventHandler(this.uxListViewSecrets_SelectedIndexChanged);
            this.uxListViewSecrets.DoubleClick += new System.EventHandler(this.uxButtonEdit_Click);
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "Name";
            columnHeader1.Width = 350;
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "Updated";
            columnHeader2.Width = 140;
            // 
            // columnHeader3
            // 
            columnHeader3.Text = "Changed by";
            columnHeader3.Width = 200;
            // 
            // uxContextMenuStrip
            // 
            this.uxContextMenuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.uxContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.uxMenuItemAddSecret,
            this.uxMenuItemAddCertificate,
            this.uxMenuItemEdit,
            this.uxMenuItemToggle,
            this.uxMenuItemDelete,
            toolStripSeparator5,
            this.uxMenuItemCopy,
            toolStripSeparator4,
            this.uxMenuItemRefresh});
            this.uxContextMenuStrip.Name = "uxContextMenuStrip";
            this.uxContextMenuStrip.Size = new System.Drawing.Size(194, 198);
            // 
            // uxMenuItemAddSecret
            // 
            this.uxMenuItemAddSecret.Enabled = false;
            this.uxMenuItemAddSecret.Image = ((System.Drawing.Image)(resources.GetObject("uxMenuItemAddSecret.Image")));
            this.uxMenuItemAddSecret.Name = "uxMenuItemAddSecret";
            this.uxMenuItemAddSecret.Size = new System.Drawing.Size(193, 26);
            this.uxMenuItemAddSecret.Text = "Add &Secret...";
            this.uxMenuItemAddSecret.Click += new System.EventHandler(this.uxButtonAddSecret_Click);
            // 
            // uxMenuItemAddCertificate
            // 
            this.uxMenuItemAddCertificate.Enabled = false;
            this.uxMenuItemAddCertificate.Image = ((System.Drawing.Image)(resources.GetObject("uxMenuItemAddCertificate.Image")));
            this.uxMenuItemAddCertificate.Name = "uxMenuItemAddCertificate";
            this.uxMenuItemAddCertificate.Size = new System.Drawing.Size(193, 26);
            this.uxMenuItemAddCertificate.Text = "Add Ce&rtificate...";
            // 
            // uxMenuItemEdit
            // 
            this.uxMenuItemEdit.Enabled = false;
            this.uxMenuItemEdit.Image = ((System.Drawing.Image)(resources.GetObject("uxMenuItemEdit.Image")));
            this.uxMenuItemEdit.Name = "uxMenuItemEdit";
            this.uxMenuItemEdit.Size = new System.Drawing.Size(193, 26);
            this.uxMenuItemEdit.Text = "&Edit...";
            this.uxMenuItemEdit.Click += new System.EventHandler(this.uxButtonEdit_Click);
            // 
            // uxMenuItemToggle
            // 
            this.uxMenuItemToggle.Enabled = false;
            this.uxMenuItemToggle.Image = ((System.Drawing.Image)(resources.GetObject("uxMenuItemToggle.Image")));
            this.uxMenuItemToggle.Name = "uxMenuItemToggle";
            this.uxMenuItemToggle.Size = new System.Drawing.Size(193, 26);
            this.uxMenuItemToggle.Text = "Disabl&e...";
            this.uxMenuItemToggle.Click += new System.EventHandler(this.uxButtonToggle_Click);
            // 
            // uxMenuItemDelete
            // 
            this.uxMenuItemDelete.Enabled = false;
            this.uxMenuItemDelete.Image = ((System.Drawing.Image)(resources.GetObject("uxMenuItemDelete.Image")));
            this.uxMenuItemDelete.Name = "uxMenuItemDelete";
            this.uxMenuItemDelete.Size = new System.Drawing.Size(193, 26);
            this.uxMenuItemDelete.Text = "&Delete...";
            this.uxMenuItemDelete.Click += new System.EventHandler(this.uxButtonDelete_Click);
            // 
            // toolStripSeparator5
            // 
            toolStripSeparator5.Name = "toolStripSeparator5";
            toolStripSeparator5.Size = new System.Drawing.Size(190, 6);
            // 
            // uxMenuItemCopy
            // 
            this.uxMenuItemCopy.Enabled = false;
            this.uxMenuItemCopy.Image = ((System.Drawing.Image)(resources.GetObject("uxMenuItemCopy.Image")));
            this.uxMenuItemCopy.Name = "uxMenuItemCopy";
            this.uxMenuItemCopy.Size = new System.Drawing.Size(193, 26);
            this.uxMenuItemCopy.Text = "&Copy";
            this.uxMenuItemCopy.Click += new System.EventHandler(this.uxButtonCopy_Click);
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new System.Drawing.Size(190, 6);
            // 
            // uxMenuItemRefresh
            // 
            this.uxMenuItemRefresh.Image = ((System.Drawing.Image)(resources.GetObject("uxMenuItemRefresh.Image")));
            this.uxMenuItemRefresh.Name = "uxMenuItemRefresh";
            this.uxMenuItemRefresh.Size = new System.Drawing.Size(193, 26);
            this.uxMenuItemRefresh.Text = "Re&fresh";
            this.uxMenuItemRefresh.Click += new System.EventHandler(this.uxButtonList_Click);
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
            this.uxPropertyGridSecret.Size = new System.Drawing.Size(1154, 265);
            this.uxPropertyGridSecret.TabIndex = 0;
            this.uxPropertyGridSecret.ToolbarVisible = false;
            // 
            // uxToolStrip
            // 
            uxToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            uxToolStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            uxToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            toolStripLabel1,
            this.uxComboBoxEnv,
            toolStripLabel2,
            this.uxComboBoxGeo,
            this.uxButtonList,
            toolStripSeparator1,
            this.uxButtonAdd,
            this.uxButtonEdit,
            this.uxButtonToggle,
            this.uxButtonDelete,
            toolStripSeparator2,
            this.uxButtonCopy,
            toolStripSeparator3,
            this.uxButtonHelp,
            this.uxButtonExit});
            uxToolStrip.Location = new System.Drawing.Point(3, 0);
            uxToolStrip.Name = "uxToolStrip";
            uxToolStrip.Size = new System.Drawing.Size(943, 28);
            uxToolStrip.TabIndex = 0;
            // 
            // toolStripLabel1
            // 
            toolStripLabel1.Name = "toolStripLabel1";
            toolStripLabel1.Size = new System.Drawing.Size(92, 25);
            toolStripLabel1.Text = "Environment";
            // 
            // uxComboBoxEnv
            // 
            this.uxComboBoxEnv.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.uxComboBoxEnv.Items.AddRange(new object[] {
            "int",
            "ppe",
            "prod"});
            this.uxComboBoxEnv.Name = "uxComboBoxEnv";
            this.uxComboBoxEnv.Size = new System.Drawing.Size(121, 28);
            // 
            // toolStripLabel2
            // 
            toolStripLabel2.Name = "toolStripLabel2";
            toolStripLabel2.Size = new System.Drawing.Size(36, 25);
            toolStripLabel2.Text = "Geo";
            // 
            // uxComboBoxGeo
            // 
            this.uxComboBoxGeo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.uxComboBoxGeo.Items.AddRange(new object[] {
            "us - United States",
            "eu - Europe",
            "as - Asia",
            "jp - Japan",
            "au - Australia",
            "in - India"});
            this.uxComboBoxGeo.Name = "uxComboBoxGeo";
            this.uxComboBoxGeo.Size = new System.Drawing.Size(121, 28);
            // 
            // uxButtonList
            // 
            this.uxButtonList.Image = ((System.Drawing.Image)(resources.GetObject("uxButtonList.Image")));
            this.uxButtonList.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.uxButtonList.Name = "uxButtonList";
            this.uxButtonList.Size = new System.Drawing.Size(55, 25);
            this.uxButtonList.Text = "&List";
            this.uxButtonList.Click += new System.EventHandler(this.uxButtonList_Click);
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(6, 28);
            // 
            // uxButtonAdd
            // 
            this.uxButtonAdd.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.uxAddSecret,
            this.uxAddCertificate});
            this.uxButtonAdd.Enabled = false;
            this.uxButtonAdd.Image = ((System.Drawing.Image)(resources.GetObject("uxButtonAdd.Image")));
            this.uxButtonAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.uxButtonAdd.Name = "uxButtonAdd";
            this.uxButtonAdd.Size = new System.Drawing.Size(76, 25);
            this.uxButtonAdd.Text = "&Add";
            this.uxButtonAdd.Click += new System.EventHandler(this.uxButtonAddSecret_Click);
            // 
            // uxAddSecret
            // 
            this.uxAddSecret.Image = ((System.Drawing.Image)(resources.GetObject("uxAddSecret.Image")));
            this.uxAddSecret.Name = "uxAddSecret";
            this.uxAddSecret.Size = new System.Drawing.Size(161, 26);
            this.uxAddSecret.Text = "&Secret...";
            this.uxAddSecret.Click += new System.EventHandler(this.uxButtonAddSecret_Click);
            // 
            // uxAddCertificate
            // 
            this.uxAddCertificate.Image = ((System.Drawing.Image)(resources.GetObject("uxAddCertificate.Image")));
            this.uxAddCertificate.Name = "uxAddCertificate";
            this.uxAddCertificate.Size = new System.Drawing.Size(161, 26);
            this.uxAddCertificate.Text = "Ce&rtificate...";
            this.uxAddCertificate.Click += new System.EventHandler(this.uxAddCertificate_Click);
            // 
            // uxButtonEdit
            // 
            this.uxButtonEdit.Enabled = false;
            this.uxButtonEdit.Image = ((System.Drawing.Image)(resources.GetObject("uxButtonEdit.Image")));
            this.uxButtonEdit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.uxButtonEdit.Name = "uxButtonEdit";
            this.uxButtonEdit.Size = new System.Drawing.Size(59, 25);
            this.uxButtonEdit.Text = "&Edit";
            this.uxButtonEdit.Click += new System.EventHandler(this.uxButtonEdit_Click);
            // 
            // uxButtonToggle
            // 
            this.uxButtonToggle.Enabled = false;
            this.uxButtonToggle.Image = ((System.Drawing.Image)(resources.GetObject("uxButtonToggle.Image")));
            this.uxButtonToggle.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.uxButtonToggle.Name = "uxButtonToggle";
            this.uxButtonToggle.Size = new System.Drawing.Size(83, 25);
            this.uxButtonToggle.Text = "Disabl&e";
            this.uxButtonToggle.Click += new System.EventHandler(this.uxButtonToggle_Click);
            // 
            // uxButtonDelete
            // 
            this.uxButtonDelete.Enabled = false;
            this.uxButtonDelete.Image = ((System.Drawing.Image)(resources.GetObject("uxButtonDelete.Image")));
            this.uxButtonDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.uxButtonDelete.Name = "uxButtonDelete";
            this.uxButtonDelete.Size = new System.Drawing.Size(77, 25);
            this.uxButtonDelete.Text = "&Delete";
            this.uxButtonDelete.Click += new System.EventHandler(this.uxButtonDelete_Click);
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new System.Drawing.Size(6, 28);
            // 
            // uxButtonCopy
            // 
            this.uxButtonCopy.Enabled = false;
            this.uxButtonCopy.Image = ((System.Drawing.Image)(resources.GetObject("uxButtonCopy.Image")));
            this.uxButtonCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.uxButtonCopy.Name = "uxButtonCopy";
            this.uxButtonCopy.Size = new System.Drawing.Size(67, 25);
            this.uxButtonCopy.Text = "&Copy";
            this.uxButtonCopy.Click += new System.EventHandler(this.uxButtonCopy_Click);
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new System.Drawing.Size(6, 28);
            // 
            // uxButtonHelp
            // 
            this.uxButtonHelp.Image = ((System.Drawing.Image)(resources.GetObject("uxButtonHelp.Image")));
            this.uxButtonHelp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.uxButtonHelp.Name = "uxButtonHelp";
            this.uxButtonHelp.Size = new System.Drawing.Size(65, 25);
            this.uxButtonHelp.Text = "&Help";
            // 
            // uxButtonExit
            // 
            this.uxButtonExit.Image = ((System.Drawing.Image)(resources.GetObject("uxButtonExit.Image")));
            this.uxButtonExit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.uxButtonExit.Name = "uxButtonExit";
            this.uxButtonExit.Size = new System.Drawing.Size(57, 25);
            this.uxButtonExit.Text = "E&xit";
            this.uxButtonExit.Click += new System.EventHandler(this.uxButtonExit_Click);
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
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(1154, 562);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(1154, 615);
            this.toolStripContainer1.TabIndex = 8;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(uxToolStrip);
            // 
            // uxStatusStrip
            // 
            this.uxStatusStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.uxStatusStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.uxStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.uxStatusLabel});
            this.uxStatusStrip.Location = new System.Drawing.Point(0, 0);
            this.uxStatusStrip.Name = "uxStatusStrip";
            this.uxStatusStrip.Size = new System.Drawing.Size(1154, 25);
            this.uxStatusStrip.TabIndex = 0;
            // 
            // uxStatusLabel
            // 
            this.uxStatusLabel.Name = "uxStatusLabel";
            this.uxStatusLabel.Size = new System.Drawing.Size(50, 20);
            this.uxStatusLabel.Text = "Ready";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1154, 615);
            this.Controls.Add(this.toolStripContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MinimumSize = new System.Drawing.Size(799, 598);
            this.Name = "MainForm";
            this.Text = "Windows Defender Services - Azure Key Vaults Explorer";
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(splitContainer1)).EndInit();
            splitContainer1.ResumeLayout(false);
            this.uxContextMenuStrip.ResumeLayout(false);
            uxToolStrip.ResumeLayout(false);
            uxToolStrip.PerformLayout();
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
        private System.Windows.Forms.ListView uxListViewSecrets;
        private System.Windows.Forms.PropertyGrid uxPropertyGridSecret;
        private System.Windows.Forms.ToolStripComboBox uxComboBoxEnv;
        private System.Windows.Forms.ToolStripComboBox uxComboBoxGeo;
        private System.Windows.Forms.ToolStripButton uxButtonList;
        private System.Windows.Forms.ToolStripButton uxButtonCopy;
        private System.Windows.Forms.ToolStripButton uxButtonHelp;
        private System.Windows.Forms.ToolStripButton uxButtonEdit;
        private System.Windows.Forms.ToolStripButton uxButtonDelete;
        private System.Windows.Forms.ToolStripSplitButton uxButtonAdd;
        private System.Windows.Forms.ToolStripMenuItem uxAddSecret;
        private System.Windows.Forms.ToolStripMenuItem uxAddCertificate;
        private System.Windows.Forms.ToolStripButton uxButtonExit;
        private System.Windows.Forms.ContextMenuStrip uxContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem uxMenuItemAddSecret;
        private System.Windows.Forms.ToolStripMenuItem uxMenuItemAddCertificate;
        private System.Windows.Forms.ToolStripMenuItem uxMenuItemEdit;
        private System.Windows.Forms.ToolStripMenuItem uxMenuItemDelete;
        private System.Windows.Forms.ToolStripMenuItem uxMenuItemRefresh;
        private System.Windows.Forms.ToolStripMenuItem uxMenuItemCopy;
        private System.Windows.Forms.ToolStripButton uxButtonToggle;
        private System.Windows.Forms.ToolStripMenuItem uxMenuItemToggle;
    }
}

