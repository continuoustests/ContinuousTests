using System;
using System.Collections.Generic;
namespace AutoTest.Messages
{
	public class EditorEngineMessage
	{
		public string Command { get; private set; }
		public List<string> Arguments { get; private set; }
		
		public EditorEngineMessage(string command, IEnumerable<string> arguments)
		{
			Command = command;
			Arguments = new List<string>();
			Arguments.AddRange(arguments);
		}
		
		public static EditorEngineMessage New(string message)
		{
			var chunks = splitMessage(message);
			if (chunks.Count == 0)
				return new EditorEngineMessage("", new string[] {});
			return new EditorEngineMessage(chunks[0], getRemainingChunks(chunks));
		}
		
		private static List<string> splitMessage(string message)
		{
			var list = new List<string>();
			var start = 0;
			var escapeDepth = 0;
			for (int i = 0; i < message.Length; i++)
			{
				var current = message.Substring(i, 1);
				if (current == "\"")
				{
					escapeDepth++;
					if (escapeDepth == 1)
						start = i + 1;
				}
				if ((current == " " && escapeDepth == 0) || escapeDepth == 2 || i == message.Length - 1)
				{
					var length = i - start;
					if (i == message.Length - 1 && escapeDepth != 2)
						length = i - start + 1;
					var item = message.Substring(start, length).Trim();
					if (item.Length > 0)
						list.Add(item);
					start = i + 1;
					escapeDepth = 0;
				}
			}
			return list;
		}
		
		private static IEnumerable<string> getRemainingChunks(List<string> chunks)
		{
			if (chunks.Count < 2)
				return new string[] {};
			var list = new List<string>();
			for (int i = 1; i < chunks.Count; i++)
				list.Add(chunks[i]);
			return list;
		}
	}
}

