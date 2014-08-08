//
// Document.cs
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
using Microsoft.Cci.Pdb;

namespace SyntaxTree.Pdb
{
	/// <summary>
	/// A source file document.
	/// </summary>
	public sealed class Document : IEquatable<Document>
	{
		/// <summary>
		/// The full name of the document.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The guid associated with the language of the document.
		/// </summary>
		public Guid Language { get; set; }

		/// <summary>
		/// The guid associated with the vendor of the language.
		/// </summary>
		public Guid LanguageVendor { get; set; }

		/// <summary>
		/// The guid associted with the type of the document.
		/// </summary>
		public Guid DocumentType { get; set; }

		public Document()
		{
		}

		internal Document(PdbSource source) : this()
		{
			this.Name = source.name;
			this.Language = source.language;
			this.LanguageVendor = source.vendor;
			this.DocumentType = source.doctype;
		}

		public override int GetHashCode()
		{
			return this.Name.GetHashCode()
				^ this.Language.GetHashCode()
				^ this.LanguageVendor.GetHashCode()
				^ this.DocumentType.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as Document);
		}

		public bool Equals(Document other)
		{
			if (other == null)
				return false;

			return this.Name == other.Name
				&& this.Language == other.Language
				&& this.LanguageVendor == other.LanguageVendor
				&& this.DocumentType == other.DocumentType;
		}
	}
}
