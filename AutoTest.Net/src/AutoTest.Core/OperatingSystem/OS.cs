using System;
using System.Diagnostics;

namespace AutoTest
{
    public static class OS
    {
        private static bool? _isWindows;
        private static bool? _isUnix;
        private static bool? _isOSX;

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
                        Core.DebugLog.Debug.WriteDebug("Running on Windows");
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
                if (_isOSX == null)
                    setUnixAndLinux();
                return (bool) _isOSX;
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
                    var process = new Process
                                      {
                                          StartInfo =
                                              new ProcessStartInfo("uname", "-a")
                                                  {
                                                      RedirectStandardOutput = true,
                                                      WindowStyle = ProcessWindowStyle.Hidden,
                                                      UseShellExecute = false,
                                                      CreateNoWindow = true
                                                  }
                                      };

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
                Core.DebugLog.Debug.WriteDebug("Running on OSX");
            if (_isUnix == true)
                Core.DebugLog.Debug.WriteDebug("Running on Unix");
        }
    }
}
