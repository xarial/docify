//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.Docify.Core.Exceptions
{
    public class MultiplePluginsPerNameException : UserMessageException
    {
        public MultiplePluginsPerNameException(string pluginName) 
            : base($"Plugin '{pluginName}' contains more than one plugin") 
        {
        }
    }
}
