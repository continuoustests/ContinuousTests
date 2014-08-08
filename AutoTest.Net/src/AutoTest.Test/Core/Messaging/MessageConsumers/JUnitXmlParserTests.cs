using System;
using System.IO;
using NUnit.Framework;
using AutoTest.Messages;
using AutoTest.Core.Messaging.MessageConsumers;
using AutoTest.Core.ForeignLanguageProviders.Php;

namespace AutoTest.Test.Core.Messaging.MessageConsumers
{
	[TestFixture]
	public class JUnitXmlParserTests
	{
		[Test]
		public void When_parsing_an_invalid_xml_it_returns_emptylist()
		{
			Assert.That(
				JUnitXmlParser.Parse("bleh", "/mytest/location").Count,
				Is.EqualTo(0));
		}

		[Test]
		public void When_parsing_a_valid_xml_it_will_return_tests()
		{
			var result = JUnitXmlParser.Parse(getXml(), "/mytest/location");
			Assert.That(result.Count, Is.EqualTo(2));
			Assert.That(result[0].Project, Is.EqualTo("Acme\\DemoBundle\\Tests\\Utility\\ParserTestst"));
			Assert.That(result[0].Assembly, Is.EqualTo("/mytest/location"));
			Assert.That(result[0].TimeSpent, Is.EqualTo(TimeSpan.FromMilliseconds(4)));
			Assert.That(result[0].Runner, Is.EqualTo(TestRunner.PhpUnit));
			Assert.That(result[0].All.Length, Is.EqualTo(2));
			Assert.That(result[0].Passed.Length, Is.EqualTo(1));
			Assert.That(result[0].Failed.Length, Is.EqualTo(1));
			Assert.That(result[0].Ignored.Length, Is.EqualTo(0));

			Assert.That(result[0].Passed[0].Runner, Is.EqualTo(TestRunner.PhpUnit));
			Assert.That(result[0].Passed[0].Status, Is.EqualTo(TestRunStatus.Passed));
			Assert.That(result[0].Passed[0].Name, Is.EqualTo("Acme\\DemoBundle\\Tests\\Utility\\ParserTestst\\testPassingTest"));
			Assert.That(result[0].Passed[0].Message, Is.EqualTo(""));
			Assert.That(result[0].Passed[0].StackTrace.Length, Is.EqualTo(0));

			Assert.That(result[0].Failed[0].Runner, Is.EqualTo(TestRunner.PhpUnit));
			Assert.That(result[0].Failed[0].Status, Is.EqualTo(TestRunStatus.Failed));
			Assert.That(result[0].Failed[0].Name, Is.EqualTo("Acme\\DemoBundle\\Tests\\Utility\\ParserTestst\\testFailingTest"));
			Assert.That(result[0].Failed[0].Message, Is.EqualTo("Acme\\DemoBundle\\Tests\\Utility\\ParserTestst::testFailingTest" + Environment.NewLine + "Failed asserting that false matches expected true."));
			Assert.That(result[0].Failed[0].StackTrace.Length, Is.EqualTo(2));
			Assert.That(result[0].Failed[0].StackTrace[0].Method, Is.EqualTo(""));
			Assert.That(result[0].Failed[0].StackTrace[0].File, Is.EqualTo("/home/ack/src/tmp/SymfonyTest/src/Acme/DemoBundle/Tests/Utility/ParserTest.php"));
			Assert.That(result[0].Failed[0].StackTrace[0].LineNumber, Is.EqualTo(12));
			Assert.That(result[0].Failed[0].StackTrace[0].ToString(), Is.EqualTo("/home/ack/src/tmp/SymfonyTest/src/Acme/DemoBundle/Tests/Utility/ParserTest.php:12"));
			Assert.That(result[0].Failed[0].StackTrace[1].Method, Is.EqualTo(""));
			Assert.That(result[0].Failed[0].StackTrace[1].File, Is.EqualTo("/home/ack/src/tmp/SymfonyTest/src/Acme/DemoBundle/Tests/Utility/ParserTest.php"));
			Assert.That(result[0].Failed[0].StackTrace[1].LineNumber, Is.EqualTo(11));
			Assert.That(result[0].Failed[0].StackTrace[1].ToString(), Is.EqualTo("/home/ack/src/tmp/SymfonyTest/src/Acme/DemoBundle/Tests/Utility/ParserTest.php:11"));

			Assert.That(result[1].Project, Is.EqualTo("Acme\\DemoBundle\\Tests\\Controller\\DemoControllerTest"));
			Assert.That(result[1].Assembly, Is.EqualTo("/mytest/location"));
			Assert.That(result[1].TimeSpent, Is.EqualTo(TimeSpan.FromMilliseconds(195)));
			Assert.That(result[1].Runner, Is.EqualTo(TestRunner.PhpUnit));
			Assert.That(result[1].All.Length, Is.EqualTo(2));
			Assert.That(result[1].Passed.Length, Is.EqualTo(1));
			Assert.That(result[1].Failed.Length, Is.EqualTo(1));
			Assert.That(result[1].Ignored.Length, Is.EqualTo(0));

			Assert.That(result[1].Passed[0].Runner, Is.EqualTo(TestRunner.PhpUnit));
			Assert.That(result[1].Passed[0].Status, Is.EqualTo(TestRunStatus.Passed));
			Assert.That(result[1].Passed[0].Name, Is.EqualTo("Acme\\DemoBundle\\Tests\\Controller\\DemoControllerTest\\testIndex"));
			Assert.That(result[1].Passed[0].Message, Is.EqualTo(""));
			Assert.That(result[1].Passed[0].StackTrace.Length, Is.EqualTo(0));

			Assert.That(result[1].Failed[0].Runner, Is.EqualTo(TestRunner.PhpUnit));
			Assert.That(result[1].Failed[0].Status, Is.EqualTo(TestRunStatus.Failed));
			Assert.That(result[1].Failed[0].Name, Is.EqualTo("Melin\\ModuleCommunityBundle\\Tests\\Unit\\QueryProviderTest\\testFailing"));
			Assert.That(result[1].Failed[0].Message, Is.EqualTo("Melin\\ModuleCommunityBundle\\Tests\\Unit\\QueryProviderTest::testFailing" + Environment.NewLine + "Undefined index: jNameQuery"));
			Assert.That(result[1].Failed[0].StackTrace.Length, Is.EqualTo(3));
			Assert.That(result[1].Failed[0].StackTrace[0].Method, Is.EqualTo(""));
			Assert.That(result[1].Failed[0].StackTrace[0].File, Is.EqualTo("/home/ack/src/melin/community/src/Melin/ModuleCommunityBundle/Command/Query/QuestionQueryProvider.php"));
			Assert.That(result[1].Failed[0].StackTrace[0].LineNumber, Is.EqualTo(17));
			Assert.That(result[1].Failed[0].StackTrace[0].ToString(), Is.EqualTo("/home/ack/src/melin/community/src/Melin/ModuleCommunityBundle/Command/Query/QuestionQueryProvider.php:17"));
			Assert.That(result[1].Failed[0].StackTrace[1].Method, Is.EqualTo(""));
			Assert.That(result[1].Failed[0].StackTrace[1].File, Is.EqualTo("/home/ack/src/melin/community/src/Melin/ModuleCommunityBundle/Command/Query/QuestionQueryProvider.php"));
			Assert.That(result[1].Failed[0].StackTrace[1].LineNumber, Is.EqualTo(52));
			Assert.That(result[1].Failed[0].StackTrace[1].ToString(), Is.EqualTo("/home/ack/src/melin/community/src/Melin/ModuleCommunityBundle/Command/Query/QuestionQueryProvider.php:52"));
			Assert.That(result[1].Failed[0].StackTrace[2].Method, Is.EqualTo(""));
			Assert.That(result[1].Failed[0].StackTrace[2].File, Is.EqualTo("/home/ack/src/melin/community/src/Melin/ModuleCommunityBundle/Tests/Unit/QueryProviderTest.php"));
			Assert.That(result[1].Failed[0].StackTrace[2].LineNumber, Is.EqualTo(12));
			Assert.That(result[1].Failed[0].StackTrace[2].ToString(), Is.EqualTo("/home/ack/src/melin/community/src/Melin/ModuleCommunityBundle/Tests/Unit/QueryProviderTest.php:12"));
		}

