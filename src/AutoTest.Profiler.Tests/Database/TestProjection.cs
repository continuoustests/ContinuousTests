namespace AutoTest.Profiler.Tests.Database
{
    internal class TestProjection : ICouplingGraphProjection
    {
        public string GetSnapshotExtension()
        {
            throw new System.NotImplementedException();
        }

        public void Index(TestRunInformation testInformation)
        {
            IndexCalledCount++;
        }

        public int IndexCalledCount { get; set; }


        public void Remove(TestRunInformation testInformation)
        {
            RemoveCalledCount++;
        }

        public void SnapShotTo(string filename, long marker)
        {
            throw new System.NotImplementedException();
        }

        public long LoadFromSnapshot(string filename)
        {
            throw new System.NotImplementedException();
        }

        public void Clear()
        {
            
        }

        public int RemoveCalledCount { get; set; }
    }
}