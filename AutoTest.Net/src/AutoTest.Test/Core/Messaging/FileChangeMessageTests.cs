using System;
using AutoTest.Core.Messaging;
using NUnit.Framework;
using AutoTest.Core.FileSystem;
using System.IO;
using AutoTest.Messages;

namespace AutoTest.Test.Core.Messaging
{
    [TestFixture]
    public class FileChangeMessageTests : MessageTestBase<FileChangeMessage>
    {
        protected override FileChangeMessage CreateMessage()
        {
            var fileChange = new FileChangeMessage();
            fileChange.AddFile(new ChangedFile(Path.GetFullPath("App.config")));
            return fileChange;
        }

        [Test]
        public override void Should_be_immutable()
        {
            should_be_immutable_test();
        }

        [Test]
        public void Should_have_file_info() 
        { 
            var message = CreateMessage(); 
            message.Files[0].FullName.ShouldEqual(Path.GetFullPath("App.config"));
            message.Files[0].Extension.ShouldEqual(".config");
            message.Files[0].Name.ShouldEqual("App.config");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SHould_not_allow_null_info()
        {
            new ChangedFile(null);
        }

        [Test]
        public void Should_initialize_from_strings()
        {
            var message = new FileChangeMessage();
            message.AddFile(new ChangedFile(Path.Combine("2", "1.1")));
            message.Files[0].FullName.ShouldEqual(Path.Combine("2", "1.1"));
            message.Files[0].Name.ShouldEqual("1.1");
            message.Files[0].Extension.ShouldEqual(".1");
        }
    }
}