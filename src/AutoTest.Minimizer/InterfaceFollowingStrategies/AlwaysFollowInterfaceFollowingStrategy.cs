using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace AutoTest.Minimizer.InterfaceFollowingStrategies
{
    class AlwaysFollowInterfaceFollowingStrategy : IInterfaceFollowingStrategy
    {
        public bool ShouldContinueAfter(MemberReference memberReference)
        {
            return true;
        }
    }
}
