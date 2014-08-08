using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using AutoTest.Core.FileSystem;
using AutoTest.Core.Messaging.MessageConsumers;
using AutoTest.Messages;

namespace AutoTest.Core.TestRunners.TestRunners
{
    public interface IMSpecCommandLineBuilder
    {
        string Build(MSpecTestRunner.Run run);
    }

    public class MSpecCommandLineBuilder : IMSpecCommandLineBuilder
    {
        readonly IFileSystemService _fileSystem;

        public MSpecCommandLineBuilder(IFileSystemService fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public string Build(MSpecTestRunner.Run run)
        {
            return new[]
                   {
                       TimeInfo(run),
                       Filters(run),
                       XmlReport(run),
                       Assemblies(run)
                   }
                .SelectMany(x => x)
                .Aggregate((acc, curr) => acc + " " + curr);
        }

        IEnumerable<string> TimeInfo(MSpecTestRunner.Run run)
        {
            yield return String.Format("--timeinfo");
        }

        IEnumerable<string> Filters(MSpecTestRunner.Run run)
        {
            var filters = CreateFilterFile(run.RunInfos);

            if (filters == null)
            {
                yield break;
            }

            run.RegisterCleanup(() =>
                {
                    if (_fileSystem.FileExists(filters))
                    {
                        File.Delete(filters);
                    }
                });

            yield return String.Format("--filter \"{0}\"", filters);
        }

        static string CreateFilterFile(IEnumerable<TestRunInfo> runInfos)
        {
            var filtered = runInfos
                .Select(x => x.GetTestsFor(TestRunner.MSpec))
                .Where(x => x.Any())
                .SelectMany(x => x);

            if (!filtered.Any())
            {
                return null;
            }

            var builder = new StringBuilder();
            filtered
                .ToList()
                .ForEach(x => builder.AppendLine(x));

            var filterFile = Path.GetTempFileName();
            File.WriteAllText(filterFile, builder.ToString());

            return filterFile;
        }

        IEnumerable<string> XmlReport(MSpecTestRunner.Run run)
        {
            var report = Path.GetTempFileName();

            run.RegisterResultHarvester(()=>
                {
                    var parser = new MSpecReportParser(report, run);
                    return parser.Parse();
                });

            run.RegisterCleanup(() =>
                {
                    if (_fileSystem.FileExists(report))
                    {
                        File.Delete(report);
                    }
                });

            yield return String.Format("--xml \"{0}\"", report);
        }

        static IEnumerable<string> Assemblies(MSpecTestRunner.Run run)
        {
            return run.RunInfos
                .Select(x => x.Assembly)
                .Distinct()
                .Select(x => "\"" + x + "\"");
        } 
    }
}