using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AutoTest.Core.FileSystem
{
    public class FileSystemService : IFileSystemService
    {
        public string[] GetFiles(string path, string searchPattern)
        {
            return Directory.GetFiles(path, searchPattern, SearchOption.AllDirectories);
        }

        public string ReadFileAsText(string path)
        {
            using (var reader = new StreamReader(path))
            {
                return reader.ReadToEnd();
            }
        }

        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public bool FileExists(string file)
        {
            return File.Exists(file);
        }

        public void CopyFile(string source, string destination)
        {
            if (File.Exists(destination))
                File.Delete(destination);
            File.Copy(source, destination);
        }

        public void DeleteFile(string source)
        {
            File.Delete(source);
        }
    }
}
