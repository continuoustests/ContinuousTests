using System;
using System.Linq;
using NUnit.Framework;
using PowerAssert.Infrastructure;
using PowerAssert.Infrastructure.Nodes;

namespace PowerAssertTests
{
    [TestFixture]
    public class NodeFormatterTest
    {
        [Test]
        public void FormatConstant()
        {
            string[] s = NodeFormatter.Format(new ConstantNode {Text = "5", Value = null});
            AssertLines(new[] { "5" }, s);
        }

        [Test]
        public void FormatOperator()
        {
            string[] s = NodeFormatter.Format(new BinaryNode
                                              {
                                                  Operator = "==",
                                                  Value = "false",
                                                  Left = new ConstantNode {Text = "5"},
                                                  Right = new ConstantNode {Text = "6"}
                                              });

            string[] expected = {
                                    "5 == 6",
                                    "  false"
                                };

            AssertLines(expected, s);
        }

        [Test]
        public void FormatTwoOperators()
        {
            string[] s = NodeFormatter.Format(new BinaryNode
                                              {
                                                  Operator = "==",
                                                  Value = "false",
                                                  Left = new ConstantNode {Text = "31"},
                                                  Right = new BinaryNode
                                                  {
                                                      Operator = "*",
                                                      Value = "30",
                                                      Left = new ConstantNode { Text = "5" },
                                                      Right = new ConstantNode { Text = "6" }
                                                  }
                                              });

            string[] expected = {
                                    "31 == 5 * 6",
                                    "   │    ∙",
                                    "   │    30",
                                    "   false"
                                };

            AssertLines(expected, s);
        }

        static void AssertLines(string[] expected, string[] actual)
        {
            Assert.AreEqual(string.Join(Environment.NewLine, expected), string.Join(Environment.NewLine, actual));
        }
    }
}