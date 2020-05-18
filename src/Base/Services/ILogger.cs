﻿//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

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
