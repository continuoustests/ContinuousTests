namespace AutoTest.Client.UI
{
    partial class NewReleaseForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewReleaseForm));
            this.labelTitle = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.linkLabelDownloadlink = new System.Windows.Forms.LinkLabel();
            this.button1 = new System.Windows.Forms.Button();
            this.comboBoxCheckFrequency = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // labelTitle
            // 
            this.labelTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTitle.Location = new System.Drawing.Point(12, 9);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(728, 28);
            this.labelTitle.TabIndex = 0;
            this.labelTitle.Text = "label";
            this.labelTitle.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(318, 40);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(422, 308);
            this.textBox1.TabIndex = 1;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::AutoTest.Client.Properties.Resources._7;
            this.pictureBox1.Location = new System.Drawing.Point(12, 40);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(300, 367);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // linkLabelDownloadlink
            // 
            this.linkLabelDownloadlink.AutoSize = true;
            this.linkLabelDownloadlink.Location = new System.Drawing.Point(318, 351);
            this.linkLabelDownloadlink.Name = "linkLabelDownloadlink";
            this.linkLabelDownloadlink.Size = new System.Drawing.Size(233, 13);
            this.linkLabelDownloadlink.TabIndex = 3;
            this.linkLabelDownloadlink.TabStop = true;
            this.linkLabelDownloadlink.Text = "Download new version of ContinuousTests here";
            this.linkLabelDownloadlink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelDownloadlink_LinkClicked);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 31);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // comboBoxCheckFrequency
            // 
            this.comboBoxCheckFrequency.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCheckFrequency.FormattingEnabled = true;
            this.comboBoxCheckFrequency.Items.AddRange(new object[] {
            "Check For New Release On Every Startup",
            "Ignore Until Next Release Is Out",
            "Don\'t Ever Check For New Releases Automatically"});
            this.comboBoxCheckFrequency.Location = new System.Drawing.Point(321, 385);
            this.comboBoxCheckFrequency.Name = "comboBoxCheckFrequency";
            this.comboBoxCheckFrequency.Size = new System.Drawing.Size(419, 21);
            this.comboBoxCheckFrequency.TabIndex = 5;
            this.comboBoxCheckFrequency.SelectedIndexChanged += new System.EventHandler(this.comboBoxCheckFrequency_SelectedIndexChanged);
            // 
            // NewReleaseForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(752, 419);
            this.Controls.Add(this.comboBoxCheckFrequency);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.linkLabelDownloadlink);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.labelTitle);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "NewReleaseForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "New Version of ContinuousTests Released";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.LinkLabel linkLabelDownloadlink;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ComboBox comboBoxCheckFrequency;
    }
}