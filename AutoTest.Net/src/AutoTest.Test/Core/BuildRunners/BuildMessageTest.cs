using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.Core.BuildRunners;
using AutoTest.Messages;

namespace AutoTest.Test.Core.BuildRunners
{
    [TestFixture]
    public class BuildMessageTest
    {
        [Test]
        public void Should_be_equal()
        {
            var message1 = new BuildMessage();
            message1.ErrorMessage = "error message";
            message1.File = "file";
            message1.LineNumber = 15;
            message1.LinePosition = 20;

            var message2 = new BuildMessage();
            message2.ErrorMessage = "error message";
            message2.File = "file";
            message2.LineNumber = 15;
            message2.LinePosition = 20;

            message1.Equals(message2).ShouldBeTrue();
        }

        [Test]
        public void Should_not_be_equal()
        {
            var message1 = new BuildMessage();
            message1.ErrorMessage = "error message";
            message1.File = "file";
            message1.LineNumber = 15;
            message1.LinePosition = 20;

            var message2 = new BuildMessage();
            message2.ErrorMessage = "another error message";
            message2.File = "file";
            message2.LineNumber = 17;
            message2.LinePosition = 10;

            message1.Equals(message2).ShouldBeFalse();
        }
    }
}
