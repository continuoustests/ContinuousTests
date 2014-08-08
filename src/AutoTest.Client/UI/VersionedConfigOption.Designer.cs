namespace AutoTest.Client.UI
{
    partial class VersionedConfigOption
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
            this.listViewItems = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.labelDescription = new System.Windows.Forms.Label();
            this.textBoxPath = new System.Windows.Forms.TextBox();
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.comboBoxVersions = new System.Windows.Forms.ComboBox();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.radioButtonExclude = new System.Windows.Forms.RadioButton();
            this.radioButtonMerge = new System.Windows.Forms.RadioButton();
            this.radioButtonOverride = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // listViewItems
            // 
            this.listViewItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewItems.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listViewItems.FullRowSelect = true;
            this.listViewItems.Location = new System.Drawing.Point(3, 49);
            this.listViewItems.Name = "listViewItems";
            this.listViewItems.Size = new System.Drawing.Size(597, 61);
            this.listViewItems.TabIndex = 3;
            this.listViewItems.UseCompatibleStateImageBehavior = false;
            this.listViewItems.View = System.Windows.Forms.View.Details;
            this.listViewItems.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listViewItems_KeyDown);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Executable";
            this.columnHeader1.Width = 442;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Version";
            this.columnHeader2.Width = 125;
            // 
            // labelDescription
            // 
            this.labelDescription.AutoSize = true;
            this.labelDescription.Location = new System.Drawing.Point(3, 5);
            this.labelDescription.Name = "labelDescription";
            this.labelDescription.Size = new System.Drawing.Size(210, 13);
            this.labelDescription.TabIndex = 2;
            this.labelDescription.Text = "Build executables (msbuild.exe / xbuild.bat)";
            // 
            // textBoxPath
            // 
            this.textBoxPath.Location = new System.Drawing.Point(3, 23);
            this.textBoxPath.Name = "textBoxPath";
            this.textBoxPath.Size = new System.Drawing.Size(351, 20);
            this.textBoxPath.TabIndex = 4;
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.Location = new System.Drawing.Point(358, 21);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(28, 23);
            this.buttonBrowse.TabIndex = 5;
            this.buttonBrowse.Text = "...";
            this.buttonBrowse.UseVisualStyleBackColor = true;
            this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
            // 
            // comboBoxVersions
            // 
            this.comboBoxVersions.FormattingEnabled = true;
            this.comboBoxVersions.Location = new System.Drawing.Point(393, 21);
            this.comboBoxVersions.Name = "comboBoxVersions";
            this.comboBoxVersions.Size = new System.Drawing.Size(155, 21);
            this.comboBoxVersions.TabIndex = 6;
            // 
            // buttonAdd
            // 
            this.buttonAdd.Location = new System.Drawing.Point(555, 21);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(45, 23);
            this.buttonAdd.TabIndex = 7;
            this.buttonAdd.Text = "Add";
            this.buttonAdd.UseVisualStyleBackColor = true;
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // radioButtonExclude
            // 
            this.radioButtonExclude.AutoSize = true;
            this.radioButtonExclude.Location = new System.Drawing.Point(538, 4);
            this.radioButtonExclude.Name = "radioButtonExclude";
            this.radioButtonExclude.Size = new System.Drawing.Size(63, 17);
            this.radioButtonExclude.TabIndex = 8;
            this.radioButtonExclude.Text = "Exclude";
            this.radioButtonExclude.UseVisualStyleBackColor = true;
            this.radioButtonExclude.CheckedChanged += new System.EventHandler(radioButtonExclude_CheckedChanged);
            // 
            // radioButtonMerge
            // 
            this.radioButtonMerge.AutoSize = true;
            this.radioButtonMerge.Location = new System.Drawing.Point(478, 4);
            this.radioButtonMerge.Name = "radioButtonMerge";
            this.radioButtonMerge.Size = new System.Drawing.Size(55, 17);
            this.radioButtonMerge.TabIndex = 9;
            this.radioButtonMerge.Text = "Merge";
            this.radioButtonMerge.UseVisualStyleBackColor = true;
            this.radioButtonMerge.CheckedChanged += new System.EventHandler(radioButtonExclude_CheckedChanged);
            // 
            // radioButtonOverride
            // 
            this.radioButtonOverride.AutoSize = true;
            this.radioButtonOverride.Checked = true;
            this.radioButtonOverride.Location = new System.Drawing.Point(408, 4);
            this.radioButtonOverride.Name = "radioButtonOverride";
            this.radioButtonOverride.Size = new System.Drawing.Size(65, 17);
            this.radioButtonOverride.TabIndex = 10;
            this.radioButtonOverride.TabStop = true;
            this.radioButtonOverride.Text = "Override";
            this.radioButtonOverride.UseVisualStyleBackColor = true;
            this.radioButtonOverride.CheckedChanged += new System.EventHandler(radioButtonExclude_CheckedChanged);
            // 
            // VersionedConfigOption
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.radioButtonOverride);
            this.Controls.Add(this.radioButtonExclude);
            this.Controls.Add(this.buttonAdd);
            this.Controls.Add(this.comboBoxVersions);
            this.Controls.Add(this.buttonBrowse);
            this.Controls.Add(this.textBoxPath);
            this.Controls.Add(this.listViewItems);
            this.Controls.Add(this.labelDescription);
            this.Controls.Add(this.radioButtonMerge);
            this.Name = "VersionedConfigOption";
            this.Size = new System.Drawing.Size(604, 114);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listViewItems;
        private System.Windows.Forms.Label labelDescription;
        private System.Windows.Forms.TextBox textBoxPath;
        private System.Windows.Forms.Button buttonBrowse;
        private System.Windows.Forms.ComboBox comboBoxVersions;
        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.RadioButton radioButtonExclude;
        private System.Windows.Forms.RadioButton radioButtonMerge;
        private System.Windows.Forms.RadioButton radioButtonOverride;
    }
}
