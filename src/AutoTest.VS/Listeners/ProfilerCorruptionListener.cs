using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using AutoTest.Client;
using AutoTest.Client.UI;
using AutoTest.VM.Messages;
using EnvDTE80;

namespace AutoTest.VS.Listeners
{
    class ProfilerCorruptionListener : IMessageListener
    {
        private readonly DTE2 _application;
        private readonly ATEClient _client;
        private readonly SynchronizationContext _syncContext;

        public ProfilerCorruptionListener(DTE2 application, ATEClient client)
        {
            //seemed like as good a place as any to register the visualizers since its the only thing that uses them.
            _syncContext = AsyncOperationManager.SynchronizationContext;
            _application = application;
            _client = client;
        }

        public void Connecting(int port, bool startPaused)
        {
        }

        public void Disconnecting(int port)
        {
        }

        public void IncomingMessage(object message)
        {
            _syncContext.Post(x =>
                                  {
                                      if (x.GetType() == typeof(ProfilerLoadErrorOccurredMessage))
                                      {
                                          var rerun = MessageBox.Show(
                                              "Sorry but we need to rerun a full build. Would you like it to be done now?\nIf not you can always manually run from the continous tests menu.",
                                              "Continuous Tests: Rerun All?", MessageBoxButtons.YesNo);
                                          if(rerun == DialogResult.Yes)
                                          {
                                               _client.RunAll();            
                                          }
                                      }
                             
                                  }, message);
        }
    }
}