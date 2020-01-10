//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.Docify.Base.Data
{
    public class Metadata : Dictionary<string, dynamic>
    {
        public Metadata() : base(StringComparer.CurrentCultureIgnoreCase)
        {
        }

        public Metadata(Dictionary<string, dynamic> parameters) 
            : base(parameters, StringComparer.CurrentCultureIgnoreCase)
        {
        }
    }
}
