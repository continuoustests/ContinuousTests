using System;
using System.IO;
using System.Linq;
using System.Dynamic;
using System.Threading;
using System.Reflection;
using System.Windows.Forms;
using BellyRub;
using BellyRub.UI;
using AutoTest.Client;
using AutoTest.Client.Logging;
using AutoTest.Client.Handlers;
using AutoTest.Core.FileSystem;

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
            if (hasArg(ref args, "-h") || hasArg(ref args, "/h")) {
                Console.WriteLine("USAGE: ContinuousTests PATH [--bellyrub-headless] [--bellyrub] [--leave-in-background] [--bellyrub-ports=SERVER,CHANNEL]");
                return;
            }
            var bellyRubHeadless = hasArg(ref args, "--bellyrub-headless");
            var bellyRub = hasArg(ref args, "--bellyrub");
            var leaveInBackground = hasArg(ref args, "--leave-in-background");
            var port = getArg(ref args, "--bellyrub-ports");
            var localConfig = getArg(ref args, "--local-config-location");
            var path = getPath(args);
            if (path != null) {
                if (bellyRub || bellyRubHeadless)
                    runBellyRub(localConfig, bellyRubHeadless, path, leaveInBackground, port);
                else
                    runWinforms(localConfig, path);
            } 
        }

        static bool hasArg(ref string[] args, string arg)
        {
            var has = args.Any(x => x == arg);
            args = args.Where(x => x != arg).ToArray();
            return has;
        }

        static string getArg(ref string[] args, string arg)
        {
            var found = args.FirstOrDefault(x => x.StartsWith(arg+"="));
            args = args.Where(x => x != found).ToArray();
            if (found != null)
                return found.Substring(arg.Length+1, found.Length - (arg.Length+1));
            return null;
        }

        static void runBellyRub(string localConfig, bool headless, string path, bool leaveInBackground, string portString)
        {
            int port, channelPort;
            if (portString != null) {
                var portChunks = portString.Split(new[] {','});
                if (portChunks.Length > 0) {
                    if (!int.TryParse(portChunks[0], out port))
                        port = 0;
                } else {
                    port = 0;
                }
                if (portChunks.Length > 1) {
                    if (!int.TryParse(portChunks[1], out channelPort))
                        channelPort = 0;
                } else {
                    channelPort = 0;
                }
            } else {
                port = 0;
                channelPort = 0;
            }
            var site = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "site");
            var engine = new BellyEngine(site);
            Console.CancelKeyPress += (sender, e) => {
                engine.Stop();
                while (engine.HasConnectedClients) {
                    Thread.Sleep(50);
                }
            };
            Browser browser = null;
            if (headless) {
                if (port > 0) {
                    if (channelPort > 0)
                        engine.StartHeadless(port, channelPort);
                    else
                        engine.StartHeadless(port);
                } else {
                    engine.StartHeadless();
                } 
                Console.WriteLine("url: "+engine.ServerUrl);
                Console.WriteLine("waiting for client to connect..");
            } else {
                engine.OnSendException((ex) => Console.WriteLine(ex.ToString()));
                if (port > 0) {
                    if (channelPort > 0)
                        browser = engine.Start(port, channelPort, new Point(100,100));
                    else
                        browser = engine.Start(port, new Point(100,100));
                } else {
                    browser = engine.Start(new Point(100,100));
                } 
            } 

            var proxy = new BellyRubProxy(path, engine, browser);

            engine.WaitForFirstClientToConnect(60);
            if (!leaveInBackground && browser != null)
                browser.BringToFront(); 
            
            run(localConfig, path, proxy);
            proxy.SetClient(Client);
            if (headless) {
                Console.WriteLine("type exit to quit ContinuousTests");
                while (Console.ReadLine() != "exit") {
                    Thread.Sleep(10);
                }
            } else {
                while (engine.HasConnectedClients) {
                    Thread.Sleep(50);
                }
            }
            exit();
        }

        static void runWinforms(string localConfig, string path)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
 
            var form = new RunFeedbackForm(path);
            run(localConfig, path, form);
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

        static void run(string localConfig, string path, IStartupHandler handler)
        {
            if (localConfig != null) {
                var parser = new PathParser(localConfig);
                if (Directory.Exists(Path.Combine(path, localConfig)))
                    localConfig = parser.ToAbsolute(path);
            }
            Logger.SetListener(new FileLogger());
            Client = new ATEClient();
            Client.Start(new StartupParams(path, localConfig), handler);
        }

        static void exit()
        {
            Client.Stop();
            System.Threading.Thread.Sleep(300);
        }
    }
}
