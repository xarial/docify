//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Core.Exceptions
{
    public class SiteParsingException : Exception
    {
        public IFile[] Files { get; }

        public SiteParsingException(IFile[] files) : base($"{files.Length} file(s) were not parsed")
        {
            Files = files;
        }
    }
}
