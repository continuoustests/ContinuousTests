using System;
using System.IO;

namespace AutoTest.Client.Logging
{
    public class FileLogger : IErrorListener
    {
        private readonly object _lock = new object();
        private readonly string _file;
        private readonly bool _loggingEnabled = false;

        public FileLogger()
        {
            _loggingEnabled = getLoggingConfiguration();
            _file = Path.Combine(GetPath(), "AutoTest.Client.log");
        }

        public void OnError(string message)
        {
            if (!_loggingEnabled) return;
            lock (_lock)
            {
                for (int i = 0; i < 5; i++)
                {
                    try
                    {
                        using (var writer = new StreamWriter(_file, true))
                        {
                            writer.WriteLine(message);
                            break;
                        }
                    }
                    catch
                    {
                        System.Threading.Thread.Sleep(10);
                    }
                }
            }
        }

        private static bool getLoggingConfiguration()
        {
            var file = GetConfigurationPath();
            if (!File.Exists(file))
                return false;
            var content = File.ReadAllText(file).ToLower();
            return content.Contains("<debugging>true</debugging>");
        }

        public static string GetConfigurationPath()
        {
            return Path.Combine(GetPath(), "AutoTest.config");
        }

        private static string GetPath()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var mm = Path.Combine(appData, "MightyMoose");
            if (!Directory.Exists(mm))
                Directory.CreateDirectory(mm);
            return mm;
        }
    }
}
