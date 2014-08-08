using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace AutoTest.Client.HTTP
{
    public class Browse
    {
        public static void To(string url)
        {
            var process = new Process();
            process.StartInfo = new ProcessStartInfo(url);
            process.Start();
        }
    }
}
