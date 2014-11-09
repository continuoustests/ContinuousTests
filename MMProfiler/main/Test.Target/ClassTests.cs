using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Test.Class.Sample;

namespace Test.Class.Sample
{
    public class BaseClass
    {
        public void MethodInBase(){}
    }
    public class ClassWithBase : BaseClass { }

    public abstract class AbstractClass
    {
        public void MethodInAbstract(){}
    }
    public class ClassWithAbstractBase : AbstractClass{}

    // innerclasses
}

namespace Test.Target
{
    [TestFixture]
    public class ClassTests
    {
        [Test]
        public void ClassWithBase_Ctor_Test()
        {
            new ClassWithBase();
        }

        [Test]
        public void ClassWithBase_CallBaseMethod_Test()
        {
            (new ClassWithBase()).MethodInBase();
        }

        [Test]
        public void ClassWithAbstractBase_Ctor_Test()
        {
            new ClassWithAbstractBase();
        }

        [Test]
        public void ClassWithAbstractBase_CallBaseMethod_Test()
        {
            (new ClassWithAbstractBase()).MethodInAbstract();
        }
    }
}
