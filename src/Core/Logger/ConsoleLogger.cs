//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System;
using Xarial.Docify.Base.Services;

namespace Xarial.Docify.Core.Logger
{
    public class ConsoleLogger : ILogger
    {
        private readonly bool m_Verbose;

        public ConsoleLogger(bool verbose) 
        {
            m_Verbose = verbose;
        }

        public void LogError(string msg, bool verbose = false) => WriteLine(msg, verbose, ConsoleColor.Red);
        public void LogInformation(string msg, bool verbose = false) => WriteLine(msg, verbose, ConsoleColor.Green);
        public void LogWarning(string msg, bool verbose = false) => WriteLine(msg, verbose, ConsoleColor.Yellow);

        private void WriteLine(string msg, bool verbose, ConsoleColor color)
        {
            if (m_Verbose || !verbose)
            {
                Console.ForegroundColor = color;
                Console.WriteLine(msg);
                Console.ResetColor();
            }
        }
    }
}
