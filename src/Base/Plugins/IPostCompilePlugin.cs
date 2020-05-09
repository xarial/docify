using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Base.Plugins
{
    public interface IPostCompilePlugin : IPlugin
    {
        IAsyncEnumerable<IFile> AddFilesPostCompile();
    }
}
