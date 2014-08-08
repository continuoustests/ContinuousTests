//
// IMetaDataEmit.cs
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

using System.Runtime.InteropServices;

namespace SyntaxTree.Pdb
{
	[Guid("ba3fee4c-ecb9-4e41-83b7-183fa41cd859")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImport]
	interface IMetaDataEmit
	{
		void __SetModuleProps();
		void __Save();
		void __SaveToStream();
		void __GetSaveSize();
		void __DefineTypeDef();
		void __DefineNestedType();
		void __SetHandler();
		void __DefineMethod();
		void __DefineMethodImpl();
		void __DefineTypeRefByName();
		void __DefineImportType();
		void __DefineMemberRef();
		void __DefineImportMember();
		void __DefineEvent();
		void __SetClassLayout();
		void __DeleteClassLayout();
		void __SetFieldMarshal();
		void __DeleteFieldMarshal();
		void __DefinePermissionSet();
		void __SetRVA();
		void __GetTokenFromSig();
		void __DefineModuleRef();
		void __SetParent();
		void __GetTokenFromTypeSpec();
		void __SaveToMemory();
		void __DefineUserString();
		void __DeleteToken();
		void __SetMethodProps();
		void __SetTypeDefProps();
		void __SetEventProps();
		void __SetPermissionSetProps();
		void __DefinePinvokeMap();
		void __SetPinvokeMap();
		void __DeletePinvokeMap();
		void __DefineCustomAttribute();
		void __SetCustomAttributeValue();
		void __DefineField();
		void __DefineProperty();
		void __DefineParam();
		void __SetFieldProps();
		void __SetPropertyProps();
		void __SetParamProps();
		void __DefineSecurityAttributeSet();
		void __ApplyEditAndContinue();
		void __TranslateSigWithScope();
		void __SetMethodImplFlags();
		void __SetFieldRVA();
		void __Merge();
		void __MergeEnd();
	}
}
