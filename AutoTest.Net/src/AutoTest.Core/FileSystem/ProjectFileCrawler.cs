namespace AutoTest.Core.FileSystem
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Collections.Generic;
	using AutoTest.Messages;

    public interface ICrawlForProjectFiles
    {
        ChangedFile[] FindParent(string startDirectory, string fileExtension);
    }
    
    public class ProjectFileCrawler : ICrawlForProjectFiles
    {
        public ChangedFile[] FindParent(string startDirectory, string fileExtension)
        {
            return Find(new DirectoryInfo(startDirectory), new Func<FileInfo, bool>(x => x.Extension.Equals(fileExtension)));
        }

        static ChangedFile[] Find(DirectoryInfo info, Func<FileInfo, bool> predicate)
        {
            if(info == null)
                return new ChangedFile[] { };
            if (!info.Exists)
                return new ChangedFile[] { };
            var files = info.GetFiles().Where(predicate).ToArray();
            if (files.Length > 0)
            {
                var changedProjects = new List<ChangedFile>();
                foreach (var file in files)
                    changedProjects.Add(new ChangedFile(file.FullName));
                return changedProjects.ToArray();
            }
            return Find(info.Parent, predicate);
        }
    }
}