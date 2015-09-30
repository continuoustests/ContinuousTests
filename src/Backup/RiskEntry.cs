using System;
using System.Collections.Generic;

namespace AutoTest.VS.RiskClassifier
{
    public class RiskEntry
    {
        private List<string> testEntries = new List<string>();
        private readonly string _signature;
        private bool _isFilled;
        private bool _exists;
        private int _numberOfAssociatedTests;
        private int _riskMetric;
        private int _timesCalled;
        private double _averageTime;
        private double _averageTimeUnder;
        

        public EventHandler<RiskEntryChangedArgs> Changed;
        public EventHandler<RiskEntryInvalidatedArgs> Invalidated;
        private int _nodeCount;
        private int _complexity;
        private int _testsScore;
        private int _graphScore;

        public RiskEntry(string signature, int numberOfAssociatedTests, int riskMetric, int nodeCount)
        {
            _isFilled = false;
            _exists = false;
            _signature = signature;
            _riskMetric = riskMetric;
            _numberOfAssociatedTests = numberOfAssociatedTests;
            _nodeCount = nodeCount;
        }

        public int GraphScore
        {
            get { return _graphScore; }
        }

        public int TestsScore
        {
            get { return _testsScore; }
        }

        public int Complexity
        {
            get { return _complexity; }
        }

        public int NodeCount
        {
            get { return _nodeCount; }
        }

        public bool Exists
        {
            get { return _exists; }
        }

        public bool IsFilled
        {
            get { return _isFilled; }
        }

        public int RiskMetric
        {
            get { return _riskMetric; }
        }

        public int NumberOfAssociatedTests
        {
            get { return _numberOfAssociatedTests; }
        }

        public string Signature
        {
            get { return _signature; }
        }

        public bool IsTest
        {
            get { return testEntries != null && testEntries.Count > 0; }
        }

        public int TimesCalled { get { return _timesCalled; } }
        public double AverageTime { get { return _averageTime; } }
        public double AverageTimeUnder { get { return _averageTimeUnder; } }

        public bool NeedsRefresh { get; set; }

        public string TestType
        {
            get
            {
                return testEntries.Count == 0 ? "" : testEntries[0];
            }
        }

        public void SetNewData(int numberOfTests, bool found, List<string> descriptors, int riskMetric, int nodeCount, int callCount, double averageTime, double averageTimeUnder, int testsScore, int graphScore, int complexity)
        {
            NeedsRefresh = false;
            _isFilled = true;
            _exists = found;
            _numberOfAssociatedTests = numberOfTests;
            _riskMetric = riskMetric;
            testEntries = descriptors;
            _averageTimeUnder = averageTimeUnder;
            _timesCalled = callCount;
            _averageTime = averageTime;
            _nodeCount = nodeCount;
            _testsScore = testsScore;
            _graphScore = graphScore;
            _complexity = complexity;
            RaiseChanged();
        }

        private void RaiseChanged()
        {
            if(Changed != null)
            {
                Changed(this, new RiskEntryChangedArgs());
            }
        }

        private void RaiseInvalidated()
        {
            if (Invalidated != null)
            {
                Invalidated(this, new RiskEntryInvalidatedArgs());
            }
        }

        public void Invalidate()
        {
            NeedsRefresh = true;
            RaiseInvalidated();
        }

        public void Update()
        {
            EntryCache.Update(Signature);
        }

        public void InvalidateNoUpdate()
        {
            RaiseChanged();
        }
    }
}