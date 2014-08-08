using System;

namespace AutoTest.Minimizer.Extensions
{
    internal class BackGroundProcessException : Exception
    {
        public BackGroundProcessException(Exception innerException) : base("background error occurred", innerException)
        {
        }
    }
}