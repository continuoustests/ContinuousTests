//
// Metadata.cs
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
	class Metadata : IMetaDataEmit, IMetaDataImport
	{
		private readonly IMetadataProvider metadataProvider;

		public Metadata(IMetadataProvider metadataProvider)
		{
			this.metadataProvider = metadataProvider;
		}

		private static void WriteIntPtr(IntPtr ptr, int value)
		{
			if (ptr == IntPtr.Zero)
				return;

			Marshal.WriteInt32(ptr, value);
		}

		private static void WriteString(string str, IntPtr buffer, int bufferSize, IntPtr chars)
		{
			var length = str.Length + 1 >= bufferSize ? bufferSize - 1 : str.Length;
			var offset = 0;

			for (int i = 0; i < length; i++)
			{
				Marshal.WriteInt16(buffer, offset, str[i]);
				offset += 2;
			}

			Marshal.WriteInt16(buffer, offset, 0);
			WriteIntPtr(chars, length + 1);
		}

		public void GetMethodProps(int mb, IntPtr pClass, IntPtr szMethod, int cchMethod, IntPtr pchMethod, IntPtr pdwAttr, IntPtr ppvSigBlob, IntPtr pcbSigBlob, IntPtr pulCodeRVA, IntPtr pdwImplFlags)
		{
			string name;
			int attributes, implAttributes, rva;

			if (!metadataProvider.GetMethodMetadata(mb, out name, out attributes, out implAttributes, out rva))
			{
				WriteString("", szMethod, cchMethod, pchMethod);
				return;
			}

			WriteString(name, szMethod, cchMethod, pchMethod);
			WriteIntPtr(pdwAttr, attributes);
			WriteIntPtr(pulCodeRVA, rva);
			WriteIntPtr(pdwImplFlags, implAttributes);
		}

		public void GetTypeDefProps(int td, IntPtr szTypeDef, int cchTypeDef, IntPtr pchTypeDef, IntPtr pdwTypeDefFlags, IntPtr ptkExtends)
		{
			string name;
			int attributes, baseTypeToken;
			if (!metadataProvider.GetTypeMetadata(td, out name, out attributes, out baseTypeToken))
			{
				WriteString("", szTypeDef, cchTypeDef, pchTypeDef);
				return;
			}

			WriteString(name, szTypeDef, cchTypeDef, pchTypeDef);
			WriteIntPtr(pdwTypeDefFlags, attributes);
			WriteIntPtr(ptkExtends, baseTypeToken);
		}

		public void GetNestedClassProps(int tdNestedClass, IntPtr ptdEnclosingClass)
		{
			int declaringTypeToken;
			if (!metadataProvider.GetNestedTypeDeclaringType(tdNestedClass, out declaringTypeToken))
			{
				WriteIntPtr(ptdEnclosingClass, 0);
				return;
			}

			WriteIntPtr(ptdEnclosingClass, declaringTypeToken);
		}

		#region Implementation of IMetaDataEmit

		public void __SetModuleProps()
		{
			throw new NotImplementedException();
		}

		public void __Save()
		{
			throw new NotImplementedException();
		}

		public void __SaveToStream()
		{
			throw new NotImplementedException();
		}

		public void __GetSaveSize()
		{
			throw new NotImplementedException();
		}

		public void __DefineTypeDef()
		{
			throw new NotImplementedException();
		}

		public void __DefineNestedType()
		{
			throw new NotImplementedException();
		}

		public void __SetHandler()
		{
			throw new NotImplementedException();
		}

		public void __DefineMethod()
		{
			throw new NotImplementedException();
		}

		public void __DefineMethodImpl()
		{
			throw new NotImplementedException();
		}

		public void __DefineTypeRefByName()
		{
			throw new NotImplementedException();
		}

		public void __DefineImportType()
		{
			throw new NotImplementedException();
		}

		public void __DefineMemberRef()
		{
			throw new NotImplementedException();
		}

		public void __DefineImportMember()
		{
			throw new NotImplementedException();
		}

		public void __DefineEvent()
		{
			throw new NotImplementedException();
		}

		public void __SetClassLayout()
		{
			throw new NotImplementedException();
		}

		public void __DeleteClassLayout()
		{
			throw new NotImplementedException();
		}

		public void __SetFieldMarshal()
		{
			throw new NotImplementedException();
		}

		public void __DeleteFieldMarshal()
		{
			throw new NotImplementedException();
		}

		public void __DefinePermissionSet()
		{
			throw new NotImplementedException();
		}

		public void __SetRVA()
		{
			throw new NotImplementedException();
		}

		public void __GetTokenFromSig()
		{
			throw new NotImplementedException();
		}

		public void __DefineModuleRef()
		{
			throw new NotImplementedException();
		}

		public void __SetParent()
		{
			throw new NotImplementedException();
		}

		public void __GetTokenFromTypeSpec()
		{
			throw new NotImplementedException();
		}

		public void __SaveToMemory()
		{
			throw new NotImplementedException();
		}

		public void __DefineUserString()
		{
			throw new NotImplementedException();
		}

		public void __DeleteToken()
		{
			throw new NotImplementedException();
		}

		public void __SetMethodProps()
		{
			throw new NotImplementedException();
		}

		public void __SetTypeDefProps()
		{
			throw new NotImplementedException();
		}

		public void __SetEventProps()
		{
			throw new NotImplementedException();
		}

		public void __SetPermissionSetProps()
		{
			throw new NotImplementedException();
		}

		public void __DefinePinvokeMap()
		{
			throw new NotImplementedException();
		}

		public void __SetPinvokeMap()
		{
			throw new NotImplementedException();
		}

		public void __DeletePinvokeMap()
		{
			throw new NotImplementedException();
		}

		public void __DefineCustomAttribute()
		{
			throw new NotImplementedException();
		}

		public void __SetCustomAttributeValue()
		{
			throw new NotImplementedException();
		}

		public void __DefineField()
		{
			throw new NotImplementedException();
		}

		public void __DefineProperty()
		{
			throw new NotImplementedException();
		}

		public void __DefineParam()
		{
			throw new NotImplementedException();
		}

		public void __SetFieldProps()
		{
			throw new NotImplementedException();
		}

		public void __SetPropertyProps()
		{
			throw new NotImplementedException();
		}

		public void __SetParamProps()
		{
			throw new NotImplementedException();
		}

		public void __DefineSecurityAttributeSet()
		{
			throw new NotImplementedException();
		}

		public void __ApplyEditAndContinue()
		{
			throw new NotImplementedException();
		}

		public void __TranslateSigWithScope()
		{
			throw new NotImplementedException();
		}

		public void __SetMethodImplFlags()
		{
			throw new NotImplementedException();
		}

		public void __SetFieldRVA()
		{
			throw new NotImplementedException();
		}

		public void __Merge()
		{
			throw new NotImplementedException();
		}

		public void __MergeEnd()
		{
			throw new NotImplementedException();
		}

		#endregion

		#region Implementation of IMetaDataImport

		public void __CloseEnum()
		{
			throw new NotImplementedException();
		}

		public void __CountEnum()
		{
			throw new NotImplementedException();
		}

		public void __ResetEnum()
		{
			throw new NotImplementedException();
		}

		public void __EnumTypeDefs()
		{
			throw new NotImplementedException();
		}

		public void __EnumInterfaceImpls()
		{
			throw new NotImplementedException();
		}

		public void __EnumTypeRefs()
		{
			throw new NotImplementedException();
		}

		public void __FindTypeDefByName()
		{
			throw new NotImplementedException();
		}

		public void __GetScopeProps()
		{
			throw new NotImplementedException();
		}

		public void __GetModuleFromScope()
		{
			throw new NotImplementedException();
		}

		public void __GetInterfaceImplProps()
		{
			throw new NotImplementedException();
		}

		public void __GetTypeRefProps()
		{
			throw new NotImplementedException();
		}

		public void __ResolveTypeRef()
		{
			throw new NotImplementedException();
		}

		public void __EnumMembers()
		{
			throw new NotImplementedException();
		}

		public void __EnumMembersWithName()
		{
			throw new NotImplementedException();
		}

		public void __EnumMethods()
		{
			throw new NotImplementedException();
		}

		public void __EnumMethodsWithName()
		{
			throw new NotImplementedException();
		}

		public void __EnumFields()
		{
			throw new NotImplementedException();
		}

		public void __EnumFieldsWithName()
		{
			throw new NotImplementedException();
		}

		public void __EnumParams()
		{
			throw new NotImplementedException();
		}

		public void __EnumMemberRefs()
		{
			throw new NotImplementedException();
		}

		public void __EnumMethodImpls()
		{
			throw new NotImplementedException();
		}

		public void __EnumPermissionSets()
		{
			throw new NotImplementedException();
		}

		public void __FindMember()
		{
			throw new NotImplementedException();
		}

		public void __FindMethod()
		{
			throw new NotImplementedException();
		}

		public void __FindField()
		{
			throw new NotImplementedException();
		}

		public void __FindMemberRef()
		{
			throw new NotImplementedException();
		}

		public void __GetMemberRefProps()
		{
			throw new NotImplementedException();
		}

		public void __EnumProperties()
		{
			throw new NotImplementedException();
		}

		public void __EnumEvents()
		{
			throw new NotImplementedException();
		}

		public void __GetEventProps()
		{
			throw new NotImplementedException();
		}

		public void __EnumMethodSemantics()
		{
			throw new NotImplementedException();
		}

		public void __GetMethodSemantics()
		{
			throw new NotImplementedException();
		}

		public void __GetClassLayout()
		{
			throw new NotImplementedException();
		}

		public void __GetFieldMarshal()
		{
			throw new NotImplementedException();
		}

		public void __GetRVA()
		{
			throw new NotImplementedException();
		}

		public void __GetPermissionSetProps()
		{
			throw new NotImplementedException();
		}

		public void __GetSigFromToken()
		{
			throw new NotImplementedException();
		}

		public void __GetModuleRefProps()
		{
			throw new NotImplementedException();
		}

		public void __EnumModuleRefs()
		{
			throw new NotImplementedException();
		}

		public void __GetTypeSpecFromToken()
		{
			throw new NotImplementedException();
		}

		public void __GetNameFromToken()
		{
			throw new NotImplementedException();
		}

		public void __EnumUnresolvedMethods()
		{
			throw new NotImplementedException();
		}

		public void __GetUserString()
		{
			throw new NotImplementedException();
		}

		public void __GetPinvokeMap()
		{
			throw new NotImplementedException();
		}

		public void __EnumSignatures()
		{
			throw new NotImplementedException();
		}

		public void __EnumTypeSpecs()
		{
			throw new NotImplementedException();
		}

		public void __EnumUserStrings()
		{
			throw new NotImplementedException();
		}

		public void __GetParamForMethodIndex()
		{
			throw new NotImplementedException();
		}

		public void __EnumCustomAttributes()
		{
			throw new NotImplementedException();
		}

		public void __GetCustomAttributeProps()
		{
			throw new NotImplementedException();
		}

		public void __FindTypeRef()
		{
			throw new NotImplementedException();
		}

		public void __GetMemberProps()
		{
			throw new NotImplementedException();
		}

		public void __GetFieldProps()
		{
			throw new NotImplementedException();
		}

		public void __GetPropertyProps()
		{
			throw new NotImplementedException();
		}

		public void __GetParamProps()
		{
			throw new NotImplementedException();
		}

		public void __GetCustomAttributeByName()
		{
			throw new NotImplementedException();
		}

		public void __IsValidToken()
		{
			throw new NotImplementedException();
		}

		public void __GetNativeCallConvFromSig()
		{
			throw new NotImplementedException();
		}

		public void __IsGlobal()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}