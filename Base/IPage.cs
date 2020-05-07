//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Base
{
    public interface IPage : IFrame
    {
        List<IPage> SubPages { get; }
        List<IFile> Assets { get; }
        ILocation Location { get; }
    }
}
