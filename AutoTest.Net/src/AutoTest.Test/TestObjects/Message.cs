using AutoTest.Core.Messaging;
using AutoTest.Messages;
using System.IO;
using System;

namespace AutoTest.Test.TestObjects
{
    internal class Message : IMessage
    {
        public Message(string body)
        {
            Body = body;
        }

        public string Body { get; private set; }
		
		#region ICustomBinarySerializable implementation
		public void WriteDataTo(BinaryWriter writer)
		{
			throw new NotImplementedException ();
		}

		public void SetDataFrom(BinaryReader reader)
		{
			throw new NotImplementedException ();
		}
		#endregion
    }
}