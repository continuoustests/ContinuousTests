using System;
using System.IO;
using AutoTest.Core.FileSystem;
using AutoTest.Core.Messaging;
using NUnit.Framework;
using Rhino.Mocks;
using AutoTest.Core.Configuration;
using AutoTest.Test.Core.Messaging.Fakes;
using AutoTest.Core.FileSystem.ProjectLocators;
using AutoTest.Core.Messaging.MessageConsumers;
using AutoTest.Core.Caching;
using AutoTest.Core.Caching.Projects;
using AutoTest.Messages;
using AutoTest.Core;

namespace AutoTest.Test.Core.Messaging.MessageConsumers
{
    [TestFixture]
    public class FileChangeConsumerTest
    {
        private IServiceLocator _services;
        private IMessageBus _bus;
        private IConfiguration _config;
        private FileChangeConsumer _subject;

        [SetUp]
        public void testSetup()
        {
            _services = MockRepository.GenerateMock<IServiceLocator>();
            _bus = MockRepository.GenerateMock<IMessageBus>();
            _config = MockRepository.GenerateMock<IConfiguration>();
            _config.Stub(x => x.Providers).Return(".NET");
            _subject = new FileChangeConsumer(_services, _bus, _config);
        }

        [Test]
        public void Should_not_publish_if_no_project_was_found()
        {
            var locator = new FakeProjectLocator(new ChangedFile[] { });
            _services.Stub(s => s.LocateAll<ILocateProjects>()).Return(new ILocateProjects[] { locator });
            var message = new FileChangeMessage();
            message.AddFile(new ChangedFile("somefile.cs"));
            _subject.Consume(message);
            _bus.AssertWasNotCalled(b => b.Publish<ProjectChangeMessage>(null), b => b.IgnoreArguments());
        }

        [Test]
        public void Should_publish_if_project_was_found()
        {
            var locator = new FakeProjectLocator(new ChangedFile[] { new ChangedFile("someproject.csproj") });
            _services.Stub(s => s.LocateAll<ILocateProjects>()).Return(new ILocateProjects[] { locator });
            var fileChange = new FileChangeMessage();
            fileChange.AddFile(new ChangedFile("asdf.cs"));
            _subject.Consume(fileChange);
            _bus.AssertWasCalled(b => b.Publish<ProjectChangeMessage>(null), b => b.IgnoreArguments());
        }

        [Test]
        public void When_finding_multiple_projects_all_should_be_published()
        {
            var locator = new FakeProjectLocator(new ChangedFile[]
                                                     {
                                                         new ChangedFile("FirstRootProject.csproj"),
                                                         new ChangedFile("someproject.csproj")
                                                     });
            _services.Stub(s => s.LocateAll<ILocateProjects>()).Return(new ILocateProjects[] { locator });
            var fileChange = new FileChangeMessage();
            fileChange.AddFile(new ChangedFile(Path.Combine("SubFolder", Path.Combine("SecondSub", "asdf.cs"))));
            _subject.Consume(fileChange);
            _bus.AssertWasCalled(b => b.Publish<ProjectChangeMessage>(Arg<ProjectChangeMessage>.Matches(p => p.Files.Length.Equals(2))));
        }

        [Test]
        public void When_finding_more_projects_by_multiple_locators_the_ones_closest_to_the_changed_file_should_be_published()
        {
            var locator1 = new FakeProjectLocator(new ChangedFile[]
                                                     {
                                                         new ChangedFile("FirstRootProject.csproj")
                                                     });
            var locator2 = new FakeProjectLocator(new ChangedFile[]
                                                     {
                                                         new ChangedFile(Path.Combine("SubFolder", "FirstSubProject.vbproj"))
                                                     });
            var locator3 = new FakeProjectLocator(new ChangedFile[]
                                                     {
                                                         new ChangedFile(Path.Combine("SubFolder", "SecondSubProject.xproj")),
                                                         new ChangedFile(Path.Combine("SubFolder", "ThirdSubProject.xproj"))
                                                     });
            _services.Stub(s => s.LocateAll<ILocateProjects>()).Return(new ILocateProjects[] { locator1, locator2, locator3 });
            var fileChange = new FileChangeMessage();
            fileChange.AddFile(new ChangedFile(Path.Combine("SubFolder", Path.Combine("SecondSub", "asdf.cs"))));
            _subject.Consume(fileChange);
            _bus.AssertWasCalled(b => b.Publish<ProjectChangeMessage>(
                                          Arg<ProjectChangeMessage>.Matches(
                                              p => p.Files.Length.Equals(3) &&
                                                   p.Files[0].Name.Equals("FirstSubProject.vbproj") &&
                                                   p.Files[1].Name.Equals("SecondSubProject.xproj") &&
                                                   p.Files[2].Name.Equals("ThirdSubProject.xproj"))));
        }
    }
}