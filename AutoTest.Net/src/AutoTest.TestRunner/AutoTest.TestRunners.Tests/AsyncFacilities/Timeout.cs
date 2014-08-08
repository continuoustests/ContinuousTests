using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace AutoTest.TestRunners.Tests.AsyncFacilities
{
    public class Timeout
    {
        public static Timeout AfterTwoSeconds()
        {
            return new Timeout(DateTime.Now.AddSeconds(2));
        }

        public static Timeout AfterFiveSeconds()
        {
            return new Timeout(DateTime.Now.AddSeconds(5));
        }

        public static Timeout AfterFiftyMilliseconds()
        {
            return new Timeout(DateTime.Now.AddMilliseconds(50));
        }

        private DateTime _timeout;

        public Timeout(DateTime timeout)
        {
            _timeout = timeout;
        }

        public void IfNot(Func<bool> completion)
        {
            while (DateTime.Now < _timeout)
            {
                Thread.Sleep(10);
                if (completion.Invoke())
                    return;
            }
        }
    }
}
