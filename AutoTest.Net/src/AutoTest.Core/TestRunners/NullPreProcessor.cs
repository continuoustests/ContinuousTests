using System;
using AutoTest.Core.Messaging.MessageConsumers;
using AutoTest.Messages;
namespace AutoTest.Core.TestRunners
{
	class NullPreProcessor : IPreProcessTestruns
	{
		#region IPreProcessTestruns implementation
        public PreProcessedTesRuns PreProcess(PreProcessedTesRuns preProcessed)
		{
            return preProcessed;
		}

        public void RunFinished(TestRunResults[] results)
        {
        }
		#endregion
	}
}

