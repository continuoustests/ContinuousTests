using System;
using NUnit.Framework;
using AutoTest.Core.Launchers;
using AutoTest.Messages;
namespace AutoTest.Test.Launchers
{
	[TestFixture]
	public class EditorEngineMessageTests
	{
		[Test]
		public void Should_parse_empty_command()
		{
			var message = EditorEngineMessage.New("");
			message.Command.ShouldEqual("");
			message.Arguments.Count.ShouldEqual(0);
		}
		
		[Test]
		public void Should_remove_empty_entries()
		{
			var message = EditorEngineMessage.New("     ");
			message.Command.ShouldEqual("");
			message.Arguments.Count.ShouldEqual(0);
		}
		
		[Test]
		public void Should_parse_command_without_arguments()
		{
			var message = EditorEngineMessage.New("command");
			message.Command.ShouldEqual("command");
			message.Arguments.Count.ShouldEqual(0);
		}
		
		[Test]
		public void Should_parse_command_with_arguments()
		{
			var message = EditorEngineMessage.New("command argument1 argument2");
			message.Command.ShouldEqual("command");
			message.Arguments.Count.ShouldEqual(2);
			message.Arguments[0].ShouldEqual("argument1");
			message.Arguments[1].ShouldEqual("argument2");
		}
		
		[Test]
		public void Should_parse_command_with_arguments_escaped_argument()
		{
			var message = EditorEngineMessage.New("command \"this is one argument\"");
			message.Command.ShouldEqual("command");
			message.Arguments.Count.ShouldEqual(1);
			message.Arguments[0].ShouldEqual("this is one argument");
		}
		
		[Test]
		public void Should_parse_command_with_arguments_mixed_argument()
		{
			var message = EditorEngineMessage.New("command \"this is one argument\" \"another\" last");
			message.Command.ShouldEqual("command");
			message.Arguments.Count.ShouldEqual(3);
			message.Arguments[0].ShouldEqual("this is one argument");
			message.Arguments[1].ShouldEqual("another");
			message.Arguments[2].ShouldEqual("last");
		}
	}
}

