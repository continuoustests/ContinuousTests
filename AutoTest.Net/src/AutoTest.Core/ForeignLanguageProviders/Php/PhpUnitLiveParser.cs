using System;

namespace AutoTest.Core.ForeignLanguageProviders.Php
{
	public class PhpUnitLiveParser
	{
		public string Test { get; private set; }
        public string Class { get; private set; }
		public int TestsCompleted { get; private set; }

		public bool Parse(string line)
		{
			var modified = "";
            if (line.StartsWith("not ok"))
                modified = line.Substring(6, line.Length - 6);
            if (line.StartsWith("ok"))
                modified = line.Substring(2, line.Length - 2);
            if (modified == "")
                return false;
            var chunks = modified.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            if (chunks.Length < 3)
                return false;
            int testNum;
            if (!int.TryParse(chunks[0], out testNum))
                return false;
            Test = chunks[2];
            if (!Test.Contains("::") && chunks.Length > 3)
            	Test = chunks[3];
            var testChunks = Test.Split(new[] {"::"}, StringSplitOptions.RemoveEmptyEntries);
            if (testChunks.Length != 2)
                return false;
            Test = testChunks[1];
            Class = testChunks[0];
            TestsCompleted = testNum;
			return true;
		}
	}
}