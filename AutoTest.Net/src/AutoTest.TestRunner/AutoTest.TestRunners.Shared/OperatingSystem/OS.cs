using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AutoTest.TestRunners
{
    public static class OS
    {
        private static bool? _isWindows = null;
        private static bool? _isUnix = null;
        private static bool? _isOSX = null;

        public static bool IsWindows {
            get {
                if (_isWindows == null) {
                    _isWindows = 
                        Environment.OSVersion.Platform == PlatformID.Win32S ||
                        Environment.OSVersion.Platform == PlatformID.Win32NT ||
                        Environment.OSVersion.Platform == PlatformID.Win32Windows ||
                        Environment.OSVersion.Platform == PlatformID.WinCE ||
                        Environment.OSVersion.Platform == PlatformID.Xbox;
                    if (_isWindows == true)
                        Shared.Logging.Logger.Write("Running on Windows");
                }
                return (bool) _isWindows;
            }
        }

        public static bool IsPosix {
            get {
                return IsUnix || IsOSX;
            }
        }

        public static bool IsUnix {
            get {
                if (_isUnix == null)
                    setUnixAndLinux();
                return (bool) _isUnix;
            }
        }

        public static bool IsOSX {
            get {
                if (_isUnix == null)
                    setUnixAndLinux();
                return (bool) _isUnix;
            }
        }

        private static void setUnixAndLinux()
        {
            try
            {
                if (IsWindows) {
                    _isOSX = false;
                    _isUnix = false;
                } else  {
                    Process process = new Process();
                    process.StartInfo = new ProcessStartInfo("uname", "-a");
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = true;

                    process.Start();
                    var output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                    _isOSX = output.Contains("Darwin Kernel");
                    _isUnix = !_isOSX;
                }
            }
            catch
            {
                _isOSX = false;
                _isUnix = false;
            }
            if (_isOSX == true)
                Shared.Logging.Logger.Write("Running on OSX");
            if (_isUnix == true)
                Shared.Logging.Logger.Write("Running on Unix");
        }
    }
}
