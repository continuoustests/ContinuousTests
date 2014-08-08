namespace AutoTest.Profiler
{
    public class CountsAndTimes
    {
        private int _timesCalled;
        private double _totalTime;
        private double _maxTime = double.MinValue;
        private double _minTime = double.MaxValue;
        private double _totalTimeUnder;
        private double _minTimeUnder = double.MaxValue;
        private double _maxTimeUnder = double.MinValue;

        public int TimesCalled { get { return _timesCalled; } }
        public double TotalTime { get { return _totalTime; } }
        public double AverageTime { get { return _totalTime / _timesCalled; } }
        public double MaxTime { get { return _maxTime; } }
        public double MinTime { get { return _minTime; } }
        public double TotalTimeUnder { get { return _totalTimeUnder; } }
        public double AverageTimeUnder { get { return _totalTimeUnder / _timesCalled; } }
        public double MaxTimeUnder { get { return _maxTimeUnder; } }
        public double MinTimeUnder { get { return _minTimeUnder; } }

        public CountsAndTimes() {}

        public CountsAndTimes(int timesCalled, double totalTime, double totalTimeUnder, double maxTime, 
                              double maxTimeUnder, double minTime, double minTimeUnder)
        {
            _timesCalled = timesCalled;
            _totalTime = totalTime;
            _totalTimeUnder = totalTimeUnder;
            _maxTime = maxTime;
            _maxTimeUnder = maxTimeUnder;
            _minTime = minTime;
            _minTimeUnder = minTimeUnder;
        }

        public void ProcessNewEntry(CallChain c)
        {
            _timesCalled++;
            var timeunder = c.EndTime - c.StartTime;
            _totalTimeUnder += timeunder;
            if (timeunder > _maxTimeUnder) _maxTimeUnder = timeunder;
            if (timeunder < _minTimeUnder) _minTimeUnder = timeunder;
            var time = timeunder - GetChildrenTime(c);
            if (time > _maxTime) _maxTime = time;
            if (time < _minTime) _minTime = time; 
            _totalTime += time;
        }

        public void RemoveEntry(CallChain c)
        {
            _timesCalled--;
            if (_timesCalled < 0) _timesCalled = 0;
            var timeunder = c.EndTime - c.StartTime;
            _totalTimeUnder -= timeunder;
            if (_totalTimeUnder < 0) _totalTimeUnder = 0;
            var time = timeunder - GetChildrenTime(c);
            _totalTime -= time;
            if (_totalTime < 0) _totalTime = 0;
        }

        private double GetChildrenTime(CallChain c)
        {
            double ret = 0;
            if(c.Children != null && c.Children.Count > 0) {
                foreach (var child in c.Children)
                {
                    if (child.Name == c.Name) continue;
                    ret += child.EndTime - child.StartTime;
                }
            }
            return ret;
        }

        public static CountsAndTimes Empty = new CountsAndTimes();
    }
}