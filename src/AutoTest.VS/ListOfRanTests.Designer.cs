namespace AutoTest.VS
{
    partial class ContinuousTests_ListOfRanTests
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
            this.treeViewLastRun = new System.Windows.Forms.TreeView();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.radioButtonExpanded = new System.Windows.Forms.RadioButton();
            this.radioButtonCollapsed = new System.Windows.Forms.RadioButton();
            this.labelTestCount = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // treeViewLastRun
            // 
            this.treeViewLastRun.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.treeViewLastRun.ImageIndex = 0;
            this.treeViewLastRun.ImageList = this.imageList;
            this.treeViewLastRun.Location = new System.Drawing.Point(3, 23);
            this.treeViewLastRun.Name = "treeViewLastRun";
            this.treeViewLastRun.SelectedImageIndex = 0;
            this.treeViewLastRun.Size = new System.Drawing.Size(324, 334);
            this.treeViewLastRun.TabIndex = 0;
            this.treeViewLastRun.DoubleClick += new System.EventHandler(this.treeViewLastRun_DoubleClick);
            this.treeViewLastRun.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeViewLastRun_KeyDown);
            // 
            // imageList
            // 
            this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth24Bit;
            this.imageList.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // radioButtonExpanded
            // 
            this.radioButtonExpanded.AutoSize = true;
            this.radioButtonExpanded.Checked = true;
            this.radioButtonExpanded.Location = new System.Drawing.Point(4, 4);
            this.radioButtonExpanded.Name = "radioButtonExpanded";
            this.radioButtonExpanded.Size = new System.Drawing.Size(73, 17);
            this.radioButtonExpanded.TabIndex = 1;
            this.radioButtonExpanded.Text = "Expanded";
            this.radioButtonExpanded.UseVisualStyleBackColor = true;
            this.radioButtonExpanded.CheckedChanged += new System.EventHandler(this.radioButtonExpanded_CheckedChanged);
            // 
            // radioButtonCollapsed
            // 
            this.radioButtonCollapsed.AutoSize = true;
            this.radioButtonCollapsed.Location = new System.Drawing.Point(83, 4);
            this.radioButtonCollapsed.Name = "radioButtonCollapsed";
            this.radioButtonCollapsed.Size = new System.Drawing.Size(71, 17);
            this.radioButtonCollapsed.TabIndex = 2;
            this.radioButtonCollapsed.Text = "Collapsed";
            this.radioButtonCollapsed.UseVisualStyleBackColor = true;
            this.radioButtonCollapsed.CheckedChanged += new System.EventHandler(this.radioButtonCollapsed_CheckedChanged);
            // 
            // labelTestCount
            // 
            this.labelTestCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTestCount.Location = new System.Drawing.Point(227, 6);
            this.labelTestCount.Name = "labelTestCount";
            this.labelTestCount.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.labelTestCount.Size = new System.Drawing.Size(100, 15);
            this.labelTestCount.TabIndex = 3;
            this.labelTestCount.Text = "0";
            // 
            // ContinuousTests_ListOfRanTests
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.labelTestCount);
            this.Controls.Add(this.radioButtonCollapsed);
            this.Controls.Add(this.radioButtonExpanded);
            this.Controls.Add(this.treeViewLastRun);
            this.Name = "ContinuousTests_ListOfRanTests";
            this.Size = new System.Drawing.Size(330, 360);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treeViewLastRun;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.RadioButton radioButtonExpanded;
        private System.Windows.Forms.RadioButton radioButtonCollapsed;
        private System.Windows.Forms.Label labelTestCount;
    }
}
