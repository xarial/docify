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
    public class Configuration : Dictionary<string, dynamic>
    {
        public Configuration() : base()
        {
        }

        public Configuration(Dictionary<string, dynamic> parameters) : base(parameters) 
        {
        }
    }
}
