//
// Scope.cs
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

using System.Collections.ObjectModel;
using Microsoft.Cci.Pdb;

namespace SyntaxTree.Pdb
{
	/// <summary>
	/// A lexical scope.
	/// </summary>
	public sealed class Scope : IScope
	{
		private readonly Collection<Variable> variables;
		private readonly Collection<Constant> constants;
		private readonly Collection<Scope> scopes;

		/// <summary>
		/// The IL offset where the scope begins.
		/// </summary>
		public int Offset { get; set; }

		/// <summary>
		/// The length of the scope.
		/// </summary>
		public int Length { get; set; }

		/// <summary>
		/// The variables defined in this scope.
		/// </summary>
		public Collection<Variable> Variables { get { return variables; } }

		/// <summary>
		/// The constants defined in this scope.
		/// </summary>
		public Collection<Constant> Constants { get { return constants; } }

		/// <summary>
		/// The nested scopes defined in this scope.
		/// </summary>
		public Collection<Scope> Scopes { get { return scopes; } }

		public Scope()
		{
			this.variables = new Collection<Variable>();
			this.constants = new Collection<Constant>();
			this.scopes = new Collection<Scope>();
		}

		internal Scope(PdbScope scope) : this()
		{
			this.Offset = (int) scope.offset;
			this.Length = (int) scope.length;
			this.ReadScope(scope.slots, scope.constants, scope.scopes);
		}
	}
}