		private string getXml() {
			return
				string.Format(
					"<?xml version=\"1.0\" encoding=\"UTF-8\"?>{0}" +
					"<testsuites>{0}" +
					"  <testsuite name=\"Acme\\DemoBundle\\Tests\\Utility\\ParserTestst\" file=\"/home/ack/src/tmp/SymfonyTest/src/Acme/DemoBundle/Tests/Utility/ParserTest.php\" namespace=\"Acme\\DemoBundle\\Tests\\Utility\" fullPackage=\"Acme.DemoBundle.Tests.Utility\" tests=\"2\" assertions=\"1\" failures=\"1\" errors=\"0\" time=\"0.003987\">{0}" +
					"    <testcase name=\"testPassingTest\" class=\"Acme\\DemoBundle\\Tests\\Utility\\ParserTestst\" file=\"/home/ack/src/tmp/SymfonyTest/src/Acme/DemoBundle/Tests/Utility/ParserTest.php\" line=\"8\" assertions=\"0\" time=\"0.000140\"/>{0}" +
					"    <testcase name=\"testFailingTest\" class=\"Acme\\DemoBundle\\Tests\\Utility\\ParserTestst\" file=\"/home/ack/src/tmp/SymfonyTest/src/Acme/DemoBundle/Tests/Utility/ParserTest.php\" line=\"11\" assertions=\"1\" time=\"0.003847\">{0}" +
					"      <failure type=\"PHPUnit_Framework_ExpectationFailedException\">Acme\\DemoBundle\\Tests\\Utility\\ParserTestst::testFailingTest{0}" +
					"Failed asserting that false matches expected true.{0}" +
					"{0}" +
					"/home/ack/src/tmp/SymfonyTest/src/Acme/DemoBundle/Tests/Utility/ParserTest.php:12{0}" +
					"/home/ack/src/tmp/SymfonyTest/src/Acme/DemoBundle/Tests/Utility/ParserTest.php:11{0}" +
					"		</failure>{0}" +
					"    </testcase>{0}" +
					"  </testsuite>{0}" +
					"  <testsuite name=\"Project Test Suite\" tests=\"3\" assertions=\"2\" failures=\"1\" errors=\"0\" time=\"0.200313\">{0}" +
					"    <testsuite name=\"Acme\\DemoBundle\\Tests\\Controller\\DemoControllerTest\" file=\"/home/ack/src/tmp/SymfonyTest/src/Acme/DemoBundle/Tests/Controller/DemoControllerTest.php\" namespace=\"Acme\\DemoBundle\\Tests\\Controller\" fullPackage=\"Acme.DemoBundle.Tests.Controller\" tests=\"1\" assertions=\"1\" failures=\"0\" errors=\"0\" time=\"0.194546\">{0}" +
					"      <testcase name=\"testIndex\" class=\"Acme\\DemoBundle\\Tests\\Controller\\DemoControllerTest\" file=\"/home/ack/src/tmp/SymfonyTest/src/Acme/DemoBundle/Tests/Controller/DemoControllerTest.php\" line=\"9\" assertions=\"1\" time=\"0.194546\"/>{0}" +
					"		<testcase name=\"testFailing\" class=\"Melin\\ModuleCommunityBundle\\Tests\\Unit\\QueryProviderTest\" file=\"/home/ack/src/melin/community/src/Melin/ModuleCommunityBundle/Tests/Unit/QueryProviderTest.php\" line=\"9\" assertions=\"0\" time=\"0.001254\">{0}" +
					"	        <error type=\"PHPUnit_Framework_Error_Notice\">Melin\\ModuleCommunityBundle\\Tests\\Unit\\QueryProviderTest::testFailing{0}" +
					"Undefined index: jNameQuery{0}" +
					"{0}" +
					"/home/ack/src/melin/community/src/Melin/ModuleCommunityBundle/Command/Query/QuestionQueryProvider.php:17{0}" +
					"/home/ack/src/melin/community/src/Melin/ModuleCommunityBundle/Command/Query/QuestionQueryProvider.php:52{0}" +
					"/home/ack/src/melin/community/src/Melin/ModuleCommunityBundle/Tests/Unit/QueryProviderTest.php:12{0}" +
					"	</error>{0}" +
					"	      </testcase>{0}" +
					"    </testsuite>{0}" +
					"    <testsuite name=\"Acme\\DemoBundle\\Tests\\Controller\\Test2ControllerTest\" file=\"/home/ack/src/tmp/SymfonyTest/src/Acme/DemoBundle/Tests/Controller/Test2ControllerTest.php\" namespace=\"Acme\\DemoBundle\\Tests\\Controller\" fullPackage=\"Acme.DemoBundle.Tests.Controller\" tests=\"0\" assertions=\"0\" failures=\"0\" errors=\"0\" time=\"0.000000\"/>{0}" +
					"  </testsuite>{0}" +
					"</testsuites>{0}"
					,Environment.NewLine);
		}
	}

