namespace AutoTest.Client.UI
{
    partial class ConfigOverrideControl
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
            this.radioButtonOverride = new System.Windows.Forms.RadioButton();
            this.radioButtonExclude = new System.Windows.Forms.RadioButton();
            this.radioButtonMerge = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // radioButtonOverride
            // 
            this.radioButtonOverride.AutoSize = true;
            this.radioButtonOverride.Checked = true;
            this.radioButtonOverride.Location = new System.Drawing.Point(2, 3);
            this.radioButtonOverride.Name = "radioButtonOverride";
            this.radioButtonOverride.Size = new System.Drawing.Size(65, 17);
            this.radioButtonOverride.TabIndex = 16;
            this.radioButtonOverride.TabStop = true;
            this.radioButtonOverride.Text = "Override";
            this.radioButtonOverride.UseVisualStyleBackColor = true;
            this.radioButtonOverride.CheckedChanged += new System.EventHandler(this.radioButtonOverride_CheckedChanged);
            // 
            // radioButtonExclude
            // 
            this.radioButtonExclude.AutoSize = true;
            this.radioButtonExclude.Location = new System.Drawing.Point(132, 3);
            this.radioButtonExclude.Name = "radioButtonExclude";
            this.radioButtonExclude.Size = new System.Drawing.Size(63, 17);
            this.radioButtonExclude.TabIndex = 14;
            this.radioButtonExclude.Text = "Exclude";
            this.radioButtonExclude.UseVisualStyleBackColor = true;
            this.radioButtonExclude.CheckedChanged += new System.EventHandler(this.radioButtonExclude_CheckedChanged);
            // 
            // radioButtonMerge
            // 
            this.radioButtonMerge.AutoSize = true;
            this.radioButtonMerge.Location = new System.Drawing.Point(72, 3);
            this.radioButtonMerge.Name = "radioButtonMerge";
            this.radioButtonMerge.Size = new System.Drawing.Size(55, 17);
            this.radioButtonMerge.TabIndex = 15;
            this.radioButtonMerge.Text = "Merge";
            this.radioButtonMerge.UseVisualStyleBackColor = true;
            this.radioButtonMerge.CheckedChanged += new System.EventHandler(this.radioButtonMerge_CheckedChanged);
            // 
            // ConfigOverrideControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.radioButtonOverride);
            this.Controls.Add(this.radioButtonExclude);
            this.Controls.Add(this.radioButtonMerge);
            this.Name = "ConfigOverrideControl";
            this.Size = new System.Drawing.Size(196, 23);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton radioButtonOverride;
        private System.Windows.Forms.RadioButton radioButtonExclude;
        private System.Windows.Forms.RadioButton radioButtonMerge;
    }
}
