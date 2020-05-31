//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

namespace Xarial.Docify.Base.Services
{
    public interface ILogger
    {
        void Log(string msg);
        void LogWarning(string msg);
        void LogError(string msg);
        void LogInformation(string msg);
    }
}
