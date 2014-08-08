using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AutoTest.Core.DebugLog;
using AutoTest.TestRunners.Shared.AssemblyAnalysis;
namespace AutoTest.Core.FileSystem
{
	public class AssemblyParser : IRetrieveAssemblyIdentifiers
	{	
		public int GetAssemblyIdentifier(string assembly)
		{
			var fileInfo = new FileInfo(assembly);
			var builder = new StringBuilder();
			builder.Append(fileInfo.Length.ToString());
			builder.Append("|");
            using (var reader = Reflect.On(assembly))
            {
                foreach (var reference in reader.GetReferences())
                    builder.Append(reference);
            }
			return builder.ToString().GetHashCode();
		}
	}
}

