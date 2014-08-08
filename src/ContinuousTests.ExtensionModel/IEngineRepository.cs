using System;

namespace ContinuousTests.ExtensionModel
{
    /// <summary>
    /// ContinuousTests engine repository. Use this class to connect to existing or create new ContinuousTests engines.
    /// </summary>
    public interface IEngineRepository
    {
        /// <summary>
        /// Get engines from token. Token can be solution file or watch folder
        /// </summary>
        /// <param name="token">The engine watch token to look for. The token is either the solution file or the watch folder</param>
        /// <returns>Engine</returns>
        IEngine GetEngine(string watchToken);

        /// <summary>
        /// Get engines from token. Token can be solution file or watch folder
        /// </summary>
        /// <param name="token">The engine watch token to look for. The token is either the solution file or the watch folder</param>
        /// <param name="caseInsensitive">Wether to match the token ignoring case</param>
        /// <returns>Engine</returns>
        IEngine GetEngine(string token, bool caseInsensitive);

        /// <summary>
        /// Starts a new ContinuousTests engine. NB! If this assembly is not located in the ContinuousTests folder you will want to use the overload specifying the ContinuousTests location.
        /// </summary>
        /// <param name="watchToken">Fullpath to solution file or directory to watch</param>
        /// <returns>Engine</returns>
        IEngine StartEngine(string watchToken);

        /// <summary>
        /// Starts a new ContinuousTests engine.
        /// </summary>
        /// <param name="watchToken">Fullpath to solution file or directory to watch</param>
        /// <param name="continuousTestsPath">Path to where the ContinuousTests files are located</param>
        /// <returns>Engine</returns>
        IEngine StartEngine(string watchToken, string continuousTestsPath);
    }
}
