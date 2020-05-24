//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Base.Context;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Base.Services
{
    public interface ILayoutParser
    {
        //TODO: rename to validate layout
        bool ContainsPlaceholder(string content);
        Task<string> InsertContent(ITemplate layout, string content, ISite site, IPage page, string url);
    }
}
