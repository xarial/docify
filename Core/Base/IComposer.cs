//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.Docify.Core.Base
{
    public interface IComposer
    {
        Site ComposeSite(IEnumerable<ISourceFile> elements, string baseUrl);
    }
}
