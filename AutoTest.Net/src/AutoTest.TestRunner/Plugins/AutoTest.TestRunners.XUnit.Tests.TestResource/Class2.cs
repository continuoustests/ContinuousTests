using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace AutoTest.TestRunners.XUnit.Tests.TestResource
{
    public class Class2
    {
        [Fact]
        public void Anothger_passing_test()
        {
            Assert.Equal(1, 1);
        }
    }
}
