using System.IO;

namespace AutoTest.Core.FileSystem
{
    public interface IWatchValidator
    {
        string GetIgnorePatterns();
        bool ShouldPublish(string filePath);
    }
}