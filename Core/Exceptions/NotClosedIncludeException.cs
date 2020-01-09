//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.Docify.Core.Exceptions
{
    public class NotClosedIncludeException : Exception
    {
        public NotClosedIncludeException(string content, string closingTag)
            : base($"Closing tag '{closingTag}' is missing for include '{content.Substring(0, Math.Min(50, content.Length))}'")
        {
        }
    }
}
