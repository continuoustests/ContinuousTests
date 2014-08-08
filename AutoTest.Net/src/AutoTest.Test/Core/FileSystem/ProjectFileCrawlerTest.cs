using AutoTest.Core.FileSystem;
using NUnit.Framework;
using System.IO;
using System.Linq;

namespace AutoTest.Test.Core
{
    [TestFixture]
    public class ProjectFileCrawlerTest
    {
        private string _path;
        private ProjectFileCrawler _fileCrawler;

        [SetUp]
        public void SetUp()
        {
            _path = Path.GetFullPath(Path.Combine("TestResources", "VS2008"));
            _fileCrawler = new ProjectFileCrawler();
        }

        [Test]
        public void Should_find_dlls_two_steps_down()
        {
            var dlls = _fileCrawler.FindParent(_path, ".config");
            // Does a foreach since it's 1 in visual studio and 2 in monodevelop
            // The important part is that all fetched are .config files
            foreach (var dll in dlls)
                dll.Extension.ShouldEqual(".config");
        }

        [Test]
        public void Should_return_null_if_no_fiels_found()
        {
            var files = _fileCrawler.FindParent(_path, ".ImPrettySureThisExtensionIsUnexistingOnThisSystem");
            files.Length.ShouldEqual(0);
        }
    }
}