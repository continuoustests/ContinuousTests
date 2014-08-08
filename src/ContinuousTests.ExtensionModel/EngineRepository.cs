using System.Collections.Generic;
using AutoTest.VM.Messages.Communication;
using System.IO;

namespace ContinuousTests.ExtensionModel
{
    /// <summary>
    /// ContinuousTests engine repository. Use this class to connect to existing or create new ContinuousTests engines.
    /// </summary>
    public class EngineRepository : IEngineRepository
    {
        /// <summary>
        /// Get engines from token. Token can be solution file or watch folder
        /// </summary>
        /// <param name="token">The engine watch token to look for. The token is either the solution file or the watch folder</param>
        /// <returns>Engine</returns>
        public IEngine GetEngine(string token)
        {
            return GetEngine(token, false);
        }

        /// <summary>
        /// Get engines from token. Token can be solution file or watch folder
        /// </summary>
        /// <param name="token">The engine watch token to look for. The token is either the solution file or the watch folder</param>
        /// <param name="caseInsensitive">Wether to match the token ignoring case</param>
        /// <returns>Engine</returns>
        public IEngine GetEngine(string token, bool caseInsensitive)
        {
            VMHandle activeHandle = null;
            var connect = new VMConnectHandle();
            var handles = connect.GetFromToken(token, caseInsensitive);
            var toDelete = new List<string>();
            foreach (var handle in handles)
            {
                if (canConnect(handle))
                {
                    activeHandle = handle;
                    break;
                }
                toDelete.Add(handle.File);
            }
            toDelete.ForEach(x => { try { File.Delete(x); } catch { } });
            if (activeHandle == null)
                return null;
            return new Engine(activeHandle);
        }

        /// <summary>
        /// Starts a new ContinuousTests engine. NB! If this assembly is not located in the ContinuousTests folder you will want to use the overload specifying the ContinuousTests location.
        /// </summary>
        /// <param name="watchToken">Fullpath to solution file or directory to watch</param>
        /// <returns>Engine</returns>
        public IEngine StartEngine(string watchToken)
        {
            return new Engine(watchToken,  Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));
        }

        /// <summary>
        /// Starts a new ContinuousTests engine.
        /// </summary>
        /// <param name="watchToken">Fullpath to solution file or directory to watch</param>
        /// <param name="continuousTestsPath">Path to where the ContinuousTests files are located</param>
        /// <returns>Engine</returns>
        public IEngine StartEngine(string watchToken, string continuousTestsPath)
        {
            return new Engine(watchToken, continuousTestsPath);
        }

        private bool canConnect(VMHandle handle)
        {
            try
            {
                var client = new NetClient(null);
                client.Connect(handle.IP, handle.Port);
                var isConnected = client.IsConnected;
                if (isConnected)
                    client.Disconnect();
                return isConnected;
            }
            catch
            {
            }
            return false;
        }
    }
}
