using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.XPath;

using AutoTest.Core.DebugLog;
using AutoTest.Core.Messaging.MessageConsumers;
using AutoTest.Messages;

namespace AutoTest.Core.TestRunners.TestRunners
{
    public class MSpecReportParser
    {
        readonly string _reportFile;
        readonly MSpecTestRunner.Run _run;

        public MSpecReportParser(string reportFile, MSpecTestRunner.Run run)
        {
            _reportFile = reportFile;
            _run = run;
        }

        public IEnumerable<TestRunResults> Parse()
        {
            var doc = new XPathDocument(_reportFile);
            var nav = doc.CreateNavigator();

            var assemblies = nav.Select("/MSpec/assembly").OfType<XPathNavigator>();

            return assemblies
                .Select(assembly => new Assembly
                                    {
                                        AssemblyLocation = assembly.GetAttribute("location", ""),
                                        TimeSpent = ReadTimeSpan(assembly),
                                        ContextSpecifications = ContextSpecificationsFor(assembly)
                                    })
                .Select(x =>
                    {
                        x.AssociatedRunInfo = FindRunInfo(_run, x);
                        return x;
                    })
                .Where(x => x.AssociatedRunInfo != null)
                .Select(x =>
                    {
                        var results = new TestRunResults(x.AssociatedRunInfo.Project.Key,
                                                         x.AssociatedRunInfo.Assembly,
                                                         _run.RunInfos.Any(),
                                                         TestRunner.MSpec,
                                                         TestResultsFor(x.ContextSpecifications));
                        results.SetTimeSpent(x.TimeSpent);
                        return results;
                    })
                .ToList();
        }

        static TimeSpan ReadTimeSpan(XPathNavigator assembly)
        {
            var timeString = assembly.GetAttribute("time", "");

            double time;
            if (double.TryParse(timeString, NumberStyles.Number, CultureInfo.InvariantCulture, out time))
            {
                return TimeSpan.FromMilliseconds(time);
            }

            return TimeSpan.Zero;
        }

        static TestResult[] TestResultsFor(IEnumerable<ContextSpecification> contextSpecifications)
        {
            return contextSpecifications
                .Select(x => new TestResult(TestRunner.MSpec, x.Status, x.Name, x.Message, x.StackTrace))
                .ToArray();
        }

        static TestRunInfo FindRunInfo(MSpecTestRunner.Run run, Assembly assembly)
        {
            var runInfo = run.RunInfos
                .FirstOrDefault(x => StringComparer.Ordinal.Equals(x.Assembly, assembly.AssemblyLocation));

            if (runInfo == null)
            {
                Debug.WriteError("Could not match reported assembly {0} to any of the tested assemblies", assembly.AssemblyLocation);
            }

            return runInfo;
        }

        static IEnumerable<ContextSpecification> ContextSpecificationsFor(XPathNavigator assembly)
        {
            return assembly.Select("//context").OfType<XPathNavigator>()
                .Select(context => new
                                   {
                                       Element = context,
                                       TypeName = context.GetAttribute("type-name", "")
                                   })
                .SelectMany(x => SpecificationsFor(x.Element),
                            (context, spec) => new ContextSpecification
                                               {
                                                   Name = context.TypeName + "." + spec.FieldName,
                                                   Status = spec.Status,
                                                   Message = spec.Message,
                                                   StackTrace = spec.StackTrace
                                               });
        }

        static IEnumerable<Specification> SpecificationsFor(XPathNavigator context)
        {
            return context
                .Select("specification").OfType<XPathNavigator>()
                .Select(spec => new Specification
                                {
                                    FieldName = spec.GetAttribute("field-name", ""),
                                    Status = StatusFor(spec),
                                    Message = MessageFor(spec),
                                    StackTrace = StackTraceFor(spec),
                                });
        }

        static TestRunStatus StatusFor(XPathNavigator spec)
        {
            var status = new[]
                         {
                             Result.Failed,
                             Result.Passed,
                             Result.NotImplemented,
                             Result.Ignored
                         }
                .FirstOrDefault(x => x(spec).HasValue);

            if (status == null)
            {
                throw new NotSupportedException("Unknown test run status reported my mspec.exe: " + Result.String(spec));
            }

            return status(spec).Value;
        }

        static string MessageFor(XPathNavigator spec)
        {
            if (Result.NotImplemented(spec).HasValue)
            {
                return "not implemented";
            }

            if (Result.Failed(spec).HasValue)
            {
                return spec.SelectSingleNode("error/message").Value;
            }

            return null;
        }

        static IStackLine[] StackTraceFor(XPathNavigator spec)
        {
            if (!Result.Failed(spec).HasValue)
            {
                return null;
            }

            var stackTrace = spec.SelectSingleNode("error/stack-trace").Value;

            return stackTrace
                .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                .Select(line => new NUnitStackLine(line))
                .ToArray();
        }

        class Assembly
        {
            public string AssemblyLocation;
            public TestRunInfo AssociatedRunInfo;
            public IEnumerable<ContextSpecification> ContextSpecifications;
            public TimeSpan TimeSpent;
        }

        class ContextSpecification
        {
            string _message;
            string _name;
            IStackLine[] _stackTrace;

            public string Name
            {
                get { return _name ?? String.Empty; }
                set { _name = value; }
            }

            public string Message
            {
                get { return _message ?? String.Empty; }
                set { _message = value; }
            }

            public IStackLine[] StackTrace
            {
                get { return _stackTrace ?? new IStackLine[0]; }
                set { _stackTrace = value; }
            }

            public TestRunStatus Status
            {
                get;
                set;
            }
        }

        class Specification
        {
            public string FieldName;
            public string Message;
            public IStackLine[] StackTrace;
            public TestRunStatus Status;
        }

        static class Result
        {
            public static readonly Func<XPathNavigator, string> String = x => x.GetAttribute("status", "");

            public static readonly Func<XPathNavigator, TestRunStatus?> Failed =
                x => Match(String(x), TestRunStatus.Failed, "failed", "exception");

            public static readonly Func<XPathNavigator, TestRunStatus?> Ignored =
                x => Match(String(x), TestRunStatus.Ignored, "ignored");

            public static readonly Func<XPathNavigator, TestRunStatus?> NotImplemented =
                x => Match(String(x), TestRunStatus.Ignored, "not-implemented");

            public static readonly Func<XPathNavigator, TestRunStatus?> Passed =
                x => Match(String(x), TestRunStatus.Passed, "passed");

            static TestRunStatus? Match(string result, TestRunStatus status, params string[] resultStrings)
            {
                if (resultStrings.Any(x => x == result))
                {
                    return status;
                }

                return null;
            }
        }
    }
}