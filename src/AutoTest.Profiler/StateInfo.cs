using System;
using System.Collections.Generic;

namespace AutoTest.Profiler
{
    class StateInfo
    {
        public TestRunInformation Info;
        public long Awaiting;
        public Stack<CallChain> CallChain = new Stack<CallChain>();
        public States State = States.Awaiting;
        public Stack<CallChain> Setups = new Stack<CallChain>();
        public Stack<CallChain> Teardowns = new Stack<CallChain>();

        
    }
}