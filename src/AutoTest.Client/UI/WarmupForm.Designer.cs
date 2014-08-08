namespace AutoTest.Client.UI
{
    partial class WarmupForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WarmupForm));
            this.pictureBoxFlex = new System.Windows.Forms.PictureBox();
            this.pictureBoxRelax = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.radioButtonManual = new System.Windows.Forms.RadioButton();
            this.radioButtonAuto = new System.Windows.Forms.RadioButton();
            this.radioButtonMighty = new System.Windows.Forms.RadioButton();
            this.checkBoxNeverShow = new System.Windows.Forms.CheckBox();
            this.checkBoxLowMemMode = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxFlex)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRelax)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBoxFlex
            // 
            this.pictureBoxFlex.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxFlex.Image = global::AutoTest.Client.Properties.Resources.BeefyMoose_Flex;
            this.pictureBoxFlex.Location = new System.Drawing.Point(12, 12);
            this.pictureBoxFlex.Name = "pictureBoxFlex";
            this.pictureBoxFlex.Size = new System.Drawing.Size(231, 189);
            this.pictureBoxFlex.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxFlex.TabIndex = 1;
            this.pictureBoxFlex.TabStop = false;
            // 
            // pictureBoxRelax
            // 
            this.pictureBoxRelax.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxRelax.Image = global::AutoTest.Client.Properties.Resources.BeefyMoose_Relax;
            this.pictureBoxRelax.Location = new System.Drawing.Point(12, 12);
            this.pictureBoxRelax.Name = "pictureBoxRelax";
            this.pictureBoxRelax.Size = new System.Drawing.Size(231, 189);
            this.pictureBoxRelax.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxRelax.TabIndex = 0;
            this.pictureBoxRelax.TabStop = false;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(261, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(380, 86);
            this.label1.TabIndex = 2;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOk.Location = new System.Drawing.Point(485, 178);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 3;
            this.buttonOk.Text = "&Ok";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(566, 178);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "&Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(261, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(295, 18);
            this.label2.TabIndex = 5;
            this.label2.Text = "Warmup solution for ContinuousTests";
            // 
            // radioButtonManual
            // 
            this.radioButtonManual.AutoSize = true;
            this.radioButtonManual.Location = new System.Drawing.Point(546, 131);
            this.radioButtonManual.Name = "radioButtonManual";
            this.radioButtonManual.Size = new System.Drawing.Size(95, 17);
            this.radioButtonManual.TabIndex = 8;
            this.radioButtonManual.Text = "No Automation";
            this.radioButtonManual.UseVisualStyleBackColor = true;
            // 
            // radioButtonAuto
            // 
            this.radioButtonAuto.AutoSize = true;
            this.radioButtonAuto.Location = new System.Drawing.Point(420, 131);
            this.radioButtonAuto.Name = "radioButtonAuto";
            this.radioButtonAuto.Size = new System.Drawing.Size(120, 17);
            this.radioButtonAuto.TabIndex = 7;
            this.radioButtonAuto.Text = "Automate Build Only";
            this.radioButtonAuto.UseVisualStyleBackColor = true;
            // 
            // radioButtonMighty
            // 
            this.radioButtonMighty.AutoSize = true;
            this.radioButtonMighty.Checked = true;
            this.radioButtonMighty.Location = new System.Drawing.Point(286, 131);
            this.radioButtonMighty.Name = "radioButtonMighty";
            this.radioButtonMighty.Size = new System.Drawing.Size(134, 17);
            this.radioButtonMighty.TabIndex = 6;
            this.radioButtonMighty.TabStop = true;
            this.radioButtonMighty.Text = "Automate Build + Tests";
            this.radioButtonMighty.UseVisualStyleBackColor = true;
            // 
            // checkBoxNeverShow
            // 
            this.checkBoxNeverShow.AutoSize = true;
            this.checkBoxNeverShow.Location = new System.Drawing.Point(264, 183);
            this.checkBoxNeverShow.Name = "checkBoxNeverShow";
            this.checkBoxNeverShow.Size = new System.Drawing.Size(185, 17);
            this.checkBoxNeverShow.TabIndex = 9;
            this.checkBoxNeverShow.Text = "Don\'t show this dialog ever again!";
            this.checkBoxNeverShow.UseVisualStyleBackColor = true;
            // 
            // checkBoxLowMemMode
            // 
            this.checkBoxLowMemMode.AutoSize = true;
            this.checkBoxLowMemMode.Location = new System.Drawing.Point(286, 155);
            this.checkBoxLowMemMode.Name = "checkBoxLowMemMode";
            this.checkBoxLowMemMode.Size = new System.Drawing.Size(329, 17);
            this.checkBoxLowMemMode.TabIndex = 10;
            this.checkBoxLowMemMode.Text = "Turn Off Test Minimizer, Graphs and Risc Margins (Low Memory)";
            this.checkBoxLowMemMode.UseVisualStyleBackColor = true;
            // 
            // WarmupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(653, 213);
            this.Controls.Add(this.checkBoxLowMemMode);
            this.Controls.Add(this.checkBoxNeverShow);
            this.Controls.Add(this.radioButtonManual);
            this.Controls.Add(this.radioButtonAuto);
            this.Controls.Add(this.radioButtonMighty);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBoxRelax);
            this.Controls.Add(this.pictureBoxFlex);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WarmupForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Warmup Solution";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxFlex)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRelax)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxRelax;
        private System.Windows.Forms.PictureBox pictureBoxFlex;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton radioButtonManual;
        private System.Windows.Forms.RadioButton radioButtonAuto;
        private System.Windows.Forms.RadioButton radioButtonMighty;
        private System.Windows.Forms.CheckBox checkBoxNeverShow;
        private System.Windows.Forms.CheckBox checkBoxLowMemMode;
    }
}