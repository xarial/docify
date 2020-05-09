//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;

namespace Xarial.Docify.Base
{
    public interface ILocation 
    {
        IReadOnlyList<string> Path { get; }
        string FileName { get; }

        ILocation Copy(string fileName, IEnumerable<string> path);
    }    
}
