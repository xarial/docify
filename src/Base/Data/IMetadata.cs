//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System.Collections.Generic;

namespace Xarial.Docify.Base.Data
{
    public interface IMetadata : IDictionary<string, object>
    {
        IMetadata Copy(IDictionary<string, object> data);
    }
}
