namespace Microsoft.PS.Common.Vault.Explorer
{
    partial class SubscriptionsManagerDialog
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
            System.Windows.Forms.ColumnHeader uxColumnName;
            System.Windows.Forms.ColumnHeader uxColumnId;
            System.Windows.Forms.ColumnHeader columnHeader1;
            System.Windows.Forms.ColumnHeader columnHeader2;
            System.Windows.Forms.ToolStripLabel toolStripLabel1;
            System.Windows.Forms.SplitContainer splitContainer1;
            System.Windows.Forms.SplitContainer splitContainer2;
            System.Windows.Forms.ImageList imageList1;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SubscriptionsManagerDialog));
            System.Windows.Forms.ToolStrip toolStrip1;
            this.uxListViewSubscriptions = new System.Windows.Forms.ListView();
            this.uxListViewVaults = new System.Windows.Forms.ListView();
            this.uxPropertyGridVault = new System.Windows.Forms.PropertyGrid();
            this.uxComboBoxAccounts = new System.Windows.Forms.ToolStripComboBox();
            this.uxButtonCancelOperation = new System.Windows.Forms.ToolStripButton();
            this.uxProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.uxStatusLabel = new System.Windows.Forms.ToolStripLabel();
            this.uxButtonCancel = new System.Windows.Forms.Button();
            this.uxButtonOK = new System.Windows.Forms.Button();
            uxColumnName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            uxColumnId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            splitContainer1 = new System.Windows.Forms.SplitContainer();
            splitContainer2 = new System.Windows.Forms.SplitContainer();
            imageList1 = new System.Windows.Forms.ImageList(this.components);
            toolStrip1 = new System.Windows.Forms.ToolStrip();
            ((System.ComponentModel.ISupportInitialize)(splitContainer1)).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(splitContainer2)).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.Panel2.SuspendLayout();
            splitContainer2.SuspendLayout();
            toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // uxColumnName
            // 
            uxColumnName.Text = "Subscription name";
            uxColumnName.Width = 300;
            // 
            // uxColumnId
            // 
            uxColumnId.Text = "Subscription Id";
            uxColumnId.Width = 250;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "Vault name";
            columnHeader1.Width = 300;
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "Resource group name";
            columnHeader2.Width = 250;
            // 
            // toolStripLabel1
            // 
            toolStripLabel1.Name = "toolStripLabel1";
            toolStripLabel1.Size = new System.Drawing.Size(52, 22);
            toolStripLabel1.Text = "Account";
            // 
            // splitContainer1
            // 
            splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            splitContainer1.Location = new System.Drawing.Point(9, 28);
            splitContainer1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(this.uxPropertyGridVault);
            splitContainer1.Size = new System.Drawing.Size(604, 486);
            splitContainer1.SplitterDistance = 344;
            splitContainer1.SplitterWidth = 3;
            splitContainer1.TabIndex = 11;
            // 
            // splitContainer2
            // 
            splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer2.Location = new System.Drawing.Point(0, 0);
            splitContainer2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            splitContainer2.Name = "splitContainer2";
            splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            splitContainer2.Panel1.Controls.Add(this.uxListViewSubscriptions);
            // 
            // splitContainer2.Panel2
            // 
            splitContainer2.Panel2.Controls.Add(this.uxListViewVaults);
            splitContainer2.Size = new System.Drawing.Size(604, 344);
            splitContainer2.SplitterDistance = 149;
            splitContainer2.SplitterWidth = 3;
            splitContainer2.TabIndex = 3;
            // 
            // uxListViewSubscriptions
            // 
            this.uxListViewSubscriptions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            uxColumnName,
            uxColumnId});
            this.uxListViewSubscriptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uxListViewSubscriptions.FullRowSelect = true;
            this.uxListViewSubscriptions.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.uxListViewSubscriptions.HideSelection = false;
            this.uxListViewSubscriptions.Location = new System.Drawing.Point(0, 0);
            this.uxListViewSubscriptions.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.uxListViewSubscriptions.MultiSelect = false;
            this.uxListViewSubscriptions.Name = "uxListViewSubscriptions";
            this.uxListViewSubscriptions.ShowItemToolTips = true;
            this.uxListViewSubscriptions.Size = new System.Drawing.Size(604, 149);
            this.uxListViewSubscriptions.SmallImageList = imageList1;
            this.uxListViewSubscriptions.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.uxListViewSubscriptions.TabIndex = 3;
            this.uxListViewSubscriptions.UseCompatibleStateImageBehavior = false;
            this.uxListViewSubscriptions.View = System.Windows.Forms.View.Details;
            this.uxListViewSubscriptions.SelectedIndexChanged += new System.EventHandler(this.uxListViewSubscriptions_SelectedIndexChanged);
            // 
            // imageList1
            // 
            imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            imageList1.TransparentColor = System.Drawing.Color.Transparent;
            imageList1.Images.SetKeyName(0, "newspaper.png");
            imageList1.Images.SetKeyName(1, "lock.png");
            // 
            // uxListViewVaults
            // 
            this.uxListViewVaults.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            columnHeader1,
            columnHeader2});
            this.uxListViewVaults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uxListViewVaults.FullRowSelect = true;
            this.uxListViewVaults.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.uxListViewVaults.HideSelection = false;
            this.uxListViewVaults.Location = new System.Drawing.Point(0, 0);
            this.uxListViewVaults.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.uxListViewVaults.MultiSelect = false;
            this.uxListViewVaults.Name = "uxListViewVaults";
            this.uxListViewVaults.ShowItemToolTips = true;
            this.uxListViewVaults.Size = new System.Drawing.Size(604, 192);
            this.uxListViewVaults.SmallImageList = imageList1;
            this.uxListViewVaults.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.uxListViewVaults.TabIndex = 5;
            this.uxListViewVaults.UseCompatibleStateImageBehavior = false;
            this.uxListViewVaults.View = System.Windows.Forms.View.Details;
            this.uxListViewVaults.SelectedIndexChanged += new System.EventHandler(this.uxListViewVaults_SelectedIndexChanged);
            // 
            // uxPropertyGridVault
            // 
            this.uxPropertyGridVault.DisabledItemForeColor = System.Drawing.SystemColors.WindowText;
            this.uxPropertyGridVault.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uxPropertyGridVault.HelpVisible = false;
            this.uxPropertyGridVault.Location = new System.Drawing.Point(0, 0);
            this.uxPropertyGridVault.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.uxPropertyGridVault.Name = "uxPropertyGridVault";
            this.uxPropertyGridVault.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.uxPropertyGridVault.Size = new System.Drawing.Size(604, 139);
            this.uxPropertyGridVault.TabIndex = 12;
            this.uxPropertyGridVault.ToolbarVisible = false;
            // 
            // toolStrip1
            // 
            toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            toolStripLabel1,
            this.uxComboBoxAccounts,
            this.uxButtonCancelOperation,
            this.uxProgressBar,
            this.uxStatusLabel});
            toolStrip1.Location = new System.Drawing.Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new System.Drawing.Size(622, 25);
            toolStrip1.TabIndex = 10;
            toolStrip1.Text = "toolStrip1";
            // 
            // uxComboBoxAccounts
            // 
            this.uxComboBoxAccounts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.uxComboBoxAccounts.DropDownWidth = 250;
            this.uxComboBoxAccounts.Name = "uxComboBoxAccounts";
            this.uxComboBoxAccounts.Size = new System.Drawing.Size(264, 25);
            this.uxComboBoxAccounts.SelectedIndexChanged += new System.EventHandler(this.uxComboBoxAccounts_SelectedIndexChanged);
            // 
            // uxButtonCancelOperation
            // 
            this.uxButtonCancelOperation.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.uxButtonCancelOperation.AutoSize = false;
            this.uxButtonCancelOperation.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.uxButtonCancelOperation.Image = global::Microsoft.PS.Common.Vault.Explorer.Properties.Resources.cancel;
            this.uxButtonCancelOperation.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.uxButtonCancelOperation.Name = "uxButtonCancelOperation";
            this.uxButtonCancelOperation.Size = new System.Drawing.Size(24, 25);
            this.uxButtonCancelOperation.Text = "Cancel operation";
            this.uxButtonCancelOperation.Visible = false;
            // 
            // uxProgressBar
            // 
            this.uxProgressBar.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.uxProgressBar.AutoSize = false;
            this.uxProgressBar.Name = "uxProgressBar";
            this.uxProgressBar.Size = new System.Drawing.Size(75, 18);
            this.uxProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.uxProgressBar.Visible = false;
            // 
            // uxStatusLabel
            // 
            this.uxStatusLabel.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.uxStatusLabel.Name = "uxStatusLabel";
            this.uxStatusLabel.Size = new System.Drawing.Size(39, 22);
            this.uxStatusLabel.Text = "Ready";
            this.uxStatusLabel.Visible = false;
            // 
            // uxButtonCancel
            // 
            this.uxButtonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.uxButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.uxButtonCancel.Location = new System.Drawing.Point(538, 525);
            this.uxButtonCancel.Name = "uxButtonCancel";
            this.uxButtonCancel.Size = new System.Drawing.Size(75, 23);
            this.uxButtonCancel.TabIndex = 7;
            this.uxButtonCancel.Text = "Cancel";
            this.uxButtonCancel.UseVisualStyleBackColor = true;
            // 
            // uxButtonOK
            // 
            this.uxButtonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.uxButtonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.uxButtonOK.Enabled = false;
            this.uxButtonOK.Location = new System.Drawing.Point(458, 525);
            this.uxButtonOK.Name = "uxButtonOK";
            this.uxButtonOK.Size = new System.Drawing.Size(75, 23);
            this.uxButtonOK.TabIndex = 6;
            this.uxButtonOK.Text = "OK";
            this.uxButtonOK.UseVisualStyleBackColor = true;
            // 
            // SubscriptionsManagerDialog
            // 
            this.AcceptButton = this.uxButtonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.uxButtonCancel;
            this.ClientSize = new System.Drawing.Size(622, 558);
            this.Controls.Add(splitContainer1);
            this.Controls.Add(toolStrip1);
            this.Controls.Add(this.uxButtonCancel);
            this.Controls.Add(this.uxButtonOK);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MinimizeBox = false;
            this.Name = "SubscriptionsManagerDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Subscriptions Manager";
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(splitContainer1)).EndInit();
            splitContainer1.ResumeLayout(false);
            splitContainer2.Panel1.ResumeLayout(false);
            splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(splitContainer2)).EndInit();
            splitContainer2.ResumeLayout(false);
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ListView uxListViewSubscriptions;
        private System.Windows.Forms.ListView uxListViewVaults;
        private System.Windows.Forms.Button uxButtonCancel;
        private System.Windows.Forms.Button uxButtonOK;
        private System.Windows.Forms.ToolStripComboBox uxComboBoxAccounts;
        private System.Windows.Forms.ToolStripProgressBar uxProgressBar;
        private System.Windows.Forms.ToolStripButton uxButtonCancelOperation;
        private System.Windows.Forms.ToolStripLabel uxStatusLabel;
        private System.Windows.Forms.PropertyGrid uxPropertyGridVault;
    }
}