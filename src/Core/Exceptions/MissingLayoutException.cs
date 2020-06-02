//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System;

namespace Xarial.Docify.Core.Exceptions
{
    public class MissingLayoutException : UserMessageException
    {
        public MissingLayoutException(string layoutName)
            : base($"Layout cannot be found: {layoutName}")
        {
        }
    }
}
