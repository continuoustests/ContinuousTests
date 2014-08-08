using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using AutoTest.Client;
using EnvDTE;
using AutoTest.Client.Logging;
using ContinuousTests.VS;
using AutoTest.VS.Util.CommandHandling;

namespace AutoTest.VS.CommandHandling
{
    public class RealtimeToggler : ICommandHandler
    {
        private readonly ATEClient _client;

        public RealtimeToggler(ATEClient client)
        {
            _client = client;
        }
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll", CharSet=CharSet.Auto,ExactSpelling=true)] 
        public static extern IntPtr SetFocus(HandleRef hWnd);
        public void Exec(vsCommandExecOption ExecuteOption, ref object VariantIn, ref object VariantOut, ref bool Handled)
        {
            // Only toggle realtime mode in mighty mode
            if (!_client.IsRunning || _client.MMConfiguration.BuildExecutables.Count() == 0)
            {
                return;
            }

            var newState = _client.MMConfiguration.RealtimeFeedback ? "off" : "on";
            
            try
            {
                var ptr = GetForegroundWindow();
                var image = newState == "on" ? "maniac.png" : "mighty.png";
                var win = new TransparentImageWindow(image);
                win.Show();
                SetFocus(new HandleRef(null, ptr));
                Logger.Write("Toggling realtime mode " + newState);
                _client.MMConfiguration.OverrideRealtimeFeedback(!_client.MMConfiguration.RealtimeFeedback);
                _client.RefreshConfig();

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void QueryStatus(vsCommandStatusTextWanted NeededText, ref vsCommandStatus StatusOption, ref object CommandText)
        {
            StatusOption = _client.IsRunning && Connect.IsSolutionOpened
                               ? vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled
                               : vsCommandStatus.vsCommandStatusSupported;
            return;
        }

        public string Name
        {
            get { return "ContinuousTests.VS.Connect.ContinuousTests_RealtimeToggler"; }
        }
    }
}
