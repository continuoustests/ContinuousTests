using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.TestRunners.Shared.Communication;
using AutoTest.TestRunners.Tests.AsyncFacilities;

namespace AutoTest.TestRunners.Tests.Communication
{
    [TestFixture]
    public class When_two_servers_sends_messages_through_a_junction
    {
		[Category("slow")]
        [Test]
        public void client_receives_both_messages()
        {
            var pipe1 = Guid.NewGuid().ToString();
            var pipe2 = Guid.NewGuid().ToString();
            var pipe3 = Guid.NewGuid().ToString();
            using (var server1 = new PipeServer(pipe1))
            {
                using (var server2 = new PipeServer(pipe2))
                {
                    using (var junction = new PipeJunction(pipe3))
                    {
                        junction.Combine(pipe1);
                        junction.Combine(pipe2);
                        server1.Send("Message from server 1");
                        server2.Send("Message from server 2");

                        var messages = new List<string>();
                        var client = new PipeClient();
                        new System.Threading.Thread(() => client.Listen(pipe3, (m) => messages.Add(m))).Start();
                        Timeout.AfterTwoSeconds().IfNot(() => messages.Count == 2);
                        Assert.That(messages.Count, Is.EqualTo(2));
                    }
                }
            }
        }
    }
}
