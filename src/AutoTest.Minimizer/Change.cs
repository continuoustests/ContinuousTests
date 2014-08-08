namespace AutoTest.Minimizer
{
    public enum ChangeType
    {
        Add,
        Remove,
        Modify
    }

    public class Change<T>
    {
        public readonly ChangeType ChangeType;
        public readonly T ItemChanged;

        public Change(ChangeType changeType, T itemChanged)
        {
            ChangeType = changeType;
            ItemChanged = itemChanged;
        }
    }
}