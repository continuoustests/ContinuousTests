using System.Collections.Generic;

namespace AutoTest.Graphs
{
    public class AffectedGraphNode
    {
        public string DisplayName;
        public bool IsInterface;
        public bool IsTest;
        public bool IsRootNode;
        public string Name;
        public string FullName;
        public List<TestDescriptor> TestDescriptors;
        public bool IsChange;
        public bool InTestAssembly;
        public string Assembly;
        public string Type;
        public int Complexity;

        public bool Profiled { get; set; }


        public AffectedGraphNode(string displayName, bool isInterface, bool isTest, bool isRootNode, string name, string fullName, string assembly, string type, List<TestDescriptor> testDescriptors, bool isChange, bool inTestAssembly, int complexity)
        {
            DisplayName = displayName;
            TestDescriptors = testDescriptors;
            IsChange = isChange;
            InTestAssembly = inTestAssembly;
            Type = type;
            Assembly = assembly;
            FullName = fullName;
            IsRootNode = isRootNode;
            Name = name;
            IsTest = isTest;
            IsInterface = isInterface;
            Complexity = complexity;
        }

        public void MarkAsProfiled()
        {
            Profiled = true;
        }
    }
}