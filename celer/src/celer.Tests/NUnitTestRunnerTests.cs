using System;
using celer.Core;
using celer.Core.TestRunners;
using celer.Tests.NUnitData;
using NUnit.Framework;

namespace celer.Tests
{
    [TestFixture]
    public class nUnitTestCases    {
        private static ITestRunner GetRunnerFor<T>(string name)
        {
            var method = typeof(T).GetMethod(name);
            if (method == null)
                throw new MethodNotFoundException("method " + name + " not found on " + typeof(T).Name);
            return new NUnitTestRunner(method);
        }

        private static RunResult RunTest<T>(string name)
        {
            var runner = GetRunnerFor<T>(name);
            return runner.Run();
        }

        [Test]
        public void simple_test_with_no_assert()
        {
            var result = RunTest<NUnitTestFixture>("test_with_no_assert");
            Assert.IsTrue(result.WasRun);
            Assert.IsNull(result.Exception);
            Assert.IsTrue(result.Passed);
        }

        [Test]
        public void simple_test_with_passing_assert()
        {
            var result = RunTest<NUnitTestFixture>("test_with_passing_assert");
            Assert.IsTrue(result.WasRun);
            Assert.IsNull(result.Exception);
            Assert.IsTrue(result.Passed);
        }

        [Test]
        public void test_with_unexpected_exception()
        {
            var result = RunTest<NUnitTestFixture>("test_that_throws_unexpected_exception");
            Assert.IsTrue(result.WasRun);
            Assert.IsNotNull(result.Exception);
            Assert.AreEqual(typeof (ArgumentException),result.Exception.GetType());
            Assert.IsFalse(result.Passed);
        }

        [Test]
        public void test_with_failing_assert()
        {
            var result = RunTest<NUnitTestFixture>("test_with_failing_assert");
            Assert.IsTrue(result.WasRun);
            Assert.IsNotNull(result.Exception);
            Assert.IsFalse(result.Passed);            
        }

        [Test]
        public void test_with_expected_exception()
        {
            var result = RunTest<NUnitTestFixture>("test_with_expected_exception");
            Assert.IsTrue(result.WasRun);
            Assert.IsNotNull(result.Exception);
            Assert.IsTrue(result.Passed);
        }

        [Test]
        public void setup_gets_run()
        {
            
        }
    }
}
