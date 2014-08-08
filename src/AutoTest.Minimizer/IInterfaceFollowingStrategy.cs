using Mono.Cecil;

namespace AutoTest.Minimizer
{
    internal interface IInterfaceFollowingStrategy
    {
        bool ShouldContinueAfter(MemberReference memberReference);
    }
}