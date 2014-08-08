using System;
namespace AutoTest.Messages
{
	public class Link
    {
		public int Start { get; private set; }
		public int Length { get; private set; }

        public Link(int start, int length)
        {
            Start = start;
            Length = length;            
        }
    }
}

