namespace Microsoft.Vault.Explorer
{
    partial class PasswordDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PasswordDialog));
            System.Windows.Forms.Button uxButtonOK;
            System.Windows.Forms.Button uxButtonCancel;
            this.uxTextBoxPassword = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.uxCheckBoxDisplayPwd = new System.Windows.Forms.CheckBox();
            label1 = new System.Windows.Forms.Label();
            uxButtonOK = new System.Windows.Forms.Button();
            uxButtonCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(86, 9);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(56, 13);
            label1.TabIndex = 0;
            label1.Text = "Password:";
            // 
            // uxTextBoxPassword
            // 
            this.uxTextBoxPassword.Location = new System.Drawing.Point(89, 25);
            this.uxTextBoxPassword.Name = "uxTextBoxPassword";
            this.uxTextBoxPassword.Size = new System.Drawing.Size(302, 20);
            this.uxTextBoxPassword.TabIndex = 1;
            this.uxTextBoxPassword.UseSystemPasswordChar = true;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(-1, -2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(81, 97);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // uxButtonOK
            // 
            uxButtonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            uxButtonOK.Location = new System.Drawing.Point(235, 66);
            uxButtonOK.Name = "uxButtonOK";
            uxButtonOK.Size = new System.Drawing.Size(75, 23);
            uxButtonOK.TabIndex = 3;
            uxButtonOK.Text = "&OK";
            uxButtonOK.UseVisualStyleBackColor = true;
            // 
            // uxButtonCancel
            // 
            uxButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            uxButtonCancel.Location = new System.Drawing.Point(316, 66);
            uxButtonCancel.Name = "uxButtonCancel";
            uxButtonCancel.Size = new System.Drawing.Size(75, 23);
            uxButtonCancel.TabIndex = 4;
            uxButtonCancel.Text = "&Cancel";
            uxButtonCancel.UseVisualStyleBackColor = true;
            // 
            // uxCheckBoxDisplayPwd
            // 
            this.uxCheckBoxDisplayPwd.AutoSize = true;
            this.uxCheckBoxDisplayPwd.Location = new System.Drawing.Point(89, 51);
            this.uxCheckBoxDisplayPwd.Name = "uxCheckBoxDisplayPwd";
            this.uxCheckBoxDisplayPwd.Size = new System.Drawing.Size(109, 17);
            this.uxCheckBoxDisplayPwd.TabIndex = 2;
            this.uxCheckBoxDisplayPwd.Text = "&Display Password";
            this.uxCheckBoxDisplayPwd.UseVisualStyleBackColor = true;
            this.uxCheckBoxDisplayPwd.CheckedChanged += new System.EventHandler(this.uxCheckBoxDisplayPwd_CheckedChanged);
            // 
            // PasswordDialog
            // 
            this.AcceptButton = uxButtonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = uxButtonCancel;
            this.ClientSize = new System.Drawing.Size(400, 95);
            this.Controls.Add(this.uxCheckBoxDisplayPwd);
            this.Controls.Add(uxButtonCancel);
            this.Controls.Add(uxButtonOK);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.uxTextBoxPassword);
            this.Controls.Add(label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PasswordDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Enter Password";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox uxTextBoxPassword;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.CheckBox uxCheckBoxDisplayPwd;
    }
}