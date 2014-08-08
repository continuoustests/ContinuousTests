namespace AutoTest.UI
{
    partial class RunFeedback
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
            this.linkLabelCancelRun = new System.Windows.Forms.LinkLabel();
            this._toolTipProvider = new System.Windows.Forms.ToolTip(this.components);
            this.linkLabelErrorDescription = new System.Windows.Forms.LinkLabel();
            this.linkLabelTestDetails = new System.Windows.Forms.LinkLabel();
            this.linkLabelDebugTest = new System.Windows.Forms.LinkLabel();
            this.label1 = new System.Windows.Forms.Label();
            this.listViewFeedback = new System.Windows.Forms.ListView();
            this.columnType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnMessage = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.pictureBoxWorking = new System.Windows.Forms.PictureBox();
            this.pictureMoose = new System.Windows.Forms.PictureBox();
            this.pictureBoxRed = new System.Windows.Forms.PictureBox();
            this.pictureBoxGray = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWorking)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureMoose)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxGray)).BeginInit();
            this.SuspendLayout();
            // 
            // linkLabelCancelRun
            // 
            this.linkLabelCancelRun.AutoSize = true;
            this.linkLabelCancelRun.LinkColor = System.Drawing.Color.Blue;
            this.linkLabelCancelRun.Location = new System.Drawing.Point(302, 6);
            this.linkLabelCancelRun.Name = "linkLabelCancelRun";
            this.linkLabelCancelRun.Size = new System.Drawing.Size(71, 13);
            this.linkLabelCancelRun.TabIndex = 13;
            this.linkLabelCancelRun.TabStop = true;
            this.linkLabelCancelRun.Text = "Abort Run (A)";
            this.linkLabelCancelRun.VisitedLinkColor = System.Drawing.Color.Blue;
            this.linkLabelCancelRun.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelSystemMessages_LinkClicked);
            // 
            // linkLabelErrorDescription
            // 
            this.linkLabelErrorDescription.AutoSize = true;
            this.linkLabelErrorDescription.Location = new System.Drawing.Point(37, 6);
            this.linkLabelErrorDescription.Name = "linkLabelErrorDescription";
            this.linkLabelErrorDescription.Size = new System.Drawing.Size(119, 13);
            this.linkLabelErrorDescription.TabIndex = 12;
            this.linkLabelErrorDescription.TabStop = true;
            this.linkLabelErrorDescription.Text = "Extended Information (I)";
            this._toolTipProvider.SetToolTip(this.linkLabelErrorDescription, "Show detailed information about the test (I\r\n)");
            this.linkLabelErrorDescription.Visible = false;
            this.linkLabelErrorDescription.VisitedLinkColor = System.Drawing.Color.Blue;
            this.linkLabelErrorDescription.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelErrorDescription_LinkClicked);
            // 
            // linkLabelTestDetails
            // 
            this.linkLabelTestDetails.AutoSize = true;
            this.linkLabelTestDetails.Location = new System.Drawing.Point(223, 6);
            this.linkLabelTestDetails.Name = "linkLabelTestDetails";
            this.linkLabelTestDetails.Size = new System.Drawing.Size(73, 13);
            this.linkLabelTestDetails.TabIndex = 11;
            this.linkLabelTestDetails.TabStop = true;
            this.linkLabelTestDetails.Text = "Test output (I)";
            this._toolTipProvider.SetToolTip(this.linkLabelTestDetails, "Show detailed information about the test (I)");
            this.linkLabelTestDetails.Visible = false;
            this.linkLabelTestDetails.VisitedLinkColor = System.Drawing.Color.Blue;
            this.linkLabelTestDetails.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelTestDetails_LinkClicked);
            // 
            // linkLabelDebugTest
            // 
            this.linkLabelDebugTest.AutoSize = true;
            this.linkLabelDebugTest.Location = new System.Drawing.Point(162, 6);
            this.linkLabelDebugTest.Name = "linkLabelDebugTest";
            this.linkLabelDebugTest.Size = new System.Drawing.Size(56, 13);
            this.linkLabelDebugTest.TabIndex = 10;
            this.linkLabelDebugTest.TabStop = true;
            this.linkLabelDebugTest.Text = "Debug (D)";
            this._toolTipProvider.SetToolTip(this.linkLabelDebugTest, "Debug selected test (T)");
            this.linkLabelDebugTest.Visible = false;
            this.linkLabelDebugTest.VisitedLinkColor = System.Drawing.Color.Blue;
            this.linkLabelDebugTest.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelDebugTest_LinkClicked);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 13);
            this.label1.TabIndex = 8;
            // 
            // listViewFeedback
            // 
            this.listViewFeedback.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewFeedback.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnType,
            this.columnMessage});
            this.listViewFeedback.FullRowSelect = true;
            this.listViewFeedback.HideSelection = false;
            this.listViewFeedback.Location = new System.Drawing.Point(0, 25);
            this.listViewFeedback.Name = "listViewFeedback";
            this.listViewFeedback.Size = new System.Drawing.Size(413, 76);
            this.listViewFeedback.TabIndex = 9;
            this.listViewFeedback.UseCompatibleStateImageBehavior = false;
            this.listViewFeedback.View = System.Windows.Forms.View.Details;
            this.listViewFeedback.SelectedIndexChanged += new System.EventHandler(this.listViewFeedback_SelectedIndexChanged);
            this.listViewFeedback.DoubleClick += new System.EventHandler(this.listViewFeedback_DoubleClick);
            this.listViewFeedback.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listViewFeedback_KeyDown);
            this.listViewFeedback.Resize += new System.EventHandler(this.listViewFeedback_Resize);
            // 
            // columnType
            // 
            this.columnType.Text = "Type";
            this.columnType.Width = 70;
            // 
            // columnMessage
            // 
            this.columnMessage.Text = "Message";
            this.columnMessage.Width = 1017;
            // 
            // pictureBoxWorking
            // 
            this.pictureBoxWorking.ErrorImage = null;
            this.pictureBoxWorking.ImageLocation = "progress.gif";
            this.pictureBoxWorking.InitialImage = null;
            this.pictureBoxWorking.Location = new System.Drawing.Point(2, 2);
            this.pictureBoxWorking.Name = "pictureBoxWorking";
            this.pictureBoxWorking.Size = new System.Drawing.Size(20, 20);
            this.pictureBoxWorking.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxWorking.TabIndex = 15;
            this.pictureBoxWorking.TabStop = false;
            this.pictureBoxWorking.Visible = false;
            // 
            // pictureMoose
            // 
            this.pictureMoose.Image = global::AutoTest.UI.Properties.Resources.circleWIN;
            this.pictureMoose.Location = new System.Drawing.Point(2, 2);
            this.pictureMoose.Name = "pictureMoose";
            this.pictureMoose.Size = new System.Drawing.Size(20, 20);
            this.pictureMoose.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureMoose.TabIndex = 14;
            this.pictureMoose.TabStop = false;
            // 
            // pictureBoxRed
            // 
            this.pictureBoxRed.Image = global::AutoTest.UI.Properties.Resources.circleFAIL;
            this.pictureBoxRed.Location = new System.Drawing.Point(2, 2);
            this.pictureBoxRed.Name = "pictureBoxRed";
            this.pictureBoxRed.Size = new System.Drawing.Size(20, 20);
            this.pictureBoxRed.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxRed.TabIndex = 16;
            this.pictureBoxRed.TabStop = false;
            // 
            // pictureBoxGray
            // 
            this.pictureBoxGray.Image = global::AutoTest.UI.Properties.Resources.circleAbort;
            this.pictureBoxGray.Location = new System.Drawing.Point(2, 2);
            this.pictureBoxGray.Name = "pictureBoxGray";
            this.pictureBoxGray.Size = new System.Drawing.Size(20, 20);
            this.pictureBoxGray.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxGray.TabIndex = 17;
            this.pictureBoxGray.TabStop = false;
            // 
            // RunFeedback
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pictureBoxGray);
            this.Controls.Add(this.pictureBoxRed);
            this.Controls.Add(this.pictureBoxWorking);
            this.Controls.Add(this.pictureMoose);
            this.Controls.Add(this.linkLabelCancelRun);
            this.Controls.Add(this.linkLabelErrorDescription);
            this.Controls.Add(this.linkLabelTestDetails);
            this.Controls.Add(this.linkLabelDebugTest);
            this.Controls.Add(this.listViewFeedback);
            this.Controls.Add(this.label1);
            this.Name = "RunFeedback";
            this.Size = new System.Drawing.Size(413, 101);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWorking)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureMoose)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxGray)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.LinkLabel linkLabelCancelRun;
        public System.Windows.Forms.ToolTip _toolTipProvider;
        public System.Windows.Forms.LinkLabel linkLabelErrorDescription;
        public System.Windows.Forms.LinkLabel linkLabelTestDetails;
        public System.Windows.Forms.LinkLabel linkLabelDebugTest;
        public System.Windows.Forms.ListView listViewFeedback;
        public System.Windows.Forms.ColumnHeader columnType;
        public System.Windows.Forms.ColumnHeader columnMessage;
        public System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureMoose;
        private System.Windows.Forms.PictureBox pictureBoxWorking;
        private System.Windows.Forms.PictureBox pictureBoxRed;
        private System.Windows.Forms.PictureBox pictureBoxGray;
    }
}