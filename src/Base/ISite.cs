//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System.Collections.Generic;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Base
{
    public interface ISite
    {
        string BaseUrl { get; }
        IPage MainPage { get; }
        List<ITemplate> Layouts { get; }
        List<ITemplate> Includes { get; }
        IConfiguration Configuration { get; }
    }
}
