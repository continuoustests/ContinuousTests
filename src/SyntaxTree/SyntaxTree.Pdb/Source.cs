//
// Source.cs
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
	/// Represents a source file containing the sequence points
	/// of the function body.
	/// </summary>
	public sealed class Source
	{
		private readonly Collection<SequencePoint> sequencePoints;

		/// <summary>
		/// The document containing the function body.
		/// </summary>
		public Document Document { get; set; }

		/// <summary>
		/// The sequence points of the function body in this source file.
		/// </summary>
		public Collection<SequencePoint> SequencePoints { get { return sequencePoints; } }

		public Source()
		{
			this.sequencePoints = new Collection<SequencePoint>();
		}

		internal Source(PdbLines lines) : this()
		{
			this.Document = new Document(lines.file);

			sequencePoints.AddRange(lines.lines.Select(l => new SequencePoint(l)));
		}
	}
}
