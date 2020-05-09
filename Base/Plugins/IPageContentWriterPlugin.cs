﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Base.Plugins
{
    public interface IPageContentWriterPlugin : IPlugin
    {
        Task<string> WritePageContent(string content, IMetadata data, string url);
    }
}
