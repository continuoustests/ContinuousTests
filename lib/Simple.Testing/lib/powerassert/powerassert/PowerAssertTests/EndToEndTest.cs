using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using NUnit.Framework;
using PowerAssert;
using PowerAssert.Infrastructure;
using PowerAssert.Infrastructure.Nodes;

namespace PowerAssertTests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestFixture]
    public class EndToEndTest
    {


        [Test]
        public void PrintResults()
        {
            int x = 11;
            int y = 6;
            DateTime d = new DateTime(2010, 3, 1);
            Expression<Func<bool>> expression = () => x + 5 == d.Month * y;
            Node constantNode = ExpressionParser.Parse(expression.Body);
            string[] strings = NodeFormatter.Format(constantNode);
            string s = string.Join(Environment.NewLine, strings);
            Console.Out.WriteLine(s);
        }

        [Test]
        [Ignore("This test will fail for demo purposes")]
        public void RunComplexExpression()
        {
            int x = 11;
            int y = 6;
            DateTime d = new DateTime(2010, 3, 1);
            PAssert.IsTrue(() => x + 5 == d.Month * y);
        }

        static int field = 11;
        [Test]
        [Ignore("This test will fail for demo purposes")]
        public void RunComplexExpressionWithStaticField()
        {
            int y = 6;
            DateTime d = new DateTime(2010, 3, 1);
            PAssert.IsTrue(() => field + 5 == d.Month * y);
        }

        [Test]
        [Ignore("This test will fail for demo purposes")]
        public void RunComplexExpression2()
        {
            string x = " lalalaa ";
            int i = 10;
            PAssert.IsTrue(() => x.Trim().Length == Math.Max(4, new int[] { 5, 4, i / 3, 2 }[0]));
        }

        [Test]
        [Ignore("This test will fail for demo purposes")]
        public void RunComplexExpression3()
        {
            List<int> l = new List<int> { 1, 2, 3 };
            bool b = false;
            PAssert.IsTrue(() => l[2].ToString() == (b ? "three" : "four"));
        } 
        
        [Test]
        [Ignore("This test will fail for demo purposes")]
        public void RunStringCompare()
        {
            string s = "hello, bobby";
            Tuple<string> t = new Tuple<string>("hello, Bobby");
            PAssert.IsTrue(() => s == t.Item1);
        }

        [Test]
        [Ignore("This test will fail for demo purposes")]
        public void RunRoundingEdgeCase()
        {
            double d = 3;
            int i = 2;


            PAssert.IsTrue(() => 4.5 == d + 3 / i);
        }

        [Test]
        [Ignore("This test will fail for demo purposes")]
        public void EqualsButNotOperatorEquals()
        {
            var t1 = new Tuple<string>("foo");
            var t2 = new Tuple<string>("foo");

            PAssert.IsTrue(() => t1 == t2);
        }

        [Test]
        [Ignore("This test will fail for demo purposes")]
        public void SequenceEqualButNotOperatorEquals()
        {
            object list = new List<int>{1,2,3};
            object array = new[] { 1, 2, 3 };
            PAssert.IsTrue(() => list == array);
        }

        [Test]
        [Ignore("This test will fail for demo purposes")]
        public void PrintingLinqStatements()
        {
            var list = Enumerable.Range(0, 150);
            PAssert.IsTrue(() => list.Where(x=>x%2==0).Sum() == 0);
        }

        [Test]
        [Ignore("This test will fail for demo purposes")]
        public void PrintingLinqExpressionStatements()
        {
            var list = Enumerable.Range(0, 150);
            PAssert.IsTrue(() => (from l in list where l%2==0 select l).Sum() == 0);
        }

        [Test]
        [Ignore("This test will fail for demo purposes")]
        public void PrintingComplexLinqExpressionStatements()
        {
            var list = Enumerable.Range(0, 5);
            PAssert.IsTrue(() => (from x in list from y in list where x > y select x + "," + y).Count() == 0);
        }

        [Test]
        [Ignore("This test will fail for demo purposes")]
        public void PrintingEnumerablesWithNulls()
        {
            var list = new List<int?>{1,2,null,4,5};
            PAssert.IsTrue(() => list.Sum() == null);
        }
        
        [Test]
        [Ignore("This test will fail for demo purposes")]
        public void PrintingUnaryNot()
        {
            var b = true;
            PAssert.IsTrue(() => !b);
        } 
        
        [Test]
        [Ignore("This test will fail for demo purposes")]
        public void PrintingUnaryNegate()
        {
            var b = 5;
            PAssert.IsTrue(() => -b==0);
        }

        [Test]
        [Ignore("This test will fail for demo purposes")]
        public void PrintingIsTest()
        {
            var b = new object();
            PAssert.IsTrue(() => b is string);
        }
    }
}
