namespace AutoTest.VS.SequenceDiagramGenerators
{
    partial class SequenceDiagram
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
            this._view = new Northwoods.Go.GoView();
            this._label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // goView1
            // 
            this._view.ArrowMoveLarge = 10F;
            this._view.ArrowMoveSmall = 1F;
            this._view.Dock = System.Windows.Forms.DockStyle.Fill;
            this._view.DragsRealtime = true;
            this._view.GridCellSizeHeight = 15F;
            this._view.GridOriginY = 75F;
            this._view.GridStyle = Northwoods.Go.GoViewGridStyle.None;
            this._view.GridUnboundedSpots = 24;
            this._view.Location = new System.Drawing.Point(0, 0);
            this._view.Name = "_view";
            this._view.Size = new System.Drawing.Size(284, 262);
            this._view.TabIndex = 0;
            this._view.Text = "goView1";
            // 
            // label1
            // 
            this._label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._label1.Location = new System.Drawing.Point(0, 2);
            this._label1.Name = "_label1";
            this._label1.Size = new System.Drawing.Size(661, 30);
            this._label1.TabIndex = 1;
            // 
            // SequenceDiagram
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this._view);
            this.Controls.Add(this._label1);
            this.KeyPreview = true;
            this.Name = "SequenceDiagram";
            this.Text = "Sequence Diagram";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.GoDiagramPlayWindow_KeyDown);
            this.ResumeLayout(false);

        }

        #endregion
    }
}