using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.Messaging;

namespace AutoTest.Test.Core.Messaging.TestClasses
{
    class RunMessageConsumer
    {
        public bool RunStartedMessageEventWasCalled = false;
        public bool RunFinishedMessageEventWasCalled = false;
        public bool BuildMessageEventWasCalled = false;
        public bool TestRunMessageEventWasCalled = false;
        public bool RunInformationMessageEventCalled = false;

        public RunMessageConsumer(IMessageBus bus)
        {
            bus.OnRunStartedMessage += new EventHandler<RunStartedMessageEventArgs>(bus_OnRunStartedMessage);
            bus.OnRunFinishedMessage += new EventHandler<RunFinishedMessageEventArgs>(bus_OnRunFinishedMessage);
            bus.OnBuildMessage += new EventHandler<BuildMessageEventArgs>(bus_OnOnBuildMessage);
            bus.OnTestRunMessage += new EventHandler<TestRunMessageEventArgs>(bus_OnTestRunMessage);
            bus.OnRunInformationMessage += new EventHandler<RunInformationMessageEventArgs>(bus_OnRunInformationMessage);
        }

        void bus_OnRunInformationMessage(object sender, RunInformationMessageEventArgs e)
        {
            RunInformationMessageEventCalled = true;
        }

        void bus_OnRunStartedMessage(object sender, RunStartedMessageEventArgs e)
        {
            RunStartedMessageEventWasCalled = true;
        }

        void bus_OnRunFinishedMessage(object sender, RunFinishedMessageEventArgs e)
        {
            RunFinishedMessageEventWasCalled = true;
        }

        void bus_OnOnBuildMessage(object sender, BuildMessageEventArgs e)
        {
            BuildMessageEventWasCalled = true;
        }

        void bus_OnTestRunMessage(object sender, TestRunMessageEventArgs e)
        {
            TestRunMessageEventWasCalled = true;
        }
    }
}
