using System;
using System.IO;
using System.Text;

namespace AutoTest.Profiler.Tests
{
    public class EntryBuilder
    {
        private readonly EntryType _entryType;
        private int _sequence = 12;
        private string _method;
        private int _thread = 2;
        private long _functionid = 42;
        private double _time = 47.66;
        private string _runtime = "";

        public EntryBuilder(EntryType entryType, int i)
        {
            int x = 5;
            _entryType = entryType;
            _sequence = i;
        }

        public double Time
        {
            get { return _time; }
        }

        public string Method
        {
            get { return _method; }
        }

        public int Sequence
        {
            get { return _sequence; }
        }

        public EntryType EntryType1
        {
            get { return _entryType; }
        }

        public EntryBuilder At(double time)
        {
            _time = time;
            return this;
        }

        public EntryBuilder For(string method)
        {
            _method = method;
            return this;
        }

        public void WriteTo(BinaryWriter writer)
        {
            /*
             * BYTE - 1 for Enter, 2 for Leave, 3 for TailCall
             * ULONG (4 bytes) - sequence #
             * double (8 bytes) - time elapsed
             * DWORD (4 bytes) - threadId
             * ULONGLONG (8Bytes) - functionId
             * DWORD (4 bytes) - #of unicode chars in the following block
             * - runtime string
             * DWORD (4 bytes) - #of unicode chars in the following block
             * - metadata string
             */
            writer.Write((byte) _entryType);
            writer.Write((Int32)_sequence);
            writer.Write((Double)_time);
            writer.Write((Int32) _thread);
            writer.Write((Int64)_functionid);
            if(_entryType == EntryType.Enter)
            {
                WriteString(writer, _method);
                WriteString(writer, _runtime);
            }
        }

        private void WriteString(BinaryWriter writer, string method)
        {
            writer.Write((Int32)method.Length);
            writer.Write(Encoding.Unicode.GetBytes(method));
        }

        public EntryBuilder WithFunctionId(long functionId)
        {
            _functionid = functionId;
            return this;

        }

        public EntryBuilder RuntimeOf(string runtimemethod)
        {
            this._runtime = runtimemethod;
            return this;
        }

        public EntryBuilder OnThread(int i)
        {
            _thread = i;
            return this;
        }
    }
}