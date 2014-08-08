//
// PdbTestBase.cs
//
// Copyright (c) 2011 SyntaxTree
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Cci;
using Microsoft.Cci.Pdb;
using Mono.Cecil;
using NUnit.Framework;

namespace SyntaxTree.Pdb.Test
{
	public abstract class PdbTestBase
	{
		private string tempPath;
		protected ModuleDefinition module;
		protected string pdbFileName;

		internal static void AssertFunction(PdbFunction originalFunction, PdbFunction function)
		{
			Assert.AreEqual(originalFunction.token, function.token);
			AssertLines(originalFunction.lines, function.lines);
			AssertScopes(originalFunction.scopes, function.scopes);
			AssertConstants(originalFunction.constants, function.constants);
			Assert.AreEqual(originalFunction.slotToken, function.slotToken);
			AssertSlots(originalFunction.slots, function.slots);
			Assert.AreEqual(originalFunction.iteratorClass, function.iteratorClass);
			AssertIteratorScopes(originalFunction.iteratorScopes, function.iteratorScopes);
		}

		private static void AssertIteratorScopes(List<ILocalScope> originalScopes, List<ILocalScope> scopes)
		{
			AssertLists(originalScopes, scopes);

			for (int i = 0; i < (originalScopes ?? new List<ILocalScope>()).Count; i++)
			{
				Assert.AreEqual(originalScopes[i].Offset, scopes[i].Offset);
				Assert.AreEqual(originalScopes[i].Length, scopes[i].Length);
			}
		}

		private static void AssertLines(PdbLines[] originalLines, PdbLines[] lines)
		{
			AssertLists(originalLines, lines);

			for (int i = 0; i < (originalLines ?? new PdbLines[0]).Length; i++)
			{
				AssertFiles(originalLines[i].file, lines[i].file);
				AssertLines(originalLines[i].lines, lines[i].lines);
			}
		}

		private static void AssertLines(PdbLine[] originalLines, PdbLine[] lines)
		{
			AssertLists(originalLines, lines);

			for (int i = 0; i < originalLines.Length; i++)
			{
				Assert.AreEqual(originalLines[i].lineBegin, lines[i].lineBegin);
				Assert.AreEqual(originalLines[i].lineEnd, lines[i].lineEnd);
				Assert.AreEqual(originalLines[i].colBegin, lines[i].colBegin);
				Assert.AreEqual(originalLines[i].colEnd, lines[i].colEnd);
			}
		}

		private static void AssertFiles(PdbSource originalFile, PdbSource file)
		{
			Assert.AreEqual(originalFile.name, file.name);
			Assert.AreEqual(originalFile.language, file.language);
			Assert.AreEqual(originalFile.vendor, file.vendor);
			Assert.AreEqual(originalFile.doctype, file.doctype);
		}

		private static void AssertScopes(PdbScope[] originalScopes, PdbScope[] scopes)
		{
			AssertLists(originalScopes, scopes);

			for (int i = 0; i < originalScopes.Length; i++)
			{
				Assert.AreEqual(originalScopes[i].offset, scopes[i].offset);
				Assert.AreEqual(originalScopes[i].length, scopes[i].length);

				AssertSlots(originalScopes[i].slots, scopes[i].slots);
				AssertConstants(originalScopes[i].constants, scopes[i].constants);

				AssertScopes(originalScopes[i].scopes, scopes[i].scopes);
			}
		}

		private static void AssertConstants(PdbConstant[] originalConstants, PdbConstant[] constants)
		{
			AssertLists(originalConstants, constants);

			for (int i = 0; i < originalConstants.Length; i++)
			{
				Assert.AreEqual(originalConstants[i].name, constants[i].name);
				Assert.AreEqual(originalConstants[i].token, constants[i].token);
				Assert.AreEqual(originalConstants[i].value, constants[i].value);
			}
		}

		private static void AssertSlots(PdbSlot[] originalSlots, PdbSlot[] slots)
		{
		    int x = 5;
			AssertLists(originalSlots, slots);

			for (int i = 0; i < originalSlots.Length; i++)
			{
				Assert.AreEqual(originalSlots[i].name, slots[i].name);
				Assert.AreEqual(originalSlots[i].slot, slots[i].slot);
			}
		}

		private static void AssertLists(IList original, IList array)
		{
			if (original == null)
			{
				Assert.IsNull(array);
				return;
			}

			Assert.IsNotNull(array);
			Assert.AreEqual(original.Count, array.Count);
		} 

		private static string PdbFileNameOf(ModuleDefinition module)
		{
			return Path.GetFullPath(Path.ChangeExtension(module.FullyQualifiedName, ".pdb"));
		}

		internal void RunTest(string name, out PdbFunction original, out PdbFunction rewritten)
		{
			int originalAge;
			Guid originalGuid;
			PdbFunction[] originalFunctions;
			using (var file = File.OpenRead(pdbFileName))
				originalFunctions = PdbFile.LoadFunctions(file, true, out originalAge, out originalGuid);

			var method = GetMethod(name);
			original = originalFunctions.Single(f => f.token == method.MetadataToken.ToUInt32());

			var pdb = ProgramDatabase.Read(pdbFileName);
			pdb.Write(pdbFileName, new CecilMetadataProvider(module));

			int rewrittenAge;
			Guid rewrittenGuid;
			PdbFunction[] rewrittenFunctions;
			using (var file = File.OpenRead(pdbFileName))
				rewrittenFunctions = PdbFile.LoadFunctions(file, true, out rewrittenAge, out rewrittenGuid);

			rewritten = rewrittenFunctions.Single(f => f.token == method.MetadataToken.ToUInt32());

			Assert.AreEqual(originalAge, rewrittenAge);
			Assert.AreEqual(originalGuid, rewrittenGuid);
		}

		MethodDefinition GetMethod (string fullName)
		{
			var separator = fullName.LastIndexOf('.');
			return module.GetType(fullName.Substring(0, separator)).Methods.Single(m => m.Name == fullName.Substring(separator + 1));
		}

		[SetUp]
		public void SetupRewritingTest()
		{
			tempPath = Path.GetTempFileName();
			File.Delete(tempPath);
			Directory.CreateDirectory(tempPath);

			var moduleFile = new Uri(typeof(PdbTest).Assembly.CodeBase).LocalPath;
			var tempModule = Path.Combine(tempPath, Path.GetFileName(moduleFile));
			File.Copy(moduleFile, tempModule);
			File.Copy(Path.ChangeExtension(moduleFile, ".pdb"), Path.Combine(tempPath, Path.GetFileNameWithoutExtension(moduleFile) + ".pdb"));

            using (module = ModuleDefinition.ReadModule(tempModule))
            {
                pdbFileName = PdbFileNameOf(module);
            }
		}

		[TearDown]
		public void TearDownRewritingTest()
		{
			Directory.Delete(tempPath, recursive: true);
		}
	}
}
