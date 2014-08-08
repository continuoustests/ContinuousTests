using Mono.Cecil;

namespace AutoTest.Profiler
{
    public class ProfilerEntry
    {
        public ProfileType Type { get; set; }
        public int Sequence { get; set; }
        public int Thread { get; set; }
        public long Functionid { get; set; }
        public double Time { get; set; }
        public string Method { get; set; }
        public string Runtime { get; set; }
        public bool Found { get; set; }
        public bool IsTest { get; set; }

        public bool IsFixtureConstructor;
        public MemberReference Reference;
        public string OriginalRuntime { get; set; }
        public bool IsSetup{ get; set; }
        public bool IsTeardown { get; set; }

        public ProfilerEntry(ProfileType type, int sequence, int thread, long functionid, long time, string method, string runtime)
        {
            Type = type;
            Sequence = sequence;
            Thread = thread;
            Functionid = functionid;
            Time = time;
            Method = method;
            Runtime = runtime;
        }

        public ProfilerEntry()
        {
        }
    }
}