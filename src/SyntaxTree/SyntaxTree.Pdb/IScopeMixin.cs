//
// IScopeMixin.cs
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

using System.Linq;
using Microsoft.Cci.Pdb;

namespace SyntaxTree.Pdb
{
	static class IScopeMixin
	{
		public static void ReadScope(this IScope self, PdbSlot[] slots, PdbConstant[] constants, PdbScope[] scopes)
		{
			self.ReadVariables(slots);
			self.ReadConstants(constants);
			self.ReadScopes(scopes);
		}

		public static void ReadVariables(this IScope self, PdbSlot[] slots)
		{
			self.Variables.AddRange(slots.Select(s => new Variable(s)));
		}

		public static void ReadConstants(this IScope self, PdbConstant[] constants)
		{
			self.Constants.AddRange(constants.Select(c => new Constant(c)));
		}

		public static void ReadScopes(this IScope self, PdbScope[] scopes)
		{
			self.Scopes.AddRange(scopes.Select(s => new Scope(s)));
		}
	}
}
