using System;
using AutoTest.Core.DebugLog;
namespace AutoTest.Core.Configuration
{
	public interface ILocateWriteLocation
	{
        string GetLogfile();
		string GetConfigurationFile();
	}
}

