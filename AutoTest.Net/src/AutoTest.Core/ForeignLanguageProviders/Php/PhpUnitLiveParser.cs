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
            var ns = chunks[2];
            if (!ns.Contains("::") && chunks.Length > 3)
            	ns = chunks[3];
            Test = NameFromNamespace(ns);
            if (Test == null)
                return false;
            Class = ClassFromNamespace(ns);
            TestsCompleted = testNum;
			return true;
		}

        public static string NameFromNamespace(string ns)
        {
            var test = ns;
            var testChunks = test.Split(new[] {"::"}, StringSplitOptions.RemoveEmptyEntries);
            if (testChunks.Length != 2)
                return null;
            return ToFriendlyName(testChunks[1].Replace("_", " "));
        }

        public static string ToFriendlyName(string name)
        {
            name = name.Replace("_", " ");
            return name.Substring(4, name.Length - 4);
        }

        public static string ClassFromNamespace(string ns)
        {
            var testChunks = ns.Split(new[] {"::"}, StringSplitOptions.RemoveEmptyEntries);
            if (testChunks.Length != 2)
                return null;
            return testChunks[0];
        }
	}
}
