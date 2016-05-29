namespace Microsoft.PS.Common.Vault.Explorer
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
            this.uxButtonCancel = new System.Windows.Forms.Button();
            this.uxButtonOK = new System.Windows.Forms.Button();
            this.uxPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.SuspendLayout();
            // 
            // uxButtonCancel
            // 
            this.uxButtonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.uxButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.uxButtonCancel.Location = new System.Drawing.Point(469, 474);
            this.uxButtonCancel.Margin = new System.Windows.Forms.Padding(4);
            this.uxButtonCancel.Name = "uxButtonCancel";
            this.uxButtonCancel.Size = new System.Drawing.Size(100, 28);
            this.uxButtonCancel.TabIndex = 8;
            this.uxButtonCancel.Text = "Cancel";
            this.uxButtonCancel.UseVisualStyleBackColor = true;
            // 
            // uxButtonOK
            // 
            this.uxButtonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.uxButtonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.uxButtonOK.Enabled = false;
            this.uxButtonOK.Location = new System.Drawing.Point(361, 474);
            this.uxButtonOK.Margin = new System.Windows.Forms.Padding(4);
            this.uxButtonOK.Name = "uxButtonOK";
            this.uxButtonOK.Size = new System.Drawing.Size(100, 28);
            this.uxButtonOK.TabIndex = 7;
            this.uxButtonOK.Text = "OK";
            this.uxButtonOK.UseVisualStyleBackColor = true;
            this.uxButtonOK.Click += new System.EventHandler(this.uxButtonOK_Click);
            // 
            // uxPropertyGrid
            // 
            this.uxPropertyGrid.Location = new System.Drawing.Point(12, 12);
            this.uxPropertyGrid.Name = "uxPropertyGrid";
            this.uxPropertyGrid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.uxPropertyGrid.Size = new System.Drawing.Size(557, 455);
            this.uxPropertyGrid.TabIndex = 9;
            this.uxPropertyGrid.ToolbarVisible = false;
            // 
            // SettingsDialog
            // 
            this.AcceptButton = this.uxButtonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.uxButtonCancel;
            this.ClientSize = new System.Drawing.Size(582, 515);
            this.Controls.Add(this.uxPropertyGrid);
            this.Controls.Add(this.uxButtonCancel);
            this.Controls.Add(this.uxButtonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SettingsDialog_FormClosed);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button uxButtonCancel;
        private System.Windows.Forms.Button uxButtonOK;
        private System.Windows.Forms.PropertyGrid uxPropertyGrid;
    }
}