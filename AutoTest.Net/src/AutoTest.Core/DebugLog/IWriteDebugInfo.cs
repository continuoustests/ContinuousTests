using System;
namespace AutoTest.Core.DebugLog
{
	public interface IWriteDebugInfo
	{
		void SetRecycleSize(long size);
        void WriteError(string message);
        void WriteInfo(string message);
		void WriteDebug(string message);
        void WritePreProcessor(string message);
        void WriteDetail(string message);
	}
}