	[TestFixture]
	public class PhpUnitParseErrorParserTests
	{
		[Test]
		public void When_parse_error_is_wrongly_formatted_it_will_return_a_test_of_type_any_with_all_output()
		{
			var test = PhpUnitParseErrorParser.Parse("some error");

			Assert.That(test.Runner, Is.EqualTo(TestRunner.PhpParseError));
			Assert.That(test.Status, Is.EqualTo(TestRunStatus.Failed));
			Assert.That(test.Name, Is.EqualTo("some error"));
			Assert.That(test.Message, Is.EqualTo("some error"));
			Assert.That(test.StackTrace.Length, Is.EqualTo(0));
		}

		[Test]
		public void When_parse_error_is_formatted_as_parse_stack_it_will_return_test_of_type_any_with_stacktrace()
		{
			var test = PhpUnitParseErrorParser.Parse(getParseError());

			Assert.That(test.Runner, Is.EqualTo(TestRunner.PhpParseError));
			Assert.That(test.Status, Is.EqualTo(TestRunStatus.Failed));
			Assert.That(test.Name, Is.EqualTo("syntax error, unexpected 'return' (T_RETURN) in /home/ack/src/tmp/SymfonyTest/src/Acme/DemoBundle/Utility/Parser.php on line 8"));
			Assert.That(test.Message, Is.EqualTo("syntax error, unexpected 'return' (T_RETURN) in /home/ack/src/tmp/SymfonyTest/src/Acme/DemoBundle/Utility/Parser.php on line 8"));
			Assert.That(test.StackTrace.Length, Is.EqualTo(15));
			Assert.That(test.StackTrace[0].Method, Is.EqualTo(""));
			Assert.That(test.StackTrace[0].File, Is.EqualTo("/home/ack/src/tmp/SymfonyTest/src/Acme/DemoBundle/Utility/Parser.php"));
			Assert.That(test.StackTrace[0].LineNumber, Is.EqualTo(8));
			Assert.That(test.StackTrace[0].ToString(), Is.EqualTo("/home/ack/src/tmp/SymfonyTest/src/Acme/DemoBundle/Utility/Parser.php:8"));
			Assert.That(test.StackTrace[1].Method, Is.EqualTo("{main}()"));
			Assert.That(test.StackTrace[1].File, Is.EqualTo("/usr/bin/phpunit"));
			Assert.That(test.StackTrace[1].LineNumber, Is.EqualTo(0));
			Assert.That(test.StackTrace[12].Method, Is.EqualTo("ReflectionMethod->invokeArgs()"));
			Assert.That(test.StackTrace[12].File, Is.EqualTo("/usr/share/php/PHPUnit/Framework/TestCase.php"));
			Assert.That(test.StackTrace[12].LineNumber, Is.EqualTo(942));
		}

