using Mono.Cecil;

namespace AutoTest.Minimizer
{
    internal class ChangeContext
    {
        public TypeDefinition Context { get; set; }

        public string Member { get; set; }

        public ChangeContext(string member, TypeDefinition context)
        {
            Context = context;
            Member = member;
        }
    }
}