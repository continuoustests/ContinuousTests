namespace AutoTest.Client.UI
{
    partial class ConfigurationForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigurationForm));
            this.tabControlMM = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.checkBoxAnalytics = new System.Windows.Forms.CheckBox();
            this.textBoxParallelMSBuild = new System.Windows.Forms.TextBox();
            this.lblMSBuildParallelCount = new System.Windows.Forms.Label();
            this.checkBoxDisableAll = new System.Windows.Forms.CheckBox();
            this.checkBoxRealtimeFeedback = new System.Windows.Forms.CheckBox();
            this.comboBoxBuildSetup = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.radioButtonManual = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.radioButtonAuto = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.radioButtonMighty = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.textBoxIgnoreFilePatterns = new System.Windows.Forms.TextBox();
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.textBoxIgnoreFile = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.checkBoxOverlayNotififations = new System.Windows.Forms.CheckBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.checkBoxIgnored = new System.Windows.Forms.CheckBox();
            this.checkBoxFailing = new System.Windows.Forms.CheckBox();
            this.checkBoxWarnings = new System.Windows.Forms.CheckBox();
            this.checkBoxBuildErrors = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.checkBoxMinimizerLogging = new System.Windows.Forms.CheckBox();
            this.checkBoxStartPaused = new System.Windows.Forms.CheckBox();
            this.comboBoxGraphProvider = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.checkBoxNotifyOnRunFinished = new System.Windows.Forms.CheckBox();
            this.checkBoxNotifyOnRunStart = new System.Windows.Forms.CheckBox();
            this.textBoxGrowlPath = new System.Windows.Forms.TextBox();
            this.labelGrowl = new System.Windows.Forms.Label();
            this.checkBoxDebug = new System.Windows.Forms.CheckBox();
            this.textBoxCustomOutput = new System.Windows.Forms.TextBox();
            this.labelOutputFolder = new System.Windows.Forms.Label();
            this.buttonIgnoreCategory = new System.Windows.Forms.Button();
            this.textBoxIgnoreCategory = new System.Windows.Forms.TextBox();
            this.listViewIgnoreCategory = new System.Windows.Forms.ListView();
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.labelIgnoreCategories = new System.Windows.Forms.Label();
            this.buttonAddIgnoreAssembly = new System.Windows.Forms.Button();
            this.textBoxIgnoreAssembly = new System.Windows.Forms.TextBox();
            this.listViewIgnoreAssembly = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.labelIgnoreAssembly = new System.Windows.Forms.Label();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.checkBoxUseMargins = new System.Windows.Forms.CheckBox();
            this.checkBoxCompatibilityMode = new System.Windows.Forms.CheckBox();
            this.checkBoxRunAssembliesInParallel = new System.Windows.Forms.CheckBox();
            this.comboBoxMinimizer = new System.Windows.Forms.ComboBox();
            this.label16 = new System.Windows.Forms.Label();
            this.buttonAddProfilerNamespace = new System.Windows.Forms.Button();
            this.textBoxProfilerNamespace = new System.Windows.Forms.TextBox();
            this.listViewProfilerNamespaces = new System.Windows.Forms.ListView();
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label14 = new System.Windows.Forms.Label();
            this.comboBoxProfiler = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.buttonMinAsmBrowse = new System.Windows.Forms.Button();
            this.buttonMAsmBrowse = new System.Windows.Forms.Button();
            this.textBoxMinimizerAssembly = new System.Windows.Forms.TextBox();
            this.listViewMinimizerAssemblies = new System.Windows.Forms.ListView();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label7 = new System.Windows.Forms.Label();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.linkLabelOpenConfig = new System.Windows.Forms.LinkLabel();
            this.label12 = new System.Windows.Forms.Label();
            this.labelGlobalConfig = new System.Windows.Forms.Label();
            this.tabControlMM.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControlMM
            // 
            this.tabControlMM.Controls.Add(this.tabPage1);
            this.tabControlMM.Controls.Add(this.tabPage2);
            this.tabControlMM.Controls.Add(this.tabPage3);
            this.tabControlMM.Controls.Add(this.tabPage4);
            this.tabControlMM.Location = new System.Drawing.Point(12, 12);
            this.tabControlMM.Name = "tabControlMM";
            this.tabControlMM.SelectedIndex = 0;
            this.tabControlMM.Size = new System.Drawing.Size(659, 371);
            this.tabControlMM.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.checkBoxAnalytics);
            this.tabPage1.Controls.Add(this.textBoxParallelMSBuild);
            this.tabPage1.Controls.Add(this.lblMSBuildParallelCount);
            this.tabPage1.Controls.Add(this.checkBoxDisableAll);
            this.tabPage1.Controls.Add(this.checkBoxRealtimeFeedback);
            this.tabPage1.Controls.Add(this.comboBoxBuildSetup);
            this.tabPage1.Controls.Add(this.label6);
            this.tabPage1.Controls.Add(this.radioButtonManual);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.radioButtonAuto);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.radioButtonMighty);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.label15);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(651, 345);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Setup";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // checkBoxAnalytics
            // 
            this.checkBoxAnalytics.AutoSize = true;
            this.checkBoxAnalytics.Location = new System.Drawing.Point(271, 322);
            this.checkBoxAnalytics.Name = "checkBoxAnalytics";
            this.checkBoxAnalytics.Size = new System.Drawing.Size(374, 17);
            this.checkBoxAnalytics.TabIndex = 13;
            this.checkBoxAnalytics.Text = "Help improve Continuous Tests by providing us with anonymous feedback";
            this.checkBoxAnalytics.UseVisualStyleBackColor = true;
            // 
            // textBoxParallelMSBuild
            // 
            this.textBoxParallelMSBuild.Location = new System.Drawing.Point(502, 282);
            this.textBoxParallelMSBuild.Name = "textBoxParallelMSBuild";
            this.textBoxParallelMSBuild.Size = new System.Drawing.Size(18, 20);
            this.textBoxParallelMSBuild.TabIndex = 12;
            this.textBoxParallelMSBuild.Text = "1";
            this.textBoxParallelMSBuild.TextChanged += new System.EventHandler(this.textBoxParallelMSBuild_TextChanged);
            // 
            // lblMSBuildParallelCount
            // 
            this.lblMSBuildParallelCount.AutoSize = true;
            this.lblMSBuildParallelCount.Location = new System.Drawing.Point(331, 285);
            this.lblMSBuildParallelCount.Name = "lblMSBuildParallelCount";
            this.lblMSBuildParallelCount.Size = new System.Drawing.Size(168, 13);
            this.lblMSBuildParallelCount.TabIndex = 11;
            this.lblMSBuildParallelCount.Text = "Number Of Parallel MSBuild Builds";
            // 
            // checkBoxDisableAll
            // 
            this.checkBoxDisableAll.AutoSize = true;
            this.checkBoxDisableAll.Location = new System.Drawing.Point(10, 322);
            this.checkBoxDisableAll.Name = "checkBoxDisableAll";
            this.checkBoxDisableAll.Size = new System.Drawing.Size(215, 17);
            this.checkBoxDisableAll.TabIndex = 10;
            this.checkBoxDisableAll.Text = "Disable ContinuousTests for all solutions";
            this.checkBoxDisableAll.UseVisualStyleBackColor = true;
            // 
            // checkBoxRealtimeFeedback
            // 
            this.checkBoxRealtimeFeedback.AutoSize = true;
            this.checkBoxRealtimeFeedback.Location = new System.Drawing.Point(122, 107);
            this.checkBoxRealtimeFeedback.Name = "checkBoxRealtimeFeedback";
            this.checkBoxRealtimeFeedback.Size = new System.Drawing.Size(307, 17);
            this.checkBoxRealtimeFeedback.TabIndex = 9;
            this.checkBoxRealtimeFeedback.Text = "Enable Realtime Feedack For Unsaved Changes At Startup";
            this.checkBoxRealtimeFeedback.UseVisualStyleBackColor = true;
            this.checkBoxRealtimeFeedback.CheckedChanged += new System.EventHandler(this.checkBoxRealtimeFeedback_CheckedChanged);
            // 
            // comboBoxBuildSetup
            // 
            this.comboBoxBuildSetup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxBuildSetup.FormattingEnabled = true;
            this.comboBoxBuildSetup.Items.AddRange(new object[] {
            "Build Solution",
            "Build Changed Projects"});
            this.comboBoxBuildSetup.Location = new System.Drawing.Point(76, 282);
            this.comboBoxBuildSetup.Name = "comboBoxBuildSetup";
            this.comboBoxBuildSetup.Size = new System.Drawing.Size(235, 21);
            this.comboBoxBuildSetup.TabIndex = 8;
            this.comboBoxBuildSetup.SelectedIndexChanged += new System.EventHandler(this.comboBoxBuildSetup_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(6, 211);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(638, 33);
            this.label6.TabIndex = 6;
            this.label6.Text = resources.GetString("label6.Text");
            // 
            // radioButtonManual
            // 
            this.radioButtonManual.AutoSize = true;
            this.radioButtonManual.Location = new System.Drawing.Point(25, 242);
            this.radioButtonManual.Name = "radioButtonManual";
            this.radioButtonManual.Size = new System.Drawing.Size(95, 17);
            this.radioButtonManual.TabIndex = 5;
            this.radioButtonManual.Text = "Manual-Moose";
            this.radioButtonManual.UseVisualStyleBackColor = true;
            this.radioButtonManual.CheckedChanged += new System.EventHandler(this.radioButtonManual_CheckedChanged);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(6, 146);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(638, 33);
            this.label3.TabIndex = 4;
            this.label3.Text = resources.GetString("label3.Text");
            // 
            // radioButtonAuto
            // 
            this.radioButtonAuto.AutoSize = true;
            this.radioButtonAuto.Location = new System.Drawing.Point(25, 177);
            this.radioButtonAuto.Name = "radioButtonAuto";
            this.radioButtonAuto.Size = new System.Drawing.Size(82, 17);
            this.radioButtonAuto.TabIndex = 3;
            this.radioButtonAuto.Text = "Auto-Moose";
            this.radioButtonAuto.UseVisualStyleBackColor = true;
            this.radioButtonAuto.CheckedChanged += new System.EventHandler(this.radioButtonAuto_CheckedChanged);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(6, 76);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(638, 33);
            this.label2.TabIndex = 2;
            this.label2.Text = resources.GetString("label2.Text");
            // 
            // radioButtonMighty
            // 
            this.radioButtonMighty.AutoSize = true;
            this.radioButtonMighty.Checked = true;
            this.radioButtonMighty.Location = new System.Drawing.Point(25, 106);
            this.radioButtonMighty.Name = "radioButtonMighty";
            this.radioButtonMighty.Size = new System.Drawing.Size(91, 17);
            this.radioButtonMighty.TabIndex = 1;
            this.radioButtonMighty.TabStop = true;
            this.radioButtonMighty.Text = "Mighty-Moose";
            this.radioButtonMighty.UseVisualStyleBackColor = true;
            this.radioButtonMighty.CheckedChanged += new System.EventHandler(this.radioButtonMighty_CheckedChanged);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(7, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(638, 55);
            this.label1.TabIndex = 0;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(9, 285);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(61, 13);
            this.label15.TabIndex = 7;
            this.label15.Text = "Build Setup";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.textBoxIgnoreFilePatterns);
            this.tabPage2.Controls.Add(this.buttonBrowse);
            this.tabPage2.Controls.Add(this.textBoxIgnoreFile);
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(651, 345);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Ignore patterns";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // textBoxIgnoreFilePatterns
            // 
            this.textBoxIgnoreFilePatterns.Location = new System.Drawing.Point(10, 92);
            this.textBoxIgnoreFilePatterns.Multiline = true;
            this.textBoxIgnoreFilePatterns.Name = "textBoxIgnoreFilePatterns";
            this.textBoxIgnoreFilePatterns.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxIgnoreFilePatterns.Size = new System.Drawing.Size(598, 244);
            this.textBoxIgnoreFilePatterns.TabIndex = 3;
            this.textBoxIgnoreFilePatterns.TextChanged += new System.EventHandler(this.textBoxIgnoreFilePatterns_TextChanged);
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.Location = new System.Drawing.Point(612, 65);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(31, 23);
            this.buttonBrowse.TabIndex = 2;
            this.buttonBrowse.Text = "...";
            this.buttonBrowse.UseVisualStyleBackColor = true;
            this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
            // 
            // textBoxIgnoreFile
            // 
            this.textBoxIgnoreFile.Location = new System.Drawing.Point(10, 67);
            this.textBoxIgnoreFile.Name = "textBoxIgnoreFile";
            this.textBoxIgnoreFile.Size = new System.Drawing.Size(598, 20);
            this.textBoxIgnoreFile.TabIndex = 1;
            this.textBoxIgnoreFile.TextChanged += new System.EventHandler(this.textBoxIgnoreFile_TextChanged);
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(7, 7);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(638, 53);
            this.label4.TabIndex = 0;
            this.label4.Text = resources.GetString("label4.Text");
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.checkBoxOverlayNotififations);
            this.tabPage3.Controls.Add(this.label13);
            this.tabPage3.Controls.Add(this.label11);
            this.tabPage3.Controls.Add(this.checkBoxIgnored);
            this.tabPage3.Controls.Add(this.checkBoxFailing);
            this.tabPage3.Controls.Add(this.checkBoxWarnings);
            this.tabPage3.Controls.Add(this.checkBoxBuildErrors);
            this.tabPage3.Controls.Add(this.label10);
            this.tabPage3.Controls.Add(this.label9);
            this.tabPage3.Controls.Add(this.checkBoxMinimizerLogging);
            this.tabPage3.Controls.Add(this.checkBoxStartPaused);
            this.tabPage3.Controls.Add(this.comboBoxGraphProvider);
            this.tabPage3.Controls.Add(this.label5);
            this.tabPage3.Controls.Add(this.checkBoxNotifyOnRunFinished);
            this.tabPage3.Controls.Add(this.checkBoxNotifyOnRunStart);
            this.tabPage3.Controls.Add(this.textBoxGrowlPath);
            this.tabPage3.Controls.Add(this.labelGrowl);
            this.tabPage3.Controls.Add(this.checkBoxDebug);
            this.tabPage3.Controls.Add(this.textBoxCustomOutput);
            this.tabPage3.Controls.Add(this.labelOutputFolder);
            this.tabPage3.Controls.Add(this.buttonIgnoreCategory);
            this.tabPage3.Controls.Add(this.textBoxIgnoreCategory);
            this.tabPage3.Controls.Add(this.listViewIgnoreCategory);
            this.tabPage3.Controls.Add(this.labelIgnoreCategories);
            this.tabPage3.Controls.Add(this.buttonAddIgnoreAssembly);
            this.tabPage3.Controls.Add(this.textBoxIgnoreAssembly);
            this.tabPage3.Controls.Add(this.listViewIgnoreAssembly);
            this.tabPage3.Controls.Add(this.labelIgnoreAssembly);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(651, 345);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Various";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // checkBoxOverlayNotififations
            // 
            this.checkBoxOverlayNotififations.AutoSize = true;
            this.checkBoxOverlayNotififations.Location = new System.Drawing.Point(310, 251);
            this.checkBoxOverlayNotififations.Name = "checkBoxOverlayNotififations";
            this.checkBoxOverlayNotififations.Size = new System.Drawing.Size(121, 17);
            this.checkBoxOverlayNotififations.TabIndex = 75;
            this.checkBoxOverlayNotififations.Text = "Overlay notifications";
            this.checkBoxOverlayNotififations.UseVisualStyleBackColor = true;
            this.checkBoxOverlayNotififations.CheckedChanged += new System.EventHandler(this.checkBoxOverlayNotififations_CheckedChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(309, 190);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(130, 13);
            this.label13.TabIndex = 74;
            this.label13.Text = "Success/Fail Notifications";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(556, 190);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(71, 13);
            this.label11.TabIndex = 73;
            this.label11.Text = "Startup Mode";
            // 
            // checkBoxIgnored
            // 
            this.checkBoxIgnored.AutoSize = true;
            this.checkBoxIgnored.Checked = true;
            this.checkBoxIgnored.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxIgnored.Location = new System.Drawing.Point(450, 276);
            this.checkBoxIgnored.Name = "checkBoxIgnored";
            this.checkBoxIgnored.Size = new System.Drawing.Size(91, 17);
            this.checkBoxIgnored.TabIndex = 72;
            this.checkBoxIgnored.Text = "Ignored Tests";
            this.checkBoxIgnored.UseVisualStyleBackColor = true;
            this.checkBoxIgnored.CheckedChanged += new System.EventHandler(this.checkBoxIgnored_CheckedChanged);
            // 
            // checkBoxFailing
            // 
            this.checkBoxFailing.AutoSize = true;
            this.checkBoxFailing.Checked = true;
            this.checkBoxFailing.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxFailing.Location = new System.Drawing.Point(450, 253);
            this.checkBoxFailing.Name = "checkBoxFailing";
            this.checkBoxFailing.Size = new System.Drawing.Size(85, 17);
            this.checkBoxFailing.TabIndex = 71;
            this.checkBoxFailing.Text = "Failing Tests";
            this.checkBoxFailing.UseVisualStyleBackColor = true;
            this.checkBoxFailing.CheckedChanged += new System.EventHandler(this.checkBoxFailing_CheckedChanged);
            // 
            // checkBoxWarnings
            // 
            this.checkBoxWarnings.AutoSize = true;
            this.checkBoxWarnings.Checked = true;
            this.checkBoxWarnings.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxWarnings.Location = new System.Drawing.Point(450, 230);
            this.checkBoxWarnings.Name = "checkBoxWarnings";
            this.checkBoxWarnings.Size = new System.Drawing.Size(97, 17);
            this.checkBoxWarnings.TabIndex = 70;
            this.checkBoxWarnings.Text = "Build Warnings";
            this.checkBoxWarnings.UseVisualStyleBackColor = true;
            this.checkBoxWarnings.CheckedChanged += new System.EventHandler(this.checkBoxWarnings_CheckedChanged);
            // 
            // checkBoxBuildErrors
            // 
            this.checkBoxBuildErrors.AutoSize = true;
            this.checkBoxBuildErrors.Checked = true;
            this.checkBoxBuildErrors.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxBuildErrors.Location = new System.Drawing.Point(450, 208);
            this.checkBoxBuildErrors.Name = "checkBoxBuildErrors";
            this.checkBoxBuildErrors.Size = new System.Drawing.Size(79, 17);
            this.checkBoxBuildErrors.TabIndex = 69;
            this.checkBoxBuildErrors.Text = "Build Errors";
            this.checkBoxBuildErrors.UseVisualStyleBackColor = true;
            this.checkBoxBuildErrors.CheckedChanged += new System.EventHandler(this.checkBoxBuildErrors_CheckedChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(447, 190);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(34, 13);
            this.label10.TabIndex = 68;
            this.label10.Text = "Show";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(309, 279);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(45, 13);
            this.label9.TabIndex = 67;
            this.label9.Text = "Logging";
            // 
            // checkBoxMinimizerLogging
            // 
            this.checkBoxMinimizerLogging.AutoSize = true;
            this.checkBoxMinimizerLogging.Location = new System.Drawing.Point(310, 316);
            this.checkBoxMinimizerLogging.Name = "checkBoxMinimizerLogging";
            this.checkBoxMinimizerLogging.Size = new System.Drawing.Size(134, 17);
            this.checkBoxMinimizerLogging.TabIndex = 65;
            this.checkBoxMinimizerLogging.Text = "Test Minimizer Logging";
            this.checkBoxMinimizerLogging.UseVisualStyleBackColor = true;
            this.checkBoxMinimizerLogging.CheckedChanged += new System.EventHandler(this.checkBoxMinimizerLogging_CheckedChanged);
            // 
            // checkBoxStartPaused
            // 
            this.checkBoxStartPaused.AutoSize = true;
            this.checkBoxStartPaused.Location = new System.Drawing.Point(559, 208);
            this.checkBoxStartPaused.Name = "checkBoxStartPaused";
            this.checkBoxStartPaused.Size = new System.Drawing.Size(87, 17);
            this.checkBoxStartPaused.TabIndex = 64;
            this.checkBoxStartPaused.Text = "Start Paused";
            this.checkBoxStartPaused.UseVisualStyleBackColor = true;
            this.checkBoxStartPaused.CheckedChanged += new System.EventHandler(this.checkBoxStartPaused_CheckedChanged_1);
            // 
            // comboBoxGraphProvider
            // 
            this.comboBoxGraphProvider.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxGraphProvider.FormattingEnabled = true;
            this.comboBoxGraphProvider.Items.AddRange(new object[] {
            "Overlay (Dark)",
            "Overlay (Light)",
            "GraphViz",
            "Visual Studio (Requires Ultimate)",
            "Window"});
            this.comboBoxGraphProvider.Location = new System.Drawing.Point(9, 256);
            this.comboBoxGraphProvider.Name = "comboBoxGraphProvider";
            this.comboBoxGraphProvider.Size = new System.Drawing.Size(277, 21);
            this.comboBoxGraphProvider.TabIndex = 62;
            this.comboBoxGraphProvider.SelectedIndexChanged += new System.EventHandler(this.comboBoxGraphProvider_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 239);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(145, 13);
            this.label5.TabIndex = 61;
            this.label5.Text = "Provider for displaying graphs";
            // 
            // checkBoxNotifyOnRunFinished
            // 
            this.checkBoxNotifyOnRunFinished.AutoSize = true;
            this.checkBoxNotifyOnRunFinished.Checked = true;
            this.checkBoxNotifyOnRunFinished.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxNotifyOnRunFinished.Location = new System.Drawing.Point(310, 230);
            this.checkBoxNotifyOnRunFinished.Name = "checkBoxNotifyOnRunFinished";
            this.checkBoxNotifyOnRunFinished.Size = new System.Drawing.Size(125, 17);
            this.checkBoxNotifyOnRunFinished.TabIndex = 60;
            this.checkBoxNotifyOnRunFinished.Text = "Notify on run finished";
            this.checkBoxNotifyOnRunFinished.UseVisualStyleBackColor = true;
            this.checkBoxNotifyOnRunFinished.CheckedChanged += new System.EventHandler(this.checkBoxNotifyOnRunFinished_CheckedChanged);
            // 
            // checkBoxNotifyOnRunStart
            // 
            this.checkBoxNotifyOnRunStart.AutoSize = true;
            this.checkBoxNotifyOnRunStart.Checked = true;
            this.checkBoxNotifyOnRunStart.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxNotifyOnRunStart.Location = new System.Drawing.Point(310, 208);
            this.checkBoxNotifyOnRunStart.Name = "checkBoxNotifyOnRunStart";
            this.checkBoxNotifyOnRunStart.Size = new System.Drawing.Size(121, 17);
            this.checkBoxNotifyOnRunStart.TabIndex = 59;
            this.checkBoxNotifyOnRunStart.Text = "Notify on run started";
            this.checkBoxNotifyOnRunStart.UseVisualStyleBackColor = true;
            this.checkBoxNotifyOnRunStart.CheckedChanged += new System.EventHandler(this.checkBoxNotifyOnRunStart_CheckedChanged);
            // 
            // textBoxGrowlPath
            // 
            this.textBoxGrowlPath.Location = new System.Drawing.Point(6, 208);
            this.textBoxGrowlPath.Name = "textBoxGrowlPath";
            this.textBoxGrowlPath.Size = new System.Drawing.Size(280, 20);
            this.textBoxGrowlPath.TabIndex = 58;
            this.textBoxGrowlPath.TextChanged += new System.EventHandler(this.textBoxGrowlPath_TextChanged);
            // 
            // labelGrowl
            // 
            this.labelGrowl.AutoSize = true;
            this.labelGrowl.Location = new System.Drawing.Point(6, 190);
            this.labelGrowl.Name = "labelGrowl";
            this.labelGrowl.Size = new System.Drawing.Size(165, 13);
            this.labelGrowl.TabIndex = 57;
            this.labelGrowl.Text = "Growl notify path (growlnotify.exe)";
            // 
            // checkBoxDebug
            // 
            this.checkBoxDebug.AutoSize = true;
            this.checkBoxDebug.Location = new System.Drawing.Point(310, 295);
            this.checkBoxDebug.Name = "checkBoxDebug";
            this.checkBoxDebug.Size = new System.Drawing.Size(64, 17);
            this.checkBoxDebug.TabIndex = 56;
            this.checkBoxDebug.Text = "Logging";
            this.checkBoxDebug.UseVisualStyleBackColor = true;
            this.checkBoxDebug.CheckedChanged += new System.EventHandler(this.checkBoxDebug_CheckedChanged);
            // 
            // textBoxCustomOutput
            // 
            this.textBoxCustomOutput.Location = new System.Drawing.Point(6, 306);
            this.textBoxCustomOutput.Name = "textBoxCustomOutput";
            this.textBoxCustomOutput.Size = new System.Drawing.Size(280, 20);
            this.textBoxCustomOutput.TabIndex = 55;
            this.textBoxCustomOutput.TextChanged += new System.EventHandler(this.textBoxCustomOutput_TextChanged);
            // 
            // labelOutputFolder
            // 
            this.labelOutputFolder.AutoSize = true;
            this.labelOutputFolder.Location = new System.Drawing.Point(6, 288);
            this.labelOutputFolder.Name = "labelOutputFolder";
            this.labelOutputFolder.Size = new System.Drawing.Size(274, 13);
            this.labelOutputFolder.TabIndex = 54;
            this.labelOutputFolder.Text = "Custom build output folder (absolute of relative to project)";
            // 
            // buttonIgnoreCategory
            // 
            this.buttonIgnoreCategory.Location = new System.Drawing.Point(603, 21);
            this.buttonIgnoreCategory.Name = "buttonIgnoreCategory";
            this.buttonIgnoreCategory.Size = new System.Drawing.Size(45, 23);
            this.buttonIgnoreCategory.TabIndex = 53;
            this.buttonIgnoreCategory.Text = "Add";
            this.buttonIgnoreCategory.UseVisualStyleBackColor = true;
            this.buttonIgnoreCategory.Click += new System.EventHandler(this.buttonIgnoreCategory_Click);
            // 
            // textBoxIgnoreCategory
            // 
            this.textBoxIgnoreCategory.Location = new System.Drawing.Point(310, 23);
            this.textBoxIgnoreCategory.Name = "textBoxIgnoreCategory";
            this.textBoxIgnoreCategory.Size = new System.Drawing.Size(287, 20);
            this.textBoxIgnoreCategory.TabIndex = 52;
            // 
            // listViewIgnoreCategory
            // 
            this.listViewIgnoreCategory.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2});
            this.listViewIgnoreCategory.FullRowSelect = true;
            this.listViewIgnoreCategory.Location = new System.Drawing.Point(310, 50);
            this.listViewIgnoreCategory.Name = "listViewIgnoreCategory";
            this.listViewIgnoreCategory.Size = new System.Drawing.Size(338, 133);
            this.listViewIgnoreCategory.TabIndex = 51;
            this.listViewIgnoreCategory.UseCompatibleStateImageBehavior = false;
            this.listViewIgnoreCategory.View = System.Windows.Forms.View.Details;
            this.listViewIgnoreCategory.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listViewIgnoreCategory_KeyDown);
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Category to ignore";
            this.columnHeader2.Width = 312;
            // 
            // labelIgnoreCategories
            // 
            this.labelIgnoreCategories.AutoSize = true;
            this.labelIgnoreCategories.Location = new System.Drawing.Point(307, 7);
            this.labelIgnoreCategories.Name = "labelIgnoreCategories";
            this.labelIgnoreCategories.Size = new System.Drawing.Size(124, 13);
            this.labelIgnoreCategories.TabIndex = 50;
            this.labelIgnoreCategories.Text = "Test categories to ignore";
            // 
            // buttonAddIgnoreAssembly
            // 
            this.buttonAddIgnoreAssembly.Location = new System.Drawing.Point(246, 20);
            this.buttonAddIgnoreAssembly.Name = "buttonAddIgnoreAssembly";
            this.buttonAddIgnoreAssembly.Size = new System.Drawing.Size(45, 23);
            this.buttonAddIgnoreAssembly.TabIndex = 49;
            this.buttonAddIgnoreAssembly.Text = "Add";
            this.buttonAddIgnoreAssembly.UseVisualStyleBackColor = true;
            this.buttonAddIgnoreAssembly.Click += new System.EventHandler(this.buttonAddIgnoreAssembly_Click);
            // 
            // textBoxIgnoreAssembly
            // 
            this.textBoxIgnoreAssembly.Location = new System.Drawing.Point(6, 23);
            this.textBoxIgnoreAssembly.Name = "textBoxIgnoreAssembly";
            this.textBoxIgnoreAssembly.Size = new System.Drawing.Size(237, 20);
            this.textBoxIgnoreAssembly.TabIndex = 48;
            // 
            // listViewIgnoreAssembly
            // 
            this.listViewIgnoreAssembly.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.listViewIgnoreAssembly.FullRowSelect = true;
            this.listViewIgnoreAssembly.Location = new System.Drawing.Point(6, 49);
            this.listViewIgnoreAssembly.Name = "listViewIgnoreAssembly";
            this.listViewIgnoreAssembly.Size = new System.Drawing.Size(280, 133);
            this.listViewIgnoreAssembly.TabIndex = 47;
            this.listViewIgnoreAssembly.UseCompatibleStateImageBehavior = false;
            this.listViewIgnoreAssembly.View = System.Windows.Forms.View.Details;
            this.listViewIgnoreAssembly.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listViewIgnoreAssembly_KeyDown);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Assembly pattern to exclude";
            this.columnHeader1.Width = 249;
            // 
            // labelIgnoreAssembly
            // 
            this.labelIgnoreAssembly.AutoSize = true;
            this.labelIgnoreAssembly.Location = new System.Drawing.Point(3, 7);
            this.labelIgnoreAssembly.Name = "labelIgnoreAssembly";
            this.labelIgnoreAssembly.Size = new System.Drawing.Size(223, 13);
            this.labelIgnoreAssembly.TabIndex = 46;
            this.labelIgnoreAssembly.Text = "Wildcard patterns for test assemblies to ignore";
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.checkBoxUseMargins);
            this.tabPage4.Controls.Add(this.checkBoxCompatibilityMode);
            this.tabPage4.Controls.Add(this.checkBoxRunAssembliesInParallel);
            this.tabPage4.Controls.Add(this.comboBoxMinimizer);
            this.tabPage4.Controls.Add(this.label16);
            this.tabPage4.Controls.Add(this.buttonAddProfilerNamespace);
            this.tabPage4.Controls.Add(this.textBoxProfilerNamespace);
            this.tabPage4.Controls.Add(this.listViewProfilerNamespaces);
            this.tabPage4.Controls.Add(this.label14);
            this.tabPage4.Controls.Add(this.comboBoxProfiler);
            this.tabPage4.Controls.Add(this.label8);
            this.tabPage4.Controls.Add(this.buttonMinAsmBrowse);
            this.tabPage4.Controls.Add(this.buttonMAsmBrowse);
            this.tabPage4.Controls.Add(this.textBoxMinimizerAssembly);
            this.tabPage4.Controls.Add(this.listViewMinimizerAssemblies);
            this.tabPage4.Controls.Add(this.label7);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(651, 345);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Test Minimizer";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // checkBoxUseMargins
            // 
            this.checkBoxUseMargins.AutoSize = true;
            this.checkBoxUseMargins.Location = new System.Drawing.Point(355, 53);
            this.checkBoxUseMargins.Name = "checkBoxUseMargins";
            this.checkBoxUseMargins.Size = new System.Drawing.Size(198, 17);
            this.checkBoxUseMargins.TabIndex = 66;
            this.checkBoxUseMargins.Text = "Show risc information in code margin";
            this.checkBoxUseMargins.UseVisualStyleBackColor = true;
            this.checkBoxUseMargins.CheckedChanged += new System.EventHandler(this.checkBoxUseMargins_CheckedChanged);
            // 
            // checkBoxCompatibilityMode
            // 
            this.checkBoxCompatibilityMode.AutoSize = true;
            this.checkBoxCompatibilityMode.Location = new System.Drawing.Point(7, 76);
            this.checkBoxCompatibilityMode.Name = "checkBoxCompatibilityMode";
            this.checkBoxCompatibilityMode.Size = new System.Drawing.Size(336, 17);
            this.checkBoxCompatibilityMode.TabIndex = 65;
            this.checkBoxCompatibilityMode.Text = "Run tests in compatibility mode (Supresses unhandled exceptions)";
            this.checkBoxCompatibilityMode.UseVisualStyleBackColor = true;
            this.checkBoxCompatibilityMode.CheckedChanged += new System.EventHandler(this.checkBoxCompatibilityMode_CheckedChanged);
            // 
            // checkBoxRunAssembliesInParallel
            // 
            this.checkBoxRunAssembliesInParallel.AutoSize = true;
            this.checkBoxRunAssembliesInParallel.Location = new System.Drawing.Point(7, 53);
            this.checkBoxRunAssembliesInParallel.Name = "checkBoxRunAssembliesInParallel";
            this.checkBoxRunAssembliesInParallel.Size = new System.Drawing.Size(282, 17);
            this.checkBoxRunAssembliesInParallel.TabIndex = 64;
            this.checkBoxRunAssembliesInParallel.Text = "Run tests for each assembly in parallel with each other";
            this.checkBoxRunAssembliesInParallel.UseVisualStyleBackColor = true;
            this.checkBoxRunAssembliesInParallel.CheckedChanged += new System.EventHandler(this.checkBoxRunAssembliesInParallel_CheckedChanged);
            // 
            // comboBoxMinimizer
            // 
            this.comboBoxMinimizer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxMinimizer.FormattingEnabled = true;
            this.comboBoxMinimizer.Items.AddRange(new object[] {
            "Run only affected tests",
            "Run all tests in compiled assemblies",
            "No risk margins, no graphs, run all tests in compiled assemblies (low memory foot" +
                "print)"});
            this.comboBoxMinimizer.Location = new System.Drawing.Point(7, 22);
            this.comboBoxMinimizer.Name = "comboBoxMinimizer";
            this.comboBoxMinimizer.Size = new System.Drawing.Size(342, 21);
            this.comboBoxMinimizer.TabIndex = 63;
            this.comboBoxMinimizer.SelectedIndexChanged += new System.EventHandler(this.comboBoxMinimizer_SelectedIndexChanged);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(4, 6);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(114, 13);
            this.label16.TabIndex = 62;
            this.label16.Text = "Minimizer configuration";
            // 
            // buttonAddProfilerNamespace
            // 
            this.buttonAddProfilerNamespace.Location = new System.Drawing.Point(602, 238);
            this.buttonAddProfilerNamespace.Name = "buttonAddProfilerNamespace";
            this.buttonAddProfilerNamespace.Size = new System.Drawing.Size(45, 23);
            this.buttonAddProfilerNamespace.TabIndex = 61;
            this.buttonAddProfilerNamespace.Text = "Add";
            this.buttonAddProfilerNamespace.UseVisualStyleBackColor = true;
            this.buttonAddProfilerNamespace.Click += new System.EventHandler(this.buttonAddProfilerNamespace_Click);
            // 
            // textBoxProfilerNamespace
            // 
            this.textBoxProfilerNamespace.Location = new System.Drawing.Point(7, 241);
            this.textBoxProfilerNamespace.Name = "textBoxProfilerNamespace";
            this.textBoxProfilerNamespace.Size = new System.Drawing.Size(589, 20);
            this.textBoxProfilerNamespace.TabIndex = 60;
            // 
            // listViewProfilerNamespaces
            // 
            this.listViewProfilerNamespaces.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4});
            this.listViewProfilerNamespaces.FullRowSelect = true;
            this.listViewProfilerNamespaces.Location = new System.Drawing.Point(7, 266);
            this.listViewProfilerNamespaces.Name = "listViewProfilerNamespaces";
            this.listViewProfilerNamespaces.Size = new System.Drawing.Size(640, 74);
            this.listViewProfilerNamespaces.TabIndex = 59;
            this.listViewProfilerNamespaces.UseCompatibleStateImageBehavior = false;
            this.listViewProfilerNamespaces.View = System.Windows.Forms.View.Details;
            this.listViewProfilerNamespaces.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listViewProfilerNamespaces_KeyDown);
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Namespace";
            this.columnHeader4.Width = 617;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(4, 225);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(317, 13);
            this.label14.TabIndex = 58;
            this.label14.Text = "Namespaces that the profiler will scan in addition to project names";
            // 
            // comboBoxProfiler
            // 
            this.comboBoxProfiler.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxProfiler.FormattingEnabled = true;
            this.comboBoxProfiler.Items.AddRange(new object[] {
            "Run profiler",
            "Do not run profiler",
            "Run profiler and auto detect large profile runs (log larger than 1Gb)"});
            this.comboBoxProfiler.Location = new System.Drawing.Point(355, 22);
            this.comboBoxProfiler.Name = "comboBoxProfiler";
            this.comboBoxProfiler.Size = new System.Drawing.Size(290, 21);
            this.comboBoxProfiler.TabIndex = 57;
            this.comboBoxProfiler.SelectedIndexChanged += new System.EventHandler(this.comboBoxProfiler_SelectedIndexChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(352, 6);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(103, 13);
            this.label8.TabIndex = 56;
            this.label8.Text = "Profiler configuration";
            // 
            // buttonMinAsmBrowse
            // 
            this.buttonMinAsmBrowse.Location = new System.Drawing.Point(577, 114);
            this.buttonMinAsmBrowse.Name = "buttonMinAsmBrowse";
            this.buttonMinAsmBrowse.Size = new System.Drawing.Size(24, 23);
            this.buttonMinAsmBrowse.TabIndex = 54;
            this.buttonMinAsmBrowse.Text = "...";
            this.buttonMinAsmBrowse.UseVisualStyleBackColor = true;
            this.buttonMinAsmBrowse.Click += new System.EventHandler(this.buttonMinAsmBrowse_Click);
            // 
            // buttonMAsmBrowse
            // 
            this.buttonMAsmBrowse.Location = new System.Drawing.Point(601, 114);
            this.buttonMAsmBrowse.Name = "buttonMAsmBrowse";
            this.buttonMAsmBrowse.Size = new System.Drawing.Size(45, 23);
            this.buttonMAsmBrowse.TabIndex = 53;
            this.buttonMAsmBrowse.Text = "Add";
            this.buttonMAsmBrowse.UseVisualStyleBackColor = true;
            this.buttonMAsmBrowse.Click += new System.EventHandler(this.buttonMAsmBrowse_Click);
            // 
            // textBoxMinimizerAssembly
            // 
            this.textBoxMinimizerAssembly.Location = new System.Drawing.Point(6, 117);
            this.textBoxMinimizerAssembly.Name = "textBoxMinimizerAssembly";
            this.textBoxMinimizerAssembly.Size = new System.Drawing.Size(567, 20);
            this.textBoxMinimizerAssembly.TabIndex = 52;
            // 
            // listViewMinimizerAssemblies
            // 
            this.listViewMinimizerAssemblies.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3});
            this.listViewMinimizerAssemblies.FullRowSelect = true;
            this.listViewMinimizerAssemblies.Location = new System.Drawing.Point(6, 142);
            this.listViewMinimizerAssemblies.Name = "listViewMinimizerAssemblies";
            this.listViewMinimizerAssemblies.Size = new System.Drawing.Size(640, 76);
            this.listViewMinimizerAssemblies.TabIndex = 51;
            this.listViewMinimizerAssemblies.UseCompatibleStateImageBehavior = false;
            this.listViewMinimizerAssemblies.View = System.Windows.Forms.View.Details;
            this.listViewMinimizerAssemblies.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listViewMinimizerAssemblies_KeyDown);
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Assembly";
            this.columnHeader3.Width = 617;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 101);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(348, 13);
            this.label7.TabIndex = 50;
            this.label7.Text = "Assemblies that the test minimizer will scan in addition to solution projects";
            // 
            // btnUpdate
            // 
            this.btnUpdate.Location = new System.Drawing.Point(591, 392);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(75, 23);
            this.btnUpdate.TabIndex = 1;
            this.btnUpdate.Text = "Update";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // linkLabelOpenConfig
            // 
            this.linkLabelOpenConfig.AutoSize = true;
            this.linkLabelOpenConfig.Location = new System.Drawing.Point(9, 392);
            this.linkLabelOpenConfig.Name = "linkLabelOpenConfig";
            this.linkLabelOpenConfig.Size = new System.Drawing.Size(117, 13);
            this.linkLabelOpenConfig.TabIndex = 2;
            this.linkLabelOpenConfig.TabStop = true;
            this.linkLabelOpenConfig.Text = "Open Configuration File";
            this.linkLabelOpenConfig.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelOpenConfig_LinkClicked);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(307, 192);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(65, 13);
            this.label12.TabIndex = 66;
            this.label12.Text = "Notifications";
            // 
            // labelGlobalConfig
            // 
            this.labelGlobalConfig.BackColor = System.Drawing.SystemColors.Control;
            this.labelGlobalConfig.Location = new System.Drawing.Point(-2, 414);
            this.labelGlobalConfig.Name = "labelGlobalConfig";
            this.labelGlobalConfig.Size = new System.Drawing.Size(13, 13);
            this.labelGlobalConfig.TabIndex = 3;
            this.labelGlobalConfig.Click += new System.EventHandler(this.labelGlobalConfig_Click);
            // 
            // ConfigurationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(683, 427);
            this.Controls.Add(this.labelGlobalConfig);
            this.Controls.Add(this.linkLabelOpenConfig);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.tabControlMM);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ConfigurationForm";
            this.Text = "ConfigurationForm";
            this.tabControlMM.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControlMM;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton radioButtonAuto;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton radioButtonMighty;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TextBox textBoxIgnoreFilePatterns;
        private System.Windows.Forms.Button buttonBrowse;
        private System.Windows.Forms.TextBox textBoxIgnoreFile;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.CheckBox checkBoxNotifyOnRunFinished;
        private System.Windows.Forms.CheckBox checkBoxNotifyOnRunStart;
        private System.Windows.Forms.TextBox textBoxGrowlPath;
        private System.Windows.Forms.Label labelGrowl;
        private System.Windows.Forms.CheckBox checkBoxDebug;
        private System.Windows.Forms.TextBox textBoxCustomOutput;
        private System.Windows.Forms.Label labelOutputFolder;
        private System.Windows.Forms.Button buttonIgnoreCategory;
        private System.Windows.Forms.TextBox textBoxIgnoreCategory;
        private System.Windows.Forms.ListView listViewIgnoreCategory;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Label labelIgnoreCategories;
        private System.Windows.Forms.Button buttonAddIgnoreAssembly;
        private System.Windows.Forms.TextBox textBoxIgnoreAssembly;
        private System.Windows.Forms.ListView listViewIgnoreAssembly;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.Label labelIgnoreAssembly;
        private System.Windows.Forms.LinkLabel linkLabelOpenConfig;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox comboBoxGraphProvider;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.RadioButton radioButtonManual;
        private System.Windows.Forms.CheckBox checkBoxStartPaused;
        private System.Windows.Forms.CheckBox checkBoxMinimizerLogging;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Button buttonMAsmBrowse;
        private System.Windows.Forms.TextBox textBoxMinimizerAssembly;
        private System.Windows.Forms.ListView listViewMinimizerAssemblies;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button buttonMinAsmBrowse;
        private System.Windows.Forms.ComboBox comboBoxProfiler;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.CheckBox checkBoxIgnored;
        private System.Windows.Forms.CheckBox checkBoxFailing;
        private System.Windows.Forms.CheckBox checkBoxWarnings;
        private System.Windows.Forms.CheckBox checkBoxBuildErrors;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Button buttonAddProfilerNamespace;
        private System.Windows.Forms.TextBox textBoxProfilerNamespace;
        private System.Windows.Forms.ListView listViewProfilerNamespaces;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.ComboBox comboBoxBuildSetup;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.CheckBox checkBoxRealtimeFeedback;
        private System.Windows.Forms.ComboBox comboBoxMinimizer;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.CheckBox checkBoxOverlayNotififations;
        private System.Windows.Forms.CheckBox checkBoxRunAssembliesInParallel;
        private System.Windows.Forms.CheckBox checkBoxCompatibilityMode;
        private System.Windows.Forms.CheckBox checkBoxUseMargins;
        private System.Windows.Forms.Label labelGlobalConfig;
        private System.Windows.Forms.CheckBox checkBoxDisableAll;
        private System.Windows.Forms.TextBox textBoxParallelMSBuild;
        private System.Windows.Forms.Label lblMSBuildParallelCount;
        private System.Windows.Forms.CheckBox checkBoxAnalytics;
    }
}