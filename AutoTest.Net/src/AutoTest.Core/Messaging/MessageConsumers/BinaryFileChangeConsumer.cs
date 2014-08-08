using System;
using System.Linq;
using System.Collections.Generic;
using AutoTest.Core.Configuration;
using AutoTest.Core.FileSystem;
using AutoTest.Core.Caching;
using AutoTest.Core.DebugLog;
using AutoTest.Messages;
using System.IO;
namespace AutoTest.Core.Messaging
{
	public class BinaryFileChangeConsumer : IConsumerOf<FileChangeMessage>
	{
		private IMessageBus _bus;
		private IRetrieveAssemblyIdentifiers _assemblyIdBuilder;
		private IConfiguration _configuration;
		
		public BinaryFileChangeConsumer(IMessageBus bus, IRetrieveAssemblyIdentifiers assemblyIdBuilder, IConfiguration config)
		{
			_bus = bus;
			_assemblyIdBuilder = assemblyIdBuilder;
			_configuration = config;
		}
		
		#region IConsumerOf[FileChangeMessage] implementation
		public void Consume(FileChangeMessage message)
		{
			if (_configuration.Providers != ".NET")
				return;
			
			Debug.WriteDebug("Consuming filechange through BinaryFileChangeConsumer");
			var assemblyFiles = new List<ChangedFile>();
			foreach (var file in message.Files)
			{
				if (file.FullName.ToLower().Contains(string.Format("{0}obj{0}", Path.DirectorySeparatorChar)))
					continue;
				if (file.Extension.ToLower().Equals(".exe"))
					assemblyFiles.Add(file);
				if (file.Extension.ToLower().Equals(".dll"))
					assemblyFiles.Add(file);
			}
			
			if (assemblyFiles.Count == 0)
				return;
			
			var files = new List<int>();
			var distinctList = new AssemblyChangeMessage();
			foreach (var file in assemblyFiles)
			{
				var id = _assemblyIdBuilder.GetAssemblyIdentifier(file.FullName);
				if (doesNotContain(id, files))
				{
					files.Add(id);
					distinctList.AddFile(file);
				}
			}
			_bus.Publish(new RunStartedMessage(distinctList.Files));
			_bus.Publish(distinctList);
		}
		#endregion
		
		private bool doesNotContain(int id, List<int> files)
		{
			foreach (var pair in files)
			{
				if (pair.Equals(id))
					return false;
			}
			return true;
		}
	}
}

