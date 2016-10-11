namespace Microsoft.Vault.Explorer
{
    partial class SecretDialog
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
            this.uxSplitContainer = new System.Windows.Forms.SplitContainer();
            this.uxLinkLabelViewCertificate = new System.Windows.Forms.LinkLabel();
            this.uxPropertyGridSecret = new System.Windows.Forms.PropertyGrid();
            this.uxLabelBytesLeft = new System.Windows.Forms.Label();
            this.uxTimerValueTypingCompleted = new System.Windows.Forms.Timer(this.components);
            this.uxMenuNewValue = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.uxMenuItemNewPassword = new System.Windows.Forms.ToolStripMenuItem();
            this.uxMenuItemNewGuid = new System.Windows.Forms.ToolStripMenuItem();
            this.uxMenuItemNewApiKey = new System.Windows.Forms.ToolStripMenuItem();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.uxSplitContainer)).BeginInit();
            this.uxSplitContainer.Panel1.SuspendLayout();
            this.uxSplitContainer.Panel2.SuspendLayout();
            this.uxSplitContainer.SuspendLayout();
            this.uxMenuNewValue.SuspendLayout();
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
            tableLayoutPanel1.Size = new System.Drawing.Size(1235, 0);
            tableLayoutPanel1.TabIndex = 8;
            // 
            // uxSplitContainer
            // 
            this.uxSplitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.uxSplitContainer.Location = new System.Drawing.Point(20, 92);
            this.uxSplitContainer.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.uxSplitContainer.Name = "uxSplitContainer";
            this.uxSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // uxSplitContainer.Panel1
            // 
            this.uxSplitContainer.Panel1.Controls.Add(this.uxLinkLabelViewCertificate);
            this.uxSplitContainer.Panel1.SizeChanged += new System.EventHandler(this.uxSplitContainer_Panel1_SizeChanged);
            // 
            // uxSplitContainer.Panel2
            // 
            this.uxSplitContainer.Panel2.Controls.Add(this.uxPropertyGridSecret);
            this.uxSplitContainer.Size = new System.Drawing.Size(1232, 471);
            this.uxSplitContainer.SplitterDistance = 351;
            this.uxSplitContainer.SplitterWidth = 6;
            this.uxSplitContainer.TabIndex = 2;
            // 
            // uxLinkLabelViewCertificate
            // 
            this.uxLinkLabelViewCertificate.AutoSize = true;
            this.uxLinkLabelViewCertificate.BackColor = System.Drawing.SystemColors.Window;
            this.uxLinkLabelViewCertificate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.uxLinkLabelViewCertificate.Location = new System.Drawing.Point(543, 161);
            this.uxLinkLabelViewCertificate.Name = "uxLinkLabelViewCertificate";
            this.uxLinkLabelViewCertificate.Size = new System.Drawing.Size(104, 17);
            this.uxLinkLabelViewCertificate.TabIndex = 10;
            this.uxLinkLabelViewCertificate.TabStop = true;
            this.uxLinkLabelViewCertificate.Text = "View Certificate";
            this.uxLinkLabelViewCertificate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.uxLinkLabelViewCertificate.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.uxLinkLabelViewCertificate_LinkClicked);
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
            this.uxPropertyGridSecret.Size = new System.Drawing.Size(1232, 114);
            this.uxPropertyGridSecret.TabIndex = 0;
            this.uxPropertyGridSecret.ToolbarVisible = false;
            // 
            // uxLabelBytesLeft
            // 
            this.uxLabelBytesLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.uxLabelBytesLeft.Location = new System.Drawing.Point(999, 68);
            this.uxLabelBytesLeft.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.uxLabelBytesLeft.Name = "uxLabelBytesLeft";
            this.uxLabelBytesLeft.Size = new System.Drawing.Size(253, 17);
            this.uxLabelBytesLeft.TabIndex = 3;
            this.uxLabelBytesLeft.Text = "xxx bytes left";
            this.uxLabelBytesLeft.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // uxTimerValueTypingCompleted
            // 
            this.uxTimerValueTypingCompleted.Interval = 250;
            this.uxTimerValueTypingCompleted.Tick += new System.EventHandler(this.uxTimerValueTypingCompleted_Tick);
            // 
            // uxMenuNewValue
            // 
            this.uxMenuNewValue.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.uxMenuNewValue.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.uxMenuItemNewPassword,
            this.uxMenuItemNewGuid,
            this.uxMenuItemNewApiKey});
            this.uxMenuNewValue.Name = "uxMenuNewValue";
            this.uxMenuNewValue.Size = new System.Drawing.Size(182, 82);
            // 
            // uxMenuItemNewPassword
            // 
            this.uxMenuItemNewPassword.Name = "uxMenuItemNewPassword";
            this.uxMenuItemNewPassword.Size = new System.Drawing.Size(181, 26);
            this.uxMenuItemNewPassword.Text = "New password";
            this.uxMenuItemNewPassword.ToolTipText = "Generates a random password of 32 to 40 chars with upper case, lower case, numeri" +
    "c and few special chars";
            this.uxMenuItemNewPassword.Click += new System.EventHandler(this.uxMenuItemNewPassword_Click);
            // 
            // uxMenuItemNewGuid
            // 
            this.uxMenuItemNewGuid.Name = "uxMenuItemNewGuid";
            this.uxMenuItemNewGuid.Size = new System.Drawing.Size(181, 26);
            this.uxMenuItemNewGuid.Text = "New guid";
            this.uxMenuItemNewGuid.ToolTipText = "Generates a random guid in D format (32 digits separated by hyphens)";
            this.uxMenuItemNewGuid.Click += new System.EventHandler(this.uxMenuItemNewGuid_Click);
            // 
            // uxMenuItemNewApiKey
            // 
            this.uxMenuItemNewApiKey.Name = "uxMenuItemNewApiKey";
            this.uxMenuItemNewApiKey.Size = new System.Drawing.Size(181, 26);
            this.uxMenuItemNewApiKey.Text = "New api key";
            this.uxMenuItemNewApiKey.ToolTipText = "Generates a random api key of 64 bytes encoded as base64";
            this.uxMenuItemNewApiKey.Click += new System.EventHandler(this.uxMenuItemNewApiKey_Click);
            // 
            // SecretDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1269, 610);
            this.Controls.Add(this.uxSplitContainer);
            this.Controls.Add(this.uxLabelBytesLeft);
            this.Controls.Add(tableLayoutPanel1);
            this.Name = "SecretDialog";
            this.uxSplitContainer.Panel1.ResumeLayout(false);
            this.uxSplitContainer.Panel1.PerformLayout();
            this.uxSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.uxSplitContainer)).EndInit();
            this.uxSplitContainer.ResumeLayout(false);
            this.uxMenuNewValue.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PropertyGrid uxPropertyGridSecret;
        private System.Windows.Forms.SplitContainer uxSplitContainer;
        private System.Windows.Forms.Label uxLabelBytesLeft;
        private System.Windows.Forms.Timer uxTimerValueTypingCompleted;
        private System.Windows.Forms.ContextMenuStrip uxMenuNewValue;
        private System.Windows.Forms.ToolStripMenuItem uxMenuItemNewPassword;
        private System.Windows.Forms.ToolStripMenuItem uxMenuItemNewGuid;
        private System.Windows.Forms.LinkLabel uxLinkLabelViewCertificate;
        private System.Windows.Forms.ToolStripMenuItem uxMenuItemNewApiKey;
    }
}
