//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Xarial.Docify.Base;

namespace Xarial.Docify.Core.Exceptions
{
    public class UnsupportedSourceFileTypesException : Exception
    {
        public UnsupportedSourceFileTypesException(IEnumerable<ISourceFile> srcFiles)
            : base($"The followin source file are not supported: {string.Join(", ", srcFiles.Select(f => f.GetType()).Distinct().Select(t => t.FullName))}")
        {
        }
    }
}
