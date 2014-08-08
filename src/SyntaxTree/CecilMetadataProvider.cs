//
// CecilMetadataProvider.cs
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

using Mono.Cecil;

namespace SyntaxTree.Pdb
{
	public class CecilMetadataProvider : IMetadataProvider
	{
		private readonly ModuleDefinition module;

		public CecilMetadataProvider(ModuleDefinition module)
		{
			this.module = module;
		}

		public bool GetTypeMetadata(int token, out string name, out int attributes, out int baseTypeToken)
		{
			name = string.Empty;
			attributes = baseTypeToken = 0;

			var type = module.LookupToken(token) as TypeDefinition;
			if (type == null)
				return false;

			name = type.FullName.Replace('/', '+');
			attributes = (int) type.Attributes;
			baseTypeToken = type.BaseType == null ? 0 : type.BaseType.MetadataToken.ToInt32();
			return true;
		}

		public bool GetNestedTypeDeclaringType(int token, out int declaringTypeToken)
		{
			declaringTypeToken = 0;

			var type = module.LookupToken(token) as TypeDefinition;
			if (type == null)
				return false;

			declaringTypeToken = type.DeclaringType == null ? 0 : type.DeclaringType.MetadataToken.ToInt32();
			return true;
		}

		public bool GetMethodMetadata(int token, out string name, out int attributes, out int implAttributes, out int rva)
		{
			name = string.Empty;
			attributes = implAttributes = rva = 0;

			var method = module.LookupToken(token) as MethodDefinition;
			if (method == null)
				return false;

			name = method.Name;
			attributes = (int) method.Attributes;
			implAttributes = (int) method.ImplAttributes;
			rva = method.RVA;
			return true;
		}

		public bool GetEntryPoint(out int entryPointToken)
		{
			entryPointToken = 0;
			if (module.EntryPoint == null)
				return false;

			entryPointToken = module.EntryPoint.MetadataToken.ToInt32();
			return true;
		}
	}
}
