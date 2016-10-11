namespace Microsoft.Vault.Explorer
{
    partial class ListViewSecrets
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ListViewSecrets));
            System.Windows.Forms.ListViewGroup listViewGroup6 = new System.Windows.Forms.ListViewGroup("", System.Windows.Forms.HorizontalAlignment.Center);
            System.Windows.Forms.ListViewGroup listViewGroup7 = new System.Windows.Forms.ListViewGroup("", System.Windows.Forms.HorizontalAlignment.Center);
            System.Windows.Forms.ListViewGroup listViewGroup8 = new System.Windows.Forms.ListViewGroup("", System.Windows.Forms.HorizontalAlignment.Center);
            System.Windows.Forms.ListViewGroup listViewGroup9 = new System.Windows.Forms.ListViewGroup("", System.Windows.Forms.HorizontalAlignment.Center);
            System.Windows.Forms.ListViewGroup listViewGroup10 = new System.Windows.Forms.ListViewGroup("", System.Windows.Forms.HorizontalAlignment.Center);
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.uxSmallImageList = new System.Windows.Forms.ImageList(this.components);
            this.uxMenuStripColumns = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.SuspendLayout();
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 450;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Updated";
            this.columnHeader2.Width = 140;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Changed by";
            this.columnHeader3.Width = 200;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Expires";
            this.columnHeader4.Width = 100;
            // 
            // uxSmallImageList
            // 
            this.uxSmallImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("uxSmallImageList.ImageStream")));
            this.uxSmallImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.uxSmallImageList.Images.SetKeyName(0, "empty.png");
            this.uxSmallImageList.Images.SetKeyName(1, "medal_bronze_1.png");
            this.uxSmallImageList.Images.SetKeyName(2, "medal_bronze_delete.png");
            this.uxSmallImageList.Images.SetKeyName(3, "certificate2.png");
            this.uxSmallImageList.Images.SetKeyName(4, "certificate2_disabled.png");
            this.uxSmallImageList.Images.SetKeyName(5, "key.png");
            this.uxSmallImageList.Images.SetKeyName(6, "key_delete.png");
            // 
            // uxMenuStripColumns
            // 
            this.uxMenuStripColumns.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.uxMenuStripColumns.Name = "uxMenuStripColumns";
            this.uxMenuStripColumns.Size = new System.Drawing.Size(67, 4);
            // 
            // ListViewSecrets
            // 
            this.AllowColumnReorder = true;
            this.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FullRowSelect = true;
            listViewGroup6.Header = "";
            listViewGroup6.HeaderAlignment = System.Windows.Forms.HorizontalAlignment.Center;
            listViewGroup6.Name = "Search Results";
            listViewGroup7.Header = "";
            listViewGroup7.HeaderAlignment = System.Windows.Forms.HorizontalAlignment.Center;
            listViewGroup7.Name = "Favorites";
            listViewGroup8.Header = "";
            listViewGroup8.HeaderAlignment = System.Windows.Forms.HorizontalAlignment.Center;
            listViewGroup8.Name = "Certificates";
            listViewGroup9.Header = "";
            listViewGroup9.HeaderAlignment = System.Windows.Forms.HorizontalAlignment.Center;
            listViewGroup9.Name = "Key Vault Certificates";
            listViewGroup10.Header = "";
            listViewGroup10.HeaderAlignment = System.Windows.Forms.HorizontalAlignment.Center;
            listViewGroup10.Name = "Secrets";
            listViewGroup10.Tag = "";
            this.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup6,
            listViewGroup7,
            listViewGroup8,
            listViewGroup9,
            listViewGroup10});
            this.HideSelection = false;
            this.ShowItemToolTips = true;
            this.SmallImageList = this.uxSmallImageList;
            this.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.UseCompatibleStateImageBehavior = false;
            this.View = System.Windows.Forms.View.Details;
            this.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.ListViewSecrets_ColumnClick);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ImageList uxSmallImageList;
        private System.Windows.Forms.ContextMenuStrip uxMenuStripColumns;
    }
}
