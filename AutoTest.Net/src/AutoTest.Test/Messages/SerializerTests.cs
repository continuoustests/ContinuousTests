using System;
using NUnit.Framework;
using System.Net.Sockets;
using System.IO;
using AutoTest.Messages.Serializers;
using AutoTest.Messages;
using AutoTest.Core.Messaging;
namespace AutoTest.Test
{
	[TestFixture]
	public class SerializerTests
	{
		private CustomBinaryFormatter _formatter;
		
		[SetUp]
		public void SetUp()
		{
			_formatter = new CustomBinaryFormatter();
		}
		
		[Test]
		public void Should_serialize_build_run_message()
		{
			var results = new BuildRunResults("Project");
			results.AddError(new BuildMessage() { File = "file", LineNumber = 15, LinePosition = 20, ErrorMessage = "Error message" });
			results.AddWarning(new BuildMessage() { File = "file2", LineNumber = 35, LinePosition = 40, ErrorMessage = "Error message2" });
			results.SetTimeSpent(new TimeSpan(23567));
			var message = new BuildRunMessage(results);
			var output = serializeDeserialize<BuildRunMessage>(message);
			output.Results.Project.ShouldEqual("Project");
			output.Results.TimeSpent.ShouldEqual(new TimeSpan(23567));
			output.Results.ErrorCount.ShouldEqual(1);
			output.Results.Errors[0].File.ShouldEqual("file");
			output.Results.Errors[0].LineNumber.ShouldEqual(15);
			output.Results.Errors[0].LinePosition.ShouldEqual(20);
			output.Results.Errors[0].ErrorMessage.ShouldEqual("Error message");
			output.Results.WarningCount.ShouldEqual(1);
			output.Results.Warnings[0].File.ShouldEqual("file2");
			output.Results.Warnings[0].LineNumber.ShouldEqual(35);
			output.Results.Warnings[0].LinePosition.ShouldEqual(40);
			output.Results.Warnings[0].ErrorMessage.ShouldEqual("Error message2");
		}
		
		[Test]
		public void Should_serialize_error_message()
		{
			var message = new ErrorMessage("erro message");
			var output = serializeDeserialize<ErrorMessage>(message);
			output.Error.ShouldEqual("erro message");
		}
		
		[Test]
		public void Should_serialize_information_message()
		{
			var message = new InformationMessage("information message");
			var output = serializeDeserialize<InformationMessage>(message);
			output.Message.ShouldEqual("information message");
		}
		
		[Test]
		public void Should_serialize_run_finished_message()
		{
			var runreport = new RunReport();
			runreport.AddBuild("project 1", new TimeSpan(23), true);
			runreport.AddBuild("project 2", new TimeSpan(12), false);
			runreport.AddTestRun("project 2", "assembly", new TimeSpan(52), 12, 1, 2);
            runreport.WasAborted();
			var message = new RunFinishedMessage(runreport);
			var output = serializeDeserialize<RunFinishedMessage>(message);
			output.Report.NumberOfBuildsSucceeded.ShouldEqual(1);
			output.Report.NumberOfBuildsFailed.ShouldEqual(1);
			output.Report.RunActions[0].Project.ShouldEqual("project 1");
			output.Report.RunActions[0].Type.ShouldEqual(InformationType.Build);
			output.Report.RunActions[0].Succeeded.ShouldEqual(true);
			output.Report.RunActions[0].TimeSpent.ShouldEqual(new TimeSpan(23));
			output.Report.RunActions[1].Project.ShouldEqual("project 2");
			output.Report.RunActions[1].Type.ShouldEqual(InformationType.Build);
			output.Report.RunActions[1].Succeeded.ShouldEqual(false);
			output.Report.RunActions[1].TimeSpent.ShouldEqual(new TimeSpan(12));
            output.Report.Aborted.ShouldBeTrue();
			
			output.Report.NumberOfTestsPassed.ShouldEqual(12);
			output.Report.NumberOfTestsFailed.ShouldEqual(2);
			output.Report.NumberOfTestsIgnored.ShouldEqual(1);
			output.Report.RunActions[2].Project.ShouldEqual("project 2");
			output.Report.RunActions[2].Assembly.ShouldEqual("assembly");
			output.Report.RunActions[2].Type.ShouldEqual(InformationType.TestRun);
			output.Report.RunActions[2].Succeeded.ShouldEqual(false);
			output.Report.RunActions[2].TimeSpent.ShouldEqual(new TimeSpan(52));
		}
		
