namespace AutoTest.VS.SequenceDiagramGenerators
{
    partial class GoDiagramPlayWindow
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
            this.goView1 = new Northwoods.Go.GoView();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // goView1
            // 
            this.goView1.ArrowMoveLarge = 10F;
            this.goView1.ArrowMoveSmall = 1F;
            this.goView1.BackColor = System.Drawing.Color.Black;
            this.goView1.Border3DStyle = System.Windows.Forms.Border3DStyle.Flat;
            this.goView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.goView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.goView1.DragsRealtime = true;
            this.goView1.GridCellSizeHeight = 15F;
            this.goView1.GridLineColor = System.Drawing.Color.Black;
            this.goView1.GridOriginY = 75F;
            this.goView1.GridStyle = Northwoods.Go.GoViewGridStyle.HorizontalLine;
            this.goView1.GridUnboundedSpots = 24;
            this.goView1.Location = new System.Drawing.Point(0, 0);
            this.goView1.Name = "goView1";
            this.goView1.PrimarySelectionColor = System.Drawing.Color.Transparent;
            this.goView1.SecondarySelectionColor = System.Drawing.Color.Transparent;
            this.goView1.Size = new System.Drawing.Size(284, 262);
            this.goView1.TabIndex = 0;
            this.goView1.Text = "goView1";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(0, 2);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(661, 30);
            this.label1.TabIndex = 1;
            // 
            // GoDiagramPlayWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(2)))), ((int)(((byte)(3)))));
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.goView1);
            this.Controls.Add(this.label1);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(2)))), ((int)(((byte)(3)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Name = "GoDiagramPlayWindow";
            this.Opacity = 0.8D;
            this.ShowInTaskbar = false;
            this.Text = "Sequence Diagram";
            this.TransparencyKey = System.Drawing.Color.Transparent;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.GoDiagramPlayWindow_KeyDown);
            this.ResumeLayout(false);

        }

        #endregion
    }
}