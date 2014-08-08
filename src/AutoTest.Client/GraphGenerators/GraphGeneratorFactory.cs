using System.Collections.Generic;

namespace AutoTest.Client.GraphGenerators
{
    public class GraphGeneratorFactory
    {
        private static readonly Dictionary<string, IGraphVisualization> hash = new Dictionary<string, IGraphVisualization>();
        public static void RegisterGraphVisualizer(string name, IGraphVisualization instance)
        {
            if(!hash.ContainsKey(name))
            {
                hash.Add(name, instance);
            }
        }

        public static IGraphVisualization GetVisualizerFor(string provider)
        {
            if (hash.ContainsKey(provider)) return hash[provider];
            return hash["default"];
        }
    }
}