using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.Core.BuildRunners;

namespace AutoTest.Test.Core.BuildRunners
{
    [TestFixture]
    public class MSTestSwitcharooTests
    {
        [Test]
        public void When_on_windows_and_containing_mstest_it_will_not_switch_library()
        {
            Assert.That(new MSTestSwitcharoo(PlatformID.Win32NT, "/path/to").IsGuyInCloset(project()), Is.False);
        }

        [Test]
        public void When_on_windows_and_containing_not_mstest_it_will_not_switch_library()
        {
            Assert.That(new MSTestSwitcharoo(PlatformID.Win32NT, "/path/to").IsGuyInCloset(notMSTestProject()), Is.False);
        }

        [Test]
        public void When_not_on_windows_and_not_containing_mstest_it_will_not_switch_library()
        {
            Assert.That(new MSTestSwitcharoo(PlatformID.Unix, "/path/to").IsGuyInCloset(notMSTestProject()), Is.False);
        }

        [Test]
        public void When_on_mac_and_containing_mstest_it_will_switch_library()
        {
            Assert.That(new MSTestSwitcharoo(PlatformID.MacOSX, "/path/to").IsGuyInCloset(project()), Is.True);
        }

        [Test]
        public void When_on_unix_and_containing_mstest_it_will_switch_library()
        {
            Assert.That(new MSTestSwitcharoo(PlatformID.Unix, "/path/to").IsGuyInCloset(project()), Is.True);
        }

        [Test]
        public void When_replacing_mstest_the_ms_test_reference_is_gone_while_our_impl_is_added()
        {
            var project = new MSTestSwitcharoo(PlatformID.Unix, "/path/to").PerformSwitch(this.project());
            Assert.That(project, Is.Not.StringContaining(singleLineReference()));
            Assert.That(project, Is.StringContaining(ourImplReference()));
        }

        private string project()
        {
            return "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                   "...." +
                   singleLineReference() +
                   "...";
        }

        private string notMSTestProject()
        {
            return "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                   "...." +
                   "...";
        }

        private string ourImplReference()
        {
            return
                "\t<Reference Include=\"Worst.Testing.Framework.Ever\">" + Environment.NewLine +
                "\t\t<HintPath>/path/to" + Path.DirectorySeparatorChar + "Worst.Testing.Framework.Ever.dll</HintPath>" + Environment.NewLine +
                "\t</Reference>";
        }

        private string singleLineReference()
        {
            return
                "<Reference Include=\"Microsoft.VisualStudio.QualityTools.UnitTestFramework, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a, processorArchitecture=MSIL\" />";
        }
    }
}
