using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.Core.TestRunners.TestRunners;
using AutoTest.Core.Configuration;
using Rhino.Mocks;
using System.IO;
using AutoTest.Core.FileSystem;

namespace AutoTest.Test.Core.TestRunners.TestRunners
{
    [TestFixture]
    public class MSTestRunConfigParserTest
    {
        private MSTestRunConfigParser _parser;
        private IConfiguration _configuration;
        private IFileSystemService _fs;

        [SetUp]
        public void SetUp()
        {
            _configuration = MockRepository.GenerateMock<IConfiguration>();
            _fs = MockRepository.GenerateMock<IFileSystemService>();
            _parser = new MSTestRunConfigParser(_configuration, _fs);
        }

        [Test]
        public void Should_return_null_when_no_solution_present_return_null()
        {
            _configuration.Stub(c => c.WatchPath).Return("NotExisting.sln");
            _fs.Stub(c => c.FileExists("NotExisting.sln")).Return(false);

            var runConfig = _parser.GetConfig();
            Assert.IsNull(runConfig);
        }

        [Test]
        public void Should_return_null_when_no_vsdmi_content_return_null()
        {
            _configuration.Stub(c => c.WatchPath).Return("NotExisting.sln");
            _fs.Stub(c => c.FileExists("NotExisting.sln")).Return(true);
            _configuration.Stub(c => c.WatchPath).Return("NotExisting.vsmdi");
            _fs.Stub(c => c.FileExists("NotExisting.vsmdi")).Return(true);
            _fs.Stub(c => c.ReadFileAsText("NotExisting.vsmdi")).Return(null);

            var runConfig = _parser.GetConfig();
            Assert.IsNull(runConfig);
        }

        [Test]
        public void Should_return_null_when_no_vsdmi_present_return_null()
        {
            _configuration.Stub(c => c.WatchPath).Return("NotExisting.sln");
            _fs.Stub(c => c.FileExists("NotExisting.sln")).Return(true);
            _configuration.Stub(c => c.WatchPath).Return("NotExisting.vsmdi");
            _fs.Stub(c => c.FileExists("NotExisting.vsmdi")).Return(true);
            _fs.Stub(c => c.ReadFileAsText("NotExisting.vsmdi")).Return(null);

            var runConfig = _parser.GetConfig();
            Assert.IsNull(runConfig);
        }

        [Test]
        public void Should_return_null_when_invalid_vsdmi_content_return_null()
        {
            _configuration.Stub(c => c.WatchPath).Return("NotExisting.sln");
            _fs.Stub(c => c.FileExists("NotExisting.sln")).Return(true);
            _configuration.Stub(c => c.WatchPath).Return("NotExisting.vsmdi");
            _fs.Stub(c => c.FileExists("NotExisting.vsmdi")).Return(true);
            _fs.Stub(c => c.ReadFileAsText("NotExisting.vsmdi")).Return("");

            var runConfig = _parser.GetConfig();
            Assert.IsNull(runConfig);
        }

        [Test]
        public void Should_return_null_when_non_closed_storage_tag_return_null()
        {
            _configuration.Stub(c => c.WatchPath).Return("NotExisting.sln");
            _fs.Stub(c => c.FileExists("NotExisting.sln")).Return(true);
            _configuration.Stub(c => c.WatchPath).Return("NotExisting.vsmdi");
            _fs.Stub(c => c.FileExists("NotExisting.vsmdi")).Return(true);
            _fs.Stub(c => c.ReadFileAsText("NotExisting.vsmdi")).Return("blah blah blah storage=\"something");

            var runConfig = _parser.GetConfig();
            Assert.IsNull(runConfig);
        }

        [Test]
        public void Should_return_null_when_non_existing_testrunconfig_file_return_null()
        {
            _configuration.Stub(c => c.WatchPath).Return("NotExisting.sln");
            _fs.Stub(c => c.FileExists("NotExisting.sln")).Return(true);
            _configuration.Stub(c => c.WatchPath).Return("NotExisting.vsmdi");
            _fs.Stub(c => c.FileExists("NotExisting.vsmdi")).Return(true);
            _fs.Stub(c => c.ReadFileAsText("NotExisting.vsmdi")).Return("blah blah blah storage=\"something.testrunconfig\"");
            _fs.Stub(c => c.FileExists("something.testrunconfig")).Return(false);

            var runConfig = _parser.GetConfig();
            Assert.IsNull(runConfig);
        }

        [Test]
        public void Should_return_config_if_present()
        {
            var path = string.Format("{0}Somepath{0}", Path.DirectorySeparatorChar);
            var solution = path + "SomewhereExisting.sln";
            _configuration.Stub(c => c.WatchPath).Return(solution);
            _fs.Stub(c => c.FileExists(solution)).Return(true);
            _configuration.Stub(c => c.WatchPath).Return(path + "SomewhereExisting.vsmdi");
            _fs.Stub(c => c.FileExists(path + "SomewhereExisting.vsmdi")).Return(true);
            _fs.Stub(c => c.ReadFileAsText(path + "SomewhereExisting.vsmdi")).Return("blah blah blah storage=\"localtestrun.testrunconfig\"");
            _fs.Stub(c => c.FileExists(path + "localtestrun.testrunconfig")).Return(true);

            var runConfig = _parser.GetConfig();
            Assert.AreEqual(string.Format("{0}Somepath{0}localtestrun.testrunconfig", Path.DirectorySeparatorChar), runConfig);
        }
    }
}
