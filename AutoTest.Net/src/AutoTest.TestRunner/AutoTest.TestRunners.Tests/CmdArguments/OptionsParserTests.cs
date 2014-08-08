using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.TestRunners.Shared;
using System.Diagnostics;
using AutoTest.TestRunners.Shared.Options;
using System.Reflection;
using System.IO;

namespace AutoTest.TestRunners.Tests.CmdArguments
{
    [TestFixture]
    public class OptionsParserTests
    {
        private OptionsXmlReader _parser;

        [SetUp]
        public void SetUp()
        {
            _parser = new OptionsXmlReader(getPath("TestOptions.xml"));
            _parser.Parse();
        }

        private string getPath(string relativePath)
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return Path.Combine(path, relativePath);
        }

        [Test]
        public void When_file_does_not_exist_plugins_should_be_null()
        {
            var parser = new OptionsXmlReader("");
            parser.Parse();
            Assert.That(parser.Plugins, Is.Null);
            Assert.That(parser.IsValid, Is.False);
        }

        [Test]
        public void When_file_does_not_exist_options_should_be_null()
        {
            var parser = new OptionsXmlReader("");
            parser.Parse();
            Assert.That(parser.Options, Is.Null);
        }

        [Test]
        public void When_parse_is_successful_it_should_be_set_as_valid()
        {
            Assert.That(_parser.IsValid, Is.True);
        }

        [Test]
        public void Should_get_plugins()
        {
            Assert.That(_parser.Plugins.Count(), Is.EqualTo(2));

            Assert.That(_parser.Plugins.ElementAt(0).Assembly, Is.EqualTo(@"C:\Some\Path\Assembly.dll"));
            Assert.That(_parser.Plugins.ElementAt(0).Type, Is.EqualTo("This.Is.Full.Type.Name.For.Class.Implementing.IAutoTestNetTestRunner"));

            Assert.That(_parser.Plugins.ElementAt(1).Assembly, Is.EqualTo(@"C:\Some\Path\Assembly.dll"));
            Assert.That(_parser.Plugins.ElementAt(1).Type, Is.EqualTo("Some.Class.Name"));
        }

        [Test]
        public void Should_get_runners()
        {
            Assert.That(_parser.Options.TestRuns.Count(), Is.EqualTo(2));

            Assert.That(_parser.Options.TestRuns.ElementAt(0).ID, Is.EqualTo("nunit"));
            Assert.That(_parser.Options.TestRuns.ElementAt(1).ID, Is.EqualTo("another"));
        }

        [Test]
        public void Should_get_categories()
        {
            Assert.That(_parser.Options.TestRuns.ElementAt(0).Categories.Count(), Is.EqualTo(2));

            Assert.That(_parser.Options.TestRuns.ElementAt(0).Categories.ElementAt(0), Is.EqualTo("SomeTestCategory"));
            Assert.That(_parser.Options.TestRuns.ElementAt(0).Categories.ElementAt(1), Is.EqualTo("SomeOtherTestCategory"));
        }

        [Test]
        public void Should_get_assemblies()
        {
            Assert.That(_parser.Options.TestRuns.Count(), Is.EqualTo(2));
            Assert.That(_parser.Options.TestRuns.ElementAt(0).Assemblies.Count(), Is.EqualTo(4));
            Assert.That(_parser.Options.TestRuns.ElementAt(1).Assemblies.Count(), Is.EqualTo(1));

            Assert.That(_parser.Options.TestRuns.ElementAt(0).Assemblies.ElementAt(0).Assembly, Is.EqualTo(@"C:\my\testassembly.dll"));
            Assert.That(_parser.Options.TestRuns.ElementAt(0).Assemblies.ElementAt(0).IsVerified, Is.True);

            Assert.That(_parser.Options.TestRuns.ElementAt(0).Assemblies.ElementAt(1).Assembly, Is.EqualTo(@"C:\my\testassembly.dll"));
            Assert.That(_parser.Options.TestRuns.ElementAt(0).Assemblies.ElementAt(1).IsVerified, Is.False);

            Assert.That(_parser.Options.TestRuns.ElementAt(0).Assemblies.ElementAt(2).Assembly, Is.EqualTo(@"C:\my\anothernunitassembly.dll"));
            Assert.That(_parser.Options.TestRuns.ElementAt(0).Assemblies.ElementAt(2).IsVerified, Is.False);

            Assert.That(_parser.Options.TestRuns.ElementAt(0).Assemblies.ElementAt(3).Assembly, Is.EqualTo(@"C:\my\testassembly.dll"));
            Assert.That(_parser.Options.TestRuns.ElementAt(0).Assemblies.ElementAt(3).IsVerified, Is.False);

            Assert.That(_parser.Options.TestRuns.ElementAt(1).Assemblies.ElementAt(0).Assembly, Is.EqualTo(@"C:\my\other\testassembly.dll"));
        }

        [Test]
        public void Should_get_tests()
        {
            Assert.That(_parser.Options.TestRuns.ElementAt(0).Assemblies.ElementAt(0).Tests.Count(), Is.EqualTo(2));

            Assert.That(_parser.Options.TestRuns.ElementAt(0).Assemblies.ElementAt(0).Tests.ElementAt(0), Is.EqualTo("testassembly.class.test1"));
            Assert.That(_parser.Options.TestRuns.ElementAt(0).Assemblies.ElementAt(0).Tests.ElementAt(1), Is.EqualTo("testassembly.class.test2"));
            Assert.That(_parser.Options.TestRuns.ElementAt(0).Assemblies.ElementAt(1).Tests.ElementAt(0), Is.EqualTo("testassembly.class.test3"));
        }

        [Test]
        public void Should_get_members()
        {
            Assert.That(_parser.Options.TestRuns.ElementAt(0).Assemblies.ElementAt(0).Members.Count(), Is.EqualTo(2));

            Assert.That(_parser.Options.TestRuns.ElementAt(0).Assemblies.ElementAt(0).Members.ElementAt(0), Is.EqualTo("testassembly.class2"));
            Assert.That(_parser.Options.TestRuns.ElementAt(0).Assemblies.ElementAt(0).Members.ElementAt(1), Is.EqualTo("testassembly.class3"));
        }

        [Test]
        public void Should_get_namespaces()
        {
            Assert.That(_parser.Options.TestRuns.ElementAt(0).Assemblies.ElementAt(0).Namespaces.Count(), Is.EqualTo(2));

            Assert.That(_parser.Options.TestRuns.ElementAt(0).Assemblies.ElementAt(0).Namespaces.ElementAt(0), Is.EqualTo("testassembly.somenamespace1"));
            Assert.That(_parser.Options.TestRuns.ElementAt(0).Assemblies.ElementAt(0).Namespaces.ElementAt(1), Is.EqualTo("testassembly.somenamespace2"));
        }
    }
}
