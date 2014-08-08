using AutoTest.Minimizer.Extensions;
using NUnit.Framework;

namespace AutoTest.Minimizer.Tests
{
    [TestFixture]
    public class when_checking_if_a_type_is_a_base_type_of_another_type : AssemblyTestFixture
    {
        [Test]
        public void unrelated_type_is_not_a_base_type()
        {
            var unrelated = assembly.GetTypeDefinition<UnrelatedType>();
            var derived = assembly.GetTypeDefinition<DerivedType>();
            Assert.IsFalse(unrelated.IsBaseTypeOf(derived));
        }
        
        [Test]
        public void same_type_is_base_type_of_self()
        {
            var type = assembly.GetTypeDefinition<DerivedType>();
            Assert.IsTrue(type.IsBaseTypeOf(type));
        }

        [Test]
        public void base_type_is_base_type_of_derived()
        {
            var basetype = assembly.GetTypeDefinition<BaseType>();
            var derivedtype = assembly.GetTypeDefinition<DerivedType>();
            Assert.IsTrue(basetype.IsBaseTypeOf(derivedtype));            
        }
    }

    class UnrelatedType
    {
        
    }

    class BaseType
    {
        
    } 

    class DerivedType : BaseType
    {
        
    }

    class GenericBaseType<T>
    {
        
    }
}