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
    public class LibraryFileModifiedException : UserMessageException
    {
        public LibraryFileModifiedException(string filePath)
            : base($"File in the library {filePath} was modified and will not be uninstalled. Please manually remove the target directory") 
        {
        }
    }
}
