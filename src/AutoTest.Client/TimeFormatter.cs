using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.Client
{
    public class TimeFormatter
    {
        public static string FormatTime(double amount) {
            return (amount * 1000).ToString("#.000") + " ms";
        }
    }
}
