//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
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