		[Test]
		public void When_parse_error_is_formatted_with_no_stacktrace_but_with_position_in_error_text_this_is_added_to_stacktrace()
		{
			var test = PhpUnitParseErrorParser.Parse("PHP Parse error:  syntax error, unexpected 'return' (T_RETURN) in /home/ack/src/tmp/SymfonyTest/src/Acme/DemoBundle/Utility/Parser.php on line 8");

			Assert.That(test.Runner, Is.EqualTo(TestRunner.PhpParseError));
			Assert.That(test.Status, Is.EqualTo(TestRunStatus.Failed));
			Assert.That(test.Name, Is.EqualTo("syntax error, unexpected 'return' (T_RETURN) in /home/ack/src/tmp/SymfonyTest/src/Acme/DemoBundle/Utility/Parser.php on line 8"));
			Assert.That(test.Message, Is.EqualTo("syntax error, unexpected 'return' (T_RETURN) in /home/ack/src/tmp/SymfonyTest/src/Acme/DemoBundle/Utility/Parser.php on line 8"));
			Assert.That(test.StackTrace.Length, Is.EqualTo(1));
		}

		[Test]
		public void When_parse_error_is_formatted_with_no_stack_info_it_will_return_as_message()
		{
			var test = PhpUnitParseErrorParser.Parse("PHP Parse error:  syntax error, unexpected 'return' (T_RETURN)");

			Assert.That(test.Runner, Is.EqualTo(TestRunner.PhpParseError));
			Assert.That(test.Status, Is.EqualTo(TestRunStatus.Failed));
			Assert.That(test.Name, Is.EqualTo("syntax error, unexpected 'return' (T_RETURN)"));
			Assert.That(test.Message, Is.EqualTo("syntax error, unexpected 'return' (T_RETURN)"));
			Assert.That(test.StackTrace.Length, Is.EqualTo(0));
		}

