using System;
using System.IO;
using System.Dynamic;
using System.Threading;
using System.Reflection;
using System.Windows.Forms;
using BellyRub;
using BellyRub.UI;
using AutoTest.Client;
using AutoTest.Client.Logging;
using AutoTest.Client.Handlers;

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
            var path = getPath(args);
            if (path != null) {
                //runWinforms(args);
                runBellyRub(path);
            } 
        }

        static void runBellyRub(string path)
        {
            var site = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "site");
            var engine = new BellyEngine(site);
            Console.CancelKeyPress += (sender, e) => {
                engine.Stop();
                while (engine.HasConnectedClients) {
                    Thread.Sleep(50);
                }
            };
            var browser = engine 
                .OnSendException((ex) => Console.WriteLine(ex.ToString()))
                .Start(new Point(100, 100));

            var proxy = new BellyRubProxy(path, engine, browser);

            engine.WaitForFirstClientToConnect();
            browser.BringToFront(); 
            
            run(path, proxy);
            proxy.SetClient(Client);
            while (engine.HasConnectedClients) {
                Thread.Sleep(50);
            }
            exit();
        }

        static void runWinforms(string path)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
 
            var form = new RunFeedbackForm(path);
            run(path, form);
            Application.Run(form);
            exit();
        }

        static string getPath(string[] args)
        {
            var path = Environment.CurrentDirectory;
            if (args.Length > 0)
                path = Path.GetFullPath(args[0]);

            if (!Directory.Exists(path) && !File.Exists(path))
            {
                Console.WriteLine("Solution/Directory {0} does not exist", path);
                return null;
            }
            return path;
        }

        static void run(string path, IStartupHandler handler)
        {
            Logger.SetListener(new FileLogger());
            Client = new ATEClient();
            Client.Start(new StartupParams(path), handler);
        }

        static void exit()
        {
            Client.Stop();
            System.Threading.Thread.Sleep(300);
        }
    }
}
