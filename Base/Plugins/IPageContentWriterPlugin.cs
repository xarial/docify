using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Xarial.Docify.Base.Plugins
{
    public interface IPageContentWriterPlugin : IPlugin
    {
        Task<string> WritePageContent(string content, string url);
    }
}
