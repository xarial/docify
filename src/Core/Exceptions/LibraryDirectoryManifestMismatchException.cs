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
    public class LibraryDirectoryManifestMismatchException : UserMessageException
    {
        public LibraryDirectoryManifestMismatchException(string outDir, string manifestDir)
            : base($"Target directory: '{outDir}' doesn't match the manifest library directory: '{manifestDir}'. Please manually remove the target directory") 
        {
        }
    }
}