		[Test]
		public void Should_serialize_run_information_message()
		{
			var message = new RunInformationMessage(InformationType.TestRun, "project 1", "assembly", typeof(RunFinishedMessage));
			var output = serializeDeserialize<RunInformationMessage>(message);
			output.Project.ShouldEqual("project 1");
			output.Assembly.ShouldEqual("assembly");
			output.Type.ShouldEqual(InformationType.TestRun);
			output.Runner.ShouldEqual(typeof(RunFinishedMessage));
		}
		
		[Test]
		public void Should_serialize_run_started_message()
		{
			var files = new ChangedFile[] { new ChangedFile(System.Reflection.Assembly.GetExecutingAssembly().FullName) };
			var message = new RunStartedMessage(files);
			var output = serializeDeserialize<RunStartedMessage>(message);
			output.Files.Length.ShouldEqual(1);
			output.Files[0].Name.ShouldEqual(files[0].Name);
			output.Files[0].FullName.ShouldEqual(files[0].FullName);
			output.Files[0].Extension.ShouldEqual(files[0].Extension);
		}
		
		[Test]
		public void Should_serialize_warning_message()
		{
			var message = new WarningMessage("warning");
			var output = serializeDeserialize<WarningMessage>(message);
			output.Warning.ShouldEqual("warning");
		}
		
		[Test]
		public void Should_serialize_test_run_message()
		{
			var testResults = new TestResult[] { new TestResult(TestRunner.NUnit, TestRunStatus.Passed, "Test name", "message", new IStackLine[] { new StackLineMessage("method name", "file", 13) }, 34).SetDisplayName("display name") };
            var results = new TestRunResults("project 1", "assembly", false, TestRunner.NUnit, testResults);
			results.SetTimeSpent(new TimeSpan(12345));
			var message = new TestRunMessage(results);
			var output = serializeDeserialize<TestRunMessage>(message);
			output.Results.Project.ShouldEqual("project 1");
			output.Results.Assembly.ShouldEqual("assembly");
            output.Results.IsPartialTestRun.ShouldBeFalse();
			output.Results.TimeSpent.ShouldEqual(new TimeSpan(12345));
			output.Results.All.Length.ShouldEqual(1);
            output.Results.All[0].Runner.ShouldEqual(TestRunner.NUnit);
			output.Results.All[0].Status.ShouldEqual(TestRunStatus.Passed);
			output.Results.All[0].Name.ShouldEqual("Test name");
            output.Results.All[0].DisplayName.ShouldEqual("display name");
			output.Results.All[0].Message.ShouldEqual("message");
			output.Results.All[0].StackTrace[0].Method.ShouldEqual("method name");
			output.Results.All[0].StackTrace[0].File.ShouldEqual("file");
			output.Results.All[0].StackTrace[0].LineNumber.ShouldEqual(13);
            output.Results.All[0].TimeSpent.TotalMilliseconds.ShouldEqual(34);
		}
		
		[Test]
		public void Should_serialize_file_change_message()
		{
			var file = new ChangedFile(System.Reflection.Assembly.GetExecutingAssembly().FullName);
			var message = new FileChangeMessage();
			message.AddFile(file);
			var output = serializeDeserialize<FileChangeMessage>(message);
			output.Files.Length.ShouldEqual(1);
			output.Files[0].Name.ShouldEqual(file.Name);
			output.Files[0].FullName.ShouldEqual(file.FullName);
			output.Files[0].Extension.ShouldEqual(file.Extension);
		}
		
		[Test]
		public void Should_serialize_file_external_command_message()
		{
			var message = new ExternalCommandMessage("a sender", "a command");
			var output = serializeDeserialize<ExternalCommandMessage>(message);
			output.Sender.ShouldEqual("a sender");
			output.Command.ShouldEqual("a command");
		}