		private string getParseError() {
			var output =
				"PHP Parse error:  syntax error, unexpected 'return' (T_RETURN) in /home/ack/src/tmp/SymfonyTest/src/Acme/DemoBundle/Utility/Parser.php on line 8NEW_LINE_PLACEHOLDER" +
				"PHP Stack trace:NEW_LINE_PLACEHOLDER" +
				"PHP   1. {main}() /usr/bin/phpunit:0NEW_LINE_PLACEHOLDER" +
				"PHP   2. PHPUnit_TextUI_Command::main() /usr/bin/phpunit:46NEW_LINE_PLACEHOLDER" +
				"PHP   3. PHPUnit_TextUI_Command->run() /usr/share/php/PHPUnit/TextUI/Command.php:130NEW_LINE_PLACEHOLDER" +
				"PHP   4. PHPUnit_TextUI_TestRunner->doRun() /usr/share/php/PHPUnit/TextUI/Command.php:192NEW_LINE_PLACEHOLDER" +
				"PHP   5. PHPUnit_Framework_TestSuite->run() /usr/share/php/PHPUnit/TextUI/TestRunner.php:325NEW_LINE_PLACEHOLDER" +
				"PHP   6. PHPUnit_Framework_TestSuite->run() /usr/share/php/PHPUnit/Framework/TestSuite.php:705NEW_LINE_PLACEHOLDER" +
				"PHP   7. PHPUnit_Framework_TestSuite->runTest() /usr/share/php/PHPUnit/Framework/TestSuite.php:745NEW_LINE_PLACEHOLDER" +
				"PHP   8. PHPUnit_Framework_TestCase->run() /usr/share/php/PHPUnit/Framework/TestSuite.php:772NEW_LINE_PLACEHOLDER" +
				"PHP   9. PHPUnit_Framework_TestResult->run() /usr/share/php/PHPUnit/Framework/TestCase.php:751NEW_LINE_PLACEHOLDER" +
				"PHP  10. PHPUnit_Framework_TestCase->runBare() /usr/share/php/PHPUnit/Framework/TestResult.php:649NEW_LINE_PLACEHOLDER" +
				"PHP  11. PHPUnit_Framework_TestCase->runTest() /usr/share/php/PHPUnit/Framework/TestCase.php:804NEW_LINE_PLACEHOLDER" +
				"PHP  12. ReflectionMethod->invokeArgs() /usr/share/php/PHPUnit/Framework/TestCase.php:942NEW_LINE_PLACEHOLDER" +
				"PHP  13. Acme\\DemoBundle\\Tests\\Utility\\ParserTestst->testWhen_parsing_string_it_will_return_string() /usr/share/php/PHPUnit/Framework/TestCase.php:942NEW_LINE_PLACEHOLDER" +
				"PHP  14. Composer\\Autoload\\ClassLoader->loadClass() /usr/share/php/PHPUnit/Framework/TestCase.php:0";
			return output.Replace("NEW_LINE_PLACEHOLDER", Environment.NewLine);
		}
	}

	[TestFixture]
	public class PhpUnitLiveParserTests
	{
		[Test]
		public void When_given_an_invalid_line_it_will_return_false()
		{
			var parser = new PhpUnitLiveParser();
			Assert.That(parser.Parse("invalid"), Is.False);
		}

		[Test]
		public void When_given_an_ok_test_it_will_trurn_true_and_contain_the_test_and_number_of_tests()
		{
			var parser = new PhpUnitLiveParser();
			Assert.That(parser.Parse("ok 3 - Acme\\DemoBundle\\Tests\\Utility\\ParserTestst::testPassingTest"), Is.True);
			Assert.That(parser.Test, Is.EqualTo("testPassingTest"));
			Assert.That(parser.Class, Is.EqualTo("Acme\\DemoBundle\\Tests\\Utility\\ParserTestst"));
			Assert.That(parser.TestsCompleted, Is.EqualTo(3));
		}

