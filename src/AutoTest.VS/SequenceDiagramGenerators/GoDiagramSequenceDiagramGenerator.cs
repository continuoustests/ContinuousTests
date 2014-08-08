using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using AutoTest.Client.Logging;
using AutoTest.Client.SequenceDiagramGenerators;
using AutoTest.VM.Messages;
using EnvDTE80;

namespace AutoTest.VS.SequenceDiagramGenerators
{
    class GoDiagramSequenceDiagramGenerator : ISequenceDiagramVisualization
    {
        private readonly DisplayMode _mode;
        private readonly DTE2 _dte;
        private readonly bool _transparent;
        private SequenceDiagram frm;
        private string _lastSignature;

        public GoDiagramSequenceDiagramGenerator(DisplayMode mode, DTE2 dte, bool transparent)
        {
            _mode = mode;
            _dte = dte;
            _transparent = transparent;
        }

        public void GenerateAndShowDiagramFor(TestInformationGeneratedMessage message)
        {
            if (frm == null)
            {
                frm = new SequenceDiagram(_mode, _dte, _transparent);
                frm.Closed += frm_Closed;
                frm.WindowState = FormWindowState.Maximized;
            }
            var parent = Control.FromHandle(new IntPtr(_dte.MainWindow.HWnd));
            frm.Parent = parent;
            frm.Location = NativeWinPlacementAPI.GetPlacement(new IntPtr(_dte.MainWindow.HWnd));
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.CreateDiagramFor(message);
            frm.Show();
            _lastSignature = message.Item;
        }

        public string GetCurrentSignature()
        {
            return _lastSignature;
        }

        public bool WantsRefresh()
        {
            return frm != null;
        }

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        

        void frm_Closed(object sender, EventArgs e)
        {
            frm = null;
        }
    }
}
