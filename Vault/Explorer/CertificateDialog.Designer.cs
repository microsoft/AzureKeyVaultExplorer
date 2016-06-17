namespace Microsoft.PS.Common.Vault.Explorer
{
    partial class CertificateDialog
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
            System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
            this.uxLinkLabelSecretKind = new System.Windows.Forms.LinkLabel();
            this.uxTextBoxName = new System.Windows.Forms.TextBox();
            this.uxButtonOK = new System.Windows.Forms.Button();
            this.uxButtonCancel = new System.Windows.Forms.Button();
            this.uxErrorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.uxPropertyGridSecret = new System.Windows.Forms.PropertyGrid();
            this.uxToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.uxLinkLabelValue = new System.Windows.Forms.LinkLabel();
            this.uxMenuVersions = new System.Windows.Forms.ContextMenuStrip(this.components);
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.uxErrorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            tableLayoutPanel1.AutoSize = true;
            tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.Location = new System.Drawing.Point(17, 9);
            tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new System.Drawing.Size(794, 0);
            tableLayoutPanel1.TabIndex = 8;
            // 
            // uxLinkLabelSecretKind
            // 
            this.uxLinkLabelSecretKind.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.uxLinkLabelSecretKind.Image = global::Microsoft.PS.Common.Vault.Explorer.Properties.Resources.information;
            this.uxLinkLabelSecretKind.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.uxLinkLabelSecretKind.Location = new System.Drawing.Point(16, 9);
            this.uxLinkLabelSecretKind.Margin = new System.Windows.Forms.Padding(0);
            this.uxLinkLabelSecretKind.Name = "uxLinkLabelSecretKind";
            this.uxLinkLabelSecretKind.Size = new System.Drawing.Size(796, 18);
            this.uxLinkLabelSecretKind.TabIndex = 1;
            this.uxLinkLabelSecretKind.TabStop = true;
            this.uxLinkLabelSecretKind.Text = "Certificate name";
            // 
            // uxTextBoxName
            // 
            this.uxTextBoxName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.uxTextBoxName.Font = new System.Drawing.Font("Courier New", 9.75F);
            this.uxErrorProvider.SetIconAlignment(this.uxTextBoxName, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
            this.uxTextBoxName.Location = new System.Drawing.Point(20, 31);
            this.uxTextBoxName.Margin = new System.Windows.Forms.Padding(4);
            this.uxTextBoxName.MaxLength = 256;
            this.uxTextBoxName.Name = "uxTextBoxName";
            this.uxTextBoxName.Size = new System.Drawing.Size(791, 26);
            this.uxTextBoxName.TabIndex = 0;
            this.uxTextBoxName.TextChanged += new System.EventHandler(this.uxTextBoxName_TextChanged);
            // 
            // uxButtonOK
            // 
            this.uxButtonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.uxButtonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.uxButtonOK.Enabled = false;
            this.uxButtonOK.Location = new System.Drawing.Point(604, 570);
            this.uxButtonOK.Margin = new System.Windows.Forms.Padding(4);
            this.uxButtonOK.Name = "uxButtonOK";
            this.uxButtonOK.Size = new System.Drawing.Size(100, 28);
            this.uxButtonOK.TabIndex = 5;
            this.uxButtonOK.Text = "OK";
            this.uxButtonOK.UseVisualStyleBackColor = true;
            // 
            // uxButtonCancel
            // 
            this.uxButtonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.uxButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.uxButtonCancel.Location = new System.Drawing.Point(712, 570);
            this.uxButtonCancel.Margin = new System.Windows.Forms.Padding(4);
            this.uxButtonCancel.Name = "uxButtonCancel";
            this.uxButtonCancel.Size = new System.Drawing.Size(100, 28);
            this.uxButtonCancel.TabIndex = 6;
            this.uxButtonCancel.Text = "Cancel";
            this.uxButtonCancel.UseVisualStyleBackColor = true;
            // 
            // uxErrorProvider
            // 
            this.uxErrorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.uxErrorProvider.ContainerControl = this;
            // 
            // uxPropertyGridSecret
            // 
            this.uxPropertyGridSecret.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.uxPropertyGridSecret.DisabledItemForeColor = System.Drawing.SystemColors.WindowText;
            this.uxPropertyGridSecret.HelpVisible = false;
            this.uxPropertyGridSecret.Location = new System.Drawing.Point(20, 90);
            this.uxPropertyGridSecret.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.uxPropertyGridSecret.Name = "uxPropertyGridSecret";
            this.uxPropertyGridSecret.Size = new System.Drawing.Size(791, 474);
            this.uxPropertyGridSecret.TabIndex = 0;
            this.uxPropertyGridSecret.ToolbarVisible = false;
            // 
            // uxToolTip
            // 
            this.uxToolTip.AutoPopDelay = 30000;
            this.uxToolTip.InitialDelay = 500;
            this.uxToolTip.ReshowDelay = 100;
            // 
            // uxLinkLabelValue
            // 
            this.uxLinkLabelValue.AutoSize = true;
            this.uxLinkLabelValue.Location = new System.Drawing.Point(16, 68);
            this.uxLinkLabelValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.uxLinkLabelValue.Name = "uxLinkLabelValue";
            this.uxLinkLabelValue.Size = new System.Drawing.Size(58, 17);
            this.uxLinkLabelValue.TabIndex = 3;
            this.uxLinkLabelValue.TabStop = true;
            this.uxLinkLabelValue.Text = "Value ▼";
            this.uxLinkLabelValue.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.uxLinkLabelValue_LinkClicked);
            // 
            // uxMenuVersions
            // 
            this.uxMenuVersions.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.uxMenuVersions.Name = "uxMenuVersions";
            this.uxMenuVersions.Size = new System.Drawing.Size(67, 4);
            this.uxMenuVersions.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.uxMenuVersions_ItemClicked);
            // 
            // CertificateDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.uxButtonCancel;
            this.ClientSize = new System.Drawing.Size(828, 610);
            this.Controls.Add(this.uxPropertyGridSecret);
            this.Controls.Add(this.uxLinkLabelValue);
            this.Controls.Add(this.uxLinkLabelSecretKind);
            this.Controls.Add(this.uxButtonCancel);
            this.Controls.Add(this.uxButtonOK);
            this.Controls.Add(this.uxTextBoxName);
            this.Controls.Add(tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(498, 491);
            this.Name = "CertificateDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "New Certificate";
            ((System.ComponentModel.ISupportInitialize)(this.uxErrorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox uxTextBoxName;
        private System.Windows.Forms.Button uxButtonOK;
        private System.Windows.Forms.Button uxButtonCancel;
        private System.Windows.Forms.ErrorProvider uxErrorProvider;
        private System.Windows.Forms.PropertyGrid uxPropertyGridSecret;
        private System.Windows.Forms.ToolTip uxToolTip;
        private System.Windows.Forms.LinkLabel uxLinkLabelSecretKind;
        private System.Windows.Forms.LinkLabel uxLinkLabelValue;
        private System.Windows.Forms.ContextMenuStrip uxMenuVersions;
    }
}
