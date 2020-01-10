//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.Docify.Core.Exceptions
{
    public class DuplicateFragmentSourceFileException : Exception
    {
        public DuplicateFragmentSourceFileException(string fragmentName, string id)
            : base($"Specified file '{id}' from '{fragmentName}' fragment already present in the source files")
        {
        }
    }
}
