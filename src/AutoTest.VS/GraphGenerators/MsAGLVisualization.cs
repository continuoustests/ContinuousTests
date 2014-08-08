using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using AutoTest.Client;
using AutoTest.Client.GraphGenerators;
using AutoTest.Client.Logging;
using AutoTest.VM.Messages;
using EnvDTE80;

namespace AutoTest.VS
{
    public static class NativeWinPlacementAPI
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        private struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public int showCmd;
            public System.Drawing.Point ptMinPosition;
            public System.Drawing.Point ptMaxPosition;
            public System.Drawing.Rectangle rcNormalPosition;
        }

        private const UInt32 SW_HIDE = 0;
        private const UInt32 SW_SHOWNORMAL = 1;
        private const UInt32 SW_NORMAL = 1;
        private const UInt32 SW_SHOWMINIMIZED = 2;
        private const UInt32 SW_SHOWMAXIMIZED = 3;
        private const UInt32 SW_MAXIMIZE = 3;
        private const UInt32 SW_SHOWNOACTIVATE = 4;
        private const UInt32 SW_SHOW = 5;
        private const UInt32 SW_MINIMIZE = 6;
        private const UInt32 SW_SHOWMINNOACTIVE = 7;
        private const UInt32 SW_SHOWNA = 8;
        private const UInt32 SW_RESTORE = 9;

        public static Point GetPlacement(IntPtr handle)
        {
            var placement = new WINDOWPLACEMENT();
            placement.length = Marshal.SizeOf(placement);
            if (GetWindowPlacement(handle, ref placement))
            {
                var location = placement.rcNormalPosition;
                return new Point(location.X + location.Width/2, location.Y + location.Height / 2);
            }
            Logger.Write("placement not found.");
            return new Point(0, 0);
        }
    }
}
namespace AutoTest.VS.GraphGenerators
{
    public class MsAGLVisualization : IGraphVisualization
    {
        private readonly DisplayMode _mode;
        private readonly DTE2 _dte;
        private AGLVisualizer _visualizer;
        private readonly bool _transparent;
        private readonly ATEClient _client;
        private string _lastSignature;

        public MsAGLVisualization(DisplayMode mode, DTE2 _dte, bool transparent, ATEClient client)
        {
            _mode = mode;
            this._dte = _dte;
            
            _transparent = transparent;
            _client = client;
        }

        public void GenerateAndShowGraphFor(VisualGraphGeneratedMessage message)
        {
            if (_visualizer == null)
            {
                _visualizer = new AGLVisualizer(_mode, _dte, _transparent, _client);
                _visualizer.Closed += _visualizer_Closed;
                _visualizer.WindowState = FormWindowState.Maximized;
            }
            var parent = Control.FromHandle(new IntPtr(_dte.MainWindow.HWnd));
            _visualizer.Parent = parent;
            _visualizer.Location = NativeWinPlacementAPI.GetPlacement(new IntPtr(_dte.MainWindow.HWnd));
            _visualizer.StartPosition = FormStartPosition.CenterScreen;
            Logger.Write("Location is " + _visualizer.Location);
            var root = message.Nodes.First(x => x.IsRootNode);
            if(root != null)
                _lastSignature = root.FullName;
            _visualizer.Display(message);
            _visualizer.Visible = false;
            _visualizer.Show();
            
        }

        public bool WantsRefresh()
        {
            return _visualizer != null;
        }

        public string GetCurrentSignature()
        {
            return _lastSignature;
        }

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        
        void _visualizer_Closed(object sender, EventArgs e)
        {
            _visualizer = null;
        }
    }
}
