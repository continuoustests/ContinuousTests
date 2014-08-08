using System;
using NUnit.Framework;
using AutoTest.Core;
using AutoTest.Core.Configuration;
using AutoTest.Core.Messaging;
using AutoTest.Core.FileSystem;
using Rhino.Mocks;
using AutoTest.Messages;
using System.IO;
namespace AutoTest.Test.Core.Messaging.MessageConsumers
{
	[TestFixture]
	public class BinaryFileChangeConsumerTest
	{
		private IMessageBus _bus;
		private IRetrieveAssemblyIdentifiers _assemblyIdProvider;
		private BinaryFileChangeConsumer _consumer;
        private IConfiguration _config;
		private FileChangeMessage _files;
		
		[SetUp]
		public void SetUp()
		{
			_bus = MockRepository.GenerateMock<IMessageBus>();
			_config = MockRepository.GenerateMock<IConfiguration>();
            _config.Stub(x => x.Providers).Return(".NET");
			_assemblyIdProvider = MockRepository.GenerateMock<IRetrieveAssemblyIdentifiers>();
			_consumer = new BinaryFileChangeConsumer(_bus, _assemblyIdProvider, _config);
			_files = new FileChangeMessage();
		}
		
		[Test]
		public void Should_not_consume_regular_file()
		{
			_files.AddFile(new ChangedFile("someTextFile.txt"));
			_consumer.Consume(_files);
			_bus.AssertWasNotCalled(b => b.Publish<AssemblyChangeMessage>(null), b => b.IgnoreArguments());
		}
		
		[Test]
		public void Should_consume_exe_files()
		{
			_files.AddFile(new ChangedFile("myexefile.exe"));
			_consumer.Consume(_files);
			_bus.AssertWasCalled(b => b.Publish<AssemblyChangeMessage>(null), b => b.IgnoreArguments());
		}
		
		[Test]
		public void Should_consume_dll_files()
		{
			_files.AddFile(new ChangedFile("mylibrary.dll"));
			_consumer.Consume(_files);
			_bus.AssertWasCalled(b => b.Publish<AssemblyChangeMessage>(null), b => b.IgnoreArguments());
		}
		
		[Test]
		public void Should_consume_duplicate_assemblies_once()
		{
			_assemblyIdProvider.Stub(a => a.GetAssemblyIdentifier("")).IgnoreArguments().Return(0);
			_files.AddFile(new ChangedFile("mylibrary.dll"));
			_files.AddFile(new ChangedFile("another_location/mylibrary.dll"));
			_consumer.Consume(_files);
			_bus.AssertWasCalled(b => b.Publish<AssemblyChangeMessage>(Arg<AssemblyChangeMessage>.Matches(x => x.Files.Length.Equals(1))));
		}
		
		[Test]
		public void Should_ignore_assemblies_from_obj_folders()
		{
			_files.AddFile(new ChangedFile(string.Format("{0}obj{0}meh{0}myexefile.exe", Path.DirectorySeparatorChar)));
			_consumer.Consume(_files);
			_bus.AssertWasNotCalled(b => b.Publish<AssemblyChangeMessage>(null), b => b.IgnoreArguments());
		}
	}
}

