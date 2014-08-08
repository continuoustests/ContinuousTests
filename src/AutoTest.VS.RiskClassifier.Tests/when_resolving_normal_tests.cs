using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Messages;
using NUnit.Framework;

namespace AutoTest.VS.RiskClassifier.Tests
{
    [TestFixture]
    public class when_resolving_normal_tests
    {
        [Test]
        public void normal_method_resolves_to_passing_when_not_there()
        {
            var status = FailingTests.GetStatusOf("System.Void Foo.Bar::Baz()");
            Assert.IsNotNull(status);
            Assert.AreEqual("", status.text);
            Assert.AreEqual(TestStatus.Pass, status.status);
            FailingTests.Clear();
        }

        //[Test]
        //public void normal_method_resolves_failing_when_present()
        //{
        //    FailingTests.UpdateWith(new CacheMessages() { FailedToAdd = new[] { new CacheTestMessage(), } });
        //    var status = FailingTests.GetStatusOf("System.Void Foo.Bar::Baz()");
        //    Assert.IsNotNull(status);
        //    Assert.AreEqual("", status.text);
        //    Assert.AreEqual(TestStatus.NotFound, status.status);
        //    FailingTests.Clear();
        //}
    }
}
