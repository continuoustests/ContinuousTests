using System;
using NUnit.Framework;

namespace AutoTest.Minimizer.Tests
{
    [TestFixture]
    public class when_finding_method_synonyms : AssemblyTestFixture
    {
        [Test]
        public void when_no_synonyms_none_are_found()
        {
            var method = assembly.GetMethodDefinition<SimpleBase>("Test");
            var references = SynonymFinder.FindSynonymsFor(method);
            Assert.IsTrue(references.Count == 0);
        }

        [Test]
        public void wasasdSAhen_no_synonyms_none_are_found()
        {
            var method = assembly.GetMethodDefinition<SimpleBase>("Test");
            var references = SynonymFinder.FindSynonymsFor(method);
            Assert.IsTrue(references.Count == 0);
        }

         [Test]
         public void simple_base_class_is_added()
         {
            var method = assembly.GetMethodDefinition<SimpleDerived>("Test");
            var references = SynonymFinder.FindSynonymsFor(method);
            Assert.IsTrue(references.OnlyHasMember<SimpleBase>("Test"));
         }

        //one test missing as it can't be done in C# (Renamed override). Was tested manually in IL

         [Test]
        public void shadowed_base_class_method_is_not_found()
         {
             var method = assembly.GetMethodDefinition<SimpleDerived>("Shadow");
             var references = SynonymFinder.FindSynonymsFor(method);
             Assert.IsTrue(references.Count == 0);             
         }

        [Test]
        public void shadowed_virtual_base_class_method_is_not_found()
        {
            var method = assembly.GetMethodDefinition<SimpleDerived>("VirtualShadow");
            var references = SynonymFinder.FindSynonymsFor(method);
            Assert.IsTrue(references.Count == 0);                         
        }

        [Test]
        public void can_find_simple_interface_implementation()
        {
            var method = assembly.GetMethodDefinition<Implementor>("A");
            var references = SynonymFinder.FindSynonymsFor(method);
            Assert.IsTrue(references.OnlyHasMember<IA>("A"));
        }

        [Test]
        public void can_find_base_interface_inheritance_synonyms()
        {
            var method = assembly.GetMethodDefinition<DualImplementor>("A");
            var references = SynonymFinder.FindSynonymsFor(method);
            Assert.IsTrue(references.OnlyHasMember<IBase>("A"));            
        }

        [Test]
        public void can_find_explicit_interface_implementation()
        {
            var method = assembly.GetMethodDefinition<ExplicitImplementor>("AutoTest.Minimizer.Tests.IA.A");
            var references = SynonymFinder.FindSynonymsFor(method);
            Assert.IsTrue(references.OnlyHasMember<IA>("A"));
        }

        [Test]
        public void can_find_interface_implementation_on_base()
        {
            var method = assembly.GetMethodDefinition<DerivedNoReimplementation>("A");
            var references = SynonymFinder.FindSynonymsFor(method);
            Assert.IsTrue(references.Count == 2);
            Assert.IsTrue(references.HasMember<IA>("A"));
            Assert.IsTrue(references.HasMember<BaseWithInterface>("A"));
        }

        [Test]
        public void can_find_interface_implementation_when_method_is_virtual()
        {
            var method = assembly.GetMethodDefinition<VirtualWithInterface>("A");
            var references = SynonymFinder.FindSynonymsFor(method);
            Assert.IsTrue(references.OnlyHasMember<IA>("A"));
        }
    }

    interface X
    {
        void A();
    }

    interface Y : X
    {
        
    }

    public class MultipleInheritedInterface : Y
    {
        public void A()
        {
        }
    }

    public class DualImplementor : IDerived
    {
        public void A()
        {

        }
    }

    public interface IBase
    {
        void A();
    }

    public interface IDerived : IBase
    {
    }

    public class VirtualWithInterface : IA
    {
        public virtual void A()
        {
        }
    }

    public class SimpleBase
    {
        public virtual void Test()
        {
            
        }

        public void Shadow()
        {
            
        }

        public virtual void VirtualShadow()
        {
            
        }
    }
    class SimpleDerived : SimpleBase
    {
        public override void Test()
        {
            int x = 5;
        }
        public new void Shadow()
        {
            
        }

        public new virtual void VirtualShadow()
        {
            
        }
    }

    interface IA
    {
        void A();
    }

    class Implementor : IA
    {
        public void A()
        {
            throw new NotImplementedException();
        }
    }

    class ExplicitImplementor : IA
    {
        void IA.A()
        {
            throw new NotImplementedException();
        }
    }

    class BaseWithInterface : IA
    {
        public virtual void A()
        {
            
        }
    }

    class DerivedNoReimplementation : BaseWithInterface
    {
        public override void A()
        {
            base.A();
        }
    }
}
