﻿//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.Docify.Base.Plugins
{
    public interface IPlugin
    {
        void Init(IEngine engine);
    }
    
    public interface IPlugin<TSettings> : IPlugin 
        where TSettings : new()
    {
        void Init(IEngine engine, TSettings setts);
    }
}
