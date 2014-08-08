using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.Core.Configuration;
using Rhino.Mocks;
using AutoTest.Core.Messaging;
using AutoTest.Core.Caching.Projects;
using System.IO;
using AutoTest.Core.DebugLog;
using System;

namespace AutoTest.Test.Core.Configuration
{
    class TestConfigLocator : ILocateWriteLocation
    {
        public string GetLogfile()
        {
            return "";
        }

        public string GetConfigurationFile()
        {
            var path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            return Path.Combine(path, "AutoTest.ForTests.config");
        }
    }

    [TestFixture]
    public class ConfigTest
    {
        private Config _config;
        private IMessageBus _bus;
		private string _overridConfig;

        [SetUp]
        public void SetUp()
        {
			_overridConfig = Path.GetTempFileName();
            _bus = MockRepository.GenerateMock<IMessageBus>();
            _config = new Config(_bus, new TestConfigLocator());
        }
		
		[TearDown]
		public void TearDown()
		{
			if (File.Exists(_overridConfig))
				File.Delete(_overridConfig);
		}

        [Test]
        public void Should_read_directory_to_watch()
        {
            _config.WatchDirectores[0].ShouldEqual("TestResources");
        }

        [Test]
        public void Should_read_default_build_executable()
        {
            var document = new ProjectDocument(ProjectType.CSharp);
            _config.BuildExecutable(document).ShouldEqual(@"C:\Somefolder\MSBuild.exe");
        }

        [Test]
        public void Should_get_framework_spesific_build_executable()
        {
            var document = new ProjectDocument(ProjectType.CSharp);
            document.SetFramework("v3.5");
            _config.BuildExecutable(document).ShouldEqual(@"C:\SomefolderOther\MSBuild.exe");
        }

        [Test]
        public void Should_get_product_version_spesific_build_executable()
        {
            var document = new ProjectDocument(ProjectType.CSharp);
            document.SetFramework("v3.5");
            document.SetVSVersion("9.0.30729");
            _config.BuildExecutable(document).ShouldEqual(@"C:\ProductVersionFolder\MSBuild.exe");
        }

        [Test]
        public void Should_read_default_nunit_testrunner_path()
        {
            _config.NunitTestRunner("").ShouldEqual(@"C:\Somefolder\NUnit\nunit-console.exe");
        }

        [Test]
        public void Should_read_nunit_testrunner_path()
        {
            _config.NunitTestRunner("v3.5").ShouldEqual(@"C:\SomefolderOther\NUnit\nunit-console.exe");
        }

        [Test]
        public void Should_read_default_mstest_testrunner_path()
        {
            _config.MSTestRunner("").ShouldEqual(@"C:\Somefolder\MSTest.exe");
        }

        [Test]
        public void Should_read_mstest_testrunner_path()
        {
            _config.MSTestRunner("v3.5").ShouldEqual(@"C:\SomefolderOther\MSTest.exe");
        }

        [Test]
        public void Should_read_code_editor()
        {
            var editor = _config.CodeEditor;
            editor.Executable.ShouldEqual(@"C:\Program Files (x86)\Microsoft Visual Studio 10.0\Common7\IDE\devenv");
            editor.Arguments.ShouldEqual("/Edit \"[[CodeFile]]\" /command \"Edit.Goto [[LineNumber]]\"");
        }

        [Test]
        public void Should_read_debug_flag()
        {
            var state = _config.DebuggingEnabled;
            state.ShouldBeTrue();
        }

        [Test]
        public void Should_read_default_xunit_testrunner_path()
        {
            _config.XunitTestRunner("").ShouldEqual(@"C:\Somefolder\XUnit\xunit.console.exe");
        }

        [Test]
        public void Should_read_xunit_testrunner_path()
        {
            _config.XunitTestRunner("v3.5").ShouldEqual(@"C:\SomefolderOther\XUnit\xunit.console.exe");
        }
		
		[Test]
		public void Should_read_growl_executable()
		{
			_config.GrowlNotify.ShouldEqual(@"C:\Meh\growlnotify.exe");
		}
		
		[Test]
		public void Should_read_notify_on_start()
		{
			_config.NotifyOnRunStarted.ShouldBeFalse();
		}
		
		[Test]
		public void Should_read_notify_on_completed()
		{
			_config.NotifyOnRunCompleted.ShouldBeFalse();
		}
		
