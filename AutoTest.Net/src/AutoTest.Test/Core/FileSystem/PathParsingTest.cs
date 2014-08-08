using System;
using NUnit.Framework;
using System.IO;
using AutoTest.Core.FileSystem;
namespace AutoTest.Test.FileSystem
{
	[TestFixture]
	public  class PathParsingTest
	{
		[Test]
		public void Should_retrieve_correct_path()
		{
			var path = PathParsing.GetRootDirectory();
			Directory.Exists(path).ShouldBeTrue();
		}
	}
}

