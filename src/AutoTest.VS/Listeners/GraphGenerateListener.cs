using System;
using System.ComponentModel;
using System.Threading;
using AutoTest.Client;
using AutoTest.Client.Config;
using AutoTest.Client.GraphGenerators;
using AutoTest.Client.Logging;
using AutoTest.Client.UI;
using AutoTest.VM.Messages;
using AutoTest.VS.GraphGenerators;
using EnvDTE80;
using AutoTest.Client.HTTP;
namespace AutoTest.VS.Listeners
{
    class GraphGenerateListener : IMessageListener
    {
        private readonly DTE2 _application;
        private readonly ATEClient _client;
        private readonly SynchronizationContext _syncContext;

        public GraphGenerateListener(DTE2 application, ATEClient client)
        {
            //seemed like as good a place as any to register the visualizers since its the only thing that uses them.
            _syncContext = AsyncOperationManager.SynchronizationContext;
            _application = application;
            _client = client;
            GraphGeneratorFactory.RegisterGraphVisualizer("default", new MsAGLVisualization(DisplayMode.Dark, _application, true, client));
            GraphGeneratorFactory.RegisterGraphVisualizer(GraphProvider.BUILTINLIGHT.ToString(), new MsAGLVisualization(DisplayMode.Light, _application, true, client));
            GraphGeneratorFactory.RegisterGraphVisualizer(GraphProvider.BUILTINDARK.ToString(), new MsAGLVisualization(DisplayMode.Dark, _application, true, client));
            GraphGeneratorFactory.RegisterGraphVisualizer(GraphProvider.WINDOW.ToString(), new MsAGLVisualization(DisplayMode.Light, _application, false, client));
            GraphGeneratorFactory.RegisterGraphVisualizer(GraphProvider.DGML.ToString(), new DGMLGraphVisualization(_application));
            GraphGeneratorFactory.RegisterGraphVisualizer(GraphProvider.GRAPHVIZ.ToString(), new GraphVizGraphVisualization());
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
                                      if (x.GetType().Equals(typeof(VisualGraphGeneratedMessage)))
                                      {
                                          try
                                          {
                                              
                                              var m = (VisualGraphGeneratedMessage) x;
                                              if (m.Nodes.Count == 1 && string.IsNullOrEmpty(m.Nodes[0].Assembly))
                                              {
                                                  Analytics.SendEvent("GraphNotFound");
                                                  _application.StatusBar.Text = "Node not found, has change been saved?";
                                              }
                                              else
                                              {
                                                  Analytics.SendEvent("GraphDisplayed");
                                                  Logger.Write("sending message to Graph Provider");
                                                  var visualization =
                                                      GraphGeneratorFactory.GetVisualizerFor(
                                                          _client.MMConfiguration.GraphProvider.ToString());
                                                  visualization.GenerateAndShowGraphFor(m);
                                              }
                                          }
                                          catch (Exception ex)
                                          {
                                              Analytics.SendEvent("GraphError");
                                              Logger.Write("error generating graph");
                                              Logger.Write(ex);
                                          }
                                      }
                                      if (message.GetType() == typeof(AssembliesMinimizedMessage))
                                      {
                                          try
                                          {
                                              var visualization =
                                                  GraphGeneratorFactory.GetVisualizerFor(
                                                      _client.MMConfiguration.GraphProvider.ToString());
                                              Logger.Write("wantsrefresh=" + visualization.WantsRefresh());
                                              if (visualization.WantsRefresh())
                                              {
                                                  Logger.Write("Sending Message to get Graph Refresh");
                                                  _client.GetGraphFor(visualization.GetCurrentSignature());
                                              }
                                          }
                                          catch (Exception ex)
                                          {
                                              Logger.Write("Error updating graph " + ex);
                                          }
                                      }
                                  }, message);
        }
    }
}