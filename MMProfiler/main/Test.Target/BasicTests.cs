using System;
using NUnit.Framework;
using Test.Basic.Sample;

namespace Test.Basic.Sample
{
    public class Target
    {
        public Target(){}
        public Target(string a, int b, Target[] c){}
        public static void StaticMethod() { }
        public static Target StaticMethod(Target c) { return null; }
        public static int[] StaticMethod(string[] c) { return null; }
        public static int StaticMethod(out string a) { a = ""; return 0; }
        public static string StaticMethod(ref int a) { return string.Empty; }
        public string Data { get; set; }
        public void Method() { }
        public void Method(double d){}
    }

    public static class TargetExtensions
    {
        public static void Extension(this Target t) { }
        public static void Extension(this Target t, string a) { }
    }
}

namespace Test.Target
{
    [TestFixture]
    public class BasicTests
    {
        [Test]
        public void CtorTest()
        {
            new Basic.Sample.Target();    
        }

        [Test]
        public void CtorWithParamsTest()
        {
            new Basic.Sample.Target("", 1, null);
        }

        [Test]
        public void StaticMethodTest()
        {
            Basic.Sample.Target.StaticMethod();
        }

        [Test]
        public void StaticMethodWithParamsTest()
        {
            Basic.Sample.Target x = null;
            Basic.Sample.Target.StaticMethod(x);
        }

        [Test]
        public void StaticMethodWithArrayParamTest()
        {
            Basic.Sample.Target.StaticMethod(new[]{""});
        }

        [Test]
        public void StaticMethodWithOutParamTest()
        {
            var x = "";
            Basic.Sample.Target.StaticMethod(out x);
        }

        [Test]
        public void StaticMethodWithRefParamTest()
        {
            var i = 0;
            Basic.Sample.Target.StaticMethod(ref i);
        }

        [Test]
        public void MethodTest()
        {
            (new Basic.Sample.Target()).Method();
        }

        [Test]
        public void MethodWithParamsTest()
        {
            (new Basic.Sample.Target()).Method(1.09);
        }

        [Test]
        public void PropertyTest()
        {
            var x = new Basic.Sample.Target();
            x.Data = "";
            var y = x.Data;
        }

        [Test]
        public void ExtensionTest()
        {
            var x = new Basic.Sample.Target();
            x.Extension();
        }
        [Test]
        public void ExtensionWithParamsTest()
        {
            var x = new Basic.Sample.Target();
            x.Extension("");
        }
    }
}
