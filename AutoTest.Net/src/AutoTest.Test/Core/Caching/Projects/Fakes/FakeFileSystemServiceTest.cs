using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace AutoTest.Test.Core.Caching.Projects.Fakes
{
    [TestFixture]
    public class FakeFileSystemServiceTest
    {
        [Test]
        public void Should_return_on_condition()
        {
            var fs = new FakeFileSystemService();
            fs.WhenCrawlingFor("some search pattern").Return("a project file");
            var files = fs.GetFiles("", "some search pattern");
            files[0].ShouldEqual("a project file");
        }

        [Test]
        public void Should_return_nothing_when_invalid_condition()
        {
            var fs = new FakeFileSystemService();
            fs.WhenCrawlingFor("some search pattern").Return("a project file");
            var files = fs.GetFiles("", "some other search pattern");
            files.Length.ShouldEqual(0);
        }
    }
}
