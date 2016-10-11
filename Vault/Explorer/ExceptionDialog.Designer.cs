namespace Microsoft.Vault.Explorer
{
    partial class ExceptionDialog
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
            System.Windows.Forms.PictureBox pictureBox1;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExceptionDialog));
            this.uxTextBoxExceptionDetails = new System.Windows.Forms.TextBox();
            this.uxButtonContinue = new System.Windows.Forms.Button();
            this.uxButtonQuit = new System.Windows.Forms.Button();
            this.uxRichTextBoxCaption = new System.Windows.Forms.RichTextBox();
            pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Image = global::Microsoft.Vault.Explorer.Properties.Resources.error;
            pictureBox1.Location = new System.Drawing.Point(12, 12);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new System.Drawing.Size(48, 48);
            pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // uxTextBoxExceptionDetails
            // 
            this.uxTextBoxExceptionDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.uxTextBoxExceptionDetails.Location = new System.Drawing.Point(12, 96);
            this.uxTextBoxExceptionDetails.Multiline = true;
            this.uxTextBoxExceptionDetails.Name = "uxTextBoxExceptionDetails";
            this.uxTextBoxExceptionDetails.ReadOnly = true;
            this.uxTextBoxExceptionDetails.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.uxTextBoxExceptionDetails.Size = new System.Drawing.Size(802, 343);
            this.uxTextBoxExceptionDetails.TabIndex = 1;
            // 
            // uxButtonContinue
            // 
            this.uxButtonContinue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.uxButtonContinue.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.uxButtonContinue.Location = new System.Drawing.Point(606, 446);
            this.uxButtonContinue.Margin = new System.Windows.Forms.Padding(4);
            this.uxButtonContinue.Name = "uxButtonContinue";
            this.uxButtonContinue.Size = new System.Drawing.Size(100, 28);
            this.uxButtonContinue.TabIndex = 2;
            this.uxButtonContinue.Text = "&Continue";
            this.uxButtonContinue.UseVisualStyleBackColor = true;
            // 
            // uxButtonQuit
            // 
            this.uxButtonQuit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.uxButtonQuit.DialogResult = System.Windows.Forms.DialogResult.Abort;
            this.uxButtonQuit.Location = new System.Drawing.Point(714, 446);
            this.uxButtonQuit.Margin = new System.Windows.Forms.Padding(4);
            this.uxButtonQuit.Name = "uxButtonQuit";
            this.uxButtonQuit.Size = new System.Drawing.Size(100, 28);
            this.uxButtonQuit.TabIndex = 3;
            this.uxButtonQuit.Text = "&Quit";
            this.uxButtonQuit.UseVisualStyleBackColor = true;
            this.uxButtonQuit.Click += new System.EventHandler(this.uxButtonQuit_Click);
            // 
            // uxRichTextBoxCaption
            // 
            this.uxRichTextBoxCaption.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.uxRichTextBoxCaption.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.uxRichTextBoxCaption.Location = new System.Drawing.Point(81, 12);
            this.uxRichTextBoxCaption.Name = "uxRichTextBoxCaption";
            this.uxRichTextBoxCaption.ReadOnly = true;
            this.uxRichTextBoxCaption.Size = new System.Drawing.Size(733, 78);
            this.uxRichTextBoxCaption.TabIndex = 0;
            this.uxRichTextBoxCaption.Text = "";
            // 
            // ExceptionDialog
            // 
            this.AcceptButton = this.uxButtonContinue;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.uxButtonContinue;
            this.ClientSize = new System.Drawing.Size(826, 489);
            this.Controls.Add(this.uxRichTextBoxCaption);
            this.Controls.Add(this.uxButtonQuit);
            this.Controls.Add(this.uxButtonContinue);
            this.Controls.Add(this.uxTextBoxExceptionDetails);
            this.Controls.Add(pictureBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimizeBox = false;
            this.Name = "ExceptionDialog";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Exception occured";
            ((System.ComponentModel.ISupportInitialize)(pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox uxTextBoxExceptionDetails;
        private System.Windows.Forms.Button uxButtonContinue;
        private System.Windows.Forms.Button uxButtonQuit;
        private System.Windows.Forms.RichTextBox uxRichTextBoxCaption;
    }
}