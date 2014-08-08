//
// IMetaDataImport.cs
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
	[Guid("7dac8207-d3ae-4c75-9b67-92801a497d44")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImport]
	interface IMetaDataImport
	{
		void __CloseEnum();
		void __CountEnum();
		void __ResetEnum();
		void __EnumTypeDefs();
		void __EnumInterfaceImpls();
		void __EnumTypeRefs();
		void __FindTypeDefByName();
		void __GetScopeProps();
		void __GetModuleFromScope();

		void GetTypeDefProps(int td, IntPtr szTypeDef, int cchTypeDef, IntPtr pchTypeDef, IntPtr pdwTypeDefFlags, IntPtr ptkExtends);

		void __GetInterfaceImplProps();
		void __GetTypeRefProps();
		void __ResolveTypeRef();
		void __EnumMembers();
		void __EnumMembersWithName();
		void __EnumMethods();
		void __EnumMethodsWithName();
		void __EnumFields();
		void __EnumFieldsWithName();
		void __EnumParams();
		void __EnumMemberRefs();
		void __EnumMethodImpls();
		void __EnumPermissionSets();
		void __FindMember();
		void __FindMethod();
		void __FindField();
		void __FindMemberRef();

		void GetMethodProps(int mb, IntPtr pClass, IntPtr szMethod, int cchMethod, IntPtr pchMethod, IntPtr pdwAttr, IntPtr ppvSigBlob, IntPtr pcbSigBlob, IntPtr pulCodeRVA, IntPtr pdwImplFlags);

		void __GetMemberRefProps();
		void __EnumProperties();
		void __EnumEvents();
		void __GetEventProps();
		void __EnumMethodSemantics();
		void __GetMethodSemantics();
		void __GetClassLayout();
		void __GetFieldMarshal();
		void __GetRVA();
		void __GetPermissionSetProps();
		void __GetSigFromToken();
		void __GetModuleRefProps();
		void __EnumModuleRefs();
		void __GetTypeSpecFromToken();
		void __GetNameFromToken();
		void __EnumUnresolvedMethods();
		void __GetUserString();
		void __GetPinvokeMap();
		void __EnumSignatures();
		void __EnumTypeSpecs();
		void __EnumUserStrings();
		void __GetParamForMethodIndex();
		void __EnumCustomAttributes();
		void __GetCustomAttributeProps();
		void __FindTypeRef();
		void __GetMemberProps();
		void __GetFieldProps();
		void __GetPropertyProps();
		void __GetParamProps();
		void __GetCustomAttributeByName();
		void __IsValidToken();

		void GetNestedClassProps(int tdNestedClass, IntPtr ptdEnclosingClass);

		void __GetNativeCallConvFromSig();
		void __IsGlobal();
	}
}