        [Test]
        public void Should_serialize_live_status_message()
        {
            var message = new LiveTestStatusMessage("assembly1", "currenttest", 10, 5, new LiveTestStatus[] { new LiveTestStatus("", new TestResult(TestRunner.Any, TestRunStatus.Failed, "")) }, new LiveTestStatus[] { new LiveTestStatus("", new TestResult(TestRunner.Any, TestRunStatus.Failed, "")) });
            var output = serializeDeserialize<LiveTestStatusMessage>(message);
            output.CurrentAssembly.ShouldEqual("assembly1");
            output.CurrentTest.ShouldEqual("currenttest");
            output.TotalNumberOfTests.ShouldEqual(10);
            output.TestsCompleted.ShouldEqual(5);
            output.FailedTests.Length.ShouldEqual(1);
            output.FailedButNowPassingTests.Length.ShouldEqual(1);
        }

        [Test]
        public void Should_serialize_live_status()
        {
            var message = new LiveTestStatus("assembly1", new TestResult(TestRunner.Any, TestRunStatus.Failed, "Test1"));
            var output = serializeDeserialize<LiveTestStatus>(message);
            output.Assembly.ShouldEqual("assembly1");
            output.Test.Name.ShouldEqual("Test1");
        }

        [Test]
        public void Should_serializeabort_message()
        {
            var message = new AbortMessage("some string");
            var output = serializeDeserialize(message);
            output.Reason.ShouldEqual("some string");
        }

        [Test]
        public void Should_serialize_Cache_message()
        {
            var message = new CacheMessages();
            message.AddError(new CacheBuildMessage("project1", new BuildMessage() { ErrorMessage = "message", File = "file", LineNumber = 1, LinePosition = 2 }));
            message.RemoveError(new CacheBuildMessage("project1", new BuildMessage() { ErrorMessage = "message to remove", File = "file", LineNumber = 1, LinePosition = 2 }));
            message.AddError(new CacheBuildMessage("project2", new BuildMessage() { ErrorMessage = "message2", File = "file2", LineNumber = 1, LinePosition = 2 }));
            message.AddWarning(new CacheBuildMessage("project3", new BuildMessage() { ErrorMessage = "warning message", File = "warning file", LineNumber = 3, LinePosition = 4 }));
            message.RemoveWarning(new CacheBuildMessage("project3", new BuildMessage() { ErrorMessage = "warning message to remove", File = "warning file", LineNumber = 3, LinePosition = 4 }));

            var failed = new TestResult(TestRunner.Any, TestRunStatus.Failed, "test name");
            failed.Message = "message";
            failed.StackTrace = new StackLineMessage[] { new StackLineMessage("stackmethod", "stack file", 5) };
            message.AddFailed(new CacheTestMessage("assembly1", failed));

            var failed2 = new TestResult(TestRunner.Any, TestRunStatus.Failed, "test name");
            failed2.Message = "message";
            failed2.StackTrace = new StackLineMessage[] { new StackLineMessage("stackmethod", "stack file", 5) };
            message.AddFailed(new CacheTestMessage("assembly2", failed2));

            var ignored = new TestResult(TestRunner.Any, TestRunStatus.Ignored, "test name ignored");
            ignored.Message = "message ignored";
            ignored.StackTrace = new StackLineMessage[] { new StackLineMessage("stackmethod ignored", "", 6) };
            message.AddIgnored(new CacheTestMessage("assembly2", ignored));

            ignored = new TestResult(TestRunner.Any, TestRunStatus.Ignored, "test name ignored");
            ignored.Message = "message ignored to remove";
            ignored.StackTrace = new StackLineMessage[] { new StackLineMessage("stackmethod ignored", "", 6) };
            message.RemoveTest(new CacheTestMessage("assembly2", ignored));

            var output = serializeDeserialize(message);
            Assert.AreEqual(2, output.ErrorsToAdd.Length);
            Assert.AreEqual("project1", output.ErrorsToAdd[0].Project);
            Assert.AreEqual("message", output.ErrorsToAdd[0].BuildItem.ErrorMessage);
            Assert.AreEqual("file", output.ErrorsToAdd[0].BuildItem.File);
            Assert.AreEqual(1, output.ErrorsToAdd[0].BuildItem.LineNumber);
            Assert.AreEqual(2, output.ErrorsToAdd[0].BuildItem.LinePosition);

            Assert.AreEqual(1, output.ErrorsToRemove.Length);
            Assert.AreEqual("message to remove", output.ErrorsToRemove[0].BuildItem.ErrorMessage);

            Assert.AreEqual(1, output.WarningsToAdd.Length);
            Assert.AreEqual("project3", output.WarningsToAdd[0].Project);
            Assert.AreEqual("warning message", output.WarningsToAdd[0].BuildItem.ErrorMessage);
            Assert.AreEqual("warning file", output.WarningsToAdd[0].BuildItem.File);
            Assert.AreEqual(3, output.WarningsToAdd[0].BuildItem.LineNumber);
            Assert.AreEqual(4, output.WarningsToAdd[0].BuildItem.LinePosition);

            Assert.AreEqual(1, output.WarningsToRemove.Length);
            Assert.AreEqual("warning message to remove", output.WarningsToRemove[0].BuildItem.ErrorMessage);

            Assert.AreEqual(2, output.FailedToAdd.Length);
            Assert.AreEqual("assembly1", output.FailedToAdd[0].Assembly);
            Assert.AreEqual(TestRunStatus.Failed, output.FailedToAdd[0].Test.Status);
            Assert.AreEqual("test name", output.FailedToAdd[0].Test.Name);
            Assert.AreEqual("message", output.FailedToAdd[0].Test.Message);
            Assert.AreEqual("stackmethod", output.FailedToAdd[0].Test.StackTrace[0].Method);
            Assert.AreEqual("stack file", output.FailedToAdd[0].Test.StackTrace[0].File);
            Assert.AreEqual(5, output.FailedToAdd[0].Test.StackTrace[0].LineNumber);

            Assert.AreEqual(1, output.IgnoredToAdd.Length);
            Assert.AreEqual("assembly2", output.IgnoredToAdd[0].Assembly);
            Assert.AreEqual(TestRunStatus.Ignored, output.IgnoredToAdd[0].Test.Status);
            Assert.AreEqual("test name ignored", output.IgnoredToAdd[0].Test.Name);
            Assert.AreEqual("message ignored", output.IgnoredToAdd[0].Test.Message);
            Assert.AreEqual("stackmethod ignored", output.IgnoredToAdd[0].Test.StackTrace[0].Method);
            Assert.AreEqual("", output.IgnoredToAdd[0].Test.StackTrace[0].File);
            Assert.AreEqual(6, output.IgnoredToAdd[0].Test.StackTrace[0].LineNumber);

            Assert.AreEqual(1, output.TestsToRemove.Length);
            Assert.AreEqual("message ignored to remove", output.TestsToRemove[0].Test.Message);
        }


