//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System;

namespace Xarial.Docify.Core.Exceptions
{
    public class DuplicateComponentSourceFileException : UserMessageException
    {
        public DuplicateComponentSourceFileException(string id)
            : base($"Specified file '{id}' component already present in the source files")
        {
        }
    }
}
