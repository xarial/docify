//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.Docify.Base.Exceptions
{
    public class BaseUserMessageException : Exception, IUserMessageException
    {
        public BaseUserMessageException(string message) : base(message)
        {
        }

        public BaseUserMessageException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