		[Test]
		public void Should_get_watch_ignore_file()
		{
			var file = Path.Combine("TestResources", "myignorefile.txt");
			using (var writer = new StreamWriter(file))
			{
				writer.WriteLine("MyFolder");
				writer.WriteLine(@"OtherFolder\SomeFile.txt");
				writer.WriteLine(@"OhYeah\*");
				writer.WriteLine("*TestOutput.xml");
				writer.WriteLine("!meh.txt");
				writer.WriteLine("	#Comment");
				writer.WriteLine("");
			}
			
			_config.BuildIgnoreListFromPath("TestResources");
			_config.WatchIgnoreList.Length.ShouldEqual(4);
			_config.WatchIgnoreList[0].ShouldEqual("MyFolder");
			_config.WatchIgnoreList[1].ShouldEqual(@"OtherFolder/SomeFile.txt");
			_config.WatchIgnoreList[2].ShouldEqual(@"OhYeah/*");
			_config.WatchIgnoreList[3].ShouldEqual("*TestOutput.xml");
			if (File.Exists(file))
				File.Delete(file);
		}
		
		[Test]
		public void Should_get_watch_ignore_file_from_absolute_path()
		{
			var file = Path.Combine("TestResources", "myignorefile.txt");
			using (var writer = new StreamWriter(file))
			{
				writer.WriteLine("MyFolder");
				writer.WriteLine(@"OtherFolder\SomeFile.txt");
				writer.WriteLine(@"OhYeah\*");
				writer.WriteLine("*TestOutput.xml");
				writer.WriteLine("!meh.txt");
				writer.WriteLine("	#Comment");
				writer.WriteLine("");
			}
			
			createMergeFile();
			_config.Merge(_overridConfig);
			_config.BuildIgnoreListFromPath("");
			_config.WatchIgnoreList.Length.ShouldEqual(4);
			_config.WatchIgnoreList[0].ShouldEqual("MyFolder");
			_config.WatchIgnoreList[1].ShouldEqual(@"OtherFolder/SomeFile.txt");
			_config.WatchIgnoreList[2].ShouldEqual(@"OhYeah/*");
			_config.WatchIgnoreList[3].ShouldEqual("*TestOutput.xml");
			if (File.Exists(file))
				File.Delete(file);
		}
		
		[Test]
		public void Should_set_to_empty_array_if_file_doesnt_exist()
		{
			_config.BuildIgnoreListFromPath("SomeInvalidDirectory");
			_config.WatchIgnoreList.Length.ShouldEqual(0);
		}
		
		[Test]
		public void Should_get_test_assemblies_to_ignore()
		{
			_config.TestAssembliesToIgnore[0].ShouldEqual("*System.dll");
			_config.TestAssembliesToIgnore[1].ShouldEqual("meh.exe");
		}
		
		[Test]
		public void Should_get_test_categories_to_ignore()
		{
			_config.TestCategoriesToIgnore[0].ShouldEqual("Category1");
			_config.TestCategoriesToIgnore[1].ShouldEqual("Category2");
		}
		
		[Test]
		public void Should_get_delay()
		{
			_config.FileChangeBatchDelay.ShouldEqual(95);
		}
		
		[Test]
		public void Should_get_custom_output_path()
		{
			_config.CustomOutputPath.ShouldEqual(@"bin\CustomOutput");
		}
		
		[Test]
		public void Should_get_specific_test_runner()
		{
			_config.GetSpesificNunitTestRunner("v3.5").ShouldEqual(@"C:\SomefolderOther\NUnit\nunit-console.exe");
		}
		
		[Test]
		public void Should_return_when_null_when_geting_specific_test_runner_for_nonexisting_version()
		{
			_config.GetSpesificNunitTestRunner("v1.0").ShouldBeNull();
		}
		
		[Test]
		public void Should_get_failed_first_pre_processor_setting()
		{
			_config.RerunFailedTestsFirst.ShouldBeTrue();
		}
		
