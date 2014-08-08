using System;
namespace AutoTest.Core.TestRunners
{
	public interface IDetermineIfAssemblyShouldBeTested
	{
		bool ShouldNotTestAssembly(string assembly);
	}
}

