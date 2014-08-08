using System;

namespace AutoTest.Minimizer.Extensions
{
    internal class MethodNotFoundException : Exception
    {
        public MethodNotFoundException(string s) : base(s)
        {
            
        }
    }
}