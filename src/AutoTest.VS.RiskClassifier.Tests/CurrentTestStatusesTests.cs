using AutoTest.Messages;
using NUnit.Framework;

namespace AutoTest.VS.RiskClassifier.Tests
{
    [TestFixture]
    public class CurrentTestStatusesTests
    {
        [Test]
        public void finding_a_node_tht_doesnt_exist_returns_passing()
        {
            var status = CurrentTestStatuses.GetStatusOf("System.Void Foo.Bar::Baz()");
            Assert.IsNotNull(status);
            Assert.AreEqual("", status.text);
            Assert.AreEqual(TestStatus.Pass, status.status);
            CurrentTestStatuses.Clear();
        }

        [Test]
        public void method_with_parens_resolves_to_on_without()
        {
            var msg = new CacheMessages();
            msg.AddFailed(new CacheTestMessage("foo.dll", new TestResult(TestRunner.NUnit, TestRunStatus.Failed, "Foo.Bar.Baz(1,2,3)", "fail message")));
            CurrentTestStatuses.UpdateWith(msg);
            var status = CurrentTestStatuses.GetStatusOf("System.Void Foo.Bar::Baz(int, int, int)");
            Assert.IsNotNull(status);
            Assert.AreEqual("fail message", status.text);
            Assert.AreEqual(TestStatus.Fail, status.status);
            Assert.AreEqual("Foo.Bar.Baz(1,2,3)", status.Name);
            CurrentTestStatuses.Clear();
        }

        [Test]
        public void mspec_resolves_failing_when_present()
        {
            var msg = new CacheMessages();
            msg.AddFailed(new CacheTestMessage("foo.dll", new TestResult(TestRunner.MSpec, TestRunStatus.Failed, "Foo.Bar.Baz", "fail message")));
            CurrentTestStatuses.UpdateWith(msg);
            var status = CurrentTestStatuses.GetStatusOf("Machine.Specifications.It Foo.Bar.Baz::LOLS");
            Assert.IsNotNull(status);
            Assert.AreEqual("fail message", status.text);
            Assert.AreEqual(TestStatus.Fail, status.status);
            Assert.AreEqual("Foo.Bar.Baz", status.Name);
            CurrentTestStatuses.Clear();
        }

        [Test]
        public void simpletesting_resolves_failing_when_present()
        {
            var msg = new CacheMessages();
            msg.AddFailed(new CacheTestMessage("foo.dll", new TestResult(TestRunner.SimpleTesting, TestRunStatus.Failed, "Foo.Bar.Baz", "fail message")));
            CurrentTestStatuses.UpdateWith(msg);
            var status = CurrentTestStatuses.GetStatusOf("Simple.Testing.ClientFramework.Specification Foo.Bar::Baz()");
            Assert.IsNotNull(status);
            Assert.AreEqual("fail message", status.text);
            Assert.AreEqual(TestStatus.Fail, status.status);
            Assert.AreEqual("Foo.Bar.Baz", status.Name);
            CurrentTestStatuses.Clear();
        }

        [Test]
        public void normal_method_resolves_failing_when_present()
        {
            var msg = new CacheMessages();
            msg.AddFailed(new CacheTestMessage("foo.dll", new TestResult(TestRunner.NUnit, TestRunStatus.Failed, "Foo.Bar.Baz", "fail message")));
            CurrentTestStatuses.UpdateWith(msg);
            var status = CurrentTestStatuses.GetStatusOf("System.Void Foo.Bar::Baz()");
            Assert.IsNotNull(status);
            Assert.AreEqual("fail message", status.text);
            Assert.AreEqual(TestStatus.Fail, status.status);
            Assert.AreEqual("Foo.Bar.Baz", status.Name);
            CurrentTestStatuses.Clear();
        }


        [Test]
        public void normal_method_resolves_ignored_when_present()
        {
            var msg = new CacheMessages();
            msg.AddIgnored(new CacheTestMessage("foo.dll", new TestResult(TestRunner.NUnit, TestRunStatus.Ignored, "Foo.Bar.Baz", "ignore message")));
            CurrentTestStatuses.UpdateWith(msg);
            var status = CurrentTestStatuses.GetStatusOf("System.Void Foo.Bar::Baz()");
            Assert.IsNotNull(status);
            Assert.AreEqual("ignore message", status.text);
            Assert.AreEqual(TestStatus.Ignored, status.status);
            Assert.AreEqual("Foo.Bar.Baz", status.Name);
            CurrentTestStatuses.Clear();
        }

        [Test]
        public void normal_method_resolves_ignored_past_after_being_ignored()
        {
            var msg = new CacheMessages();
            msg.AddIgnored(new CacheTestMessage("foo.dll", new TestResult(TestRunner.NUnit, TestRunStatus.Ignored, "Foo.Bar.Baz", "ignore message")));
            CurrentTestStatuses.UpdateWith(msg);
            msg = new CacheMessages();
            msg.RemoveTest(new CacheTestMessage("foo.dll", new TestResult(TestRunner.NUnit, TestRunStatus.Ignored, "Foo.Bar.Baz")));
            CurrentTestStatuses.UpdateWith(msg);
            var status = CurrentTestStatuses.GetStatusOf("System.Void Foo.Bar::Baz()");
            Assert.AreEqual(TestStatus.Pass, status.status);
            Assert.AreEqual("", status.text);
        }

        [Test]
        public void normal_method_resolves_passed_after_being_failed()
        {
            var msg = new CacheMessages();
            msg.AddIgnored(new CacheTestMessage("foo.dll", new TestResult(TestRunner.NUnit, TestRunStatus.Failed, "Foo.Bar.Baz", "ignore message")));
            CurrentTestStatuses.UpdateWith(msg);
            msg = new CacheMessages();
            msg.RemoveTest(new CacheTestMessage("foo.dll", new TestResult(TestRunner.NUnit, TestRunStatus.Ignored, "Foo.Bar.Baz")));
            CurrentTestStatuses.UpdateWith(msg);
            var status = CurrentTestStatuses.GetStatusOf("System.Void Foo.Bar::Baz()");
            Assert.AreEqual(TestStatus.Pass, status.status);
            Assert.AreEqual("", status.text);
        }
    }
}
