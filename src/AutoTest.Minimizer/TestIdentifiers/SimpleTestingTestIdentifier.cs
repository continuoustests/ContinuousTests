using System.Collections.Generic;
using System.Threading;
using Mono.Cecil;
using AutoTest.Minimizer.Extensions;
using AutoTest.Graphs;
namespace AutoTest.Minimizer.TestIdentifiers
{
    public class SimpleTestingTestIdentifier : ITestIdentifier
    {
        public bool IsTest(MemberReference reference)
        {
            var refer = reference as MethodReference;
            if (refer == null) return false;
            var resolved = refer.ThreadSafeResolve();
            var resolvedtype = refer.DeclaringType.ThreadSafeResolve();
            if (resolved != null && resolvedtype != null && (resolvedtype.IsAbstract || resolved.IsAbstract)) return false;
            return CheckSpecificationReturn(resolved.ReturnType.ThreadSafeResolve()) || CheckIEnumerableSpecificationReturn(resolved);
        }

        private static bool CheckIEnumerableSpecificationReturn(MethodDefinition resolved)
        {
                var instance2 = resolved.ReturnType as GenericInstanceType;
                if (instance2 != null && instance2.FullName.StartsWith("System.Collections.Generic.IEnumerable`1")) return CheckSpecificationReturn(instance2.GenericArguments[0].ThreadSafeResolve());
            return false;
        }

        private static bool CheckSpecificationReturn(TypeDefinition typeDef)
        {
            foreach (var t in typeDef.WalkInheritance())
            {
                var res = t.ThreadSafeResolve();
                if (res != null)
                {
                    foreach (var inter in res.Interfaces)
                    {
                        if (inter.FullName == "Simple.Testing.ClientFramework.Specification") return true;
                    }
                }
                if (t.FullName == "Simple.Testing.ClientFramework.Specification") return true;
            }
            return false;
        }

        public List<MemberAccess> GetHiddenDependenciesForTest(MethodReference refernece)
        {
            return new List<MemberAccess>();
        }

        public TestDescriptor GetTestDescriptorFor(MemberReference refer)
        {
            var reference = refer as MethodReference;
            if (reference == null) return null;
            if (!IsTest(reference)) return null;
            return new TestDescriptor(reference.Name, reference.Module.FullyQualifiedName, "simple.testing", reference.DeclaringType.FullName);
        }

        public string Tester
        {
            get { return "simpletesting"; }
        }

        public bool IsTeardown(MethodReference target)
        {
            return false;
        }

        public bool IsSetup(MethodReference target)
        {
            return false;
        }

        private MethodReference last;
        public string GetSpecificallyMangledName(MethodReference target)
        {
            if(IsTest(target))
            {
                last = target;
            }
            if(IsProfilerTest(target))
            {
                return last.GetCacheName();
            }
            return null;
        }

        public bool IsProfilerTeardown(MethodReference target)
        {
            return target.FullName.StartsWith("System.Boolean Simple.Testing.Framework.SpecificationRunner::RunTeardowns(");
        }

        public bool IsProfilerSetup(MethodReference target)
        {
            return target.FullName.StartsWith("System.Boolean Simple.Testing.Framework.SpecificationRunner::RunSetup(");
        }

        public bool IsProfilerTest(MemberReference reference)
        {
            return reference.FullName.StartsWith("System.Boolean Simple.Testing.Framework.SpecificationRunner::RunAssertions(");
        }

        public bool IsInFixture(MethodReference target)
        {
            return false;
        }
    }
}
