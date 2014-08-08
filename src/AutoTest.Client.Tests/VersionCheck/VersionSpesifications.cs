using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.Client.VersionCheck;

namespace AutoTest.Client.Tests.VersionCheck
{
    [TestFixture]
    public class When_reading_a_valid_version_xml_with_higher_version : VersionSpesification
    {
        [SetUp]
        public void Act()
        {
                var sb = new StringBuilder();
                sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                sb.AppendLine("<release>");
                sb.AppendLine("\t<version>100.0.5</version>");
                sb.AppendLine("\t<information><![CDATA[This is the information]]></information>");
                sb.AppendLine("</release>");
                readAndParseVersionXml(sb.ToString());
        }

        [Test]
        public void version_will_return_100_0_5()
        {
            Assert.That(_checker.Version, Is.EqualTo("100.0.5"));
        }

        [Test]
        public void information_will_return_information_text()
        {
            Assert.That(_checker.ReleaseNotes, Is.EqualTo("This is the information"));
        }

        [Test]
        public void we_have_released()
        {
            Assert.That(_checker.Released(), Is.True);
        }
    }

    [TestFixture]
    public class When_reading_a_valid_version_xml_with_the_same_version : VersionSpesification
    {
        [SetUp]
        public void Act()
        {
            var sb = new StringBuilder();
            var version = typeof(DidWeReleaseYet).Assembly.GetName().Version;
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<release>");
            sb.AppendLine("\t<version>" + string.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Build) + "</version>");
            sb.AppendLine("\t<information><![CDATA[This is the information]]></information>");
            sb.AppendLine("</release>");
            readAndParseVersionXml(sb.ToString());
        }

        [Test]
        public void we_have_not_released()
        {
            Assert.That(_checker.Released(), Is.False);
        }
    }

    [TestFixture]
    public class When_reading_an_invalid_version_xml : VersionSpesification
    {
        [SetUp]
        public void Act()
        {
            readAndParseVersionXml("invalid xml");
        }

        [Test]
        public void version_is_null()
        {
            Assert.That(_checker.Version, Is.Null);
        }

        [Test]
        public void release_notes_are_empty()
        {
            Assert.That(_checker.ReleaseNotes, Is.Empty);
        }

        [Test]
        public void we_have_not_released()
        {
            Assert.That(_checker.Released(), Is.False);
        }
    }

    public class VersionSpesification
    {
        protected DidWeReleaseYet _checker;

        protected void readAndParseVersionXml(string xml)
        {
            _checker = new DidWeReleaseYet(() => { return xml; });
        }
    }
}
