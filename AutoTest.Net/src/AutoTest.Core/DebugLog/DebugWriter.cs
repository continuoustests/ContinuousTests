using System;
using System.IO;
using AutoTest.Core.FileSystem;
using AutoTest.Core.Messaging;
using AutoTest.Messages;
using System.Reflection;
using AutoTest.Core.Configuration;
namespace AutoTest.Core.DebugLog
{
    public class DebugWriter : IWriteDebugInfo
	{
        private IMessageBus _bus;
		private object _padLock = new object();
		private string _logFile;
		private long _resycleSize = 1024000;

		public void SetRecycleSize(long size)
		{
			_resycleSize = size;
		}
		
		public DebugWriter(ILocateWriteLocation locator, IMessageBus bus)
		{
            _bus = bus; 
            _logFile = locator.GetLogfile();
		}
        
        public void WriteError(string message)
        {
            _bus.Publish(new ErrorMessage(message));
            write(message);
        }

        public void WriteInfo(string message)
        {
            _bus.Publish(new InformationMessage(message));
            write(message);
        }

        public void WriteDebug(string message)
        {
            write(message);
        }

        public void WritePreProcessor(string message)
        {
            write(message);
        }
        
        public void WriteDetail(string message)
        {
            write(message);
        }
		
		private void write(string text)
		{
			lock (_padLock)
            {
                try
                {
                    using (var writer = getWriter(_logFile))
                    {
                        writer.WriteLine(string.Format("{0}:{1}:{2}.{3} - {4}",
                            DateTime.Now.Hour.ToString().PadLeft(2, '0'),
                            DateTime.Now.Minute.ToString().PadLeft(2, '0'),
                            DateTime.Now.Second.ToString().PadLeft(2, '0'),
                            DateTime.Now.Millisecond.ToString().PadLeft(3, '0'), text));
                    }
                    if ((new FileInfo(_logFile)).Length > _resycleSize)
                    {
                        File.Delete(string.Format("{0}.old", _logFile));
                        File.Move(_logFile, string.Format("{0}.old", _logFile));
                    }
                }
                catch
                {
                }
            }
		}
		
		private StreamWriter getWriter(string file)
		{
			if (File.Exists(file))
				return new StreamWriter(file, true);
			else
				return new StreamWriter(file);
		}
    }
}

