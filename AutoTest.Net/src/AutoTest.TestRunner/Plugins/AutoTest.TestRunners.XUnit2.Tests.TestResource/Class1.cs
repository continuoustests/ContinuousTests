using System.Threading;
using Xunit;
using Xunit.Sdk;

namespace AutoTest.TestRunners.XUnit2.Tests.TestResource
{
    public class Class1
    {

        public Class1()
        {
            typeof (TestFrameworkProxy).ToString();
        }
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
