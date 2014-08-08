using System;
using NUnit.Framework;
using AutoTest.Core.Configuration;
using System.IO;
namespace AutoTest.Test.Core.Configuration
{
	[TestFixture]
	public class CoreSectionTest
	{
		
		private string _file;
		
		[SetUp]
		public void Setup()
		{
			_file = Path.GetTempFileName();
			using (var writer = new StreamWriter(_file))
			{
				writer.WriteLine("<configuration>");
					writer.WriteLine("<DirectoryToWatch override=\"merge\"></DirectoryToWatch>");
					writer.WriteLine("<DirectoryToWatch override=\"exclude\"></DirectoryToWatch>");
					
					writer.WriteLine("<BuildExecutable framework=\"v3.5\" override=\"exclude\"></BuildExecutable>");
					
					writer.WriteLine("<NUnitTestRunner></NUnitTestRunner>");
				
					writer.WriteLine("<Debugging override=\"exclude\">false</Debugging>");
					
					writer.WriteLine("<growlnotify override=\"merge\">C:\\Meh\\growlnotify.exe</growlnotify>");
					writer.WriteLine("<IgnoreFile override=\"exclude\"></IgnoreFile>");
				
					writer.WriteLine("<CodeEditor override=\"merge\">");
				      writer.WriteLine(@"<Executable>C:\Program Files (x86)\Microsoft Visual Studio 10.0\Common7\IDE\devenv</Executable>");
				      writer.WriteLine("<Arguments>/Edit \"[[CodeFile]]\" /command \"Edit.Goto [[LineNumber]]\"</Arguments>");
				    writer.WriteLine("</CodeEditor>");
					
					writer.WriteLine("<ShouldIgnoreTestAssembly override=\"merge\">");
						writer.WriteLine("<Assembly>*System.dll</Assembly>");
						writer.WriteLine("<Assembly>meh.exe</Assembly>");
					writer.WriteLine("</ShouldIgnoreTestAssembly>");
				writer.WriteLine("</configuration>");
			}
		}
		
		[TearDown]
		public void TearDown()
		{
			if (File.Exists(_file))
			    File.Delete(_file);
		}
		
		[Test]
		public void Should_read_merge_from_node()
		{
			var section = new CoreSection();
			section.Read(_file);
			section.WatchDirectories.ShouldMerge.ShouldBeTrue();
			section.WatchDirectories.ShouldExclude.ShouldBeFalse();
		}
		
		[Test]
		public void Should_read_from_first_node_only()
		{
			var section = new CoreSection();
			section.Read(_file);
			section.WatchDirectories.ShouldExclude.ShouldBeFalse();
		}
		
		[Test]
		public void Should_read_exclude_from_node()
		{
			var section = new CoreSection();
			section.Read(_file);
			section.BuildExecutables.ShouldExclude.ShouldBeTrue();
		}
		
		[Test]
		public void Should_set_not_read_from_file_if_not_found()
		{
			var section = new CoreSection();
			section.Read(_file);
			section.XUnitTestRunner.WasReadFromConfig.ShouldBeFalse();
		}
		
		[Test]
		public void Should_set_not_read_from_file_if_found()
		{
			var section = new CoreSection();
			section.Read(_file);
			section.NUnitTestRunner.WasReadFromConfig.ShouldBeTrue();
		}
		
		[Test]
		public void Should_read_from_boolean_nodes()
		{
			var section = new CoreSection();
			section.Read(_file);
			section.DebuggingEnabled.ShouldExclude.ShouldBeTrue();
		}
		
		[Test]
		public void Should_read_from_value_item_nodes()
		{
			var section = new CoreSection();
			section.Read(_file);
			section.WatchIgnoreFile.ShouldExclude.ShouldBeTrue();
		}
		
		[Test]
		public void Should_not_be_able_to_merge_code_editor()
		{
			var section = new CoreSection();
			section.Read(_file);
			section.CodeEditor.ShouldMerge.ShouldBeFalse();
		}
		
		[Test]
		public void Should_not_allow_merge_for_growl()
		{
			var section = new CoreSection();
			section.Read(_file);
			section.GrowlNotify.ShouldMerge.ShouldBeFalse();
		}
		
		[Test]
		public void Should_ignore_invalid_configurations()
		{
			if (File.Exists(_file))
				File.Delete(_file);
			File.WriteAllText(_file, "");
			var section = new CoreSection();
			section.Read(_file);
		}
	}
}

