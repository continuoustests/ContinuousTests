namespace AutoTest.VS.GraphGenerators
{
    partial class AGLVisualizer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AGLVisualizer));
            this.SuspendLayout();
            // 
            // AGLVisualizer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(2)))), ((int)(((byte)(3)))));
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.KeyPreview = true;
            this.Name = "AGLVisualizer";
            this.Text = "Graph Visualizer";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.AGLVisualizer_KeyDown);
            this.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.AGLVisualizer_PreviewKeyDown);
            this.ResumeLayout(false);

        }

        #endregion
    }
}