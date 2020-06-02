//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System;
using Xarial.Docify.Base;

namespace Xarial.Docify.Core.Exceptions
{
    public class DuplicatePageException : UserMessageException
    {
        public DuplicatePageException(ILocation loc)
            : base($"Specified page already exist '{loc.ToId()}'")
        {
        }
    }
}
