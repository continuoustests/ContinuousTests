using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.BuildRunners;
using AutoTest.Core.TestRunners;
using NUnit.Framework;
using AutoTest.Core.Configuration;
using Rhino.Mocks;
using AutoTest.Core.Messaging;
using AutoTest.Core.Presenters;
using AutoTest.Test.Core.Presenters.Fakes;
using System.Threading;
using AutoTest.Core.Caching;
using AutoTest.Messages;

namespace AutoTest.Test.Core.Presenters
{
    [TestFixture]
    public class RunFeedbackPresenterTest
    {
        private FakeRunFeedbackView _view;
        private RunFeedbackPresenter _presenter;
        private IMessageBus _bus;
        private FakeRunResultMerger _resultMerger;
		private IServiceLocator _locator;

        [SetUp]
        public void SetUp()
        {
            _locator = MockRepository.GenerateMock<IServiceLocator>();
            _bus = new MessageBus(_locator);
            _resultMerger = new FakeRunResultMerger();
            _view = new FakeRunFeedbackView();
            _presenter = new RunFeedbackPresenter(_bus, _resultMerger);
            _presenter.View = _view;
        }

        [Test]
        public void Should_subscribe_to_build_messages()
        {
            var message = new BuildRunMessage(new BuildRunResults(""));
            _bus.Publish<BuildRunMessage>(message);
            waitForAsyncCall(() => { return _view.BuildRunMessage != null; });
            _resultMerger.HasMergedBuildResults.ShouldBeTrue();
            _view.BuildRunMessage.ShouldBeTheSameAs(message);
        }

        [Test]
        public void Should_subscribe_to_testrun_messages()
        {
            var message = new TestRunMessage(new TestRunResults("", "", false, TestRunner.NUnit, null));
            _bus.Publish(message);
            waitForAsyncCall(() => { return _view.TestRunMessage != null; });
            _resultMerger.HasMergedTestResults.ShouldBeTrue();
            _view.TestRunMessage.ShouldBeTheSameAs(message);
        }

        [Test]
        public void Should_subscribe_to_file_change_messages()
        {
            var message = new FileChangeMessage();
            _bus.Publish(message);
            waitForAsyncCall(() => { return _view.FileChangeMessage != null; });
            _view.FileChangeMessage.ShouldBeTheSameAs(message);
        }

        [Test]
        public void Should_subscribe_to_run_started_messages()
        {
            var message = new RunStartedMessage(null);
            _bus.Publish(message);
            waitForAsyncCall(() => { return _view.RunStartedMessage != null; });
            _view.RunStartedMessage.ShouldBeTheSameAs(message);
        }

        [Test]
        public void Should_subscribe_to_run_finished_messages()
        {
            var message = new RunFinishedMessage(null);
            _bus.Publish(message);
            waitForAsyncCall(() => { return _view.RunFinishedMessage != null; });
            _view.RunFinishedMessage.ShouldBeTheSameAs(message);
        }

        [Test]
        public void Should_subscribe_to_run_information_messages()
        {
            var message = new RunInformationMessage(InformationType.Build, "", "", "".GetType());
            _bus.Publish(message);
            waitForAsyncCall(() => { return _view.RunInformationMessage != null; });
            _view.RunInformationMessage.ShouldBeTheSameAs(message);
        }
		
		[Test]
        public void Should_subscribe_to_run_external_command_messages()
        {
            var message = new ExternalCommandMessage("sender", "command");
            _bus.Publish(message);
            waitForAsyncCall(() => { return _view.ExternalCommandMessage != null; });
            _view.ExternalCommandMessage.ShouldBeTheSameAs(message);
        }

        [Test]
        public void Should_subscribe_to_live_test_feedback_messages()
        {
            var message = new LiveTestStatusMessage("", "", 0, 0, new LiveTestStatus[] { }, new LiveTestStatus[] { });
            _bus.Publish(message);
            waitForAsyncCall(() => { return _view.LiveTestStatusMessage != null; });
            _view.LiveTestStatusMessage.ShouldBeTheSameAs(message);
        }

        private void waitForAsyncCall(Func<bool> ok)
        {
			var now = DateTime.Now;
			while (!ok() && DateTime.Now.Subtract(now).TotalSeconds < 2)
            	Thread.Sleep(10);
        }
    }
}