		[Test]
		public void Should_merge_two_config_files()
		{
			createMergeFile();
			var document = new ProjectDocument(ProjectType.CSharp);
			var document35 = new ProjectDocument(ProjectType.CSharp);
			document35.SetFramework("v3.5");
			_config.Merge(_overridConfig);
				
            _config.BuildExecutable(document).ShouldEqual("");
			_config.NunitTestRunner(document.Framework).ShouldEqual(@"C:\Somefolder\NUnit\nunit-console.exe");
			_config.NunitTestRunner(document35.Framework).ShouldEqual("NewTestRunner");
			_config.GrowlNotify.ShouldEqual("another_growl_notifier");
			_config.TestAssembliesToIgnore[2].ShouldEqual("MergedRule.dll");
			_config.FileChangeBatchDelay.ShouldEqual(800);
		}

        [Test]
        public void Should_read_build_solution_when_watching_solution()
        {
            _config.WhenWatchingSolutionBuildSolution.ShouldBeTrue();
        }

        [Test]
        public void Should_build_solution()
        {
            _config.ShouldBuildSolution.ShouldBeFalse();
        }

        [Test]
        public void Should_get_autotest_runner_setting()
        {
            _config.UseAutoTestTestRunner.ShouldBeFalse();
        }

        [Test]
        public void Should_start_paused_setting()
        {
            _config.StartPaused.ShouldBeTrue();
        }

        [Test]
        public void Should_use_lowest_common_denominator_path()
        {
            _config.UseLowestCommonDenominatorAsWatchPath.ShouldBeFalse();
        }

        [Test]
        public void Should_read_key_store_of_all_settings()
        {
            _config.AllSettings("SomCustomStuff").ShouldEqual("content");
        }

        [Test]
        public void Should_merge_key_store_of_all_settings()
        {
            createMergeFile();
            _config.Merge(_overridConfig);
            _config.AllSettings("SomCustomStuff").ShouldEqual("modified content");
            _config.AllSettings("AnotherSetting").ShouldEqual("something");
        }

        [Test]
        public void Should_get_watch_all_strategy()
        {
            _config.WatchAllFiles.ShouldBeTrue();
        }

        [Test]
        public void Should_get_parallel_test_run_setting()
        {
            _config.RunAssembliesInParallel.ShouldBeTrue();
        }

        [Test]
        public void Should_get_test_runner_compatibility_mode()
        {
            _config.TestRunnerCompatibilityMode.ShouldBeTrue();
        }
		
		[Test]
        public void Should_get_log_resycle_size()
        {
            _config.LogRecycleSize.ShouldEqual(123456789);
        }

        [Test]
        public void Should_get_msbuild_additional_parameters()
        {
            _config.MSBuildAdditionalParameters.ShouldEqual("more params");
        }

        [Test]
        public void Should_get_msbuild_parallel_build_count()
        {
            Assert.That(_config.MSBuildParallelBuildCount, Is.EqualTo(3));
        }

        [Test]
		public void Should_get_projects_to_ignore()
		{
			_config.ProjectsToIgnore[0].ShouldEqual("*MyProject.csproj");
			_config.ProjectsToIgnore[1].ShouldEqual("FullPathTo/aproject.csproj");
		}

        [Test]
        public void Should_get_autotest_provider()
        {
            _config.Providers.ShouldEqual("php");
        }

		private void createMergeFile()
		{
			if (File.Exists(_overridConfig))
				File.Delete(_overridConfig);
			var path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
			var file = Path.Combine(path, Path.Combine("TestResources", "myignorefile.txt"));
			using (var writer = new StreamWriter(_overridConfig))
			{
				writer.WriteLine("<configuration>");
					writer.WriteLine("<BuildExecutable override=\"exclude\">some_text_just_to_make_sure_no_its_not_used</BuildExecutable>");
					
					writer.WriteLine("<NUnitTestRunner framework=\"v3.5\" override=\"merge\">NewTestRunner</NUnitTestRunner>");
					
					writer.WriteLine("<growlnotify>another_growl_notifier</growlnotify>");
					
					writer.WriteLine("<ShouldIgnoreTestAssembly override=\"merge\">");
						writer.WriteLine("<Assembly>MergedRule.dll</Assembly>");
					writer.WriteLine("</ShouldIgnoreTestAssembly>");
					writer.WriteLine(string.Format("<IgnoreFile>{0}</IgnoreFile>", file));
					writer.WriteLine("<changedetectiondelay>800</changedetectiondelay>");
                    writer.WriteLine("<SomCustomStuff>modified content</SomCustomStuff>");
                    writer.WriteLine("<AnotherSetting>something</AnotherSetting>");
				writer.WriteLine("</configuration>");
			}
		}
    }
}
