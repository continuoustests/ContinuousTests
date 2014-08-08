using AutoTest.Messages;
using System.IO;

namespace AutoTest.Messages
{
    public class CacheTestMessage : ICustomBinarySerializable
    {
        public string Assembly { 
            get; 
            private set; 
        }
        public TestResult Test { get;
            private set; }

        public CacheTestMessage(string assembly, TestResult result)
        {
            Assembly = assembly;
            Test = result;
        }

        public void SetDataFrom(BinaryReader reader)
        {
            Assembly = reader.ReadString();
            Test = new TestResult(TestRunner.Any, TestRunStatus.Passed, "");
            Test.SetDataFrom(reader);
        }

        public void WriteDataTo(BinaryWriter writer)
        {
            writer.Write(Assembly);
            Test.WriteDataTo(writer);
        }

        public override bool Equals(object obj)
        {
            return GetHashCode().Equals(obj.GetHashCode());
        }

        public override int GetHashCode()
        {
            // Overflow is fine, just wrap
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + (Assembly == null ? 0 : Assembly.GetHashCode());
                hash = hash * 23 + (Test == null ? 0 : Test.GetHashCode());
                return hash;
            }
        }
    }
}
