using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.Core.TestRunners.TestRunners;
using AutoTest.Core.TestRunners;
using AutoTest.Messages;
using System.IO;

namespace AutoTest.Test.Core.TestRunners
{
     [TestFixture]
    public class MSTestResponseParserTest
    {
         private MSTestResponseParser _parser;

         [SetUp]
         public void SetUp()
         {
             _parser = new MSTestResponseParser("", "", false);
         }

         [Test]
         public void Should_find_passed_test()
         {
             _parser.ParseLine("Passed               The_name_of_the_test");
             var result = _parser.Result;

             result.Passed.Length.ShouldEqual(1);
             result.All[0].Status.ShouldEqual(TestRunStatus.Passed);
             result.All[0].Name.ShouldEqual("The_name_of_the_test");
         }

         [Test]
         public void Should_find_failed_test()
         {
             _parser.ParseLine("Failed               The_name_of_the_test");
             var result = _parser.Result;

             result.Failed.Length.ShouldEqual(1);
             result.All[0].Status.ShouldEqual(TestRunStatus.Failed);
             result.All[0].Name.ShouldEqual("The_name_of_the_test");
         }

         [Test]
         public void Should_find_ignored_test()
         {
             _parser.ParseLine("Ignored               The_name_of_the_test");
             var result = _parser.Result;

             result.Ignored.Length.ShouldEqual(1);
             result.All[0].Status.ShouldEqual(TestRunStatus.Ignored);
             result.All[0].Name.ShouldEqual("The_name_of_the_test");
         }

         [Test]
         public void Should_find_inconclusive_and_treat_as_ignored()
         {
             _parser.ParseLine("Inconclusive               The_name_of_the_test");
             var result = _parser.Result;

             result.Ignored.Length.ShouldEqual(1);
             result.All[0].Status.ShouldEqual(TestRunStatus.Ignored);
             result.All[0].Name.ShouldEqual("The_name_of_the_test");
         }

         [Test]
         public void Should_find_error_message_and_stacktrace()
         {
             _parser.ParseLine("Failed               The_name_of_the_test");
             _parser.ParseLine("[errormessage] = Assert.AreEqual failed. Expected:<2>. Actual:<9>. ");
             _parser.ParseLine("an error message can have multiple lines");
             _parser.ParseLine("[errorstacktrace] =    at Order.Test.UnitTest1.MyFourthTest() in c:\\Users\\sveina\\src\\DotNET\\Private\\TDDPeering_Internals - Copy (2)\\Order.Test\\UnitTest1.cs:line 99");
             _parser.ParseLine("at Order.Test.UnitTest1.MyFourthTest() in c:\\Users\\sveina\\src\\DotNET\\Private\\TDDPeering_Internals - Copy (2)\\Order.Test\\UnitTest1.cs:line 99");
             var result = _parser.Result;

             result.Failed.Length.ShouldEqual(1);
             result.All[0].Status.ShouldEqual(TestRunStatus.Failed);
             result.All[0].Name.ShouldEqual("The_name_of_the_test");
             result.All[0].Message.ShouldEqual(string.Format("Assert.AreEqual failed. Expected:<2>. Actual:<9>.{0}an error message can have multiple lines", Environment.NewLine));
             result.All[0].StackTrace.Length.ShouldEqual(2);
             result.All[0].StackTrace[0].Method.ShouldEqual("Order.Test.UnitTest1.MyFourthTest()");
             result.All[0].StackTrace[0].File.ShouldEqual("c:\\Users\\sveina\\src\\DotNET\\Private\\TDDPeering_Internals - Copy (2)\\Order.Test\\UnitTest1.cs");
             result.All[0].StackTrace[0].LineNumber.ShouldEqual(99);
             result.All[0].StackTrace[0].ToString().ShouldEqual("at Order.Test.UnitTest1.MyFourthTest() in c:\\Users\\sveina\\src\\DotNET\\Private\\TDDPeering_Internals - Copy (2)\\Order.Test\\UnitTest1.cs:line 99");
         }

         [Test]
         public void Should_set_result_as_partial_when_partial_is_passed()
         {
             var isPartial = true;
             var parser = new MSTestResponseParser("project", "assembly", isPartial);
             var result = parser.Result;

             result.IsPartialTestRun.ShouldBeTrue();
         }

         [Test]
         public void Should_parse_fatal_error_test_run()
         {
             var content = File.ReadAllLines(string.Format("TestResources{0}MSTest{0}mstestrun_run_error.txt", Path.DirectorySeparatorChar));
             foreach (var line in content)
                 _parser.ParseLine(line);
             _parser.Result.All.Length.ShouldEqual(1);
             _parser.Result.All[0].Status.ShouldEqual(TestRunStatus.Failed);
             _parser.Result.Failed[0].Name.ShouldEqual("Nrk.OnDemand.Backend.Distribution.Engine.UnitTests.Upload.SftpClientTests.SC_ShouldUploadAndRemoveFile");
             _parser.Result.Failed[0].Message.ShouldEqual(string.Format("The agent process was stopped while the test was running.{0}One of the background threads threw exception: {0}System.InvalidCastException: Unable to cast object of type 'clohccZ' to type 'Nrk.OnDemand.Backend.Distribution.Engine.TaskManager.ITask'.", Environment.NewLine));
             _parser.Result.Failed[0].StackTrace.Length.ShouldEqual(10);
         }
    }
}
