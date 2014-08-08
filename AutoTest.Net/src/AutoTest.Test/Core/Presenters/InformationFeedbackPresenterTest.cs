using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.Core.Presenters;
using AutoTest.Test.Core.Presenters.Fakes;
using AutoTest.Core.Messaging;
using AutoTest.Test.Core.Caching.Projects.Fakes;
using Rhino.Mocks;
using AutoTest.Core.Configuration;
using System.Threading;
using AutoTest.Messages;

namespace AutoTest.Test.Core.Presenters
{
    [TestFixture]
    public class InformationFeedbackPresenterTest
    {
        private FakeInformationFeedbackView _view;
        private InformationFeedbackPresenter _presenter;
        private IMessageBus _bus;

        [SetUp]
        public void SetUp()
        {
            var locator = MockRepository.GenerateMock<IServiceLocator>();
            locator.Stub(x => x.LocateAll<IConsumerOf<ErrorMessage>>()).Return(new IConsumerOf<ErrorMessage>[] { });
            locator.Stub(x => x.LocateAll<IOverridingConsumer<ErrorMessage>>()).Return(new IOverridingConsumer<ErrorMessage>[] { });
            _bus = new MessageBus(locator);
            _view = new FakeInformationFeedbackView();
            _presenter = new InformationFeedbackPresenter(_bus);
            _presenter.View = _view;
        }

        [Test]
        public void Should_subscribe_to_information_messages()
        {
            _bus.Publish(new InformationMessage("some value"));
            waitForAsyncCall();
            _view.InformationMessage.ShouldEqual("some value");
        }

        [Test]
        public void Should_subscribe_to_warning_messages()
        {
            _bus.Publish(new WarningMessage("some warning"));
            waitForAsyncCall();
            _view.WarningMessage.ShouldEqual("some warning");
        }

        [Test]
        public void Should_subscribe_to_error_messages()
        {
            _bus.Publish(new ErrorMessage("some error"));
            waitForAsyncCall();
            _view.ErrorMessage.ShouldEqual("some error");
        }

        private void waitForAsyncCall()
        {
            Thread.Sleep(20);
        }
    }
}
