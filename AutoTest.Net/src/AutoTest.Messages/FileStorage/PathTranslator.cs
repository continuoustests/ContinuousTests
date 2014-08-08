using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace AutoTest.Messages.FileStorage
{
    public class PathTranslator
    {
        public static string STORAGE_PATH
        {
            get
            {
                var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var mm = Path.Combine(appData, "MightyMoose");
                if (!Directory.Exists(mm))
                    Directory.CreateDirectory(mm);
                var storage = Path.Combine(mm, "storage");
                if (!Directory.Exists(storage))
                    Directory.CreateDirectory(storage);
                return storage;
            }
        }
        private Func<string, bool> _directoryExists = (dir) => { return Directory.Exists(dir); };
        private Action<string> _createDirectory = (dir) => Directory.CreateDirectory(dir);

        private static List<KeyValuePair<string, string>> _translateCache = new List<KeyValuePair<string, string>>();

        private string _storagePath;
        private string _watchToken;

        public PathTranslator(string watchToken)
        {
            _storagePath = STORAGE_PATH;
            _watchToken = watchToken;
        }

        public PathTranslator(string storagePath, string watchToken, Func<string, bool> directoryExists, Action<string> createDirectory)
        {
            _storagePath = storagePath;
            _watchToken = watchToken;
            _directoryExists = directoryExists;
            _createDirectory = createDirectory;
        }

        public string Translate(string path)
        {
            if (_watchToken == null)
                return null;
            if (path == null)
                return null;

            var fileName = Path.GetFileName(path);
            if (fileName.ToLower().EndsWith("mm.dll") ||
                fileName.ToLower().EndsWith("mm.exe"))
            {
                addToCache(path, path);
                return path;
            }

            var solutionDir = getSolutionDir();
            if (solutionDir.EndsWith(Path.DirectorySeparatorChar.ToString()))
                solutionDir = solutionDir.Substring(0, solutionDir.Length - 1);
            var relativePath = path.Replace(solutionDir + Path.DirectorySeparatorChar, "");
            if (relativePath != Path.GetFileName(path))
                relativePath = getStandalonePath(relativePath);
            var storagePath = getStoragePath(solutionDir, relativePath);
            addToCache(storagePath, path);
            preparePath(storagePath);
            return storagePath;
        }

        private string getStoragePath(string solutionDir, string relativePath)
        {
            var fileName = Path.GetFileName(relativePath);
            var solutionToken = generateSolutionToken(solutionDir);
            var storagePath = "";
            if (fileName.ToLower() == "autotest.config")
                storagePath = Path.Combine(Path.Combine(Path.Combine(_storagePath, "Configuration"), solutionToken), relativePath);
            //else if (fileName.ToLower().EndsWith("mm.dll") ||
            //         fileName.ToLower().EndsWith("mm.exe"))
            //    storagePath = Path.Combine(Path.Combine(Path.Combine(_storagePath, "Cache"), solutionToken), relativePath);
            else if (fileName.ToLower().EndsWith("mm_cache.bin"))
                storagePath = Path.Combine(Path.Combine(Path.Combine(_storagePath, "Cache"), solutionToken), relativePath);
            else
                storagePath = Path.Combine(Path.Combine(_storagePath, solutionToken), relativePath);
            return storagePath;
        }

        private void preparePath(string storagePath)
        {
            if (storagePath == null)
                return;
            var directory = Path.GetDirectoryName(storagePath);
            while (directory != null)
            {
                if (_directoryExists(directory))
                    break;
                _createDirectory(directory);
                directory = Path.GetDirectoryName(directory);
            }
        }

        private void addToCache(string storagePath, string path)
        {
            if (storagePath == null)
                return;
            lock (_translateCache)
            {
                if (_translateCache.Exists(x => x.Key.Equals(storagePath)))
                    return;
                _translateCache.Add(new KeyValuePair<string, string>(storagePath, path));
            }
        }

        private string generateSolutionToken(string solutionDir)
        {
            var solution = Path.GetFileName(solutionDir);
            var encoded = md5(solutionDir);
            return solution + "_" + encoded;
        }

        private string getStandalonePath(string relativePath)
        {
            var encoded = md5(relativePath);
            return Path.Combine(
                Path.GetFileNameWithoutExtension(relativePath) + "_" + encoded,
                Path.GetFileName(relativePath));
        }

        private static string md5(string solutionDir)
        {
            var hash = new MD5CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(solutionDir));
            var builder = new StringBuilder();
            hash.ToList().ForEach(x => builder.Append(x.ToString("x2").ToLower()));
            var encoded = builder.ToString();
            return encoded;
        }

        private string getSolutionDir()
        {
            if (Path.GetExtension(_watchToken) == "")
                return _watchToken;
            return Path.GetDirectoryName(_watchToken);
        }

        public string TranslateFrom(string directory)
        {
            lock (_translateCache)
            {
                if (!_translateCache.Exists(x => x.Key.Equals(directory)))
                    return null;
               return _translateCache.First(x => x.Key.Equals(directory)).Value;
            }
        }
    }
}
