using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using System.Threading;

namespace AutoTest.TestRunners.XUnit.Tests.TestResource
{
    public class Class1
    {
        [Fact]
        public void Should_pass()
        {
            Thread.Sleep(20);
            Assert.Equal(1, 1);
        }

        [Fact]
        public void Should_fail()
        {
            Thread.Sleep(20);
            Assert.Equal(1, 10);
        }
    }
}
