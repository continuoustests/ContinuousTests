//
// IMetadataProvider.cs
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

namespace SyntaxTree.Pdb
{
	/// <summary>
	/// Interface representing a metadata information provider.
	/// </summary>
	public interface IMetadataProvider
	{
		/// <summary>
		/// Get metadata about a type.
		/// </summary>
		/// <param name="token">The metadata token of the type.</param>
		/// <param name="name">The name of the type.</param>
		/// <param name="attributes">The bit flags attributes of the type.</param>
		/// <param name="baseTypeToken">The metadata token of the base type.</param>
		/// <returns>Whether retrieving the metadata for the type token succeeded.</returns>
		bool GetTypeMetadata(int token, out string name, out int attributes, out int baseTypeToken);

		/// <summary>
		/// Get the parent type of a nested type.
		/// </summary>
		/// <param name="token">The metadata token of the nested type.</param>
		/// <param name="declaringTypeToken">The metadata token of the declaring type.</param>
		/// <returns>Whether retrieving the parent type metadata token, if any, suceeded.</returns>
		bool GetNestedTypeDeclaringType(int token, out int declaringTypeToken);

		/// <summary>
		/// Get metadata about a method.
		/// </summary>
		/// <param name="token">The metadata token of the method.</param>
		/// <param name="name">The name of the method.</param>
		/// <param name="attributes">The bit flags attributes of the method.</param>
		/// <param name="implAttributes">The big flags implementation attributes of the method.</param>
		/// <param name="rva">The RVA of the method body.</param>
		/// <returns>Whether retrieving the metadata for the method succeeded.</returns>
		bool GetMethodMetadata(int token, out string name, out int attributes, out int implAttributes, out int rva);

		/// <summary>
		/// Get the metadata token of the entry point.
		/// </summary>
		/// <param name="entryPointToken">The metadata token of the entry point.</param>
		/// <returns>Whether retrieving an entry point succeeeded.</returns>
		bool GetEntryPoint(out int entryPointToken);
	}
}