		[Test]
		public void When_given_a_failing_test_it_will_trurn_true_and_contain_the_test_and_number_of_tests()
		{
			var parser = new PhpUnitLiveParser();
			Assert.That(parser.Parse("not ok 4 - Failure: Acme\\DemoBundle\\Tests\\Utility\\ParserTestst::testFailingTest"), Is.True);
			Assert.That(parser.Test, Is.EqualTo("testFailingTest"));
			Assert.That(parser.Class, Is.EqualTo("Acme\\DemoBundle\\Tests\\Utility\\ParserTestst"));
			Assert.That(parser.TestsCompleted, Is.EqualTo(4));
		}


		[Test]
		public void When_given_error_test_run_it_will_trurn_true_and_contain_the_test_and_number_of_tests()
		{
			var parser = new PhpUnitLiveParser();
			Assert.That(parser.Parse("not ok 4 - Error: Acme\\DemoBundle\\Tests\\Utility\\ParserTestst::testFailingTest"), Is.True);
			Assert.That(parser.Test, Is.EqualTo("testFailingTest"));
			Assert.That(parser.Class, Is.EqualTo("Acme\\DemoBundle\\Tests\\Utility\\ParserTestst"));
			Assert.That(parser.TestsCompleted, Is.EqualTo(4));
		}

		[Test]
		public void When_given_an_ignored_test_it_will_trurn_true_and_contain_the_test_and_number_of_tests()
		{
			var parser = new PhpUnitLiveParser();
			Assert.That(parser.Parse("not ok 5 - Acme\\DemoBundle\\Tests\\Utility\\ParserTestst::testIgnoreTest # TODO Incomplete Test"), Is.True);
			Assert.That(parser.Test, Is.EqualTo("testIgnoreTest"));
			Assert.That(parser.Class, Is.EqualTo("Acme\\DemoBundle\\Tests\\Utility\\ParserTestst"));
			Assert.That(parser.TestsCompleted, Is.EqualTo(5));
		}
	}

	[TestFixture]
	public class PhpFileConventionMatcherTests
	{
		[Test]
		[TestCase("bleh", new string[] {})]
		[TestCase("/token/nope", new string[] {})]
		[TestCase("/token/having/MyBundle/some/file.php", new[] {"/token/having/MyBundle/Tests"})]
		public void When_given_matching_path_it_will_respond_with_test_path(string file, string[] responses)
		{
			var matcher = new PhpFileConventionMatcher("/token", new[] {"Bundle/"}, new[] {"Tests"});
			assert(matcher, file, responses);
		}

		[Test]
		[TestCase("/token/having/MyBundle/some/file.php", new[] {"/token/having/MyBundle/Tests/some"})]
		public void When_testpath_ends_with_star_it_will_apply_directorys_after_pattern(string file, string[] responses)
		{
			var matcher = new PhpFileConventionMatcher("/token", new[] {"Bundle/"}, new[] {"Tests*"});
			assert(matcher, file, responses);
		}

		[Test]
		[TestCase("/MyBundle/file.php", new[] {"/MyBundle/Tests/1","/MyBundle/Tests/2"})]
		public void When_having_multiple_test_paths_it_will_return_all_variations(string file, string[] responses)
		{
			var matcher = new PhpFileConventionMatcher("/", new[] {"Bundle/"}, new[] {"Tests/1","Tests/2"});
			assert(matcher, file, responses);
		}


		[Test]
		[TestCase("/MyBundle/file.php", new[] {"/MyBundle/Tests/1","/MyBundle/Tests/2"})]
		[TestCase("/Framework/file.php", new[] {"/Framework/Tests/1","/Framework/Tests/2"})]
		public void When_having_multiple_patterns_and_multiple_test_paths_it_will_return_all_variations(string file, string[] responses)
		{
			var matcher = new PhpFileConventionMatcher("/", new[] {"Bundle/","Framework/"}, new[] {"Tests/1","Tests/2"});
			assert(matcher, file, responses);
		}

		private void assert(PhpFileConventionMatcher matcher, string file, string[] responses)
		{
			var matches = matcher.Match(file);
			Assert.That(matches.Length, Is.EqualTo(responses.Length));
			for (int i = 0; i < matches.Length; i++)
				Assert.That(matches[i], Is.EqualTo(responses[i]));
		}
	}
}