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
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label label2;
            System.Windows.Forms.SplitContainer splitContainer1;
            System.Windows.Forms.ColumnHeader columnHeader1;
            System.Windows.Forms.ColumnHeader columnHeader2;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.uxListViewSecrets = new System.Windows.Forms.ListView();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.uxButtonRefresh = new System.Windows.Forms.Button();
            this.uxButtonSave = new System.Windows.Forms.Button();
            this.uxPropertyGridSecret = new System.Windows.Forms.PropertyGrid();
            this.uxComboBoxEnv = new System.Windows.Forms.ComboBox();
            this.uxComboBoxGeo = new System.Windows.Forms.ComboBox();
            this.uxButtonList = new System.Windows.Forms.Button();
            this.uxButtonAdd = new System.Windows.Forms.Button();
            this.uxButtonDelete = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            splitContainer1 = new System.Windows.Forms.SplitContainer();
            columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            ((System.ComponentModel.ISupportInitialize)(splitContainer1)).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(9, 20);
            label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(69, 13);
            label1.TabIndex = 1;
            label1.Text = "Environment:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(172, 20);
            label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(30, 13);
            label2.TabIndex = 4;
            label2.Text = "Geo:";
            // 
            // splitContainer1
            // 
            splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            splitContainer1.Location = new System.Drawing.Point(11, 48);
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
            splitContainer1.Panel2.Controls.Add(this.uxButtonRefresh);
            splitContainer1.Panel2.Controls.Add(this.uxButtonSave);
            splitContainer1.Panel2.Controls.Add(this.uxPropertyGridSecret);
            splitContainer1.Size = new System.Drawing.Size(566, 443);
            splitContainer1.SplitterDistance = 251;
            splitContainer1.SplitterWidth = 3;
            splitContainer1.TabIndex = 7;
            // 
            // uxListViewSecrets
            // 
            this.uxListViewSecrets.AllowColumnReorder = true;
            this.uxListViewSecrets.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            columnHeader1,
            columnHeader2,
            this.columnHeader3});
            this.uxListViewSecrets.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uxListViewSecrets.FullRowSelect = true;
            this.uxListViewSecrets.HideSelection = false;
            this.uxListViewSecrets.Location = new System.Drawing.Point(0, 0);
            this.uxListViewSecrets.Margin = new System.Windows.Forms.Padding(2);
            this.uxListViewSecrets.MultiSelect = false;
            this.uxListViewSecrets.Name = "uxListViewSecrets";
            this.uxListViewSecrets.Size = new System.Drawing.Size(566, 251);
            this.uxListViewSecrets.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.uxListViewSecrets.TabIndex = 6;
            this.uxListViewSecrets.UseCompatibleStateImageBehavior = false;
            this.uxListViewSecrets.View = System.Windows.Forms.View.Details;
            this.uxListViewSecrets.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.uxListViewSecrets_ColumnClick);
            this.uxListViewSecrets.SelectedIndexChanged += new System.EventHandler(this.uxListViewSecrets_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "Name";
            columnHeader1.Width = 180;
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "Created";
            columnHeader2.Width = 140;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Updated";
            this.columnHeader3.Width = 140;
            // 
            // uxButtonRefresh
            // 
            this.uxButtonRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.uxButtonRefresh.Enabled = false;
            this.uxButtonRefresh.Location = new System.Drawing.Point(488, 164);
            this.uxButtonRefresh.Name = "uxButtonRefresh";
            this.uxButtonRefresh.Size = new System.Drawing.Size(75, 23);
            this.uxButtonRefresh.TabIndex = 4;
            this.uxButtonRefresh.Text = "&Refresh";
            this.uxButtonRefresh.UseVisualStyleBackColor = true;
            this.uxButtonRefresh.Click += new System.EventHandler(this.uxListViewSecrets_SelectedIndexChanged);
            // 
            // uxButtonSave
            // 
            this.uxButtonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.uxButtonSave.Enabled = false;
            this.uxButtonSave.Location = new System.Drawing.Point(3, 164);
            this.uxButtonSave.Name = "uxButtonSave";
            this.uxButtonSave.Size = new System.Drawing.Size(75, 23);
            this.uxButtonSave.TabIndex = 4;
            this.uxButtonSave.Text = "&Save";
            this.uxButtonSave.UseVisualStyleBackColor = true;
            this.uxButtonSave.Click += new System.EventHandler(this.uxButtonSave_Click);
            // 
            // uxPropertyGridSecret
            // 
            this.uxPropertyGridSecret.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.uxPropertyGridSecret.HelpVisible = false;
            this.uxPropertyGridSecret.Location = new System.Drawing.Point(0, 0);
            this.uxPropertyGridSecret.Margin = new System.Windows.Forms.Padding(2);
            this.uxPropertyGridSecret.Name = "uxPropertyGridSecret";
            this.uxPropertyGridSecret.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.uxPropertyGridSecret.Size = new System.Drawing.Size(566, 159);
            this.uxPropertyGridSecret.TabIndex = 3;
            this.uxPropertyGridSecret.ToolbarVisible = false;
            // 
            // uxComboBoxEnv
            // 
            this.uxComboBoxEnv.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.uxComboBoxEnv.FormattingEnabled = true;
            this.uxComboBoxEnv.Items.AddRange(new object[] {
            "Int",
            "PPE",
            "Prod"});
            this.uxComboBoxEnv.Location = new System.Drawing.Point(82, 17);
            this.uxComboBoxEnv.Margin = new System.Windows.Forms.Padding(2);
            this.uxComboBoxEnv.Name = "uxComboBoxEnv";
            this.uxComboBoxEnv.Size = new System.Drawing.Size(80, 21);
            this.uxComboBoxEnv.TabIndex = 0;
            // 
            // uxComboBoxGeo
            // 
            this.uxComboBoxGeo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.uxComboBoxGeo.FormattingEnabled = true;
            this.uxComboBoxGeo.Items.AddRange(new object[] {
            "us - United States",
            "eu - Europe",
            "as - Asia",
            "jp - Japan",
            "au - Australia",
            "in - India"});
            this.uxComboBoxGeo.Location = new System.Drawing.Point(206, 17);
            this.uxComboBoxGeo.Margin = new System.Windows.Forms.Padding(2);
            this.uxComboBoxGeo.Name = "uxComboBoxGeo";
            this.uxComboBoxGeo.Size = new System.Drawing.Size(111, 21);
            this.uxComboBoxGeo.TabIndex = 3;
            // 
            // uxButtonList
            // 
            this.uxButtonList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.uxButtonList.Location = new System.Drawing.Point(382, 11);
            this.uxButtonList.Margin = new System.Windows.Forms.Padding(2);
            this.uxButtonList.Name = "uxButtonList";
            this.uxButtonList.Size = new System.Drawing.Size(62, 32);
            this.uxButtonList.TabIndex = 6;
            this.uxButtonList.Text = "&List";
            this.uxButtonList.UseVisualStyleBackColor = true;
            this.uxButtonList.Click += new System.EventHandler(this.uxButtonList_Click);
            // 
            // uxButtonAdd
            // 
            this.uxButtonAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.uxButtonAdd.Enabled = false;
            this.uxButtonAdd.Location = new System.Drawing.Point(448, 11);
            this.uxButtonAdd.Margin = new System.Windows.Forms.Padding(2);
            this.uxButtonAdd.Name = "uxButtonAdd";
            this.uxButtonAdd.Size = new System.Drawing.Size(62, 32);
            this.uxButtonAdd.TabIndex = 8;
            this.uxButtonAdd.Text = "&Add...";
            this.uxButtonAdd.UseVisualStyleBackColor = true;
            this.uxButtonAdd.Click += new System.EventHandler(this.uxButtonAdd_Click);
            // 
            // uxButtonDelete
            // 
            this.uxButtonDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.uxButtonDelete.Enabled = false;
            this.uxButtonDelete.Location = new System.Drawing.Point(515, 11);
            this.uxButtonDelete.Margin = new System.Windows.Forms.Padding(2);
            this.uxButtonDelete.Name = "uxButtonDelete";
            this.uxButtonDelete.Size = new System.Drawing.Size(62, 32);
            this.uxButtonDelete.TabIndex = 8;
            this.uxButtonDelete.Text = "&Delete...";
            this.uxButtonDelete.UseVisualStyleBackColor = true;
            this.uxButtonDelete.Click += new System.EventHandler(this.uxButtonDelete_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(588, 500);
            this.Controls.Add(this.uxButtonDelete);
            this.Controls.Add(this.uxButtonAdd);
            this.Controls.Add(splitContainer1);
            this.Controls.Add(this.uxButtonList);
            this.Controls.Add(label2);
            this.Controls.Add(this.uxComboBoxGeo);
            this.Controls.Add(label1);
            this.Controls.Add(this.uxComboBoxEnv);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(604, 495);
            this.Name = "MainForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "Windows Defender Services Azure Key Vaults Explorer";
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(splitContainer1)).EndInit();
            splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox uxComboBoxEnv;
        private System.Windows.Forms.ComboBox uxComboBoxGeo;
        private System.Windows.Forms.Button uxButtonList;
        private System.Windows.Forms.ListView uxListViewSecrets;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.PropertyGrid uxPropertyGridSecret;
        private System.Windows.Forms.Button uxButtonAdd;
        private System.Windows.Forms.Button uxButtonDelete;
        private System.Windows.Forms.Button uxButtonSave;
        private System.Windows.Forms.Button uxButtonRefresh;
    }
}

