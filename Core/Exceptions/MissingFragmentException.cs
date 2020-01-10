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
    public class MissingFragmentException : Exception
    {
        public MissingFragmentException(string fragName, string fragDir) : base($"Fragment '{fragName}' cannot be found in '{fragDir}'")
        { 
        }
    }
}
