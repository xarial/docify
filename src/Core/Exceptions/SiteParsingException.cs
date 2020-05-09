//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
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
