using System;
namespace AutoTest.Core.Configuration
{
	public interface IHandleDelayedConfiguration
	{
		void AddRunFailedTestsFirstPreProcessor();
	}
}

