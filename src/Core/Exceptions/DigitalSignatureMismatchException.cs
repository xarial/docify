//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Base;

namespace Xarial.Docify.Core.Exceptions
{
    public class DigitalSignatureMismatchException : UserMessageException
    {
        public DigitalSignatureMismatchException(ILocation loc) 
            : base($"Digital signature of the '{loc.ToId()}' doesn't match the file manifest")
        {
        }
    }
}
