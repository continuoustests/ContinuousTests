using System;
using System.Reflection;
using AutoTest.Core.BuildRunners;
using AutoTest.Core.Configuration;
using AutoTest.Core.Messaging;
using AutoTest.Test.TestObjects;
using NUnit.Framework;
using Castle.MicroKernel.Registration;
using System.Threading;
using AutoTest.Test.Core.Messaging.TestClasses;
using AutoTest.Core.TestRunners;
using Rhino.Mocks;
using AutoTest.Messages;

namespace AutoTest.Test.Core.Messaging
{
    [TestFixture]
    public class MessageBusTests : IDisposable
    {
        private DIContainer _container;
        private IMessageBus _bus;
        private string _threadException;
        
        public MessageBusTests()
        {
            
        }

        [SetUp]
        public void SetUp()
        {
            _container = new DIContainer();
            _container.Configure();
            _container.Container
                .Register(Component.For<IConsumerOf<StringMessage>>().ImplementedBy<Listener>())
                .Register(Component.For<IConsumerOf<StringMessage>>().Forward<IConsumerOf<IntMessage>>().ImplementedBy<BigListener>())
                .Register(Component.For<IOverridingConsumer<StringMessage>>().ImplementedBy<OverridingConsumer>().LifeStyle.Singleton);

            _bus = _container.Services.Locate<IMessageBus>();
            _threadException = "";
            _bus.OnErrorMessage += new EventHandler<ErrorMessageEventArgs>(_bus_OnErrorMessage);
        }

        [TearDown]
        public void TearDown()
        {
            _container.Dispose();
            if (_threadException.Length > 0)
                Assert.Fail("Caught thread exception{1}{0}", _threadException, Environment.NewLine);
        }

        void _bus_OnErrorMessage(object sender, ErrorMessageEventArgs e)
        {
            _threadException = e.Message.Error;
        }

        [Test]
        public void Should_be_able_to_send_message_to_bus_with_no_subscribers()
        {
            _bus.Publish(new Message("hi"));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Should_not_be_able_to_send_null_message()
        {
            _bus.Publish<IMessage>(null);
        }

        [Test]
        public void Should_be_able_to_send_string_message_and_have_it_delivered_to_all_consumers()
        {
            var message = new StringMessage();
            _bus.Publish(message);
            waitForAsyncCall();
            message.TimesConsumed.ShouldEqual(2);
        }

        [Test]
        public void Should_be_able_to_send_int_message()
        {
            var message = new IntMessage() {Consumed = false};
            _bus.Publish(message);
            waitForAsyncCall();
            message.Consumed.ShouldBeTrue();
        }

        [Test]
        public void Should_be_able_to_consume_information_message()
        {
            var consumer = new InformationMessageConsumer(_bus);
            var message = new InformationMessage("");
            _bus.Publish<InformationMessage>(message);
            waitForAsyncCall();
            consumer.InformationMessageEventWasCalled.ShouldBeTrue();

        }

        [Test]
        public void Should_be_able_to_consume_build_messages()
        {
            var consumer = new RunMessageConsumer(_bus);
            var message = new BuildRunMessage(new BuildRunResults(""));
            _bus.Publish<BuildRunMessage>(message);
            waitForAsyncCall();
            consumer.BuildMessageEventWasCalled.ShouldBeTrue();
        }

        [Test]
        public void Should_be_able_to_consume_testrun_messages()
        {
            var consumer = new RunMessageConsumer(_bus);
            var message = new TestRunMessage(new TestRunResults("", "", false, TestRunner.NUnit, new TestResult[] { }));
            _bus.Publish<TestRunMessage>(message);
            waitForAsyncCall();
            consumer.TestRunMessageEventWasCalled.ShouldBeTrue();
        }

        [Test]
        public void Should_be_able_to_consume_run_started_messages()
        {
            var consumer = new RunMessageConsumer(_bus);
            var message = new RunStartedMessage(null);
            _bus.Publish(message);
            waitForAsyncCall();
            consumer.RunStartedMessageEventWasCalled.ShouldBeTrue();
        }

        [Test]
        public void Should_be_able_to_consume_run_finished_messages()
        {
            var consumer = new RunMessageConsumer(_bus);
            var message = new RunFinishedMessage(null);
            _bus.Publish(message);
            waitForAsyncCall();
            consumer.RunFinishedMessageEventWasCalled.ShouldBeTrue();
        }

        [Test]
        public void Should_be_able_to_consume_warning_messages()
        {
            var consumer = new InformationMessageConsumer(_bus);
            var message = new WarningMessage("some warning");
            _bus.Publish(message);
            waitForAsyncCall();
            consumer.WarningMessageEventWasCalled.ShouldBeTrue();
        }

        [Test]
        public void Should_be_able_to_consume_error_messages()
        {
            // Remove error message override made in setup function
            _bus.OnErrorMessage -= _bus_OnErrorMessage;
            var consumer = new InformationMessageConsumer(_bus);
            var message = new ErrorMessage("some error");
            _bus.Publish(message);
            waitForAsyncCall();
            consumer.ErrorMessageEventCalled.ShouldBeTrue();
        }

        [Test]
        public void Should_be_able_to_consume_run_information_messages()
        {
            var consumer = new RunMessageConsumer(_bus);
            var message = new RunInformationMessage(InformationType.Build, "", "", "".GetType());
            _bus.Publish(message);
            waitForAsyncCall();
            consumer.RunInformationMessageEventCalled.ShouldBeTrue();
        }

        [Test]
        public void Should_publish_to_overriding_consumers()
        {
            var consumer = (OverridingConsumer)_container.Container.Resolve<IOverridingConsumer<StringMessage>>();
            _bus.Publish(new StringMessage());
            waitForAsyncCall();
            consumer.ConsumedMessage();
        }

        [Test]
        public void Should_terminate_already_running_consumer_when_publishing_new_message_to_overriding_consumers()
        {
            var consumer = (OverridingConsumer)_container.Container.Resolve<IOverridingConsumer<StringMessage>>();
            consumer.SetIsRunning(true);
            _bus.Publish(new StringMessage());
            waitForAsyncCall();
            consumer.ConsumedMessage();
            consumer.WasTerminated();
        }

        private void waitForAsyncCall()
        {
            Thread.Sleep(50);
        }

        //TODO: need unsubscribe
        public void Dispose()
        {
        }
    }
}
