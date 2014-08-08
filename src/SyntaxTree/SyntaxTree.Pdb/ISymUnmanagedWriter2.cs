//
// ISymUnmanagedWriter2.cs
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
using System.Runtime.InteropServices;

namespace SyntaxTree.Pdb
{
	[Guid("0b97726e-9e6d-4f05-9a26-424022093caa")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImport]
	[CoClass(typeof(CorSymWriter))]
	interface ISymUnmanagedWriter2
	{
		ISymUnmanagedDocumentWriter DefineDocument(string url, ref Guid language, ref Guid languageVendor, ref Guid documentType);
		void SetUserEntryPoint(int entryPointToken);
		void OpenMethod(int method);
		void CloseMethod();
		int OpenScope(int startOffset);
		void CloseScope(int endOffset);
		void __SetScopeRange();
		void DefineLocalVariable(string name, int attributes, int cSig, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] signature, int addrKind, int addr1, int addr2, int startOffset, int endOffset);
		void __DefineParameter();
		void __DefineField();
		void __DefineGlobalVariable();
		void Close();
		void SetSymAttribute(int token, string name, int size, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] data);
		void __OpenNamespace();
		void __CloseNamespace();
		void __UsingNamespace();
		void __SetMethodSourceRange();
		void Initialize([MarshalAs(UnmanagedType.IUnknown)] object emitter, string filename, [MarshalAs(UnmanagedType.IUnknown)] object pIStream, bool fFullBuild);

		void __GetDebugInfo();

		void DefineSequencePoints(ISymUnmanagedDocumentWriter document, int spCount,
			[MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] int[] offsets,
			[MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] int[] lines,
			[MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] int[] columns,
			[MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] int[] endLines,
			[MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] int[] endColumns);

		void __RemapToken();
		void __Initialize2();
		void __DefineConstant();
		void __Abort();

		void DefineLocalVariable2(string name, int attributes, int token, int addrKind, int addr1, int addr2, int addr3, int startOffset, int endOffset);

		void __DefineGlobalVariable2();
		void DefineConstant2(string name, object value, int token);
	}
}
