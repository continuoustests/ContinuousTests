using System;

namespace AutoTest.Profiler
{
    public class ForEveryTestContextChangeFinder : IContextChangeFinder
    {
        public bool contextChangesWhen(ProfilerEntry entry)
        {
            return entry.IsTest;
        }
    }

    public class ForEveryFixtureContextChangeFinder : IContextChangeFinder
    {
        private string last;
        public bool contextChangesWhen(ProfilerEntry entry)
        {
            if (!(entry.IsTest || entry.IsSetup)) return false;
            var space = entry.Runtime.IndexOf(' ');
            if (space == -1) return false;
            var method = entry.Runtime.IndexOf("::");
            var classname = entry.Runtime.Substring(space, method - space);
            if (classname == last) return false;
            last = classname;
            return true;
        }
    }

}