using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;
using PowerAssert.Infrastructure;
using PowerAssert.Infrastructure.Nodes;

namespace PowerAssertTests
{
    [TestFixture]
    public class ParserTest
    {
        static int field = 5;

        [Test]
        public void ParsePrimitiveConstant()
        {
            Expression<Func<int>> f = () => 5;
            ConstantNode constantNode = ExpressionParser.Parse(f.Body) as ConstantNode;
            Assert.AreEqual("5", constantNode.Text);
        }

        [Test]
        public void ParsePrimitiveStaticField()
        {
            Expression<Func<int>> f = () => field;
            ConstantNode constantNode = ExpressionParser.Parse(f.Body) as ConstantNode;
            Assert.AreEqual("field", constantNode.Text);
            Assert.AreEqual("5", constantNode.Value);
        }

        [Test]
        public void ParseStringConstant()
        {
            Expression<Func<string>> f = () => "foo";
            ConstantNode constantNode = ExpressionParser.Parse(f.Body) as ConstantNode;
            Assert.AreEqual("\"foo\"", constantNode.Text);
        }

        [Test]
        public void ParseMember()
        {
            int x = 5;
            Expression<Func<int>> f = () => x;
            Node constantNode = ExpressionParser.Parse(f.Body);
            Assert.AreEqual(new ConstantNode { Text = "x", Value = "5" }, constantNode);

        }

        [Test]
        public void ParseMemberAccess()
        {
            DateTime d = new DateTime(2010, 12, 25);
            Expression<Func<int>> f = () => d.Day;
            MemberAccessNode node = (MemberAccessNode)ExpressionParser.Parse(f.Body);

            MemberAccessNode expected = new MemberAccessNode
                                        {
                                            Container = new ConstantNode { Text = "d", Value = d.ToString() },
                                            MemberName = "Day",
                                            MemberValue = "25"
                                        };

            Assert.AreEqual(expected, node);
        }
        [Test]
        public void ParseMethodAccess()
        {
            string s = "hello";
            Expression<Func<string>> f = () => s.Substring(1);
            var node = ExpressionParser.Parse(f.Body);

            var expected = new MethodCallNode
                                        {
                                            Container = new ConstantNode { Text = "s", Value = @"""hello""" },
                                            MemberName = "Substring",
                                            MemberValue = @"""ello""",
                                            Parameters = new List<Node>() { new ConstantNode { Text = "1" } }

                                        };

            Assert.AreEqual(expected, node);
        }

        [Test]
        public void ParseMethodWithException()
        {
            Expression<Func<int>> f = () => ThrowException();
            var node = ExpressionParser.Parse(f.Body);

            var expected = new MethodCallNode
                                        {
                                            Container = new ConstantNode { Text = "PowerAssertTests.ParserTest"},
                                            MemberName = "ThrowException",
                                            MemberValue = @"DivideByZeroException: Attempted to divide by zero.",

                                        };

            Assert.AreEqual(expected, node);
        }

        int ThrowException()
        {
            var d = 0;
            return 1/d;
        }

        [Test]
        public void ParseConditional()
        {
            bool b = false;
            Expression<Func<int>> f = () => b ? 1 : 2;
            var node = ExpressionParser.Parse(f.Body);

            var expected = new ConditionalNode
                                        {
                                            Condition = new ConstantNode { Text = "b", Value = "False" },
                                            TrueValue = new ConstantNode { Text = "1" },
                                            FalseValue = new ConstantNode { Text = "2" },
                                        };

            Assert.AreEqual(expected, node);
        }

        [Test]
        public void ParseArrayCreateAndIndex()
        {
            Expression<Func<int>> f = () => new int[] { 1, 2, 3 }[1];
            var node = ExpressionParser.Parse(f.Body);

            var expected = new ArrayIndexNode
                               {
                                   Array = new NewArrayNode
                                   {
                                       Type = "int",
                                       Items = new List<Node>
                                       {
                                           new ConstantNode{Text= "1"},
                                           new ConstantNode{Text= "2"},
                                           new ConstantNode{Text= "3"},
                                       }
                                   },
                                   Index = new ConstantNode { Text = "1"},
                                   Value = "2"
                               };

            Assert.AreEqual(expected, node);
        }

        [Test]
        public void ParseCast()
        {
            double x = 5.1;
            Expression<Func<int>> f = () => (int)x;
            var node = ExpressionParser.Parse(f.Body);

            var expected = new UnaryNode
                               {
                                   Prefix = "(int)(",
                                   PrefixValue = "5",
                                   Operand = new ConstantNode(){Text = "x", Value = 5.1M.ToString()},
                                   Suffix = ")"
                               };

            Assert.AreEqual(expected, node);
        }

        [Test]
        public void ParseIsOperator()
        {
            object x = "xValue";
            Expression<Func<bool>> f = () => x is string;

            var node = ExpressionParser.Parse(f.Body);
            var expected = new BinaryNode()
                               {
                                   Left = new ConstantNode() {Text = "x", Value = "\"xValue\""},
                                   Operator = "is",
                                   Right = new ConstantNode() { Text = "string" },
                                   Value = "True"
                               };

            Assert.AreEqual(expected, node);
        }

        [Test]
        public void ParseUnaryNot()
        {
            var v = true;
            Expression<Func<bool>> f = () => !v;
            var node = ExpressionParser.Parse(f.Body);

            var expected = new UnaryNode
                               {
                                   Prefix = "!",
                                   PrefixValue = "False",
                                   Operand = new ConstantNode(){Text = "v", Value = "True"},
                               };

            Assert.AreEqual(expected, node);
        }

        [Test]
        public void ParseUnaryNegate()
        {
            var v = 5;
            Expression<Func<int>> f = () => -v;
            var node = ExpressionParser.Parse(f.Body);

            var expected = new UnaryNode
                               {
                                   Prefix = "-",
                                   PrefixValue = "-5",
                                   Operand = new ConstantNode(){Text = "v", Value = "5"},
                               };

            Assert.AreEqual(expected, node);
        }

        [Test]
        public void ParseEnumerableWithNulls()
        {
            var v = new List<int?>{1,2,null,4};
            Expression<Func<List<int?>>> f = () => v;
            var node = ExpressionParser.Parse(f.Body);

            var expected = new ConstantNode()
                               {
                                   Text = "v",
                                   Value = "{1, 2, null, 4}"

                               };

            Assert.AreEqual(expected, node);
        }

        [Test]
        public void ParseBinaryWithNullLeftHandSide()
        {
            string leftHandSide = null;
            Expression<Func<bool>> f = () => leftHandSide == "foo";
            var node = ExpressionParser.Parse(f.Body);
            var expected = new BinaryNode
            {
                Left = new ConstantNode { Text = "leftHandSide", Value = "null" },
                Right = new ConstantNode { Text = "\"foo\"" },
                Operator = "==",
                Value = "False"
            };

            Assert.AreEqual(expected, node);
        }

        [Test]
        public void ParseBinaryWithNullRightHandSide()
        {
            string rightHandSide = null;
            Expression<Func<bool>> f = () => "foo" == rightHandSide;
            var node = ExpressionParser.Parse(f.Body);
            var expected = new BinaryNode
            {
                Left = new ConstantNode { Text = "\"foo\"" },
                Right = new ConstantNode { Text = "rightHandSide", Value = "null" },
                Operator = "==",
                Value = "False"
            };

            Assert.AreEqual(expected, node);
        }
    }
}