//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Base.Data
{
    public interface IFile : IContent
    {
        ILocation Location { get; }
    }
}
