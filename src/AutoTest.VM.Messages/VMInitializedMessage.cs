using System;
using AutoTest.Messages;
using System.IO;

namespace AutoTest.VM.Messages
{
    public class VMInitializedMessage : IMessage
    {
        public Guid CorrelationId { get; private set; }
        public int ProcessId { get; private set; }
        public int Port { get; private set; }
        public string NUnitTestRunner { get; private set; }
        public string MsTestRunner { get; private set; }
        public bool LoggingEnabled { get; private set; }
        public bool StartedPaused { get; private set; }

        public VMInitializedMessage(Guid correlationId, int processId, int port, string nunitTestRunner, string msTestRunner, bool loggingEnabled, bool startedPaused)
        {
            CorrelationId = correlationId;
            ProcessId = processId;
            Port = port;
            NUnitTestRunner = nunitTestRunner;
            MsTestRunner = msTestRunner;
            LoggingEnabled = loggingEnabled;
            StartedPaused = startedPaused;
        }

        public void SetDataFrom(BinaryReader reader)
        {
            CorrelationId = new Guid(reader.ReadString());
            ProcessId = reader.ReadInt32();
            Port = reader.ReadInt32();
            NUnitTestRunner = reader.ReadString();
            MsTestRunner = reader.ReadString();
            LoggingEnabled = reader.ReadBoolean();
            StartedPaused = reader.ReadBoolean();
        }

        public void WriteDataTo(BinaryWriter writer)
        {
            writer.Write(CorrelationId.ToString());
            writer.Write(ProcessId);
            writer.Write(Port);
            writer.Write(NUnitTestRunner);
            writer.Write(MsTestRunner);
            writer.Write(LoggingEnabled);
            writer.Write(StartedPaused);
        }
    }
}
