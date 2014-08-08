using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.TestRunners.Shared.Plugins;
using AutoTest.TestRunners.Shared.Options;
using AutoTest.TestRunners.Shared.Errors;
using System.Threading;
using System.IO;
using System.Reflection;
using AutoTest.TestRunners.Shared.Logging;

namespace AutoTest.TestRunners
{
    class SubDomainRunner : MarshalByRefObject
    {
        private Plugin _plugin;
        private string _id;
        private IEnumerable<string> _categories;
        private AssemblyOptions _assembly;
        private bool _shouldLog = false;
        private string _pipeName;
        private bool _compatibilityMode;

        public SubDomainRunner(Plugin plugin, string id, IEnumerable<string> categories, AssemblyOptions assembly, bool shouldLog, string pipeName, bool compatibilityMode)
        {
            _plugin = plugin;
            _id = id;
            _categories = categories;
            _assembly = assembly;
            _shouldLog = shouldLog;
            _pipeName = pipeName;
            _compatibilityMode = compatibilityMode;
        }

        public void Run(object waitHandle)
        {
            ManualResetEvent handle = null;
            if (waitHandle != null)
                handle = (ManualResetEvent)waitHandle;
            AppDomain childDomain = null;
            try
            {
                var configFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
                if (File.Exists(_assembly.Assembly + ".config"))
                    configFile = _assembly.Assembly + ".config";
                // Construct and initialize settings for a second AppDomain.
                AppDomainSetup domainSetup = new AppDomainSetup()
                {
                    ApplicationBase = Path.GetDirectoryName(_assembly.Assembly),
                    ConfigurationFile = configFile,
                    ApplicationName = AppDomain.CurrentDomain.SetupInformation.ApplicationName,
                    LoaderOptimization = LoaderOptimization.MultiDomainHost
                };

                // Create the child AppDomain used for the service tool at runtime.
                Logger.Write("");
                Logger.Write("Starting sub domain");
                childDomain = AppDomain.CreateDomain(_plugin.Type + " app domain", null, domainSetup);

                // Create an instance of the runtime in the second AppDomain. 
                // A proxy to the object is returned.
                ITestRunner runtime = (ITestRunner)childDomain.CreateInstanceFromAndUnwrap(Assembly.GetExecutingAssembly().Location, typeof(TestRunner).FullName); //typeof(TestRunner).Assembly.FullName, typeof(TestRunner).FullName);

                // Prepare assemblies
                Logger.Write("Preparing resolver");
                runtime.SetupResolver(_shouldLog);

                // start the runtime.  call will marshal into the child runtime appdomain
                Program.AddResults(runtime.Run(_plugin, _id, new RunSettings(_assembly, _categories.ToArray(), _pipeName)));
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                if (!_compatibilityMode)
                    Program.AddResults(ErrorHandler.GetError(_id, ex));
            }
            finally
            {
                if (handle != null)
                    handle.Set();
                unloadDomain(childDomain);
                Program.WriteNow("Finished running tests for " + _assembly.Assembly);
            }
        }

        private static void unloadDomain(AppDomain childDomain)
        {
            if (childDomain != null)
            {
                try
                {
                    AppDomain.Unload(childDomain);
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                }
            }
        }
    }
}
