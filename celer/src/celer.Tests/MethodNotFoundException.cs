using System;

namespace celer.Tests
{
    internal class MethodNotFoundException : Exception
    {
        public MethodNotFoundException(string s) : base(s)
        {
        }
    }
}