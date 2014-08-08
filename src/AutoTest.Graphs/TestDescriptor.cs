namespace AutoTest.Graphs
{
    public class TestDescriptor
    {
        public readonly string Target;
        public readonly string Assembly;
        public readonly string TestRunner;
        public readonly string Type;
        public TestDescriptor(string target, string assembly, string testRunner, string type)
        {
            Target = target;
            Type = type;
            TestRunner = testRunner;
            Assembly = assembly;
        }

        public string GetKey()
        {
            return Assembly + ":" + Type + "::" + Target;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (TestDescriptor)) return false;
            return Equals((TestDescriptor) obj);
        }

        public bool Equals(TestDescriptor other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Target, Target) && Equals(other.Assembly, Assembly) && Equals(other.Type, Type);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (Target != null ? Target.GetHashCode() : 0);
                result = (result*397) ^ (Assembly != null ? Assembly.GetHashCode() : 0);
                result = (result*397) ^ (Type != null ? Type.GetHashCode() : 0);
                return result;
            }
        }
    }
}