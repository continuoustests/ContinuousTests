using System;
using AutoTest.Messages;

namespace AutoTest.VM.Messages
{
    public class VMSpawnedMessage : IMessage
    {
        public Guid CorrelationId { get; private set; }
        public int Port { get; private set; }
        public string NUnitTestRunner { get; private set; }
        public string MsTestRunner { get; private set; }
        public bool LoggingEnabled { get; private set; }
        public bool StartedPaused { get; private set; }

        public VMSpawnedMessage(Guid correlationId, int port, string nunitTestRunner, string msTestRunner, bool loggingEnabled, bool startedPaused)
        {
            CorrelationId = correlationId;
            Port = port;
            NUnitTestRunner = nunitTestRunner;
            MsTestRunner = msTestRunner;
            LoggingEnabled = loggingEnabled;
            StartedPaused = startedPaused;
        }

        public void SetDataFrom(System.IO.BinaryReader reader)
        {
            CorrelationId = new Guid(reader.ReadString());
            Port = reader.ReadInt32();
            NUnitTestRunner = reader.ReadString();
            MsTestRunner = reader.ReadString();
            LoggingEnabled = reader.ReadBoolean();
            StartedPaused = reader.ReadBoolean();
        }

        public void WriteDataTo(System.IO.BinaryWriter writer)
        {
            writer.Write((string)CorrelationId.ToString());
            writer.Write((int)Port);
            writer.Write((string)NUnitTestRunner);
            writer.Write((string)MsTestRunner);
            writer.Write((bool)LoggingEnabled);
            writer.Write(StartedPaused);
        }
    }
}
