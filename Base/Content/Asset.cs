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

namespace Xarial.Docify.Base.Content
{
    [DebuggerDisplay("{" + nameof(Location) + "}")]
    public abstract class Asset : IWritable
    {
        public Location Location { get; }

        public Asset(Location loc)
        {
            Location = loc;
        }
    }
}