        [Test]
        public void Should_serialize_Cache_message2()
        {
            var message = new CacheMessages();
            message.AddWarning(new CacheBuildMessage("project3", new BuildMessage() { ErrorMessage = "warning message", File = "warning file", LineNumber = 3, LinePosition = 4 }));

            var failed = new TestResult(TestRunner.Any, TestRunStatus.Failed, "AutoTest.Test.Core.Messaging.MessageConsumers.ProjectChangeConsumerTest.Should_run_tests");
            failed.Message = "Rhino.Mocks.Exceptions.ExpectationViolationException : ITestRunner.RunTests(any); Expected #1, Actual #0.";
            failed.StackTrace = new StackLineMessage[]
                    { 
                        new StackLineMessage("Rhino.Mocks.RhinoMocksExtensions.AssertWasCalled[T](T mock, Action`1 action, Action`1 setupConstraints)", "", 0),
                        new StackLineMessage("AutoTest.Test.Core.Messaging.MessageConsumers.ProjectChangeConsumerTest.Should_run_tests()", @"c:\Users\ack\src\AutoTest.Net\src\AutoTest.Test\Core\Messaging\MessageConsumers\ProjectChangeConsumerTest.cs", 122)
                    };
            message.AddFailed(new CacheTestMessage(@"C:\Users\ack\src\AutoTest.Net\src\AutoTest.Test\bin\Debug\AutoTest.Test.dll", failed));

            failed = new TestResult(TestRunner.Any, TestRunStatus.Failed, "AutoTest.Test.Core.Messaging.MessageConsumers.ProjectChangeConsumerTest.Should_run_builds");
            failed.Message = "Rhino.Mocks.Exceptions.ExpectationViolationException : IBuildRunner.RunBuild(\"C:\\Users\\ack\\src\\AutoTest.Net\\src\\AutoTest.Test\\bin\\Debug\\someProject.csproj\", \"C:\\Users\\ack\\src\\AutoTest.Net\\src\\AutoTest.Test\\bin\\Debug\\AutoTest.Test.dll\"); Expected #1, Actual #0.";
            failed.StackTrace = new StackLineMessage[]
                    { 
                        new StackLineMessage("Rhino.Mocks.RhinoMocksExtensions.AssertWasCalled[T](T mock, Action`1 action, Action`1 setupConstraints)", "", 0),
                        new StackLineMessage("Rhino.Mocks.RhinoMocksExtensions.AssertWasCalled[T](T mock, Action`1 action)", "", 0),
                        new StackLineMessage("AutoTest.Test.Core.Messaging.MessageConsumers.ProjectChangeConsumerTest.Should_run_builds()", @"c:\Users\ack\src\AutoTest.Net\src\AutoTest.Test\Core\Messaging\MessageConsumers\ProjectChangeConsumerTest.cs", 91)
                    };
            message.AddFailed(new CacheTestMessage(@"C:\Users\ack\src\AutoTest.Net\src\AutoTest.Test\bin\Debug\AutoTest.Test.dll", failed));

            failed = new TestResult(TestRunner.Any, TestRunStatus.Failed, "AutoTest.Test.Core.Messaging.MessageConsumers.ProjectChangeConsumerTest.Should_pre_process_run_information");
            failed.Message = "Rhino.Mocks.Exceptions.ExpectationViolationException : IPreProcessTestruns.PreProcess(any); Expected #1, Actual #0.";
            failed.StackTrace = new StackLineMessage[]
                    { 
                        new StackLineMessage("Rhino.Mocks.RhinoMocksExtensions.AssertWasCalled[T](T mock, Action`1 action, Action`1 setupConstraints)", "", 0),
                        new StackLineMessage("AutoTest.Test.Core.Messaging.MessageConsumers.ProjectChangeConsumerTest.Should_pre_process_run_information()", @"c:\Users\ack\src\AutoTest.Net\src\AutoTest.Test\Core\Messaging\MessageConsumers\ProjectChangeConsumerTest.cs", 160)
                    };
            message.AddFailed(new CacheTestMessage(@"C:\Users\ack\src\AutoTest.Net\src\AutoTest.Test\bin\Debug\AutoTest.Test.dll", failed));

            var output = serializeDeserialize(message);
            Assert.IsNotNull(output);
        }

