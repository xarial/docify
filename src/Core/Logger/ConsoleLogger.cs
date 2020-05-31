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
        public void Log(string msg) => WriteLine(msg);
        public void LogError(string msg) => WriteLine(msg, ConsoleColor.Red);
        public void LogInformation(string msg) => WriteLine(msg, ConsoleColor.Green);
        public void LogWarning(string msg) => WriteLine(msg, ConsoleColor.Yellow);

        private void WriteLine(string msg, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(msg);
            Console.ResetColor();
        }
    }
}
