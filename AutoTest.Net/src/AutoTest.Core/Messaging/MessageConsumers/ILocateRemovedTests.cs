using System;
using AutoTest.Messages;
using System.Collections.Generic;
namespace AutoTest.Core.Messaging.MessageConsumers
{
    public interface ILocateRemovedTests
    {
        TestRunResults SetRemovedTestsAsPassed(TestRunResults results, TestRunInfo[] infos);
        List<TestRunResults> RemoveUnmatchedRunInfoTests(TestRunResults[] restults, TestRunInfo[] infos);
    }
}
