using System;
using System.Diagnostics;
namespace AutoTest.Core
{
	public class MaxCmdLengthCalculator
	{
		public double GetLength()
		{
			if (OS.IsPosix)
				return getEnvironmentConfiguredLength();
			else
				return 8000;
		}
		
		private double getEnvironmentConfiguredLength()
		{
			try
			{
				var process = new Process();
				process.StartInfo = new ProcessStartInfo("getconf", "ARG_MAX");
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.CreateNoWindow = true;
				process.Start();
				var output = process.StandardOutput.ReadToEnd();
				return double.Parse(output);
			}
			catch (Exception ex)
			{
				DebugLog.Debug.WriteException(ex);
			}
			return 8000;
		}
	}
}

