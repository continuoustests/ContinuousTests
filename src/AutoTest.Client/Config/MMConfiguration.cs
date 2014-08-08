using System;
using System.Linq;
using System.Collections.Generic;

namespace AutoTest.Client.Config
{
    public enum GraphProvider
    {
        BUILTINDARK,
        BUILTINLIGHT,
        GRAPHVIZ,
        DGML,
        WINDOW
    }

    public enum ProfilerSettings
    {
        RUN,
        DONTRUN,
        RUNANDAUTODETECT
    }

    class GraphProviderConverter
    {
        public static string FromEnum(GraphProvider provider)
        {
            return provider.ToString();
        }

        public static GraphProvider FromString(string provider)
        {
            if (provider.ToUpper().Equals("GRAPHVIZ"))
                return GraphProvider.GRAPHVIZ;
            if (provider.ToUpper().Equals("DGML"))
                return GraphProvider.DGML;
            if (provider.ToUpper().Equals("BUILTINLIGHT"))
                return GraphProvider.BUILTINLIGHT;
            if (provider.ToUpper().Equals("BUILTINDARK"))
                return GraphProvider.BUILTINDARK;
            if (provider.ToUpper().Equals("WINDOW"))
                return GraphProvider.WINDOW;
            return GraphProvider.BUILTINDARK;
        }
    }

    public class MMConfiguration
    {
        private List<VersionConfig> _buildExecutables = new List<VersionConfig>();

        public IEnumerable<VersionConfig> BuildExecutables { get { return _buildExecutables; } }
        public GraphProvider GraphProvider { get; private set; }
        public bool MinimizerDebug { get; private set; }
        public bool IgnoreWarmup { get; private set; }
        public string IgnoreThisUpgrade { get; private set; }
        public bool AllDisabled { get; private set; }

        private bool _realtimeFeedback = false;
        public bool RealtimeFeedback { get { return _realtimeFeedback && BuildExecutables.Count() > 0; } }

        public bool BuildErrorsInFeedbackWindow { get; private set; }
        public bool BuildWarningsInFeedbackWindow { get; private set; }
        public bool FailingTestsInFeedbackWindow { get; private set; }
        public bool IgnoredTestsInFeedbackWindow { get; private set; }
        public bool CatMode { get; private set; }
        public bool OverlayNotifications { get; private set; }
        public string MinimizerLevel { get; private set; }
        public bool RiscEnabled { get; private set; }
        public bool AnonFeedback { get; private set; }

        public MMConfiguration()
        {
            Reload("");
        }

        public void Reload(string solutionFile)
        {
            var parser = new ConfigParser(solutionFile);
            var globalConfig = parser.ParseGlobal();
            var localConfig = parser.ParseLocal();

            _buildExecutables = merge(globalConfig.MSBuild, localConfig.MSBuild);
            GraphProvider = GraphProviderConverter.FromString(merge(globalConfig.GraphProvider, localConfig.GraphProvider, GraphProviderConverter.FromEnum(GraphProvider.BUILTINLIGHT)));
            MinimizerDebug = merge(globalConfig.MinimizerDebug, localConfig.MinimizerDebug, false);
            IgnoreWarmup = merge(globalConfig.IgnoreWarmup, localConfig.IgnoreWarmup, false);
            IgnoreThisUpgrade = merge(globalConfig.IgnoreThisUpgrade, localConfig.IgnoreThisUpgrade, "");
            _realtimeFeedback = merge(globalConfig.RealtimeFeedback, localConfig.RealtimeFeedback, false);
            BuildErrorsInFeedbackWindow = merge(globalConfig.BuildErrorsInFeedbackWindow, localConfig.BuildErrorsInFeedbackWindow, true);
            BuildWarningsInFeedbackWindow = merge(globalConfig.BuildWarningsInFeedbackWindow, localConfig.BuildWarningsInFeedbackWindow, true);
            FailingTestsInFeedbackWindow = merge(globalConfig.FailingTestsInFeedbackWindow, localConfig.FailingTestsInFeedbackWindow, true);
            CatMode = merge(globalConfig.CatMode, localConfig.CatMode, false);
            OverlayNotifications = merge(globalConfig.OverlayNotifications, localConfig.OverlayNotifications, false);
            AllDisabled = merge(globalConfig.AllDisabled, localConfig.AllDisabled, false);
            AnonFeedback = merge(globalConfig.AnonFeedback, localConfig.AnonFeedback, true);

            IgnoredTestsInFeedbackWindow = merge(globalConfig.IgnoredTestsInFeedbackWindow, localConfig.IgnoredTestsInFeedbackWindow, true);
            MinimizerLevel = merge(globalConfig.MinimizerLevel, localConfig.MinimizerLevel, "run");
            RiscEnabled = merge(globalConfig.RiscEnabled, localConfig.RiscEnabled, true);
        }

        public void OverrideRealtimeFeedback(bool newValue)
        {
            _realtimeFeedback = newValue;
        }

        private static List<T> merge<T>(List<ConfigItem<T>> global, List<ConfigItem<T>> local)
        {
            if (local.Exists(x => x.ShouldExclude))
                return new List<T>();
            if (local.Exists(x => x.ShouldMerge))
            {
                var list = new List<T>();
                list.AddRange(global.Select(x => x.Item));
                list.AddRange(local.Where(x => !list.Exists(y => y.Equals(x))).Select(x => x.Item));
                return list;
            }
            if (local.Count > 0)
                return local.Select(x => x.Item).ToList();
            return global.Select(x => x.Item).ToList();
        }

        private static T merge<T>(ConfigItem<T> global, ConfigItem<T> local, T defaultValue)
        {
            if (local == null || !local.Exists)
            {
                if (global == null)
                    return defaultValue;
                if (global.Exists)
                    return global.Item;
                return defaultValue;
            }
            if (local.ShouldExclude)
                return defaultValue;
            return local.Item;
        }
    }
}
