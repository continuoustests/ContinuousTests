using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AutoTest.VM.FileSystem
{
    public class FSProxy : IFSProxy
    {
        public string GetSpecialFolder(Environment.SpecialFolder folder)
        {
            return Environment.GetFolderPath(folder);
        }

        public string[] GetFoldersFrom(string folder, string searchPattern)
        {
            return Directory.GetDirectories(folder, searchPattern, SearchOption.TopDirectoryOnly);
        }

        public string[] GetFilesFrom(string folder, string filter)
        {
            return Directory.GetFiles(folder, filter, SearchOption.AllDirectories);
        }

        public bool FileExists(string file)
        {
            var exists =  File.Exists(file);
            var existence = exists ? "exists" : "does not exist";
            Logger.WriteInfo(string.Format("File {0} {1}", file, existence));
            return exists;
        }

        public bool DirectoryExists(string system)
        {
            return Directory.Exists(system);
        }
    }
}
