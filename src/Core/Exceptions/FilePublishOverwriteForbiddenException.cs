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
    public class FilePublishOverwriteForbiddenException : UserMessageException
    {
        public FilePublishOverwriteForbiddenException(string filePath)
            : base($"Cannot publish file to {filePath} as overwriting is forbidden")
        {
        }
    }
}
