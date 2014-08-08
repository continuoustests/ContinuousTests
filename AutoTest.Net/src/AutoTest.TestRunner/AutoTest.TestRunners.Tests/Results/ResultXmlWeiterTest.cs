using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.TestRunners.Shared.Results;
using System.IO;
using System.Reflection;

namespace AutoTest.TestRunners.Tests.Results
{
    [TestFixture]
    public class ResultXmlWeiterTest
    {
        [Test]
        public void Should_write_result()
        {
            var result = new List<TestResult>();
            result.Add(new TestResult("nunit", @"C:\Users\ack\src\AutoTest.Net\src\AutoTest.Runners.NUnit.Tests.TestResource\bin\AutoTest.NET\AutoTest.Runners.NUnit.Tests.TestResource.dll",
                "AutoTest.Runners.NUnit.Tests.TestResource.Fixture1", 100, "AutoTest.Runners.NUnit.Tests.TestResource.Fixture1.Should_fail", Shared.Results.TestState.Failed,
                "failing test"));
            result[0].AddStackLine(new StackLine() { Method = "AutoTest.Runners.NUnit.Tests.TestResource.Fixture1.Should_fail()",
                File = @"C:\Users\ack\src\AutoTest.Net\src\AutoTest.Runners.NUnit.Tests.TestResource\Fixture1.cs", Line = 21 });

            result.Add(new TestResult("nunit", @"C:\Users\ack\src\AutoTest.Net\src\AutoTest.Runners.NUnit.Tests.TestResource\bin\AutoTest.NET\AutoTest.Runners.NUnit.Tests.TestResource.dll",
                "AutoTest.Runners.NUnit.Tests.TestResource.Fixture1", 20, "AutoTest.Runners.NUnit.Tests.TestResource.Fixture1.Should_ignore", Shared.Results.TestState.Ignored,
                "ignored test"));
            result[1].AddStackLine(new StackLine() { Method = "AutoTest.Runners.NUnit.Tests.TestResource.Fixture1.Should_ignore()",
                File = @"C:\Users\ack\src\AutoTest.Net\src\AutoTest.Runners.NUnit.Tests.TestResource\Fixture1.cs", Line = 27 });

            result.Add(new TestResult("nunit", @"C:\Users\ack\src\AutoTest.Net\src\AutoTest.Runners.NUnit.Tests.TestResource\bin\AutoTest.NET\AutoTest.Runners.NUnit.Tests.TestResource.dll",
                "AutoTest.Runners.NUnit.Tests.TestResource.Fixture1", 20, "AutoTest.Runners.NUnit.Tests.TestResource.Fixture1.Should_pass", Shared.Results.TestState.Passed, ""));

            result.Add(new TestResult("nunit", @"C:\Users\ack\src\AutoTest.Net\src\AutoTest.Runners.NUnit.Tests.TestResource\bin\AutoTest.NET\AutoTest.Runners.NUnit.Tests.TestResource.dll",
                "AutoTest.Runners.NUnit.Tests.TestResource.Fixture2", 20, "AutoTest.Runners.NUnit.Tests.TestResource.Fixture2.Should_also_pass", Shared.Results.TestState.Passed, ""));

            result.Add(new TestResult("nunit", @"C:\Users\ack\src\AutoTest.Net\src\AutoTest.Runners.NUnit.Tests.TestResource\bin\AutoTest.NET\AutoTest.Runners.NUnit.Tests.TestResource.dll",
                "AutoTest.Runners.NUnit.Tests.TestResource.Fixture2", 250, "AutoTest.Runners.NUnit.Tests.TestResource.Fixture2.Should_also_pass_again", "Alternative description", Shared.Results.TestState.Passed, ""));

            var file = Path.GetTempFileName();
            var writer = new ResultsXmlWriter(result);
            writer.Write(file);

            var original = File.ReadAllText(file).Replace("\r\n", "\n");
            var generated = File.ReadAllText(getPath("Results.xml")).Replace("\r\n", "\n");

            Assert.That(original, Is.EqualTo(generated));
        }

        private string getPath(string relativePath)
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return Path.Combine(path, relativePath);
        }
    }
}
