namespace AutoTest.Profiler
{
    public interface ICouplingGraphProjection
    {
        string GetSnapshotExtension();
        void Index(TestRunInformation testInformation);
        void Remove(TestRunInformation testInformation);
        void SnapShotTo(string filename, long marker);
        long LoadFromSnapshot(string filename);
        void Clear();
    }
}