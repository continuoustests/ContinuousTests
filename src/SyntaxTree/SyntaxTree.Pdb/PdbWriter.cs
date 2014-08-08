//
// PdbWriter.cs
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
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Runtime.InteropServices;
using System.Text;

namespace SyntaxTree.Pdb
{
	class PdbWriter : IDisposable
	{
		private ISymUnmanagedWriter2 pdb;
		private readonly IMetadataProvider metadataProvider;
		private IDictionary<string, ISymUnmanagedDocumentWriter> documentWriters;

		public PdbWriter(string fileName, IMetadataProvider metadataProvider)
		{
			this.pdb = new ISymUnmanagedWriter2();
			this.metadataProvider = metadataProvider;
			this.documentWriters = new Dictionary<string, ISymUnmanagedDocumentWriter>();

			this.pdb.Initialize(new Metadata(metadataProvider), fileName, pIStream: null, fFullBuild: true);
		}

		public void Write(Function function)
		{
			pdb.OpenMethod(function.Token);

			WriteSources(function.Sources);
			WriteScopes(function.Scopes, function);
			WriteConstants(function.Constants);
			WriteVariables(function.Variables, function.VariablesToken, 0, 0);

			WriteIteratorClass(function);
			WriteIteratorScopes(function);

			// TODO
			// function.namespaceScopes
			// function.usedNamespaces
			// function.usingCounts

			pdb.CloseMethod();
		}

		private void WriteIteratorScopes(Function function)
		{
			if (function.IteratorScopes.Count == 0)
				return;

			var buffer = new ByteBuffer();
			buffer.WriteByte(4);
			buffer.WriteByte(1);
			buffer.Align(4);

			buffer.WriteByte(4);
			buffer.WriteByte(3);
			buffer.Align(4);

			var scopes = function.IteratorScopes;

			buffer.WriteInt32(scopes.Count * 8 + 12);
			buffer.WriteInt32(scopes.Count);

			foreach (var scope in scopes)
			{
				buffer.WriteInt32(scope.Offset);
				buffer.WriteInt32(scope.Offset + scope.Length);
			}

			pdb.SetSymAttribute(function.Token, "MD2", buffer.length, buffer.buffer);
		}

		private void WriteIteratorClass(Function function)
		{
			if (string.IsNullOrEmpty(function.IteratorTypeName))
				return;

			var buffer = new ByteBuffer();
			buffer.WriteByte(4);
			buffer.WriteByte(1);
			buffer.Align(4);

			buffer.WriteByte(4); // version
			buffer.WriteByte(4); // iterator
			buffer.Align(4);

			var length = 10 + (uint)function.IteratorTypeName.Length * 2;
			while (length % 4 > 0) length++;

			buffer.WriteUInt32(length);
			buffer.WriteBytes(Encoding.Unicode.GetBytes(function.IteratorTypeName));
			buffer.WriteByte(0);
			buffer.Align(4);

			pdb.SetSymAttribute(function.Token, "MD2", buffer.length, buffer.buffer);
		}

		private void WriteConstants(IEnumerable<Constant> constants)
		{
			foreach (var constant in constants)
				WriteConstant(constant);
		}

		private void WriteConstant(Constant constant)
		{
			pdb.DefineConstant2(constant.Name, constant.Value, constant.Token);
		}

		private void WriteScopes(IEnumerable<Scope> scopes, Function function)
		{
			foreach (var scope in scopes)
				WriteScope(scope, function);
		}

		private void WriteScope(Scope scope, Function function)
		{
			pdb.OpenScope(scope.Offset);

			WriteVariables(scope.Variables, function.VariablesToken, scope.Offset, scope.Length);
			WriteConstants(scope.Constants);
			WriteScopes(scope.Scopes, function);

			// TODO
			// scope.usedNamespaces

			pdb.CloseScope(scope.Offset + scope.Length);
		}

		private void WriteVariables(IEnumerable<Variable> variables, int variablesToken, int scopeOffset, int scopeLength)
		{
			foreach (var slot in variables)
				WriteVariable(variablesToken, slot, scopeOffset, scopeLength);
		}

		private void WriteVariable(int slotToken, Variable variable, int scopeOffset, int scopeLength)
		{
			pdb.DefineLocalVariable2(variable.Name, 0, slotToken, (int) SymAddressKind.ILOffset, variable.Index, 0, 0, scopeOffset, scopeOffset + scopeLength);
		}

		private void WriteSources(IList<Source> sources)
		{
			if (sources.Count == 0)
				return;

			foreach (var source in sources)
				WriteSource(source);
		}

		private void WriteSource(Source source)
		{
			var sequencePoints = source.SequencePoints;
			var count = sequencePoints.Count;

			var offsets = new int[count];
			var lines = new int[count];
			var columns = new int[count];
			var endLines = new int[count];
			var endColumns = new int[count];

			for (int i = 0; i < sequencePoints.Count; i++)
			{
				var sequencePoint = sequencePoints[i];
				offsets[i] = sequencePoint.Offset;
				lines[i] = sequencePoint.Line;
				columns[i] = sequencePoint.Column;
				endLines[i] = sequencePoint.EndLine;
				endColumns[i] = sequencePoint.EndColumn;
			}

			pdb.DefineSequencePoints(UnmanagedDocumentFor(source.Document), count, offsets, lines, columns, endLines, endColumns);
		}

		private ISymUnmanagedDocumentWriter UnmanagedDocumentFor(Document document)
		{
			ISymUnmanagedDocumentWriter documentWriter;
			if (documentWriters.TryGetValue(document.Name, out documentWriter))
				return documentWriter;

			Guid language = document.Language, vendor = document.LanguageVendor, doctype = document.DocumentType;
			documentWriter = pdb.DefineDocument(document.Name, ref language, ref vendor, ref doctype);
			documentWriters.Add(document.Name, documentWriter);
			return documentWriter;
		}

		private void WriteEntryPoint()
		{
			int entryPointToken = 0;
			if (!metadataProvider.GetEntryPoint(out entryPointToken))
				return;

			this.pdb.SetUserEntryPoint(entryPointToken);
		}

		public void Dispose()
		{
			WriteEntryPoint();
			this.pdb.Close();
			Marshal.ReleaseComObject(this.pdb);

			foreach (var documentWriter in documentWriters.Values)
				Marshal.ReleaseComObject(documentWriter);

			this.documentWriters = null;
			this.pdb = null;
		}
	}
}
