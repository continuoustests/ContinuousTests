namespace AutoTest.Client.UI
{
    partial class SystemMessageForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SystemMessageForm));
            this.informationList = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.linkLabelInfo = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // informationList
            // 
            this.informationList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.informationList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.informationList.Location = new System.Drawing.Point(12, 12);
            this.informationList.Name = "informationList";
            this.informationList.Size = new System.Drawing.Size(760, 328);
            this.informationList.TabIndex = 2;
            this.informationList.UseCompatibleStateImageBehavior = false;
            this.informationList.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Message";
            this.columnHeader1.Width = 723;
            // 
            // linkLabelInfo
            // 
            this.linkLabelInfo.AutoSize = true;
            this.linkLabelInfo.Location = new System.Drawing.Point(9, 343);
            this.linkLabelInfo.Name = "linkLabelInfo";
            this.linkLabelInfo.Size = new System.Drawing.Size(0, 13);
            this.linkLabelInfo.TabIndex = 3;
            // 
            // SystemMessageForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 362);
            this.Controls.Add(this.informationList);
            this.Controls.Add(this.linkLabelInfo);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "SystemMessageForm";
            this.Text = "ContinuousTests Internal Messages";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView informationList;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.LinkLabel linkLabelInfo;
    }
}