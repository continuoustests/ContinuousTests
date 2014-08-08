using System.Collections.Generic;
using AutoTest.Graphs;
using AutoTest.Minimizer.Extensions;
using Mono.Cecil;

namespace AutoTest.Minimizer.TestIdentifiers
{
    public class mSpecTestIdentifier : ITestIdentifier
    {
        public bool IsTest(MemberReference reference)
        {
            var refer = reference as MethodReference;
            var field = reference as FieldReference;
            MethodDefinition resolved = null;

            if (refer != null)
                resolved = refer.ThreadSafeResolve();
            var containingType = reference.DeclaringType;
            var resolvedtype = containingType.ThreadSafeResolve();
            if (resolved != null && resolvedtype != null && (resolvedtype.IsAbstract || resolved.IsAbstract))
                return false;
            if ((refer == null && field != null && field.FieldType.Name == "It")) return true;

            if (refer == null || resolved == null) return false;
            
            var ret = MSpecTranslator.TranslateGeneratedMethod(resolved);
            if (ret == null) return false;
            return ret.FieldType.Name == "It";
        }

        private static bool ContainsMSpecFields(TypeDefinition containingType)
        {
            if (containingType == null) return false;
            foreach(var field in containingType.Fields)
            {
                if (field.FieldType.FullName == "Machine.Specifications.Establish" ||
                   field.FieldType.FullName == "Machine.Specifications.It" ||
                   field.FieldType.FullName == "Machine.Specifications.Because")
                    return true;
            }
            return false;
        }

        public List<MemberAccess> GetHiddenDependenciesForTest(MethodReference refernece)
        {
            return new List<MemberAccess>();
        }

        public TestDescriptor GetTestDescriptorFor(MemberReference reference)
        {
            if (!IsTest(reference)) return null; 
             return new TestDescriptor(reference.DeclaringType.FullName.Replace("/", "+"), reference.Module.FullyQualifiedName, "mspec", reference.DeclaringType.FullName);
        }

        public string Tester
        {
            get { return "mspec"; }
        }

        public bool IsTeardown(MethodReference target)
        {
            //TODO support back tracking for mspec for profiler
            return false;
        }

        public bool IsSetup(MethodReference target)
        {
            if (!IsInFixture(target)) return false;
            var field = MSpecTranslator.TranslateGeneratedMethod(target.Resolve());
            if (field == null) return false;
            return field.FieldType.Name == "Establish" || field.FieldType.Name == "Because";
        }

        public bool IsInFixture(MethodReference target)
        {
            return ContainsMSpecFields(target.DeclaringType.ThreadSafeResolve());
        }

        public string GetSpecificallyMangledName(MethodReference target)
        {
            if (!IsTest(target)) return null;
            var ret = MSpecTranslator.TranslateGeneratedMethod(target.ThreadSafeResolve());
            return ret == null ? null : ret.GetCacheName();
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
