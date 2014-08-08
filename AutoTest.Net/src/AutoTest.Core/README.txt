AutoTest.Core overview

Application boostraping
When the application starts it needs to bootstrap to set up the application
loop and class relation setup / IoC Container init.

	Configuration/BootStrapper.cs
	Configuration/DIContainer.cs

Application flow
Most parts are triggered from file changes. From there a chain of messages
are pushed through the system.

	Messaging (Bare with me it's a bit of a mess at the moment)
	Messaging/MessageBus.cs
	Consumers => Look for IConsumerOf<Foo> in DIContainer

	Main message flow
	1. Publish FileChangeMessage
	   FileSystem/DirectoryWatcher.cs@_batchTimer_Elapsed
	   
	2.1 Consume FileChangeMessage (.cs, .csproj and so forth)
		a) Messaging/MessageConsumers/FileChangeConsumers.cs
		   Publishes ProjectChangeMessage

		b) Messaging/MessageConsumers/ProjectChangeConsumer.cs
		   Consumes ProjectChangeMessage
		   Builds projects
		   Runs tests
		   Publishes:
		   		RunStartedMessage
		   		BuildRunMessage (Through BuildSessionRunner.cs)
		   		TestRunMessage => Test results
		   		RunInformationMessage => Textual information
		   		ErrorMessage => Errors
		   		RunFinishedMessage

	2.2 Consume FileChangeMessage (.exe, .dll)
		a) Messaging/MessageConsumers/BinaryFileChangeConsumer.cs
		   Publishes:
		   		RunStartedMessage
		   		AssemblyChangeMessage

		b) Messaging/MessageConsumers/AssemblyChangeConsumer.cs
		   Consumes AssemblyChangeMessage
		   Runs tests
		   Publishes:
		   		RunStartedMessage
		   		TestRunMessage => Test results
		   		RunInformationMessage => Textual information
		   		RunFinishedMessage

