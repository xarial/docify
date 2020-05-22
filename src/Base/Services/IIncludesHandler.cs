﻿//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Base.Services
{
    public interface IIncludesHandler
    {
        Task<string> ReplaceAll(string rawContent, ISite site, IPage page, string url);
        Task ParseParameters(string includeRawContent, out string name, out IMetadata param);
        Task<string> Render(string name, IMetadata param, ISite site, IPage page, string url);
    }
}
