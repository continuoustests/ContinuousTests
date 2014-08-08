using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using AutoTest.Messages;

namespace AutoTest.Core.FileSystem.ProjectLocators
{
    public interface ILocateProjects
    {
        ChangedFile[] Locate(string file);
        bool IsProject(string file);
    }
}
