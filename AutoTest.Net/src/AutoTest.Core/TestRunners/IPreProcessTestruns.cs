using System;
using AutoTest.Core.Messaging.MessageConsumers;
using AutoTest.Messages;
namespace AutoTest.Core.TestRunners
{
	public interface IPreProcessTestruns
	{
        PreProcessedTesRuns PreProcess(PreProcessedTesRuns details);
        void RunFinished(TestRunResults[] results);
	}
}

