using Mono.Cecil;

namespace AutoTest.Minimizer.InterfaceFollowingStrategies
{
    class NeverFollowInterfaceFollowingStrategy : IInterfaceFollowingStrategy
    {
        public bool ShouldContinueAfter(MemberReference memberReference)
        {
            return false;
        }
    }
}
