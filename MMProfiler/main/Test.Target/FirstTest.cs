using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Test.Target
{


    //[TestFixture]
    //public class TestBase <X, Y>
    //{
    //    [SetUp]
    //    public void SetUp()
    //    {
    //    }
    //}

    //public class FirstTest : TestBase <string, int>
    //{
    //    [Test]
    //    public void TestZ()
    //    {
    //    }

    //    [Test]
    //    public void TestY()
    //    {
    //    }
    //}

    //TempClass[] MethodOnTestClass(TempClass @class)
    //{
    //    return null;
    //}

    //public void DeletegateCallback(int i)
    //{

    //}

    //public static double TestSimple(string data, byte[] moredata)
    //{

    //    return 0;
    //}

    //private string _testProperty;
    //public string TestProperty
    //{
    //    get { return _testProperty; }
    //    set { _testProperty = value; }
    //}

    //public T2 TestGenerics<T, T2>(T[] data, T2 data2)
    //{
    //    return default(T2);
    //}

    //public List<string> TestTypesWithGenerics(IEnumerable<List<int>> list)
    //{
    //    return null;
    //}

    //public void TestAction<T>(Action<T> action, T entity)
    //{
    //    action(entity);
    //}

    //public void TestRefArray(ref int[] data)
    //{

    //}

    //public void TestParams(int x, params object[] extras)
    //{
    //    return;
    //}

    //public int TestRef(ref string refString)
    //{
    //    return 0;
    //}

    //public int TestOut(out string outString)
    //{
    //    outString = string.Empty;
    //    return 0;
    //}

    //public delegate void DelegateTest(int s);


    //public void TestDelegate(DelegateTest dele)
    //{
    //    dele(1);
    //}

    //[CLSCompliant(false)]
    //public void VariableArguments(__arglist)
    //{
    //    ArgIterator iter = new ArgIterator(__arglist);
    //    for (int n = iter.GetRemainingCount(); n > 0; n--)
    //        Console.WriteLine(TypedReference.ToObject(iter.GetNextArg()));
    //}

    //public static class FirstTestExtension
    //{
    //    public static FirstTest TestExtension(this FirstTest s, string data)
    //    {
    //        //s.TestZ();
    //        return s;
    //    }
    //}
}
