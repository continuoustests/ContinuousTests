using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Test.Generics.Sample;

namespace Test.Generics.Sample
{
    public class BaseClass<T>{}
    public class StringClass : BaseClass<string> { }
    public class GenericClass<T> : BaseClass<T> { }
    public class GenericClass<T1, T2> : BaseClass<T1> { }
}

namespace Test.Target
{
    [TestFixture]
    public class GenericTests
    {
        [Test]
        public void BaseClassCtor_Test()
        {
            new BaseClass<int>();
            new BaseClass<double>();
        }

        [Test]
        public void BaseClassCtor_ReferenceTyped_Test()
        {
            new BaseClass<string>();
            new BaseClass<GenericTests>();
        }

        [Test]
        public void BaseClassCtor_ArrayTyped_Test()
        {
            new BaseClass<double[]>();
            new BaseClass<GenericTests[][][]>();
        }
    }
}
