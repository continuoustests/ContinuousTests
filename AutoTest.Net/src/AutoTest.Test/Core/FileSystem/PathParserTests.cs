using System;
using NUnit.Framework;
using AutoTest.Core.FileSystem;
using System.IO;
namespace AutoTest.Test.Core.FileSystem
{
	[TestFixture]
	public class PathParserTests
	{
		[Test]
		public void When_no_relative_path_it_should_return_passed_path()
		{
            var path = "/some/reference/path";
            if (OS.IsWindows)
                path = @"C:\some\reference\path";
			Assert.That(new PathParser(".ignorefile").ToAbsolute(path), Is.EqualTo(Path.Combine(path, ".ignorefile")));
		}
		
		[Test]
		public void When_relative_path_it_should_return_path_relative_to_passed_path()
		{
            var path = "/some/reference/path";
            var relative = "/some/reference";
            if (OS.IsWindows)
            {
                path = @"C:\some\reference\path";
                relative = @"C:\some\reference";
            }
			Assert.That(new PathParser("../.ignorefile").ToAbsolute(path), Is.EqualTo(Path.Combine(relative, ".ignorefile")));
		}
	}
}

