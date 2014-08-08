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
    public class When_server_sends_a_message
    {
		[Category("slow")]
        [Test]
        public void the_client_receives_it()
        {
            var pipeName = Guid.NewGuid().ToString();
            var client = new PipeClient();
            using (var server = new PipeServer(pipeName))
            {
                string receivedMessage = null;
                new System.Threading.Thread(() => client.Listen(pipeName, (m) => receivedMessage = m)).Start();
                server.Send("message sent by server");
                Timeout.AfterTwoSeconds().IfNot(() => receivedMessage != null);
                Assert.That(receivedMessage, Is.EqualTo("message sent by server"));
            }
        }
    }
}
