namespace ContinuousTests
{
    partial class RunFeedbackForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RunFeedbackForm));
            this.runFeedback1 = new AutoTest.UI.RunFeedback();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.continuousTestsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resumeEngineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pauseEngineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.buildAndTestAllToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemClearList = new System.Windows.Forms.ToolStripMenuItem();
            this.detectRecursiveBuildsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.configurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.globalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.localToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // runFeedback1
            // 
            this.runFeedback1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.runFeedback1.CanDebug = false;
            this.runFeedback1.CanGoToTypes = false;
            this.runFeedback1.ListViewWidthOffset = 0;
            this.runFeedback1.Location = new System.Drawing.Point(12, 27);
            this.runFeedback1.Name = "runFeedback1";
            this.runFeedback1.ShowIcon = true;
            this.runFeedback1.ShowRunInformation = true;
            this.runFeedback1.Size = new System.Drawing.Size(1125, 234);
            this.runFeedback1.TabIndex = 0;
            this.runFeedback1.GoToReference += new System.EventHandler<AutoTest.UI.GoToReferenceArgs>(this.runFeedback1_GoToReference);
            this.runFeedback1.CancelRun += new System.EventHandler(this.runFeedback1_CancelRun);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.continuousTestsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1150, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // continuousTestsToolStripMenuItem
            // 
            this.continuousTestsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.resumeEngineToolStripMenuItem,
            this.pauseEngineToolStripMenuItem,
            this.toolStripSeparator1,
            this.buildAndTestAllToolStripMenuItem1,
            this.toolStripMenuItemClearList,
            this.detectRecursiveBuildsToolStripMenuItem,
            this.toolStripMenuItem1,
            this.configurationToolStripMenuItem,
            this.toolStripMenuItem2,
            this.aboutToolStripMenuItem});
            this.continuousTestsToolStripMenuItem.Name = "continuousTestsToolStripMenuItem";
            this.continuousTestsToolStripMenuItem.Size = new System.Drawing.Size(108, 20);
            this.continuousTestsToolStripMenuItem.Text = "&ContinuousTests";
            // 
            // resumeEngineToolStripMenuItem
            // 
            this.resumeEngineToolStripMenuItem.Name = "resumeEngineToolStripMenuItem";
            this.resumeEngineToolStripMenuItem.Size = new System.Drawing.Size(273, 22);
            this.resumeEngineToolStripMenuItem.Text = "Resume Engine";
            this.resumeEngineToolStripMenuItem.Click += new System.EventHandler(this.ResumeEngineToolStripMenuItemClick);
            // 
            // pauseEngineToolStripMenuItem
            // 
            this.pauseEngineToolStripMenuItem.Name = "pauseEngineToolStripMenuItem";
            this.pauseEngineToolStripMenuItem.Size = new System.Drawing.Size(273, 22);
            this.pauseEngineToolStripMenuItem.Text = "Pause Engine";
            this.pauseEngineToolStripMenuItem.Click += new System.EventHandler(this.PauseEngineToolStripMenuItemClick);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(270, 6);
            // 
            // buildAndTestAllToolStripMenuItem1
            // 
            this.buildAndTestAllToolStripMenuItem1.Name = "buildAndTestAllToolStripMenuItem1";
            this.buildAndTestAllToolStripMenuItem1.Size = new System.Drawing.Size(273, 22);
            this.buildAndTestAllToolStripMenuItem1.Text = "Build And Test All";
            this.buildAndTestAllToolStripMenuItem1.Click += new System.EventHandler(this.BuildAndTestAllToolStripMenuItem1Click);
            // 
            // toolStripMenuItemClearList
            // 
            this.toolStripMenuItemClearList.Name = "toolStripMenuItemClearList";
            this.toolStripMenuItemClearList.Size = new System.Drawing.Size(273, 22);
            this.toolStripMenuItemClearList.Text = "Clear Cached Tests And Feedback List";
            this.toolStripMenuItemClearList.Click += new System.EventHandler(this.toolStripMenuItemClearList_Click);
            // 
            // detectRecursiveBuildsToolStripMenuItem
            // 
            this.detectRecursiveBuildsToolStripMenuItem.Name = "detectRecursiveBuildsToolStripMenuItem";
            this.detectRecursiveBuildsToolStripMenuItem.Size = new System.Drawing.Size(273, 22);
            this.detectRecursiveBuildsToolStripMenuItem.Text = "Detect Recursive Builds";
            this.detectRecursiveBuildsToolStripMenuItem.Click += new System.EventHandler(this.DetectRecursiveBuildsToolStripMenuItemClick);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(270, 6);
            // 
            // configurationToolStripMenuItem
            // 
            this.configurationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.globalToolStripMenuItem,
            this.localToolStripMenuItem});
            this.configurationToolStripMenuItem.Name = "configurationToolStripMenuItem";
            this.configurationToolStripMenuItem.Size = new System.Drawing.Size(273, 22);
            this.configurationToolStripMenuItem.Text = "Configuration";
            // 
            // globalToolStripMenuItem
            // 
            this.globalToolStripMenuItem.Name = "globalToolStripMenuItem";
            this.globalToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
            this.globalToolStripMenuItem.Text = "Global";
            this.globalToolStripMenuItem.Click += new System.EventHandler(this.GlobalToolStripMenuItemClick);
            // 
            // localToolStripMenuItem
            // 
            this.localToolStripMenuItem.Name = "localToolStripMenuItem";
            this.localToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
            this.localToolStripMenuItem.Text = "Solution";
            this.localToolStripMenuItem.Click += new System.EventHandler(this.LocalToolStripMenuItemClick);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(270, 6);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(273, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.AboutToolStripMenuItemClick);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Location = new System.Drawing.Point(122, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1028, 24);
            this.panel1.TabIndex = 2;
            // 
            // RunFeedbackForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1150, 273);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.runFeedback1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "RunFeedbackForm";
            this.Text = "ContinuousTests Standalone Client";
            this.Activated += new System.EventHandler(this.RunFeedbackFormActivated);
            this.Resize += new System.EventHandler(this.RunFeedbackFormResize);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private AutoTest.UI.RunFeedback runFeedback1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem continuousTestsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resumeEngineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pauseEngineToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem buildAndTestAllToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem detectRecursiveBuildsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem configurationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem globalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem localToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemClearList;
    }
}

