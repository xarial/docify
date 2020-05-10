using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Services;

namespace Xarial.Docify.Base.Plugins
{
    public delegate IAsyncEnumerable<IFile> AddFilesPostCompileDelegate();
    public delegate Task PreCompileDelegate(ISite site);
    public delegate void RenderCodeBlockDelegate(string rawCode, string lang, string args, StringBuilder html);
    public delegate void RenderImageDelegate(StringBuilder html);
    public delegate void RenderUrlDelegate(StringBuilder html);
    public delegate Task<string> WritePageContentDelegate(string content, IMetadata data, string url);

    public interface ICompilerManager
    {
        event AddFilesPostCompileDelegate AddFilesPostCompile;
        event PreCompileDelegate PreCompile;
        event RenderCodeBlockDelegate RenderCodeBlock;
        event RenderImageDelegate RenderImage;
        event RenderUrlDelegate RenderUrl;
        event WritePageContentDelegate WritePageContent;

        ICompiler Instance { get; }
        IContentTransformer ContentTransformer { get; }
    }
}
