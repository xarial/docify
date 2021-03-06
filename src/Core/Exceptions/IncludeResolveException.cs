﻿//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System;

namespace Xarial.Docify.Core.Exceptions
{
    public class IncludeResolveException : UserMessageException
    {
        public IncludeResolveException(string name, string url, Exception inner)
            : base($"Failed to resolve include '{name}' in '{url}'", inner)
        {
        }
    }
}
