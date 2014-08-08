using System;
using AutoTest.Client.Config;
using AutoTest.Client.SequenceDiagramGenerators;
using AutoTest.Client.UI;
using AutoTest.VM.Messages;
using AutoTest.Client.Logging;
using AutoTest.Client;
using System.Threading;
using System.ComponentModel;
using AutoTest.VS.SequenceDiagramGenerators;
using EnvDTE80;
using AutoTest.Client.HTTP;
namespace AutoTest.VS.Listeners
{
    class SequenceDiagramGenerateListener : IMessageListener
    {
        private readonly DTE2 _application;
        private readonly ATEClient _client;
        private readonly SynchronizationContext _syncContext;

        public SequenceDiagramGenerateListener(DTE2 application, ATEClient client)
        {
            _syncContext = AsyncOperationManager.SynchronizationContext;
            _application = application;
            _client = client;
            SequenceDiagramGeneratorFactory.RegisterGraphVisualizer("default", new GoDiagramSequenceDiagramGenerator(DisplayMode.Dark, _application, true));
            SequenceDiagramGeneratorFactory.RegisterGraphVisualizer(GraphProvider.BUILTINDARK.ToString(), new GoDiagramSequenceDiagramGenerator(DisplayMode.Dark, _application, true));
            SequenceDiagramGeneratorFactory.RegisterGraphVisualizer(GraphProvider.BUILTINLIGHT.ToString(), new GoDiagramSequenceDiagramGenerator(DisplayMode.Light, _application, true));
            SequenceDiagramGeneratorFactory.RegisterGraphVisualizer(GraphProvider.WINDOW.ToString(), new GoDiagramSequenceDiagramGenerator(DisplayMode.Light, _application, false));
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
                if (x.GetType().Equals(typeof(TestInformationGeneratedMessage)))
                {
                    try
                    {
                        
                        var m = (TestInformationGeneratedMessage) x;
                        Logger.Write("Received a TestInformation " + m.Item + " with " + CountChildren(m.Test) + " items.");
                        if(CountChildren(m.Test) > 300)
                        {
                            Analytics.SendEvent("TooBigDiagram");
                            _application.StatusBar.Text =
                                "This diagram is really big you probably don't want to see it.";
                        }
                        else if (CountChildren(m.Test) == 0)
                        {
                            Analytics.SendEvent("DiagramNotFound");
                            _application.StatusBar.Text = "Profiled data not located, probably not a test.";
                        }
                        else
                        {
                            Analytics.SendEvent("DiagramShown");
                            Logger.Write("sending message to Diagram Visualizer");
                            var visualization =
                                SequenceDiagramGeneratorFactory.GetVisualizerFor(
                                    _client.MMConfiguration.GraphProvider.ToString());
                            visualization.GenerateAndShowDiagramFor(m);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Write("error generating diagram");
                        Logger.Write(ex);
                    }
                }
                if(message.GetType() == typeof(ProfilerCompletedMessage))
                {
                    try
                    {
                        var visualization =
                            SequenceDiagramGeneratorFactory.GetVisualizerFor(
                                _client.MMConfiguration.GraphProvider.ToString());
                        Logger.Write("wantsrefresh=" + visualization.WantsRefresh());
                        if (visualization.WantsRefresh())
                        {
                            Analytics.SendEvent("RefreshDiagram");
                            Logger.Write("Sending Message to get Sequence Diagram Refresh");
                            _client.GetRuntimeTestInformationFor(visualization.GetCurrentSignature());
                        }
                    } catch(Exception ex)
                    {
                        Logger.Write("Error updating diagram " + ex);
                    }
                }
            }, message);
        }

        private static int CountChildren(Chain test)
        {
            int count = 1;
            if (test.Children == null) return 1;
            foreach(var t in test.Children)
            {
                count += CountChildren(t);
            }
            return count;
        }
    }

}
