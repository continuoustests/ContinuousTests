namespace AutoTest.UI
{
    public class CodePosition
    {
        public string File { get; private set; }
        public int LineNumber { get; private set; }
        public int Column { get; private set; }

        public CodePosition(string file, int lineNumber, int column)
        {
            File = file;
            LineNumber = lineNumber;
            Column = column;
        }
    }
}
