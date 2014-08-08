using System;
using System.Windows.Forms;
using AutoTest.Client;
using AutoTest.Client.Logging;
using System.IO;

namespace ContinuousTests
{
    static class Program
    {
        public static ATEClient Client;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var path = Environment.CurrentDirectory;
            if (args.Length > 0)
                path = Path.GetFullPath(args[0]);

            if (!Directory.Exists(path) && !File.Exists(path))
            {
                Console.WriteLine("Solution/Directory {0} does not exist", path);
                return;
            }

            var form = new RunFeedbackForm(path);
            Logger.SetListener(new FileLogger());
            Client = new ATEClient();
            Client.Start(new StartupParams(path), form);
            
            Application.Run(form);
            Client.Stop();
            System.Threading.Thread.Sleep(300);
        }
    }
}
