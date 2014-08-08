using System;
using System.Collections.Generic;
using System.Linq;

using AutoTest.Core.Caching.Projects;
using AutoTest.Core.Configuration;
using AutoTest.Core.FileSystem;
using AutoTest.Core.Messaging.MessageConsumers;
using AutoTest.Messages;
using AutoTest.TestRunners.Shared.AssemblyAnalysis;
using System.Diagnostics;

namespace AutoTest.Core.TestRunners.TestRunners
{
    internal static class EnumerableExtensions
    {
        public static IEnumerable<T> ContinueWith<T>(this IEnumerable<T> source1, Func<IEnumerable<T>> source2)
        {
            return source1.Any()
                       ? source1
                       : source2();
        }
    }

    public class MSpecTestRunner : ITestRunner
    {
        readonly IMSpecCommandLineBuilder _commandLineBuilder;
        readonly IConfiguration _configuration;
        readonly IExternalProcess _externalProcess;
        readonly IFileSystemService _fileSystem;
        readonly IAssemblyPropertyReader _assemblyReader;

        public MSpecTestRunner(IAssemblyPropertyReader referenceResolver,
                               IConfiguration configuration,
                               IFileSystemService fileSystem,
                               IExternalProcess externalProcess,
                               IMSpecCommandLineBuilder commandLineBuilder)
        {
            _assemblyReader = referenceResolver;
            _configuration = configuration;
            _fileSystem = fileSystem;
            _externalProcess = externalProcess;
            _commandLineBuilder = commandLineBuilder;
        }

        public bool CanHandleTestFor(string assembly)
        {
            var framework = _assemblyReader.GetTargetFramework(assembly);
            var file = _configuration.MSpecTestRunner(string.Format("v{0}.{1}", framework.Major, framework.Minor));
            if (!_fileSystem.FileExists(file))
                return false;

            return _assemblyReader.GetReferences(assembly).Count(x => x.Name.Equals("Machine.Specifications")) > 0;
        }

        public TestRunResults[] RunTests(TestRunInfo[] runInfos, Action<AutoTest.TestRunners.Shared.Targeting.Platform, Version, Action<ProcessStartInfo, bool>> processWrapper, Func<bool> abortWhen)
        {
            return runInfos
                .GroupBy(x => x.Project == null ? String.Empty : x.Project.Value.Framework)
                .Select(x => new Run
                             {
                                 RunnerExe = _configuration.MSpecTestRunner(x.Key),
                                 RunInfos = x.AsEnumerable()
                             })
                .Select(x => RunnerExeExists(x).ContinueWith(() => RunTestsForFramework(x)))
                .SelectMany(x => x)
                .ToArray();
        }

        static string Framework(TestRunInfo runInfo)
        {
            return runInfo.Project == null ? String.Empty : runInfo.Project.Value.Framework;
        }

        IEnumerable<TestRunResults> RunnerExeExists(Run run)
        {
            if (_fileSystem.FileExists(run.RunnerExe))
            {
                return Enumerable.Empty<TestRunResults>();
            }

            return ErrorsFor(run.RunInfos, String.Format("Machine.Specifications runner executable could not be found: {0}", run.RunnerExe));
        }

        static IEnumerable<TestRunResults> ErrorsFor(IEnumerable<TestRunInfo> runInfo, string message)
        {
            return runInfo
                .Select(x => new
                             {
                                 Project = x.Project != null ? x.Project.Key : String.Empty,
                                 x.Assembly
                             })
                .Select(x => new TestRunResults(x.Project,
                                                x.Assembly,
                                                false,
                                                TestRunner.MSpec,
                                                new[] { TestResult.Fail(message) }));
        }

        IEnumerable<TestRunResults> RunTestsForFramework(Run run)
        {
            try
            {
                var runnerExe = run.RunnerExe;
                var arguments = _commandLineBuilder.Build(run);

                var exitCode = _externalProcess.CreateAndWaitForExit(runnerExe, arguments);
                if (exitCode < (int) ExitCode.Success)
                {
                    return ErrorsFor(run.RunInfos,
                                     string.Format("Machine.Specifications runner failed with exit code {0} " +
                                                   "- did you try to use mspec.exe (for CLR 2) with a .NET 4.0 project? Please configure mspec.exe (AnyCPU, CLR 2), mspec-x86.exe (x86, CLR 2), mspec-clr4.exe (AnyCPU, CLR 4) or mspec-x86-clr4.exe (x86, CLR 4).",
                                                   exitCode));
                }

                return run.HarvestResults().ToList();
            }
            finally
            {
                run.Cleanup();
            }
        }

        public class Run
        {
            readonly List<Action> _cleanups = new List<Action>();
            readonly List<Func<IEnumerable<TestRunResults>>> _harvesters = new List<Func<IEnumerable<TestRunResults>>>();

            public string RunnerExe
            {
                get;
                set;
            }

            public IEnumerable<TestRunInfo> RunInfos
            {
                get;
                set;
            }

            public IEnumerable<Action> Cleanups
            {
                get { return _cleanups; }
            }

            public IEnumerable<Func<IEnumerable<TestRunResults>>> Harvesters
            {
                get { return _harvesters; }
            }

            public void RegisterCleanup(Action cleanup)
            {
                _cleanups.Add(cleanup);
            }
            
            public void RegisterResultHarvester(Func<IEnumerable<TestRunResults>> resultHarvester)
            {
                _harvesters.Add(resultHarvester);
            }

            public void Cleanup()
            {
                Cleanups
                    .AsEnumerable()
                    .Reverse()
                    .ToList()
                    .ForEach(x => x());
            }

            public IEnumerable<TestRunResults> HarvestResults()
            {
                return _harvesters
                    .Select(x => x())
                    .SelectMany(x => x);
            }
        }
    }
}