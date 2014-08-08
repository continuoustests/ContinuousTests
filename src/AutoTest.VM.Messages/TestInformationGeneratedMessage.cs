using System;
using System.Collections.Generic;
using System.IO;
using AutoTest.Messages;

namespace AutoTest.VM.Messages
{
    public class TestInformationGeneratedMessage : IMessage
    {
        public Guid CorrelationId { get; private set; }
        public string Item;
        public Chain Test;
        public TestInformationGeneratedMessage(string item)
        {
            Item = item;
            CorrelationId = Guid.NewGuid();
        }

        public void WriteDataTo(BinaryWriter writer)
        {
            writer.Write(CorrelationId.ToString());
            writer.Write(Item);
            Test.WriteDataTo(writer);
        }

        public void SetDataFrom(BinaryReader reader)
        {
            CorrelationId = new Guid(reader.ReadString());
            Item = reader.ReadString();
            Test = new Chain();
            Test.ReadDataFrom(reader);
        }
    }

    public class Chain
    {
        public string Name;
        public string DisplayName;
        public bool IsSetup;
        public bool IsTest;
        public bool IsTeardown;
        public List<Chain> Children = new List<Chain>();
        public double TimeStart;
        public double TimeEnd;

        public Chain(string name, string displayName)
        {
            Name = name;
            DisplayName = displayName;
        }

        public Chain() {}

        public void WriteDataTo(BinaryWriter writer)
        {
            writer.Write(Name);
            writer.Write(DisplayName);
            writer.Write(IsSetup);
            writer.Write(IsTest);
            writer.Write(IsTeardown);
            writer.Write(TimeStart);
            writer.Write(TimeEnd);
            writer.Write(Children.Count);
            foreach(var c in Children)
            {
                c.WriteDataTo(writer);
            }
        }

        public void ReadDataFrom(BinaryReader reader)
        {
            Name = reader.ReadString();
            DisplayName = reader.ReadString();
            IsSetup = reader.ReadBoolean();
            IsTest = reader.ReadBoolean();
            IsTeardown = reader.ReadBoolean();
            TimeStart = reader.ReadDouble();
            TimeEnd = reader.ReadDouble();
            var childrenCount = reader.ReadInt32();
            for(int i=0;i<childrenCount;i++)
            {
                var child = new Chain();
                child.ReadDataFrom(reader);
                Children.Add(child);
            }
        }
    }
}