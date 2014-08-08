using System;
using System.Collections.Generic;
using System.Linq;
using AutoTest.Graphs;
using AutoTest.Minimizer.Extensions;
using Mono.Cecil;

namespace AutoTest.Minimizer.TestIdentifiers
{
    public class NUnitTestIdentifier : ITestIdentifier
    {
        //TODO should we make these fully qualified names?
        public bool IsTest(MemberReference reference)
        {
            var refer = reference as MethodReference;
            if (refer == null) return false;
            var resolved = refer.ThreadSafeResolve();
            var resolvedtype = refer.DeclaringType.ThreadSafeResolve();
            if (resolved != null && resolvedtype != null && (resolvedtype.IsAbstract || resolved.IsAbstract)) return false;
            return (refer.HasAttribute("Test") || refer.HasAttribute("TestCase")) && !refer.HasAttribute("Explicit");
        }


        private bool IsNUnitFixture(TypeDefinition definition)
        {
            if (definition == null) return false;
            return definition.ContainsEffectiveAttribute("TestFixtureAttribute");
        }

        public List<MemberAccess> GetHiddenDependenciesForTest(MethodReference refernece)
        {
            var m = refernece.ThreadSafeResolve();
            var ret = new List<MemberAccess>();
            if (!IsTest(refernece)) return ret;
            foreach(var method in m.DeclaringType.GetMethodsInHierarchyWithAttribute("SetUpAttribute"))
            {
                ret.Add(MethodBuildingHelper.BuildMemberAccess(refernece, method));
            }
            foreach (var method in m.DeclaringType.GetMethodsInHierarchyWithAttribute("TestFixtureSetUpAttribute"))
            {
                ret.Add(MethodBuildingHelper.BuildMemberAccess(refernece, method));
            }
            foreach (var method in m.DeclaringType.GetMethodsInHierarchyWithAttribute("TearDownAttribute"))
            {
                ret.Add(MethodBuildingHelper.BuildMemberAccess(refernece, method));
            }
            foreach (var method in m.DeclaringType.GetMethodsInHierarchyWithAttribute("TestFixtureTearDownAttribute"))
            {
                ret.Add(MethodBuildingHelper.BuildMemberAccess(refernece, method));
            }
            return ret;
        }

        public TestDescriptor GetTestDescriptorFor(MemberReference refer)
        {
            var reference = refer as MethodReference;
            if (reference == null) return null;
            if (!IsTest(reference)) return null;
            return new TestDescriptor(reference.Name, reference.Module.FullyQualifiedName, "nunit", reference.DeclaringType.FullName);
        }

        public string Tester
        {
            get { return "nunit"; }
        }

        public bool IsTeardown(MethodReference target)
        {
            return target.HasAttribute("TearDown");
        }

        public bool IsSetup(MethodReference target)
        {
            return target.HasAttribute("SetUp");
        }

        public bool IsInFixture(MethodReference target)
        {
            return IsNUnitFixture(target.DeclaringType.ThreadSafeResolve());
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
