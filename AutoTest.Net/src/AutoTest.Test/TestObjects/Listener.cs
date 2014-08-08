using AutoTest.Core.Messaging;
using System.Threading;
using AutoTest.Messages;
using System.IO;
using System;
using NUnit.Framework;

namespace AutoTest.Test.TestObjects
{
    class StringMessage : IMessage
    {
        private object _padLock = new object();
        private int _timesConsumed = 0;

        public int TimesConsumed { get { return _timesConsumed; } }

        public void Consume()
        {
            lock (_padLock)
            {
                _timesConsumed++;
            }
        }
		
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

    class IntMessage : IMessage
	{
		public bool Consumed { get; set; }
		
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

    class BlockingMessage : IMessage
	{ 
		public bool Consumed { get; set; } 
		
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
    class BlockingMessage2 : IMessage
	{ 
		public bool Consumed { get; set; }
		
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

    internal class Listener : IConsumerOf<StringMessage>
    {
        public void Consume(StringMessage message)
        {
            message.Consume();
        }
    }

    internal class BigListener : IConsumerOf<StringMessage>, IConsumerOf<IntMessage>
    {
        public void Consume(StringMessage message)
        {
            message.Consume();
        }

        public void Consume(IntMessage message)
        {
            message.Consumed = true;
        }
    }

    internal class OverridingConsumer : IOverridingConsumer<StringMessage>
    {
        private StringMessage _message = null;
        private bool _wasTerminated = false;

        private bool _isRunning = false;
        public OverridingConsumer SetIsRunning(bool isRunning)
        {
            _isRunning = isRunning;
            return this;
        }

        public bool IsRunning
        {
            get { return _isRunning; }
        }

        public void Consume(StringMessage message)
        {
            _message = message;
            Thread.Sleep(100);
        }

        public void Terminate()
        {
            _wasTerminated = true;
        }

        public void ConsumedMessage()
        {
            Assert.IsNotNull(_message);
        }

        public void WasTerminated()
        {
            Assert.IsTrue(_wasTerminated);
        }
    }
}