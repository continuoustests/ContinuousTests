namespace AutoTest.Profiler.Tests
{
    public static class Entry
    {
        public static EntryBuilder Enter(int i)
        {
            return new EntryBuilder(EntryType.Enter, i);
        }

        public static EntryBuilder Leave(int i)
        {
            return new EntryBuilder(EntryType.Leave, i);
        }
    }
}