using System.Collections.Generic;
namespace AutoTest.Core.Caching
{
    public interface ICache
    {
        int Count { get; }
        void Add<T>(string key) where T : IRecord;
        bool Exists(string key);
        T Get<T>(string key) where T : IRecord;
        T Get<T>(int index) where T : IRecord;
        T[] GetAll<T>() where T : IRecord;
        void Reload<T>(string key) where T : IRecord;

        void Add(IEnumerable<ProjectFile> files);
        void InvalidateProjectFiles(string project);
        ProjectFile[] GetAllProjectFiles();
        bool IsProjectFile(string file);
    }
}