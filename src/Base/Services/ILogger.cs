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
        void LogWarning(string msg);

        /// <summary>
        /// Logs error message
        /// </summary>
        /// <param name="msg">Message</param>
        void LogError(string msg);

        /// <summary>
        /// Logs information message
        /// </summary>
        /// <param name="msg">Message</param>
        void LogInformation(string msg);
    }
}
