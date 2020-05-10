﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Core.Plugin.Extensions
{
    public interface IIncludesHandlerExtension
    {
        Task<string> ResolveInclude(string includeName, IMetadata md, IPage page);
    }
}
