//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System;
using System.Diagnostics;

namespace Xarial.Docify.Core.Data
{
    [DebuggerDisplay("~{" + nameof(Name) + "}")]
    public class PhantomPage : Page
    {
        public PhantomPage(string name)
            : base(name, "", new Metadata(), Guid.NewGuid().ToString(), null)
        {
        }
    }
}
