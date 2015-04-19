using System;

namespace AutoTest.Core.DebugLog
{
	public class ConsoleWriter : IWriteDebugInfo
    {
        public void SetRecycleSize(long size) {}
        public void WriteError(string message) {
            Console.WriteLine(message);
        }
        public void WriteInfo(string message) {
            Console.WriteLine(message);
        }
        public void WriteDebug(string message) {
            Console.WriteLine(message);
        }
        public void WritePreProcessor(string message) {
            Console.WriteLine(message);
        }
        public void WriteDetail(string message) {
            Console.WriteLine(message);
        }
	}
}