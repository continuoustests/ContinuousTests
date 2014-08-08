using System;
namespace AutoTest.Core.Configuration
{
	class DelayedConfigurer : IHandleDelayedConfiguration
	{
		public void AddRunFailedTestsFirstPreProcessor ()
		{
			BootStrapper.DIContainer.AddRunFailedTestsFirstPreProcessor();
		}
	}
}

