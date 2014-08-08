using System;

namespace AutoTest.Minimizer
{
    public class InvalidConnectionAddedException : Exception
    {
        public InvalidConnectionAddedException(string @from, string to) : base("cannot add a connection from " + @from + " to " + to)
        {
            
        }
    }
}