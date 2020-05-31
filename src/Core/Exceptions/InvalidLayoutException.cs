//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System;

namespace Xarial.Docify.Core.Exceptions
{
    public class InvalidLayoutException : Exception
    {
        public InvalidLayoutException(string layoutName, Exception inner)
            : base($"Invalid layout '{layoutName}'", inner)
        {
        }
    }
}