        [Test]
        public void Should_serialize_chache_test_message()
        {
            var ignored = new TestResult(TestRunner.Any, TestRunStatus.Ignored, "test name ignored");
            ignored.Message = "message ignored";
            ignored.StackTrace = new StackLineMessage[] { new StackLineMessage("stackmethod ignored", "stack file ignored", 6) };
            var message = new CacheTestMessage("assembly", ignored);
            var output = serializeDeserialize(message);
            Assert.AreEqual("assembly", output.Assembly);
            Assert.AreEqual(TestRunStatus.Ignored, output.Test.Status);
            Assert.AreEqual("test name ignored", output.Test.Name);
            Assert.AreEqual("message ignored", output.Test.Message);
            Assert.AreEqual("stackmethod ignored", output.Test.StackTrace[0].Method);
            Assert.AreEqual("stack file ignored", output.Test.StackTrace[0].File);
            Assert.AreEqual(6, output.Test.StackTrace[0].LineNumber);
        }

        [Test]
        public void Should_serialize_chache_build_message()
        {
            var item = new BuildMessage() { ErrorMessage = "message", File = "file", LineNumber = 1, LinePosition = 2 };
            var message = new CacheBuildMessage("project", item);
            var output = serializeDeserialize(message);
            Assert.AreEqual("project", output.Project);
            Assert.AreEqual("message", output.BuildItem.ErrorMessage);
            Assert.AreEqual("file", output.BuildItem.File);
            Assert.AreEqual(1, output.BuildItem.LineNumber);
            Assert.AreEqual(2, output.BuildItem.LinePosition);
        }

		private T serializeDeserialize<T>(T message)
		{
			using (var memStream = new MemoryStream())
			{
				_formatter.Serialize(memStream, message);
				memStream.Seek(0, SeekOrigin.Begin);
				return (T) _formatter.Deserialize(memStream);
			}
		}
	}
}

