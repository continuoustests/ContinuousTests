using System;
using System.IO;
namespace AutoTest.Messages
{
	public class StackLineMessage : IStackLine
	{
		private readonly string _method;
		private readonly string _file;
		private readonly int _lineNumber;
		
		public string Method {
			get {
				return _method;
			}
		}

		public string File {
			get {
				return _file;
			}
		}
        
		public int LineNumber {
			get {
				return _lineNumber;
			}
		}
		
		public StackLineMessage(string method, string file, int lineNumber)
		{
			_method = method;
			_file = file;
			_lineNumber = lineNumber;
		}
	}
}

