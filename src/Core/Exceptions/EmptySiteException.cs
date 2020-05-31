//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System;

namespace Xarial.Docify.Core.Exceptions
{
    public class EmptySiteException : Exception
    {
        public EmptySiteException() : base("Site contains no pages")
        {
        }
    }
}
