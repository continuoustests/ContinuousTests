using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;

namespace AutoTest.CoreExtensions
{
    public static class ProcessExtensions
    {
        private static Action<string> _logger = (msg) => {};

        public static void SetLogger(this Process proc, Action<string> logger) {
            _logger = logger;
        }

        public static Func<string,string> GetInterpreter = (file) => null;

        public static void Write(this Process proc, string msg) {
            try {
                proc.StandardInput.WriteLine(msg);
                proc.StandardInput.Flush();
            } catch {
            }
        }

        public static void Run(this Process proc, string command, string arguments,
                               bool visible, string workingDir) {
            Run(proc, command, arguments, visible, workingDir, new KeyValuePair<string,string>[] {});
        }
        
        public static void Run(this Process proc, string command, string arguments,
                               bool visible, string workingDir,
                               IEnumerable<KeyValuePair<string,string>> replacements) {
			arguments = replaceArgumentPlaceholders(arguments, replacements);
            prepareProcess(proc, command, arguments, visible, workingDir);
            proc.Start();
			proc.WaitForExit();
        }

        public static void Spawn(this Process proc, string command, string arguments,
                                 bool visible, string workingDir) {
            Spawn(proc, command, arguments, visible, workingDir, new KeyValuePair<string,string>[] {});
        }

        public static void Spawn(this Process proc, string command, string arguments,
                                 bool visible, string workingDir,
                                 IEnumerable<KeyValuePair<string,string>> replacements) {
			arguments = replaceArgumentPlaceholders(arguments, replacements);
            prepareProcess(proc, command, arguments, visible, workingDir);
            proc.Start();
        }

        public static IEnumerable<string> QueryAll(this Process proc, string command, string arguments,
                                                   bool visible, string workingDir,
                                                   out string[] errors) {
            return QueryAll(proc, command, arguments, visible, workingDir, new KeyValuePair<string,string>[] {}, out errors);
        }

        public static IEnumerable<string> QueryAll(this Process proc, string command, string arguments,
                                                   bool visible, string workingDir,
                                                   IEnumerable<KeyValuePair<string,string>> replacements,
                                                   out string[] errors) {
            errors = new string[] {};
			arguments = replaceArgumentPlaceholders(arguments, replacements);
            prepareProcess(proc, command, arguments, visible, workingDir);
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardError = true;
            proc.Start();
            var output = proc.StandardOutput.ReadToEnd();
            errors = 
                proc.StandardError.ReadToEnd()
                    .Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            proc.WaitForExit();
            return output.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
        }

        public static void Query(this Process proc, string command, string arguments,
                                 bool visible, string workingDir,
                                 Action<bool, string> onRecievedLine) {
             Query(proc, command, arguments, visible, workingDir, onRecievedLine, new KeyValuePair<string,string>[] {});
        }

        public static void Query(this Process proc, string command, string arguments,
                                 bool visible, string workingDir,
                                 Action<bool, string> onRecievedLine,
                                 IEnumerable<KeyValuePair<string,string>> replacements) {
            _logger("Running process");
            var process = proc;
            var retries = 0;
            var exitCode = 255;
            _logger("About to start process");
            while (exitCode == 255 && retries < 5) {
                _logger("Running query");
                exitCode = query(process, command, arguments, visible, workingDir, onRecievedLine, replacements);
                _logger("Done running with " + exitCode.ToString());
                retries++;
                // Seems to happen on linux when a file is beeing executed while being modified (locked)
                if (exitCode == 255) {
                    _logger("Recreating process");
                    process = new Process();
                    Thread.Sleep(100);
                }
                _logger("Done running process");
            }
        }

        private static int query(this Process proc, string command, string arguments,
                                 bool visible, string workingDir,
                                 Action<bool, string> onRecievedLine,
                                 IEnumerable<KeyValuePair<string,string>> replacements) {
            string tempFile = null;
			arguments = replaceArgumentPlaceholders(arguments, replacements);
            if (Environment.OSVersion.Platform != PlatformID.Unix &&
                Environment.OSVersion.Platform != PlatformID.MacOSX)
            {
                if (Path.GetExtension(command).ToLower() != ".exe") {
                    arguments = getBatchArguments(command, arguments, ref tempFile);
                    command = "cmd.exe";
                }
            }
			
            prepareProcess(proc, command, arguments, visible, workingDir);
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardInput = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardError = true;

            DataReceivedEventHandler onOutputLine = 
                (s, data) => {
                    if (data.Data != null)
                        onRecievedLine(false, data.Data);
                };
            DataReceivedEventHandler onErrorLine = 
                (s, data) => {
                    if (data.Data != null)
                        onRecievedLine(true, data.Data);
                };

			proc.OutputDataReceived += onOutputLine;
            proc.ErrorDataReceived += onErrorLine;
            if (proc.Start())
            {
                proc.BeginOutputReadLine();
                proc.BeginErrorReadLine();
                proc.WaitForExit();
            }
            proc.OutputDataReceived -= onOutputLine;
            proc.ErrorDataReceived -= onErrorLine;
            
            if (tempFile != null && File.Exists(tempFile))
                File.Delete(tempFile);
            return proc.ExitCode;
        }

        private static bool processExists(int id) {
            return Process.GetProcesses().Any(x => x.Id == id);
        }

        private static string getBatchArguments(string command, string arguments, ref string tempFile) {
            var illagalChars = new[] {"&", "<", ">", "(", ")", "@", "^", "|"};
            if (command.Contains(" ") ||
                illagalChars.Any(x => arguments.Contains(x))) {
                // Windows freaks when getting the | character
                // Have it run a temporary bat file with command as contents
                tempFile = Path.GetTempFileName() + ".bat";
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
                File.WriteAllText(tempFile, "\"" + command + "\" " + arguments);
                arguments = "/c " + tempFile;
            } else {
                arguments = "/c " + 
                    "^\"" + batchEscape(command) + "^\" " +
                    batchEscape(arguments);
            }
            return arguments;
        }

        private static string batchEscape(string text) {
            foreach (var str in new[] { "^", " ", "&", "(", ")", "[", "]", "{", "}", "=", ";", "!", "'", "+", ",", "`", "~", "\"" })
                text = text.Replace(str, "^" + str);
            return text;
        }

		private static string replaceArgumentPlaceholders(string arg,  IEnumerable<KeyValuePair<string,string>> replacements)
		{
			foreach (var replacement in replacements)
				arg = arg.Replace(replacement.Key, replacement.Value);
			return arg;
		}
        
        private static void prepareProcess(
            Process proc,
            string command,
            string arguments,
            bool visible,
            string workingDir)
        {
           
            var info = new ProcessStartInfo(command, arguments);
            info.CreateNoWindow = !visible;
            if (!visible)
                info.WindowStyle = ProcessWindowStyle.Hidden;
            info.WorkingDirectory = workingDir;
            proc.StartInfo = info;
        }                
    }
}
