using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Core.Plugin.Extensions
{
    public interface ICompilerExtension
    {
        Task<string> WritePageContent(string content, IMetadata data, string url);
        IAsyncEnumerable<IFile> AddFilesPostCompile();
        Task PreCompile(ISite site);
        void RenderCodeBlock(string rawCode, string lang, string args, StringBuilder html);
        void RenderImage(StringBuilder html);
        void RenderUrl(StringBuilder html);
    }
}
