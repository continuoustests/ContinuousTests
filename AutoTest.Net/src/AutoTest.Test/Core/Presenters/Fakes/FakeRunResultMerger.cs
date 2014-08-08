using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.Caching;
using AutoTest.Core.TestRunners;
using AutoTest.Core.BuildRunners;
using AutoTest.Core.Caching.RunResultCache;
using AutoTest.Messages;

namespace AutoTest.Test.Core.Presenters.Fakes
{
    class FakeRunResultMerger : IMergeRunResults
    {
        public bool HasMergedBuildResults { get; private set; }
        public bool HasMergedTestResults { get; private set; }

        public FakeRunResultMerger()
        {
            HasMergedBuildResults = false;
            HasMergedTestResults = false;
        }

        #region IMergeRunResults Members

        public void Merge(BuildRunResults results)
        {
            HasMergedBuildResults = true;
        }

        public void Merge(TestRunResults results)
        {
            HasMergedTestResults = true;
        }

        public void EnabledDeltas()
        {
        }

        public RunResultCacheDeltas PopDeltas()
        {
            return null;
        }

        public void Clear()
        {
        }

        #endregion
    }
}
