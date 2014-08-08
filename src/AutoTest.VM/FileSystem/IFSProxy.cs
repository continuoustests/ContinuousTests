using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.VM.FileSystem
{
    public interface IFSProxy
    {
        string GetSpecialFolder(Environment.SpecialFolder folder);
        string[] GetFoldersFrom(string folder, string searchPattern);
        string[] GetFilesFrom(string folder, string filter);
        bool FileExists(string file);
        bool DirectoryExists(string system);
    }
}
