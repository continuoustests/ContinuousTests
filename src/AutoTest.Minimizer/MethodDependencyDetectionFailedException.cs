using System;

namespace AutoTest.Minimizer
{
    public class MethodDependencyDetectionFailedException : Exception
    {
        public MethodDependencyDetectionFailedException(string cacheName, Exception inner) : base("Detection failed on method: " + cacheName, inner)
        {
        }
    }
}