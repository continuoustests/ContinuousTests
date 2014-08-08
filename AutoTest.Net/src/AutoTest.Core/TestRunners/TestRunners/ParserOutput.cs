using System;
using System.Collections.Generic;
namespace AutoTest.Core.TestRunners.TestRunners
{
	class ParserOutput
	{
		public string Project { get; private set; }
		public string Assembly { get; private set; }
		public TestResult[] Result { get; private set; }
		
		public ParserOutput(string project, string assembly, TestResult[] result)
		{
			Project = project;
			Assembly = assembly;
			Result = result;
		}
	}
}

