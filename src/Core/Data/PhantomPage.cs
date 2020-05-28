//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xarial.Docify.Base;

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
