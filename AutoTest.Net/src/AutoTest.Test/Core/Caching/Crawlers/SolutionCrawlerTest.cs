using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.Core.Caching.Crawlers;
using Rhino.Mocks;
using AutoTest.Core.FileSystem;
using AutoTest.Core.Messaging;
using AutoTest.Core.Caching;
using AutoTest.Messages;
using System.IO;
using AutoTest.Core.Caching.Projects;
using System.Reflection;

namespace AutoTest.Test.Core.Caching.Crawlers
{
    [TestFixture]
    public class SolutionCrawlerTest
    {
        [Test]
        public void Should_publish_message_on_invalid_file()
        {
            var fsService = MockRepository.GenerateMock<IFileSystemService>();
            var bus = MockRepository.GenerateMock<IMessageBus>();
            var cache = MockRepository.GenerateMock<ICache>();
            var crawler = new SolutionCrawler(fsService, bus, cache);
            crawler.Crawl("SomeInvalidSolutionFile.sln");
            
            bus.AssertWasCalled(b => b.Publish<InformationMessage>(null), b => b.IgnoreArguments());
        }

        [Test]
        public void Should_add_references_projects_to_cache_from_vs_2008_sln()
        {
            var solutionFile = getFullPath(string.Format("TestResources{0}VS2008{0}AutoTest.NET.sln", Path.DirectorySeparatorChar));
            var fsService = MockRepository.GenerateMock<IFileSystemService>();
            fsService.Stub(f => f.DirectoryExists(Path.GetDirectoryName(solutionFile))).Return(true);
            fsService.Stub(f => f.DirectoryExists(getPath(@"Tests", solutionFile))).Return(true);
            fsService.Stub(f => f.FileExists("")).IgnoreArguments().Return(true);
            fsService.Stub(f => f.FileExists("")).IgnoreArguments().Return(true);
            fsService.Stub(f => f.ReadFileAsText("")).IgnoreArguments().Return(File.ReadAllText(solutionFile));
            var bus = MockRepository.GenerateMock<IMessageBus>();
            var cache = MockRepository.GenerateMock<ICache>();
            var crawler = new SolutionCrawler(fsService, bus, cache);
            crawler.Crawl(solutionFile);

            cache.AssertWasCalled(c => c.Add<Project>(getPath(@"src\AutoTest.Core\AutoTest.Core.csproj", solutionFile)));
            cache.AssertWasCalled(c => c.Add<Project>(getPath(@"src\AutoTest.Console\AutoTest.Console.csproj", solutionFile)));
            cache.AssertWasCalled(c => c.Add<Project>(getPath(@"src\AutoTest.WinForms\AutoTest.WinForms.csproj", solutionFile)));
            cache.AssertWasCalled(c => c.Add<Project>(getPath(@"src\AutoTest.Test\AutoTest.Test.csproj", solutionFile)));
            cache.AssertWasCalled(c => c.Add<Project>(getPath(@"src\AutoTest.TestCore\AutoTest.TestCore.csproj", solutionFile)));
            cache.AssertWasCalled(c => c.Add<Project>(getPath(@"src\AutoTest.WinForms.Test\AutoTest.WinForms.Test.csproj", solutionFile)));
            cache.AssertWasCalled(c => c.Add<Project>(getPath(@"src\AutoTest.Console.Test\AutoTest.Console.Test.csproj", solutionFile)));
            cache.AssertWasCalled(c => c.Add<Project>(getPath(@"src\AutoTest.Messages\AutoTest.Messages.csproj", solutionFile)));
            cache.AssertWasNotCalled(c => c.Add<Project>(getPath(@"Tests", solutionFile)));
        }

        private string getFullPath(string relativePath)
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return Path.Combine(path, relativePath);
        }

        private string getPath(string path, string solutionFile)
        {
            path = path.Replace('\\', Path.DirectorySeparatorChar);
            return Path.Combine(Path.GetDirectoryName(solutionFile), path);
        }
    }
}
