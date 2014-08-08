using System.Diagnostics;
using System.IO;

using Debug = AutoTest.Core.DebugLog.Debug;

namespace AutoTest.Core.TestRunners.TestRunners
{
    public interface IExternalProcess
    {
        int CreateAndWaitForExit(string executable, string arguments);
    }

    internal enum ExitCode
    {
        Success = 0
    }

    internal class HiddenExternalProcess : IExternalProcess
    {
        public int CreateAndWaitForExit(string executable, string arguments)
        {
            var process = new Process
                          {
                              StartInfo = new ProcessStartInfo(executable, arguments)
                                          {
                                              RedirectStandardOutput = true,
                                              WorkingDirectory = Path.GetDirectoryName(executable),
                                              WindowStyle = ProcessWindowStyle.Hidden,
                                              UseShellExecute = false,
                                              CreateNoWindow = true
                                          }
                          };
            Debug.WriteDebug(string.Format("Launching: {0} {1}", executable, arguments));
            process.Start();

            return WaitForExit(process);
        }

        static int WaitForExit(Process process)
        {
            process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return process.ExitCode;
        }
    }
}