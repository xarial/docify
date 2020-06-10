//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System;
using System.Diagnostics;
using Xarial.Docify.Base;

namespace Xarial.Docify.Core.Data
{
    [DebuggerDisplay("~{" + nameof(Name) + "}")]
    internal class PhantomPage : Page, IPhantomPage
    {
        internal PhantomPage(string name)
            : base(name, "", new Metadata() { { "seo", false }, { "sitemap", false } }, Guid.NewGuid().ToString(), null)
        {
        }
    }
}
