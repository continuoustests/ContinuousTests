//
// Function.cs
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
using System.Linq;
using Microsoft.Cci.Pdb;

namespace SyntaxTree.Pdb
{
	/// <summary>
	/// A managed method defined in the pdb.
	/// </summary>
	public sealed class Function : IScope
	{
		private readonly Collection<Variable> variables;
		private readonly Collection<Constant> constants;
		private readonly Collection<Scope> scopes;
		private readonly Collection<IteratorScope> iteratorScopes;
		private readonly Collection<Source> sources;

		/// <summary>
		/// The metadata token of the function
		/// </summary>
		public int Token { get; set; }

		/// <summary>
		/// The metadata token of the signature of the local variables defined by this function
		/// </summary>
		public int VariablesToken { get; set; }

		/// <summary>
		/// If the function is implemented as an iterator, returns the name of
		/// the type backing the implementation of the iterator.
		/// </summary>
		public string IteratorTypeName { get; set; }

		/// <summary>
		/// The scopes defined in this functions
		/// </summary>
		public Collection<Scope> Scopes { get { return scopes; } }

		/// <summary>
		/// If the function is implementing an iterator, returns the different
		/// scopes of the state machine.
		/// </summary>
		public Collection<IteratorScope> IteratorScopes { get { return iteratorScopes; } }

		/// <summary>
		/// The top level variables of the function.
		/// </summary>
		public Collection<Variable> Variables { get { return variables; } }

		/// <summary>
		/// The top level constants of the function.
		/// </summary>
		public Collection<Constant> Constants { get { return constants; } }

		/// <summary>
		/// The sources where are defined the function.
		/// </summary>
		public Collection<Source> Sources { get { return sources; } } 

		public Function()
		{
			this.variables = new Collection<Variable>();
			this.constants = new Collection<Constant>();
			this.scopes = new Collection<Scope>();
			this.iteratorScopes = new Collection<IteratorScope>();
			this.sources = new Collection<Source>();
		}

		internal Function(PdbFunction function) : this()
		{
			this.Token = (int) function.token;
			this.VariablesToken = (int) function.slotToken;
			this.IteratorTypeName = function.iteratorClass ?? string.Empty;

			ReadIteratorScopes(function);
			ReadSources(function);

			this.ReadScope(function.slots, function.constants, function.scopes);
		}

		private void ReadSources(PdbFunction function)
		{
			if (function.lines == null)
				return;

			sources.AddRange(function.lines.Select(l => new Source(l)));
		}

		private void ReadIteratorScopes(PdbFunction function)
		{
			if (function.iteratorScopes == null)
				return;

			iteratorScopes.AddRange(function.iteratorScopes.Select(s => new IteratorScope(s)));
		}

		internal void Write(PdbWriter writer)
		{
			writer.Write(this);
		}
	}
}
