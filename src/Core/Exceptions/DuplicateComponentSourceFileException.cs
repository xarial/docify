﻿//*********************************************************************
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
    public class DuplicateComponentSourceFileException : Exception
    {
        public DuplicateComponentSourceFileException(string compName, string id)
            : base($"Specified file '{id}' from '{compName}' component already present in the source files")
        {
        }
    }
}
