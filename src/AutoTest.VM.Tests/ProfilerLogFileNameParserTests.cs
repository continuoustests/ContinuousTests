using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace AutoTest.VM.Tests
{
    [TestFixture]
    public class ProfilerLogFileNameParserTests
    {
        [Test]
        public void When_given_a_profiler_log_it_can_retrieve_process_id()
        {
            Assert.That(
                new ProfilerLogFileNameParser(@"C:\Users\ack\AppData\Local\Temp\mm_output_3288_x86_v2.0_tmpC2CE_4e4a7365-e0a2-40ca-9c48-0624ae82159a.log")
                    .GetProcessID(),
                Is.EqualTo(3288));
        }
    }
}
