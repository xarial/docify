//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Core.Data
{
    public class Metadata : Dictionary<string, object>, IMetadata
    {
        public Metadata() : base(StringComparer.CurrentCultureIgnoreCase)
        {
        }

        public Metadata(IDictionary<string, object> parameters)
            : base(parameters, StringComparer.CurrentCultureIgnoreCase)
        {
        }

        public virtual IMetadata Copy(IDictionary<string, object> data)
        {
            return new Metadata(data);
        }
    }
}
