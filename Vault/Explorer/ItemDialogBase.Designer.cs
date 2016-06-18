namespace Microsoft.PS.Common.Vault.Explorer
{
    partial class ItemDialogBase<T> where T : PropertyObject
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
            System.Windows.Forms.Panel panel1;
            this.uxTextBoxName = new System.Windows.Forms.TextBox();
            this.uxLinkLabelSecretKind = new System.Windows.Forms.LinkLabel();
            this.uxErrorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.panel2 = new System.Windows.Forms.Panel();
            this.uxButtonOK = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.uxButtonCancel = new System.Windows.Forms.Button();
            this.uxMenuSecretKind = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.uxMenuVersions = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.uxToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.uxLinkLabelValue = new System.Windows.Forms.LinkLabel();
            panel1 = new System.Windows.Forms.Panel();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.uxErrorProvider)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(this.uxTextBoxName);
            panel1.Controls.Add(this.uxLinkLabelSecretKind);
            panel1.Dock = System.Windows.Forms.DockStyle.Top;
            panel1.Location = new System.Drawing.Point(0, 0);
            panel1.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            panel1.Name = "panel1";
            panel1.Padding = new System.Windows.Forms.Padding(20, 8, 20, 0);
            panel1.Size = new System.Drawing.Size(804, 67);
            panel1.TabIndex = 4;
            // 
            // uxTextBoxName
            // 
            this.uxTextBoxName.Dock = System.Windows.Forms.DockStyle.Top;
            this.uxTextBoxName.Font = new System.Drawing.Font("Courier New", 9.75F);
            this.uxErrorProvider.SetIconAlignment(this.uxTextBoxName, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
            this.uxTextBoxName.Location = new System.Drawing.Point(20, 32);
            this.uxTextBoxName.Margin = new System.Windows.Forms.Padding(4);
            this.uxTextBoxName.MaxLength = 256;
            this.uxTextBoxName.Name = "uxTextBoxName";
            this.uxTextBoxName.Size = new System.Drawing.Size(764, 26);
            this.uxTextBoxName.TabIndex = 2;
            this.uxTextBoxName.TextChanged += new System.EventHandler(this.uxTextBoxName_TextChanged);
            // 
            // uxLinkLabelSecretKind
            // 
            this.uxLinkLabelSecretKind.Dock = System.Windows.Forms.DockStyle.Top;
            this.uxLinkLabelSecretKind.Image = global::Microsoft.PS.Common.Vault.Explorer.Properties.Resources.information;
            this.uxLinkLabelSecretKind.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.uxLinkLabelSecretKind.Location = new System.Drawing.Point(20, 8);
            this.uxLinkLabelSecretKind.Margin = new System.Windows.Forms.Padding(0);
            this.uxLinkLabelSecretKind.Name = "uxLinkLabelSecretKind";
            this.uxLinkLabelSecretKind.Size = new System.Drawing.Size(764, 24);
            this.uxLinkLabelSecretKind.TabIndex = 3;
            this.uxLinkLabelSecretKind.TabStop = true;
            this.uxLinkLabelSecretKind.Text = "Certificate name";
            this.uxLinkLabelSecretKind.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.uxLinkLabelSecretKind_LinkClicked);
            // 
            // uxErrorProvider
            // 
            this.uxErrorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.uxErrorProvider.ContainerControl = this;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.uxButtonOK);
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Controls.Add(this.uxButtonCancel);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 345);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(0, 8, 20, 8);
            this.panel2.Size = new System.Drawing.Size(804, 44);
            this.panel2.TabIndex = 5;
            // 
            // uxButtonOK
            // 
            this.uxButtonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.uxButtonOK.Dock = System.Windows.Forms.DockStyle.Right;
            this.uxButtonOK.Enabled = false;
            this.uxButtonOK.Location = new System.Drawing.Point(576, 8);
            this.uxButtonOK.Margin = new System.Windows.Forms.Padding(4, 4, 8, 4);
            this.uxButtonOK.Name = "uxButtonOK";
            this.uxButtonOK.Size = new System.Drawing.Size(100, 28);
            this.uxButtonOK.TabIndex = 7;
            this.uxButtonOK.Text = "OK";
            this.uxButtonOK.UseVisualStyleBackColor = true;
            // 
            // panel3
            // 
            this.panel3.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel3.Location = new System.Drawing.Point(676, 8);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(8, 28);
            this.panel3.TabIndex = 9;
            // 
            // uxButtonCancel
            // 
            this.uxButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.uxButtonCancel.Dock = System.Windows.Forms.DockStyle.Right;
            this.uxButtonCancel.Location = new System.Drawing.Point(684, 8);
            this.uxButtonCancel.Margin = new System.Windows.Forms.Padding(4, 4, 8, 4);
            this.uxButtonCancel.Name = "uxButtonCancel";
            this.uxButtonCancel.Size = new System.Drawing.Size(100, 28);
            this.uxButtonCancel.TabIndex = 8;
            this.uxButtonCancel.Text = "Cancel";
            this.uxButtonCancel.UseVisualStyleBackColor = true;
            // 
            // uxMenuSecretKind
            // 
            this.uxMenuSecretKind.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.uxMenuSecretKind.Name = "uxMenuSecretKind";
            this.uxMenuSecretKind.Size = new System.Drawing.Size(67, 4);
            this.uxMenuSecretKind.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.uxMenuSecretKind_ItemClicked);
            // 
            // uxMenuVersions
            // 
            this.uxMenuVersions.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.uxMenuVersions.Name = "uxMenuVersions";
            this.uxMenuVersions.Size = new System.Drawing.Size(67, 4);
            this.uxMenuVersions.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.uxMenuVersions_ItemClicked);
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
            this.uxLinkLabelValue.Location = new System.Drawing.Point(22, 68);
            this.uxLinkLabelValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.uxLinkLabelValue.Name = "uxLinkLabelValue";
            this.uxLinkLabelValue.Size = new System.Drawing.Size(58, 17);
            this.uxLinkLabelValue.TabIndex = 6;
            this.uxLinkLabelValue.TabStop = true;
            this.uxLinkLabelValue.Text = "Value ▼";
            this.uxLinkLabelValue.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.uxLinkLabelValue_LinkClicked);
            // 
            // ItemDialogBase
            // 
            this.AcceptButton = this.uxButtonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.uxButtonCancel;
            this.ClientSize = new System.Drawing.Size(804, 389);
            this.Controls.Add(this.uxLinkLabelValue);
            this.Controls.Add(this.panel2);
            this.Controls.Add(panel1);
            this.MinimizeBox = false;
            this.Name = "ItemDialogBase";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ItemDialogBase";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.uxErrorProvider)).EndInit();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected System.Windows.Forms.LinkLabel uxLinkLabelSecretKind;
        protected System.Windows.Forms.TextBox uxTextBoxName;
        protected System.Windows.Forms.ErrorProvider uxErrorProvider;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button uxButtonOK;
        private System.Windows.Forms.Button uxButtonCancel;
        private System.Windows.Forms.Panel panel3;
        protected System.Windows.Forms.ContextMenuStrip uxMenuSecretKind;
        protected System.Windows.Forms.ContextMenuStrip uxMenuVersions;
        protected System.Windows.Forms.ToolTip uxToolTip;
        protected System.Windows.Forms.LinkLabel uxLinkLabelValue;
    }
}