using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AutoTest.Server
{
    public partial class HiddenGetFocusForm : Form
    {
        public HiddenGetFocusForm()
        {
            InitializeComponent();
            Height = 10;
            Width = 10;
            ShowInTaskbar = false;
            Visible = false;
            MinimizeBox = false;
            MaximizeBox = false;
            ControlBox = false;
            Opacity = 0;
        }

        protected override CreateParams CreateParams {
            get {
                // Make the window not visible in taskbar
                // Turn on WS_EX_TOOLWINDOW style bit
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x80; 
                return cp;
            }
        }
    }
}
