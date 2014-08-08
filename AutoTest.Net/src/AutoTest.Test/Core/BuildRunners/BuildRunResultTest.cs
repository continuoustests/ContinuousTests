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
    public class BuildRunResultTest
    {
        private BuildRunResults _results;

        [SetUp]
        public void SetUp()
        {
            _results = new BuildRunResults("");
        }

        [Test]
        public void Should_add_error()
        {
            _results.AddError(new BuildMessage());
            _results.ErrorCount.ShouldEqual(1);
        }

        [Test]
        public void Should_add_warning()
        {
            _results.AddWarning(new BuildMessage());
            _results.WarningCount.ShouldEqual(1);
        }

        [Test]
        public void Should_set_time_spent()
        {
            _results.SetTimeSpent(new TimeSpan(0, 2, 10));
            _results.TimeSpent.ShouldEqual(new TimeSpan(0, 2, 10));
        }
    }
}
