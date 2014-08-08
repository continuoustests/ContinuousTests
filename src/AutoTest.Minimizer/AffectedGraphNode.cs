using System.Collections.Generic;

namespace AutoTest.Minimizer
{
    public class AffectedGraphNode
    {
        public readonly string DisplayName;
        public readonly bool IsInterface;
        public readonly bool IsTest;
        public readonly bool IsRootNode;
        public readonly string Name;
        public readonly string FullName;
        public readonly List<TestDescriptor> TestDescriptors;
        public readonly bool IsChange;
        public readonly string Assembly;
        public readonly string Type;



        public AffectedGraphNode(string displayName, bool isInterface, bool isTest, bool isRootNode, string name, string fullName, string assembly, string type, List<TestDescriptor> testDescriptors, bool isChange)
        {
            DisplayName = displayName;
            TestDescriptors = testDescriptors;
            IsChange = isChange;
            Type = type;
            Assembly = assembly;
            FullName = fullName;
            IsRootNode = isRootNode;
            Name = name;
            IsTest = isTest;
            IsInterface = isInterface;
        }
    }
}