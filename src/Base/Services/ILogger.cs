//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

namespace Xarial.Docify.Base.Services
{
    /// <summary>
    /// Service logs messages to user interface
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Logs warning message
        /// </summary>
        /// <param name="msg">Message</param>
        /// <param name="verbose">True to indicate this message is verbose and it will be output based on the user settings</param>
        void LogWarning(string msg, bool verbose = false);

        /// <summary>
        /// Logs error message
        /// </summary>
        /// <inheritdoc cref="LogWarning(string, bool)"/>
        void LogError(string msg, bool verbose = false);

        /// <summary>
        /// Logs information message
        /// </summary>
        /// <inheritdoc cref="LogWarning(string, bool)"/>
        void LogInformation(string msg, bool verbose = false);
    }
}
