//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Core.Data
{
    public class Template : Sheet, ITemplate
    {
        public Template(string name, string rawContent, string id,
            IMetadata data = null, ITemplate baseTemplate = null)
            : base(rawContent, name, data, baseTemplate, id)
        {
        }
    }
}
