namespace AutoTest.Profiler
{
    public interface IContextChangeFinder
    {
        bool contextChangesWhen(ProfilerEntry entry);
    }
}