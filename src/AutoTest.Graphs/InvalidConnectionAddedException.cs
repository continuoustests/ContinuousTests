using System;

namespace AutoTest.Graphs
{
    public class InvalidConnectionAddedException : Exception
    {
        public InvalidConnectionAddedException(string @from, string to) : base("cannot add a connection from " + @from + " to " + to)
        {
            
        }
    }
}