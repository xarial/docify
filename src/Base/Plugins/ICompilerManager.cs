//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Services;

namespace Xarial.Docify.Base.Plugins
{   
    public delegate Task PreCompileDelegate(ISite site);
    public delegate void RenderCodeBlockDelegate(string rawCode, string lang, string args, StringBuilder html);
    public delegate void RenderImageDelegate(StringBuilder html);
    public delegate void RenderUrlDelegate(StringBuilder html);
    public delegate Task<string> WritePageContentDelegate(string content, IMetadata data, string url);
    public delegate Task<IFile> PostCompileFileDelegate(IFile file);
    public delegate Task PostCompileDelegate();

    public interface ICompilerManager
    {
        event PreCompileDelegate PreCompile;
        event RenderCodeBlockDelegate RenderCodeBlock;
        event RenderImageDelegate RenderImage;
        event RenderUrlDelegate RenderUrl;
        event WritePageContentDelegate WritePageContent;
        event PostCompileFileDelegate PostCompileFile;
        event PostCompileDelegate PostCompile;

        ICompiler Instance { get; }
        IContentTransformer ContentTransformer { get; }
    }
}
