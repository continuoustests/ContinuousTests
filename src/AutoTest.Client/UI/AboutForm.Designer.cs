namespace AutoTest.Client.UI
{
    partial class AboutForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
            this.label1 = new System.Windows.Forms.Label();
            this.linkLabelRegister = new System.Windows.Forms.LinkLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.linkLabelWWW = new System.Windows.Forms.LinkLabel();
            this.linkLabelLicense = new System.Windows.Forms.LinkLabel();
            this.labelVersion = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(32, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(315, 42);
            this.label1.TabIndex = 0;
            this.label1.Text = "ContinuousTests";
            // 
            // linkLabelRegister
            // 
            this.linkLabelRegister.AutoSize = true;
            this.linkLabelRegister.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLabelRegister.Location = new System.Drawing.Point(141, 229);
            this.linkLabelRegister.Name = "linkLabelRegister";
            this.linkLabelRegister.Size = new System.Drawing.Size(100, 25);
            this.linkLabelRegister.TabIndex = 1;
            this.linkLabelRegister.TabStop = true;
            this.linkLabelRegister.Text = "Register";
            this.linkLabelRegister.VisitedLinkColor = System.Drawing.Color.Blue;
            this.linkLabelRegister.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelRegister_LinkClicked);
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(4, 89);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(375, 21);
            this.label2.TabIndex = 2;
            this.label2.Text = "Continuous Testing Tool For .Net and Mono";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(4, 115);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(375, 23);
            this.label3.TabIndex = 3;
            this.label3.Text = "Copyright © 2010-2011";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(4, 134);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(375, 22);
            this.label4.TabIndex = 4;
            this.label4.Text = "Greg Young, Svein Arne Ackenhausen";
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // linkLabelWWW
            // 
            this.linkLabelWWW.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLabelWWW.Location = new System.Drawing.Point(5, 178);
            this.linkLabelWWW.Name = "linkLabelWWW";
            this.linkLabelWWW.Size = new System.Drawing.Size(372, 23);
            this.linkLabelWWW.TabIndex = 5;
            this.linkLabelWWW.TabStop = true;
            this.linkLabelWWW.Text = "http://www.continuoustests.com";
            this.linkLabelWWW.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.linkLabelWWW.VisitedLinkColor = System.Drawing.Color.Blue;
            this.linkLabelWWW.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelWWW_LinkClicked);
            // 
            // linkLabelLicense
            // 
            this.linkLabelLicense.AutoSize = true;
            this.linkLabelLicense.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLabelLicense.Location = new System.Drawing.Point(4, 293);
            this.linkLabelLicense.Name = "linkLabelLicense";
            this.linkLabelLicense.Size = new System.Drawing.Size(55, 16);
            this.linkLabelLicense.TabIndex = 6;
            this.linkLabelLicense.TabStop = true;
            this.linkLabelLicense.Text = "License";
            this.linkLabelLicense.Visible = false;
            this.linkLabelLicense.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelLicense_LinkClicked);
            // 
            // labelVersion
            // 
            this.labelVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelVersion.Location = new System.Drawing.Point(35, 54);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(315, 24);
            this.labelVersion.TabIndex = 7;
            this.labelVersion.Text = "ContinuousTests";
            this.labelVersion.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.labelVersion.Click += new System.EventHandler(this.labelVersion_Click);
            // 
            // AboutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(384, 315);
            this.Controls.Add(this.labelVersion);
            this.Controls.Add(this.linkLabelLicense);
            this.Controls.Add(this.linkLabelWWW);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.linkLabelRegister);
            this.Controls.Add(this.label1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutForm";
            this.Text = "About ContinuousTests";
            this.Load += new System.EventHandler(this.AboutForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel linkLabelRegister;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.LinkLabel linkLabelWWW;
        private System.Windows.Forms.LinkLabel linkLabelLicense;
        private System.Windows.Forms.Label labelVersion;

    }
}