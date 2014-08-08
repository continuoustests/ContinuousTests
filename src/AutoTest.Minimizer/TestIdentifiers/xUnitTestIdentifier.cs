using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Graphs;
using AutoTest.Minimizer.Extensions;
using Mono.Cecil;

namespace AutoTest.Minimizer.TestIdentifiers
{
    public class XUnitTestIdentifier : ITestIdentifier
    {
        public bool IsTest(MemberReference reference)
        {
            var refer = reference as MethodReference;
            if (refer == null) return false;
            var resolved = refer.ThreadSafeResolve();
            var resolvedtype = refer.DeclaringType.ThreadSafeResolve();
            if (resolved != null && resolvedtype != null && (resolvedtype.IsAbstract || resolved.IsAbstract)) return false;
            return refer.HasAttribute("Fact") || refer.HasAttribute("Theory");
        }

        public List<MemberAccess> GetHiddenDependenciesForTest(MethodReference refernece)
        {
            var method = refernece.DeclaringType.ThreadSafeResolve().Methods.Where((x) => x.Name == ".ctor").FirstOrDefault();
            if(method == null) return new List<MemberAccess>();
            return new List<MemberAccess>(method.GetInheritanceHierarchy().Select((x) => MethodBuildingHelper.BuildMemberAccess(refernece, method)));
        }

        public TestDescriptor GetTestDescriptorFor(MemberReference refer)
        {

            var reference = refer as MethodReference;
            if (reference == null) return null;
            if (!IsTest(refer)) return null;
            return new TestDescriptor(reference.Name, reference.Module.FullyQualifiedName, "xunit", reference.DeclaringType.FullName);
        }

        public string Tester
        {
            get { return "xunit"; }
        }

        public bool IsTeardown(MethodReference target)
        {
            var def = target.ThreadSafeResolve();
            if (def != null  && def.Name == "Finalize" && target.Parameters.Count == 0)
            {
                return target.DeclaringType.ThreadSafeResolve().GetMethods().Any(IsTest);
            }
            return false;
        }

        public bool IsSetup(MethodReference target)
        {
            var def = target.ThreadSafeResolve();
            if (def != null && def.IsConstructor)
            {
                return IsXunitFixture(target);
            }
            return false;
        }

        public bool IsInFixture(MethodReference target)
        {
            return IsXunitFixture(target);
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

        private bool IsXunitFixture(MethodReference target)
        {
            var def = target.DeclaringType.ThreadSafeResolve();
            if(def == null) return false;
            return def.GetMethods().Any(IsTest);
        }
    }
}
