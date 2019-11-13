namespace Microsoft.Vault.Explorer
{
    partial class SettingsDialog
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
            System.Windows.Forms.TabControl uxTabControl;
            this.uxTabPageOptions = new System.Windows.Forms.TabPage();
            this.uxPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.uxTabPageAbout = new System.Windows.Forms.TabPage();
            this.uxTextBoxLicense = new System.Windows.Forms.TextBox();
            this.uxLinkLabelUserSettingsLocation = new System.Windows.Forms.LinkLabel();
            this.uxTextBoxVersions = new System.Windows.Forms.TextBox();
            this.uxLinkLabelInstallLocation = new System.Windows.Forms.LinkLabel();
            this.uxLinkLabelClearTokenCache = new System.Windows.Forms.LinkLabel();
            this.uxLinkLabelSendFeedback = new System.Windows.Forms.LinkLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.uxLinkLabelTitle = new System.Windows.Forms.LinkLabel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.uxButtonCancel = new System.Windows.Forms.Button();
            this.uxButtonOK = new System.Windows.Forms.Button();
            uxTabControl = new System.Windows.Forms.TabControl();
            uxTabControl.SuspendLayout();
            this.uxTabPageOptions.SuspendLayout();
            this.uxTabPageAbout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // uxTabControl
            // 
            uxTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            uxTabControl.Controls.Add(this.uxTabPageOptions);
            uxTabControl.Controls.Add(this.uxTabPageAbout);
            uxTabControl.Location = new System.Drawing.Point(12, 12);
            uxTabControl.Name = "uxTabControl";
            uxTabControl.SelectedIndex = 0;
            uxTabControl.Size = new System.Drawing.Size(521, 566);
            uxTabControl.TabIndex = 0;
            // 
            // uxTabPageOptions
            // 
            this.uxTabPageOptions.Controls.Add(this.uxPropertyGrid);
            this.uxTabPageOptions.Location = new System.Drawing.Point(4, 25);
            this.uxTabPageOptions.Name = "uxTabPageOptions";
            this.uxTabPageOptions.Padding = new System.Windows.Forms.Padding(3);
            this.uxTabPageOptions.Size = new System.Drawing.Size(513, 537);
            this.uxTabPageOptions.TabIndex = 0;
            this.uxTabPageOptions.Text = "Options";
            this.uxTabPageOptions.UseVisualStyleBackColor = true;
            // 
            // uxPropertyGrid
            // 
            this.uxPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uxPropertyGrid.Location = new System.Drawing.Point(3, 3);
            this.uxPropertyGrid.Name = "uxPropertyGrid";
            this.uxPropertyGrid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.uxPropertyGrid.Size = new System.Drawing.Size(507, 531);
            this.uxPropertyGrid.TabIndex = 0;
            this.uxPropertyGrid.ToolbarVisible = false;
            // 
            // uxTabPageAbout
            // 
            this.uxTabPageAbout.BackColor = System.Drawing.SystemColors.Window;
            this.uxTabPageAbout.Controls.Add(this.uxTextBoxLicense);
            this.uxTabPageAbout.Controls.Add(this.uxLinkLabelUserSettingsLocation);
            this.uxTabPageAbout.Controls.Add(this.uxTextBoxVersions);
            this.uxTabPageAbout.Controls.Add(this.uxLinkLabelInstallLocation);
            this.uxTabPageAbout.Controls.Add(this.uxLinkLabelClearTokenCache);
            this.uxTabPageAbout.Controls.Add(this.uxLinkLabelSendFeedback);
            this.uxTabPageAbout.Controls.Add(this.label2);
            this.uxTabPageAbout.Controls.Add(this.uxLinkLabelTitle);
            this.uxTabPageAbout.Controls.Add(this.pictureBox1);
            this.uxTabPageAbout.Location = new System.Drawing.Point(4, 25);
            this.uxTabPageAbout.Name = "uxTabPageAbout";
            this.uxTabPageAbout.Padding = new System.Windows.Forms.Padding(3);
            this.uxTabPageAbout.Size = new System.Drawing.Size(513, 537);
            this.uxTabPageAbout.TabIndex = 1;
            this.uxTabPageAbout.Text = "About";
            // 
            // uxTextBoxLicense
            // 
            this.uxTextBoxLicense.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.uxTextBoxLicense.Location = new System.Drawing.Point(6, 386);
            this.uxTextBoxLicense.Multiline = true;
            this.uxTextBoxLicense.Name = "uxTextBoxLicense";
            this.uxTextBoxLicense.ReadOnly = true;
            this.uxTextBoxLicense.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.uxTextBoxLicense.Size = new System.Drawing.Size(501, 145);
            this.uxTextBoxLicense.TabIndex = 7;
            // 
            // uxLinkLabelUserSettingsLocation
            // 
            this.uxLinkLabelUserSettingsLocation.AutoSize = true;
            this.uxLinkLabelUserSettingsLocation.Location = new System.Drawing.Point(6, 205);
            this.uxLinkLabelUserSettingsLocation.Name = "uxLinkLabelUserSettingsLocation";
            this.uxLinkLabelUserSettingsLocation.Size = new System.Drawing.Size(156, 17);
            this.uxLinkLabelUserSettingsLocation.TabIndex = 4;
            this.uxLinkLabelUserSettingsLocation.TabStop = true;
            this.uxLinkLabelUserSettingsLocation.Text = "User settings location...";
            this.uxLinkLabelUserSettingsLocation.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.uxLinkLabelUserSettingsLocation_LinkClicked);
            // 
            // uxTextBoxVersions
            // 
            this.uxTextBoxVersions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.uxTextBoxVersions.BackColor = System.Drawing.SystemColors.Window;
            this.uxTextBoxVersions.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.uxTextBoxVersions.Location = new System.Drawing.Point(9, 264);
            this.uxTextBoxVersions.Multiline = true;
            this.uxTextBoxVersions.Name = "uxTextBoxVersions";
            this.uxTextBoxVersions.ReadOnly = true;
            this.uxTextBoxVersions.Size = new System.Drawing.Size(498, 116);
            this.uxTextBoxVersions.TabIndex = 6;
            this.uxTextBoxVersions.Text = "Version: x.x.x.x";
            // 
            // uxLinkLabelInstallLocation
            // 
            this.uxLinkLabelInstallLocation.AutoSize = true;
            this.uxLinkLabelInstallLocation.Location = new System.Drawing.Point(6, 182);
            this.uxLinkLabelInstallLocation.Name = "uxLinkLabelInstallLocation";
            this.uxLinkLabelInstallLocation.Size = new System.Drawing.Size(109, 17);
            this.uxLinkLabelInstallLocation.TabIndex = 3;
            this.uxLinkLabelInstallLocation.TabStop = true;
            this.uxLinkLabelInstallLocation.Text = "Install location...";
            this.uxLinkLabelInstallLocation.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.uxLinkLabelInstallLocation_LinkClicked);
            // 
            // uxLinkLabelClearTokenCache
            // 
            this.uxLinkLabelClearTokenCache.AutoSize = true;
            this.uxLinkLabelClearTokenCache.Location = new System.Drawing.Point(6, 229);
            this.uxLinkLabelClearTokenCache.Name = "uxLinkLabelClearTokenCache";
            this.uxLinkLabelClearTokenCache.Size = new System.Drawing.Size(170, 17);
            this.uxLinkLabelClearTokenCache.TabIndex = 5;
            this.uxLinkLabelClearTokenCache.TabStop = true;
            this.uxLinkLabelClearTokenCache.Text = "Clear access token cache";
            this.uxLinkLabelClearTokenCache.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.uxLinkLabelClearTokenCache_LinkClicked);
            // 
            // uxLinkLabelSendFeedback
            // 
            this.uxLinkLabelSendFeedback.AutoSize = true;
            this.uxLinkLabelSendFeedback.Location = new System.Drawing.Point(6, 145);
            this.uxLinkLabelSendFeedback.Name = "uxLinkLabelSendFeedback";
            this.uxLinkLabelSendFeedback.Size = new System.Drawing.Size(342, 17);
            this.uxLinkLabelSendFeedback.TabIndex = 2;
            this.uxLinkLabelSendFeedback.TabStop = true;
            this.uxLinkLabelSendFeedback.Text = "Like it, Questions or Comments? Send us feedback...";
            this.uxLinkLabelSendFeedback.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.uxLinkLabelSendFeedback_LinkClicked);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 83);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(290, 51);
            this.label2.TabIndex = 1;
            this.label2.Text = "Copyright (c) 2018 Microsoft Corporation\r\n\r\n";
            // 
            // uxLinkLabelTitle
            // 
            this.uxLinkLabelTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.uxLinkLabelTitle.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uxLinkLabelTitle.Location = new System.Drawing.Point(76, 6);
            this.uxLinkLabelTitle.Name = "uxLinkLabelTitle";
            this.uxLinkLabelTitle.Size = new System.Drawing.Size(380, 64);
            this.uxLinkLabelTitle.TabIndex = 0;
            this.uxLinkLabelTitle.TabStop = true;
            this.uxLinkLabelTitle.Text = "Azure Key Vault Explorer";
            this.uxLinkLabelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.uxLinkLabelTitle.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.uxLinkLabelTitle_LinkClicked);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::Microsoft.Vault.Explorer.Properties.Resources.BigKey;
            this.pictureBox1.Location = new System.Drawing.Point(6, 6);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(64, 64);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // uxButtonCancel
            // 
            this.uxButtonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.uxButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.uxButtonCancel.Location = new System.Drawing.Point(432, 585);
            this.uxButtonCancel.Margin = new System.Windows.Forms.Padding(4);
            this.uxButtonCancel.Name = "uxButtonCancel";
            this.uxButtonCancel.Size = new System.Drawing.Size(100, 28);
            this.uxButtonCancel.TabIndex = 3;
            this.uxButtonCancel.Text = "Cancel";
            this.uxButtonCancel.UseVisualStyleBackColor = true;
            // 
            // uxButtonOK
            // 
            this.uxButtonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.uxButtonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.uxButtonOK.Enabled = false;
            this.uxButtonOK.Location = new System.Drawing.Point(324, 585);
            this.uxButtonOK.Margin = new System.Windows.Forms.Padding(4);
            this.uxButtonOK.Name = "uxButtonOK";
            this.uxButtonOK.Size = new System.Drawing.Size(100, 28);
            this.uxButtonOK.TabIndex = 2;
            this.uxButtonOK.Text = "OK";
            this.uxButtonOK.UseVisualStyleBackColor = true;
            this.uxButtonOK.Click += new System.EventHandler(this.uxButtonOK_Click);
            // 
            // SettingsDialog
            // 
            this.AcceptButton = this.uxButtonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.uxButtonCancel;
            this.ClientSize = new System.Drawing.Size(545, 626);
            this.Controls.Add(uxTabControl);
            this.Controls.Add(this.uxButtonCancel);
            this.Controls.Add(this.uxButtonOK);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(500, 550);
            this.Name = "SettingsDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            uxTabControl.ResumeLayout(false);
            this.uxTabPageOptions.ResumeLayout(false);
            this.uxTabPageAbout.ResumeLayout(false);
            this.uxTabPageAbout.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button uxButtonCancel;
        private System.Windows.Forms.Button uxButtonOK;
        private System.Windows.Forms.PropertyGrid uxPropertyGrid;
        private System.Windows.Forms.TabPage uxTabPageOptions;
        private System.Windows.Forms.TabPage uxTabPageAbout;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.LinkLabel uxLinkLabelTitle;
        private System.Windows.Forms.TextBox uxTextBoxVersions;
        private System.Windows.Forms.LinkLabel uxLinkLabelInstallLocation;
        private System.Windows.Forms.LinkLabel uxLinkLabelClearTokenCache;
        private System.Windows.Forms.LinkLabel uxLinkLabelSendFeedback;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.LinkLabel uxLinkLabelUserSettingsLocation;
        private System.Windows.Forms.TextBox uxTextBoxLicense;
    }
}