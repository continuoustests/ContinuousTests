using System.Threading;
using Xunit;

namespace AutoTest.TestRunners.XUnit2.Tests.TestResource
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
