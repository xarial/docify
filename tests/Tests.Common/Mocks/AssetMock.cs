//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Core.Data;

namespace Tests.Common.Mocks
{
    public class AssetMock : Asset
    {
        public AssetMock(string name, string content) 
            : this(name, ContentExtension.ToByteArray(content))
        {
        }

        public AssetMock(string name, byte[] content)
            : base(name, content, Guid.NewGuid().ToString())
        {
        }
    }
}
