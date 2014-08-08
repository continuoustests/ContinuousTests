using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace AutoTest.TestRunners.XUnit.Tests.TestResource.Anothernamespace
{
    public class Class3
    {
        [Fact]
        public void Should_again_pass()
        {
            Assert.Equal(1, 1);
        }

        [Fact]
        public void Should_again_fail()
        {
            Assert.Equal(1, 2);
        }

        [Fact(Skip="meh")]
        public void Should_ignore()
        {
        }
    }
}
