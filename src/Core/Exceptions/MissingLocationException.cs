//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
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
