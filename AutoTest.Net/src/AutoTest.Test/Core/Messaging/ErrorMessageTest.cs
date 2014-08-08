using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.Core.Messaging;
using AutoTest.Messages;

namespace AutoTest.Test.Core.Messaging
{
    [TestFixture]
    public class ErrorMessageTest
    {
        [Test]
        public void Should_format_exception()
        {
            var exception = new Exception("Error message");
            var message = new ErrorMessage(exception);
            message.Error.ShouldEqual(string.Format("Error message{0}", Environment.NewLine));
        }
    }
}
