using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.TestRunners.Shared.Results;
using System.IO;

namespace AutoTest.TestRunners.Tests.Results
{
    [TestFixture]
    public class ResultXmlReaderTests
    {
        [Test]
        public void Should_read_xml()
        {
            var reader = new ResultXmlReader("Results.xml");
            var results = reader.Read();
            Assert.That(results.Count(), Is.EqualTo(5));
            Assert.That(results.ElementAt(0).Assembly, Is.EqualTo(@"C:\Users\ack\src\AutoTest.Net\src\AutoTest.Runners.NUnit.Tests.TestResource\bin\AutoTest.NET\AutoTest.Runners.NUnit.Tests.TestResource.dll"));
            Assert.That(results.ElementAt(0).Message, Is.EqualTo("failing test"));
            Assert.That(results.ElementAt(0).State, Is.EqualTo(AutoTest.TestRunners.Shared.Results.TestState.Failed));
            Assert.That(results.ElementAt(0).TestFixture, Is.EqualTo("AutoTest.Runners.NUnit.Tests.TestResource.Fixture1"));
            Assert.That(results.ElementAt(0).DurationInMilliseconds, Is.EqualTo(100));
            Assert.That(results.ElementAt(0).TestName, Is.EqualTo("AutoTest.Runners.NUnit.Tests.TestResource.Fixture1.Should_fail"));
            Assert.That(results.ElementAt(0).TestDisplayName, Is.Null);
            Assert.That(results.ElementAt(0).StackLines.Count(), Is.EqualTo(1));
            Assert.That(results.ElementAt(0).StackLines.ElementAt(0).File, Is.EqualTo(@"C:\Users\ack\src\AutoTest.Net\src\AutoTest.Runners.NUnit.Tests.TestResource\Fixture1.cs"));
            Assert.That(results.ElementAt(0).StackLines.ElementAt(0).Line, Is.EqualTo(21));
            Assert.That(results.ElementAt(0).StackLines.ElementAt(0).Method, Is.EqualTo("AutoTest.Runners.NUnit.Tests.TestResource.Fixture1.Should_fail()"));

            Assert.That(results.ElementAt(1).State, Is.EqualTo(AutoTest.TestRunners.Shared.Results.TestState.Ignored));
            Assert.That(results.ElementAt(1).StackLines.Count(), Is.EqualTo(1));

            Assert.That(results.ElementAt(4).Assembly, Is.EqualTo(@"C:\Users\ack\src\AutoTest.Net\src\AutoTest.Runners.NUnit.Tests.TestResource\bin\AutoTest.NET\AutoTest.Runners.NUnit.Tests.TestResource.dll"));
            Assert.That(results.ElementAt(4).DurationInMilliseconds, Is.EqualTo(250));
            Assert.That(results.ElementAt(4).Message, Is.EqualTo(""));
            Assert.That(results.ElementAt(4).State, Is.EqualTo(AutoTest.TestRunners.Shared.Results.TestState.Passed));
            Assert.That(results.ElementAt(4).TestFixture, Is.EqualTo("AutoTest.Runners.NUnit.Tests.TestResource.Fixture2"));
            Assert.That(results.ElementAt(4).TestName, Is.EqualTo("AutoTest.Runners.NUnit.Tests.TestResource.Fixture2.Should_also_pass_again"));
            Assert.That(results.ElementAt(4).TestDisplayName, Is.EqualTo("Alternative description"));
            Assert.That(results.ElementAt(4).StackLines.Count(), Is.EqualTo(0));
        }

        [Test]
        public void Should_parse_from_xml_string()
        {
            var xml = "<?xml version=\"1.0\" encoding=\"utf-16\"?><results><runner id=\"NUnit\"><assembly name=\"C:\\Users\\ack\\src\\Fohjin\\Fohjin.DDD.Example\\Test.Fohjin.DDD\\bin\\AutoTest.Net\\Test.Fohjin.DDD.dll\"><fixture name=\"Test.Fohjin.DDD.Bus.When_a_single_command_gets_published_to_the_bus_containing_an_sinlge_command_handler\"><test state=\"Passed\" name=\"Test.Fohjin.DDD.Bus.When_a_single_command_gets_published_to_the_bus_containing_an_sinlge_command_handler.Then_the_execute_method_on_the_returned_command_handler_is_invoked_with_the_provided_command\" duration=\"39,945\"><message><![CDATA[]]></message></test></fixture></assembly></runner></results>";
            using (var writer = new StringReader(xml))
            {
                var results = new ResultXmlReader(writer).Read();
                Assert.That(results.Count(), Is.EqualTo(1));
                var result = results.ElementAt(0);
                Assert.That(result.TestName, Is.EqualTo("Test.Fohjin.DDD.Bus.When_a_single_command_gets_published_to_the_bus_containing_an_sinlge_command_handler.Then_the_execute_method_on_the_returned_command_handler_is_invoked_with_the_provided_command"));
            }
        }
    }
}
