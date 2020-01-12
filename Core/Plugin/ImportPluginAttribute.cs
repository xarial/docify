//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Composition;
using System.Text;

namespace Xarial.Docify.Core.Plugin
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ImportPluginAttribute : ImportManyAttribute
    {
    }
}
