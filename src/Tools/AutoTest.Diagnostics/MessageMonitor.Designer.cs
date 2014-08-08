namespace AutoTest.Diagnostics
{
    partial class MessageMonitor
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
            this.listView = new System.Windows.Forms.ListView();
            this.columnHeaderType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.listBoxNodes = new System.Windows.Forms.ListBox();
            this.columnHeaderTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // listView
            // 
            this.listView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderTime,
            this.columnHeaderType});
            this.listView.FullRowSelect = true;
            this.listView.Location = new System.Drawing.Point(248, 6);
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(705, 475);
            this.listView.TabIndex = 0;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            this.listView.DoubleClick += new System.EventHandler(this.listView_DoubleClick);
            // 
            // columnHeaderType
            // 
            this.columnHeaderType.Text = "Message Type";
            this.columnHeaderType.Width = 508;
            // 
            // listBoxNodes
            // 
            this.listBoxNodes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.listBoxNodes.FormattingEnabled = true;
            this.listBoxNodes.Location = new System.Drawing.Point(2, 6);
            this.listBoxNodes.Name = "listBoxNodes";
            this.listBoxNodes.Size = new System.Drawing.Size(240, 472);
            this.listBoxNodes.TabIndex = 1;
            this.listBoxNodes.SelectedIndexChanged += new System.EventHandler(this.listBoxNodes_SelectedIndexChanged);
            // 
            // columnHeaderTime
            // 
            this.columnHeaderTime.Text = "When";
            this.columnHeaderTime.Width = 163;
            // 
            // MessageMonitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(954, 484);
            this.Controls.Add(this.listBoxNodes);
            this.Controls.Add(this.listView);
            this.Name = "MessageMonitor";
            this.Text = "Message monitor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MessageMonitor_FormClosing);
            this.Shown += new System.EventHandler(this.MessageMonitor_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listView;
        private System.Windows.Forms.ColumnHeader columnHeaderType;
        private System.Windows.Forms.ListBox listBoxNodes;
        private System.Windows.Forms.ColumnHeader columnHeaderTime;
    }
}

