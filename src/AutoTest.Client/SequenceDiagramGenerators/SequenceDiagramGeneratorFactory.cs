using System.Collections.Generic;

namespace AutoTest.Client.SequenceDiagramGenerators
{
    public class SequenceDiagramGeneratorFactory
    {
        private static readonly Dictionary<string, ISequenceDiagramVisualization> hash = new Dictionary<string, ISequenceDiagramVisualization>();
        public static void RegisterGraphVisualizer(string name, ISequenceDiagramVisualization instance)
        {
            if (!hash.ContainsKey(name))
            {
                hash.Add(name, instance);
            }
        }

        public static ISequenceDiagramVisualization GetVisualizerFor(string provider)
        {
            if (hash.ContainsKey(provider)) return hash[provider];
            return hash["default"];
        }
    }
}