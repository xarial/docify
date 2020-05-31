//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System.IO;

namespace Xarial.Docify.Core.Exceptions
{
    public class MissingLocationException : FileNotFoundException
    {
        public MissingLocationException(string loc) : base($"'{loc}' location doesn't exist")
        {
        }
    }
}
