//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;

namespace Xarial.Docify.Core.Exceptions
{
    public class DuplicateComponentSourceFileException : Exception
    {
        public DuplicateComponentSourceFileException(string id)
            : base($"Specified file '{id}' component already present in the source files")
        {
        }
    }
}
