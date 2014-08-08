using System.IO;
namespace AutoTest.Messages
{
	public class TestRunMessage : IMessage, ICustomBinarySerializable
    {
        private TestRunResults _results;

        public TestRunResults Results { get { return _results; } }

        public TestRunMessage(TestRunResults results)
        {
            _results = results;
        }

		public void WriteDataTo(BinaryWriter writer)
		{
			_results.WriteDataTo(writer);
		}

		public void SetDataFrom(BinaryReader reader)
		{
			_results = new TestRunResults("", "", false, TestRunner.Any, new TestResult[] {});
			_results.SetDataFrom(reader);
		}
    }
}

