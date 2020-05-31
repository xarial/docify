//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;

namespace Xarial.Docify.Core.Exceptions
{
    public class DuplicateTemplateException : Exception
    {
        public DuplicateTemplateException(string templateName)
            : base($"'{templateName}' has been already registered")
        {
        }
    }
}
