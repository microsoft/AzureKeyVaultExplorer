namespace Microsoft.PS.Common.Vault.Explorer
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
            System.Windows.Forms.Label uxLabelValue;
            System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
            this.uxLinkLabelSecretKind = new System.Windows.Forms.LinkLabel();
            this.uxTextBoxName = new System.Windows.Forms.TextBox();
            this.uxButtonOK = new System.Windows.Forms.Button();
            this.uxButtonCancel = new System.Windows.Forms.Button();
            this.uxErrorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.uxSplitContainer = new System.Windows.Forms.SplitContainer();
            this.uxPropertyGridSecret = new System.Windows.Forms.PropertyGrid();
            this.uxLabelBytesLeft = new System.Windows.Forms.Label();
            this.uxToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.uxTimerValueTypingCompleted = new System.Windows.Forms.Timer(this.components);
            this.uxMenuSecretKind = new System.Windows.Forms.ContextMenuStrip(this.components);
            uxLabelValue = new System.Windows.Forms.Label();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.uxErrorProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uxSplitContainer)).BeginInit();
            this.uxSplitContainer.Panel2.SuspendLayout();
            this.uxSplitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // uxLabelValue
            // 
            uxLabelValue.AutoSize = true;
            uxLabelValue.Location = new System.Drawing.Point(12, 58);
            uxLabelValue.Name = "uxLabelValue";
            uxLabelValue.Size = new System.Drawing.Size(37, 13);
            uxLabelValue.TabIndex = 2;
            uxLabelValue.Text = "Value:";
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
            tableLayoutPanel1.Location = new System.Drawing.Point(13, 7);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new System.Drawing.Size(926, 0);
            tableLayoutPanel1.TabIndex = 8;
            // 
            // uxLinkLabelSecretKind
            // 
            this.uxLinkLabelSecretKind.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.uxLinkLabelSecretKind.Image = global::Microsoft.PS.Common.Vault.Explorer.Properties.Resources.information;
            this.uxLinkLabelSecretKind.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.uxLinkLabelSecretKind.Location = new System.Drawing.Point(12, 7);
            this.uxLinkLabelSecretKind.Margin = new System.Windows.Forms.Padding(0);
            this.uxLinkLabelSecretKind.Name = "uxLinkLabelSecretKind";
            this.uxLinkLabelSecretKind.Size = new System.Drawing.Size(928, 15);
            this.uxLinkLabelSecretKind.TabIndex = 0;
            this.uxLinkLabelSecretKind.TabStop = true;
            this.uxLinkLabelSecretKind.Text = "Custom secret name:";
            this.uxLinkLabelSecretKind.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.uxLinkLabelSecretKind_LinkClicked);
            // 
            // uxTextBoxName
            // 
            this.uxTextBoxName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.uxTextBoxName.Font = new System.Drawing.Font("Courier New", 9.75F);
            this.uxErrorProvider.SetIconAlignment(this.uxTextBoxName, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
            this.uxTextBoxName.Location = new System.Drawing.Point(15, 25);
            this.uxTextBoxName.MaxLength = 256;
            this.uxTextBoxName.Name = "uxTextBoxName";
            this.uxTextBoxName.Size = new System.Drawing.Size(925, 22);
            this.uxTextBoxName.TabIndex = 1;
            this.uxTextBoxName.TextChanged += new System.EventHandler(this.uxTextBoxName_TextChanged);
            // 
            // uxButtonOK
            // 
            this.uxButtonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.uxButtonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.uxButtonOK.Enabled = false;
            this.uxButtonOK.Location = new System.Drawing.Point(784, 463);
            this.uxButtonOK.Name = "uxButtonOK";
            this.uxButtonOK.Size = new System.Drawing.Size(75, 23);
            this.uxButtonOK.TabIndex = 5;
            this.uxButtonOK.Text = "OK";
            this.uxButtonOK.UseVisualStyleBackColor = true;
            // 
            // uxButtonCancel
            // 
            this.uxButtonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.uxButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.uxButtonCancel.Location = new System.Drawing.Point(865, 463);
            this.uxButtonCancel.Name = "uxButtonCancel";
            this.uxButtonCancel.Size = new System.Drawing.Size(75, 23);
            this.uxButtonCancel.TabIndex = 6;
            this.uxButtonCancel.Text = "Cancel";
            this.uxButtonCancel.UseVisualStyleBackColor = true;
            // 
            // uxErrorProvider
            // 
            this.uxErrorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.uxErrorProvider.ContainerControl = this;
            // 
            // uxSplitContainer
            // 
            this.uxSplitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.uxErrorProvider.SetIconAlignment(this.uxSplitContainer, System.Windows.Forms.ErrorIconAlignment.TopLeft);
            this.uxSplitContainer.Location = new System.Drawing.Point(15, 75);
            this.uxSplitContainer.Margin = new System.Windows.Forms.Padding(2);
            this.uxSplitContainer.Name = "uxSplitContainer";
            this.uxSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // uxSplitContainer.Panel1
            // 
            this.uxErrorProvider.SetIconAlignment(this.uxSplitContainer.Panel1, System.Windows.Forms.ErrorIconAlignment.TopLeft);
            // 
            // uxSplitContainer.Panel2
            // 
            this.uxSplitContainer.Panel2.Controls.Add(this.uxPropertyGridSecret);
            this.uxSplitContainer.Size = new System.Drawing.Size(924, 383);
            this.uxSplitContainer.SplitterDistance = 288;
            this.uxSplitContainer.SplitterWidth = 5;
            this.uxSplitContainer.TabIndex = 4;
            // 
            // uxPropertyGridSecret
            // 
            this.uxPropertyGridSecret.DisabledItemForeColor = System.Drawing.SystemColors.WindowText;
            this.uxPropertyGridSecret.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uxPropertyGridSecret.HelpVisible = false;
            this.uxPropertyGridSecret.Location = new System.Drawing.Point(0, 0);
            this.uxPropertyGridSecret.Margin = new System.Windows.Forms.Padding(2);
            this.uxPropertyGridSecret.Name = "uxPropertyGridSecret";
            this.uxPropertyGridSecret.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.uxPropertyGridSecret.Size = new System.Drawing.Size(924, 90);
            this.uxPropertyGridSecret.TabIndex = 0;
            this.uxPropertyGridSecret.ToolbarVisible = false;
            // 
            // uxLabelBytesLeft
            // 
            this.uxLabelBytesLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.uxLabelBytesLeft.Location = new System.Drawing.Point(749, 58);
            this.uxLabelBytesLeft.Name = "uxLabelBytesLeft";
            this.uxLabelBytesLeft.Size = new System.Drawing.Size(190, 14);
            this.uxLabelBytesLeft.TabIndex = 3;
            this.uxLabelBytesLeft.Text = "xxx bytes left";
            this.uxLabelBytesLeft.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // uxToolTip
            // 
            this.uxToolTip.AutoPopDelay = 30000;
            this.uxToolTip.InitialDelay = 500;
            this.uxToolTip.ReshowDelay = 100;
            // 
            // uxTimerValueTypingCompleted
            // 
            this.uxTimerValueTypingCompleted.Interval = 250;
            this.uxTimerValueTypingCompleted.Tick += new System.EventHandler(this.uxTimerValueTypingCompleted_Tick);
            // 
            // uxMenuSecretKind
            // 
            this.uxMenuSecretKind.Name = "uxMenuSecretKind";
            this.uxMenuSecretKind.Size = new System.Drawing.Size(61, 4);
            this.uxMenuSecretKind.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.uxMenuSecretKind_ItemClicked);
            // 
            // SecretDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.uxButtonCancel;
            this.ClientSize = new System.Drawing.Size(952, 496);
            this.Controls.Add(this.uxLinkLabelSecretKind);
            this.Controls.Add(this.uxSplitContainer);
            this.Controls.Add(this.uxButtonCancel);
            this.Controls.Add(this.uxButtonOK);
            this.Controls.Add(this.uxLabelBytesLeft);
            this.Controls.Add(uxLabelValue);
            this.Controls.Add(this.uxTextBoxName);
            this.Controls.Add(tableLayoutPanel1);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(378, 411);
            this.Name = "SecretDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "New Secret";
            ((System.ComponentModel.ISupportInitialize)(this.uxErrorProvider)).EndInit();
            this.uxSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.uxSplitContainer)).EndInit();
            this.uxSplitContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox uxTextBoxName;
        private System.Windows.Forms.Button uxButtonOK;
        private System.Windows.Forms.Button uxButtonCancel;
        private System.Windows.Forms.ErrorProvider uxErrorProvider;
        private System.Windows.Forms.PropertyGrid uxPropertyGridSecret;
        private System.Windows.Forms.SplitContainer uxSplitContainer;
        private System.Windows.Forms.Label uxLabelBytesLeft;
        private System.Windows.Forms.ToolTip uxToolTip;
        private System.Windows.Forms.Timer uxTimerValueTypingCompleted;
        private System.Windows.Forms.LinkLabel uxLinkLabelSecretKind;
        private System.Windows.Forms.ContextMenuStrip uxMenuSecretKind;
    }
}
