using System;
using NUnit.Framework;
using AutoTest.Core;
namespace AutoTest.Test
{
	[TestFixture]
	public class MaxCmdLengthCalculatorTest
	{
		[Test]
		public void Should_return_max_argument_length()
		{
			var calculator = new MaxCmdLengthCalculator();
			var length = calculator.GetLength();
			Assert.IsTrue(length > 0);
		}
	}
}

