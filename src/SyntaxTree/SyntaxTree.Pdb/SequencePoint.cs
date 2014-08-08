//
// SequencePoint.cs
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
	/// A sequence point represents a text span of source associated
	/// with an IL offset.
	/// </summary>
	public sealed class SequencePoint : IEquatable<SequencePoint>
	{
		/// <summary>
		/// The IL offset of the sequence point.
		/// </summary>
		public int Offset { get; set; }

		/// <summary>
		/// The line of the sequence point.
		/// </summary>
		public int Line { get; set; }

		/// <summary>
		/// The column of the sequence point.
		/// </summary>
		public int Column { get; set; }

		/// <summary>
		/// The ending line of the sequence point.
		/// </summary>
		public int EndLine { get; set; }

		/// <summary>
		/// The ending column of the sequence point.
		/// </summary>
		public int EndColumn { get; set; }

		/// <summary>
		/// Returns whether the sequence point is hidden.
		/// </summary>
		public bool IsHidden { get { return Line == 0xfeefee; } }

		public SequencePoint()
		{
		}

		internal SequencePoint(PdbLine line) : this()
		{
			this.Offset = (int) line.offset;
			this.Line = (int) line.lineBegin;
			this.Column = line.colBegin;
			this.EndLine = (int) line.lineEnd;
			this.EndColumn = line.colEnd;
		}

		public override int GetHashCode()
		{
			return this.Offset ^ this.Line ^ this.Column ^ this.EndLine ^ this.EndColumn;
		}

		public bool Equals(SequencePoint other)
		{
			if (other == null)
				return false;

			return this.Offset == other.Offset
				&& this.Line == other.Line
				&& this.Column == other.Column
				&& this.EndLine == other.EndLine
				&& this.EndColumn == other.EndColumn;
		}
	}
}
