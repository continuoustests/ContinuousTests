using System;
using System.Collections.Generic;
using System.IO;
using AutoTest.Messages;

namespace AutoTest.VM.Messages
{
    //TODO GREG: REFACTOR THIS SHIT TO USE PVC WAY SIMPLER
    public class VisualGraphGeneratedMessage : IMessage
    {
        public Guid CorrelationId { get; private set; }
        public List<GraphNode> Nodes = new List<GraphNode>();
        public List<Connection> Connections = new List<Connection>();

        public void WriteDataTo(BinaryWriter writer)
        {
            writer.Write(CorrelationId.ToString());
            writer.Write(Nodes.Count);
            foreach (var t in Nodes)
            {
                writer.Write(t.Assembly);
                writer.Write(t.DisplayName);
                writer.Write(t.FullName);
                writer.Write(t.IsInterface);
                writer.Write(t.IsRootNode);
                writer.Write(t.IsTest);
                writer.Write(t.Type);
                writer.Write(t.Name);
                writer.Write(t.InTestAssembly);
                writer.Write(t.IsProfiledTest);
                writer.Write(t.Complexity);
                writer.Write(t.IsChange);
            }
            writer.Write(Connections.Count);
            foreach (var t in Connections)
            {
                writer.Write(t.From);
                writer.Write(t.To);
            }
        }
        
        public void SetDataFrom(BinaryReader reader)
        {
            CorrelationId = new Guid(reader.ReadString());        
            var nodes = reader.ReadInt32();
            Nodes = new List<GraphNode>();
            Connections = new List<Connection>();
            for(var i=0;i<nodes;i++)
            {
                var node = new GraphNode
                               {
                                   Assembly = reader.ReadString(),
                                   DisplayName = reader.ReadString(),
                                   FullName = reader.ReadString(),
                                   IsInterface = reader.ReadBoolean(),
                                   IsRootNode = reader.ReadBoolean(),
                                   IsTest = reader.ReadBoolean(),
                                   Type = reader.ReadString(),
                                   Name = reader.ReadString(),
                                   InTestAssembly = reader.ReadBoolean(),
                                   IsProfiledTest = reader.ReadBoolean(),
                                   Complexity = reader.ReadInt32(),
                                   IsChange = reader.ReadBoolean()
                               };
                Nodes.Add(node);    
            }
            var connections = reader.ReadInt32();
            for(var i=0;i<connections;i++)
            {
                var connection = new Connection
                                     {
                                         From = reader.ReadString(),
                                         To = reader.ReadString()
                                     };
                Connections.Add(connection);
            }
        }
    }

    public class Connection
    {
        public string From;
        public string To;
    }

    public class GraphNode
    {
        public string DisplayName;
        public bool IsInterface;
        public bool IsTest;
        public string Name;
        public bool IsRootNode;
        public string FullName;
        public string Assembly;
        public string Type;
        public bool InTestAssembly;
        public bool IsProfiledTest;
        public int Complexity;
        public bool IsChange;
    }
}
