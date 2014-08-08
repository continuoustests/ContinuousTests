using System;
using System.Drawing;
using System.Windows.Forms;

namespace AutoTest.VS
{
    public sealed partial class TransparentImageWindow : Form
    {
        private Timer t;
        public TransparentImageWindow(string image)
        {
            Opacity = 1;
            InitializeComponent();
            BackgroundImage = GetImage(image);
            t = new Timer {Interval = 50, Enabled = true};
            t.Tick += t_Tick;
            t.Start();
        }

        void t_Tick(object sender, EventArgs e)
        {
            var newval = Opacity - .05;
            if (newval < 0)
            {
                newval = 0;
                t.Stop();
                Close();
            } 
            Opacity = newval;
        }

        public Image GetImage(string name)
        {
            System.Reflection.Assembly thisExe;
            thisExe = System.Reflection.Assembly.GetExecutingAssembly();
            System.IO.Stream file =
                thisExe.GetManifestResourceStream("AutoTest.VS.Resources." + name);
            return Image.FromStream(file);
        }
    }
}
