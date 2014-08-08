using System;
using System.IO;
using System.Runtime.Serialization;
using AutoTest.Messages;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
namespace AutoTest.Messages.Serializers
{
	[StructLayout(LayoutKind.Explicit)]
    public struct IntToBytes
    {
        public IntToBytes(Int32 _value) { b0 = b1 = b2 = b3 = 0; i32 = _value; }
        public IntToBytes(byte _b0, byte _b1, byte _b2, byte _b3)
        {
            i32 = 0;
            b0 = _b0;
            b1 = _b1;
            b2 = _b2;
            b3 = _b3;
        }
        [FieldOffset(0)]
        public Int32 i32;
        [FieldOffset(0)]
        public byte b0;
        [FieldOffset(1)]
        public byte b1;
        [FieldOffset(2)]
        public byte b2;
        [FieldOffset(3)]
        public byte b3;
    }

    public class CustomBinaryFormatter : IFormatter
    {
        private readonly MemoryStream _writeStream;
        private readonly MemoryStream _readStream;
        private readonly BinaryWriter _writer;
        private readonly BinaryReader _reader;
        private readonly Dictionary<Type, int> _byType = new Dictionary<Type, int>();
        private readonly Dictionary<int, Type> _byId = new Dictionary<int, Type>();
        private readonly byte[] _lengthBuffer = new byte[4];
        private readonly byte[] _copyBuffer;

        public CustomBinaryFormatter()
        {
            _copyBuffer = new byte[1000];
            _writeStream = new MemoryStream();
            _readStream = new MemoryStream();
            _writer = new BinaryWriter(_writeStream);
            _reader = new BinaryReader(_readStream);

            Register<BuildMessage>(1);
            Register<BuildRunMessage>(2);
            Register<BuildRunResults>(3);
            Register<RunFinishedMessage>(4);
            Register<RunReport>(5);
            Register<RunAction>(6);
            Register<ErrorMessage>(7);
            Register<InformationMessage>(8);
            Register<RunInformationMessage>(9);
            Register<RunStartedMessage>(10);
            Register<WarningMessage>(11);
            Register<FileChangeMessage>(12);
            Register<AssemblyChangeMessage>(13);
            Register<ProjectChangeMessage>(14);
            Register<TestRunResults>(15);
            Register<TestResult>(16);
            Register<TestRunMessage>(17);
            Register<ExternalCommandMessage>(18);
            Register<LiveTestStatus>(19);
            Register<LiveTestStatusMessage>(20);
            Register<AbortMessage>(21);
            Register<CacheBuildMessage>(22);
            Register<CacheTestMessage>(23);
            Register<CacheMessages>(24);
            Register<OnDemandTestRunMessage>(25);
        }

        public void Register<T>(int typeId) where T : ICustomBinarySerializable
        {
            _byId.Add(typeId, typeof(T));
            _byType.Add(typeof(T), typeId);
        }

        public object Deserialize(Stream serializationStream)
        {
            if (serializationStream.Read(_lengthBuffer, 0, 4) != 4)
                throw new SerializationException("Could not read length from the stream.");
            var length = new IntToBytes(_lengthBuffer[0], _lengthBuffer[1], _lengthBuffer[2], _lengthBuffer[3]);
            _readStream.Seek(0L, SeekOrigin.Begin);
            int read = 0;
            int toRead = 0;
            while (read < length.i32)
            {
                toRead = length.i32 > _copyBuffer.Length ? _copyBuffer.Length : length.i32;
                var current = serializationStream.Read(_copyBuffer, 0, toRead);
                _readStream.Write(_copyBuffer, 0, current);
                read += current;

            }
            _readStream.Seek(0L, SeekOrigin.Begin);
            int typeid = _reader.ReadInt32();
            Type t;
            if (!_byId.TryGetValue(typeid, out t))
                throw new SerializationException("TypeId " + typeid + " is not a registerred type id");
            var obj = FormatterServices.GetUninitializedObject(t);
            var deserialize = (ICustomBinarySerializable)obj;
            deserialize.SetDataFrom(_reader);
            if (_readStream.Position != length.i32)
                throw new SerializationException("object of type " + t + " did not read its entire buffer during deserialization. This is most likely an inbalance between the writes and the reads of the object.");
            return deserialize;
        }

        public void Serialize(Stream serializationStream, object graph)
        {
            int key;
            if (!_byType.TryGetValue(graph.GetType(), out key))
                throw new SerializationException(graph.GetType() + " has not been registered with the serializer");
            var c = (ICustomBinarySerializable)graph; //this will always work due to generic constraint on the Register  
            _writeStream.Seek(0L, SeekOrigin.Begin);
            _writer.Write(key);
            c.WriteDataTo(_writer);
            var length = new IntToBytes((int)_writeStream.Position);
            serializationStream.WriteByte(length.b0);
            serializationStream.WriteByte(length.b1);
            serializationStream.WriteByte(length.b2);
            serializationStream.WriteByte(length.b3);
            serializationStream.Write(_writeStream.GetBuffer(), 0, (int)_writeStream.Position);
        }

        public ISurrogateSelector SurrogateSelector { get; set; }

        public SerializationBinder Binder { get; set; }

        public StreamingContext Context { get; set; }
    }
}

