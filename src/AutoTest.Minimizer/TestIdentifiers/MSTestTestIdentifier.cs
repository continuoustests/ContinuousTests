using System.Collections.Generic;
using System.Threading;
using AutoTest.Graphs;
using AutoTest.Minimizer.Extensions;
using Mono.Cecil;

namespace AutoTest.Minimizer.TestIdentifiers
{
    public class MSTestTestIdentifier : ITestIdentifier
    {
        public bool IsTest(MemberReference reference)
        {
            var refer = reference as MethodReference;
            if (refer == null) return false;
            var resolved = refer.ThreadSafeResolve();
            var resolvedtype = refer.DeclaringType.ThreadSafeResolve();
            if (resolved != null && resolvedtype != null && (resolvedtype.IsAbstract || resolved.IsAbstract)) return false;
            return refer.HasAttribute("TestMethod");
        }

        private static bool IsMsTestFixture(TypeDefinition definition)
        {
            if (definition == null) return false;
            return definition.ContainsAttribute("TestClassAttribute");
        }

        public List<MemberAccess> GetHiddenDependenciesForTest(MethodReference refernece)
        {
            var m = refernece.ThreadSafeResolve();
            var ret = new List<MemberAccess>();
            if (!IsTest(refernece)) return ret;
            foreach (var method in m.DeclaringType.GetMethodsInHierarchyWithAttribute("ClassInitializeAttribute"))
            {
                ret.Add(MethodBuildingHelper.BuildMemberAccess(refernece, method));
            }
            foreach (var method in m.DeclaringType.GetMethodsInHierarchyWithAttribute("ClassCleanupAttribute"))
            {
                ret.Add(MethodBuildingHelper.BuildMemberAccess(refernece, method));
            }
            foreach (var method in m.DeclaringType.GetMethodsInHierarchyWithAttribute("TestInitializeAttribute"))
            {
                ret.Add(MethodBuildingHelper.BuildMemberAccess(refernece, method));
            }
            foreach (var method in m.DeclaringType.GetMethodsInHierarchyWithAttribute("TestCleanupAttribute"))
            {
                ret.Add(MethodBuildingHelper.BuildMemberAccess(refernece, method));
            }
            return ret;
        }

        public TestDescriptor GetTestDescriptorFor(MemberReference refer)
        {
            var reference = refer as MethodReference;
            if (reference == null) return null;
            if(!IsTest(reference)) return null;
            return new TestDescriptor(reference.Name,reference.Module.FullyQualifiedName,"mstest", reference.DeclaringType.FullName);
        }

        public string Tester
        {
            get { return "mstest"; }
        }

        public bool IsTeardown(MethodReference target)
        {
            return target.HasAttribute("TestCleanup") || target.HasAttribute("ClassCleanup");
        }
        
        public bool IsSetup(MethodReference target)
        {
            return target.HasAttribute("TestInitialize") || target.HasAttribute("ClassInitialize");
        }

        public bool IsInFixture(MethodReference target)
        {
            return IsMsTestFixture(target.DeclaringType.ThreadSafeResolve());
        }

        public string GetSpecificallyMangledName(MethodReference target)
        {
            return null;
        }

        public bool IsProfilerTeardown(MethodReference target)
        {
            return IsTeardown(target);
        }

        public bool IsProfilerSetup(MethodReference target)
        {
            return IsSetup(target);
        }

        public bool IsProfilerTest(MemberReference reference)
        {
            return IsTest(reference);
        }
    }
}
