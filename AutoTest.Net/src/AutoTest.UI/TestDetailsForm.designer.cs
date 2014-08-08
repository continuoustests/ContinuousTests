namespace AutoTest.UI
{
    partial class TestDetailsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TestDetailsForm));
            this.textBoxContent = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBoxLine = new System.Windows.Forms.PictureBox();
            this.textBoxFocusHolder = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLine)).BeginInit();
            this.SuspendLayout();
            // 
            // textBoxContent
            // 
            this.textBoxContent.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.textBoxContent.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxContent.Location = new System.Drawing.Point(28, 4);
            this.textBoxContent.Multiline = true;
            this.textBoxContent.Name = "textBoxContent";
            this.textBoxContent.ReadOnly = true;
            this.textBoxContent.Size = new System.Drawing.Size(504, 184);
            this.textBoxContent.TabIndex = 0;
            this.textBoxContent.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxContent_KeyDown);
            this.textBoxContent.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBoxContent_KeyUp);
            this.textBoxContent.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.textBoxContent_MouseDoubleClick);
            this.textBoxContent.MouseDown += new System.Windows.Forms.MouseEventHandler(this.textBoxContent_MouseDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, -4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "label1";
            this.label1.Visible = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.Control;
            this.pictureBox1.Image = global::AutoTest.UI.Properties.Resources._1305752593_move;
            this.pictureBox1.Location = new System.Drawing.Point(6, 4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(18, 13);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBoxLine
            // 
            this.pictureBoxLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(131)))), ((int)(((byte)(160)))), ((int)(((byte)(181)))));
            this.pictureBoxLine.Location = new System.Drawing.Point(25, 4);
            this.pictureBoxLine.Name = "pictureBoxLine";
            this.pictureBoxLine.Size = new System.Drawing.Size(4, 29);
            this.pictureBoxLine.TabIndex = 3;
            this.pictureBoxLine.TabStop = false;
            // 
            // textBoxFocusHolder
            // 
            this.textBoxFocusHolder.Location = new System.Drawing.Point(6, 185);
            this.textBoxFocusHolder.Name = "textBoxFocusHolder";
            this.textBoxFocusHolder.Size = new System.Drawing.Size(16, 20);
            this.textBoxFocusHolder.TabIndex = 4;
            this.textBoxFocusHolder.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxFocusHolder_KeyDown);
            this.textBoxFocusHolder.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBoxFocusHolder_KeyUp);
            // 
            // TestDetailsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(544, 208);
            this.Controls.Add(this.textBoxFocusHolder);
            this.Controls.Add(this.pictureBoxLine);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.textBoxContent);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TestDetailsForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TestDetailsForm2_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLine)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxContent;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBoxLine;
        private System.Windows.Forms.TextBox textBoxFocusHolder;
    }
}