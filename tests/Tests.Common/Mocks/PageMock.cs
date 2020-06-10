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
using Xarial.Docify.Base.Data;
using Xarial.Docify.Core.Data;

namespace Tests.Common.Mocks
{
    public class PageMock : Page
    {
        public PageMock(string name, string rawContent, Template layout = null)
            : this(name, rawContent, new Metadata(), layout)
        {

        }

        public PageMock(string name, string rawContent, IMetadata data, Template layout = null)
            : this(rawContent, name, data, Guid.NewGuid().ToString(), layout)
        {
        }

        public PageMock(string name, string rawContent, IMetadata data, string id, Template layout = null)
            : base(rawContent, name, data, id, layout)
        {
        }
    }

    public class PhantomPageMock : PageMock, IPhantomPage
    {
        public PhantomPageMock(string name) : base(name, "") 
        {
        }
    }
}
