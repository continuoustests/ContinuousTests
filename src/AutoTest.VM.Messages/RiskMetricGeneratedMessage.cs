using System;
using System.Collections.Generic;
using System.IO;
using AutoTest.Messages;

namespace AutoTest.VM.Messages
{
    public class RiskMetricGeneratedMessage : IMessage
    {
        public Guid CorrelationId { get; private set; }
        public string Signature { get; set; }
        public int NumberOfTests { get; set; }
        public List<string> Descriptors = new List<string>();
        public bool Found;
        public int NodeCount;
        public double TotalTime;
        public double TotalTimeUnder;
        public double AverageTime;
        public double AverageTimeUnder;
        public int CalledCount;
        public int GraphScore;
        public int TestsScore;
        public int Complexity;

        public int RiskMetric { get; set; }

        public void WriteDataTo(BinaryWriter writer)
        {
            writer.Write(CorrelationId.ToString());
            writer.Write(Signature);
            writer.Write(NumberOfTests);
            writer.Write(RiskMetric);
            writer.Write(Found);
            writer.Write(NodeCount);
            writer.Write(Descriptors.Count);
            writer.Write(TotalTime);
            writer.Write(TotalTimeUnder);
            writer.Write(AverageTime);
            writer.Write(AverageTimeUnder);
            writer.Write(CalledCount);
            writer.Write(GraphScore);
            writer.Write(TestsScore);
            writer.Write(Complexity);
            foreach (var t in Descriptors)
            {
                writer.Write(t);
            }
        }

        public void SetDataFrom(BinaryReader reader)
        {
            CorrelationId = new Guid(reader.ReadString());
            Signature = reader.ReadString();
            NumberOfTests = reader.ReadInt32();
            RiskMetric = reader.ReadInt32();
            Found = reader.ReadBoolean();
            NodeCount = reader.ReadInt32();
            var count = reader.ReadInt32();
            Descriptors = new List<string>();
            TotalTime = reader.ReadDouble();
            TotalTimeUnder = reader.ReadDouble();
            AverageTime = reader.ReadDouble();
            AverageTimeUnder = reader.ReadDouble();
            CalledCount = reader.ReadInt32();
            GraphScore = reader.ReadInt32();
            TestsScore = reader.ReadInt32();
            Complexity = reader.ReadInt32();
            for(var i=0;i<count;i++)
            {
                Descriptors.Add(reader.ReadString());
            }
       }
    }
}