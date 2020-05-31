//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using Markdig.Renderers;
using Markdig.Syntax;
using System.Text;
using Markdig.Renderers.Html;
using System.IO;
using Xarial.Docify.Core.Plugin.Extensions;

namespace Xarial.Docify.Core.Compiler.MarkdigMarkdownParser
{
    public class ObservableCodeBlockRenderer : CodeBlockRenderer
    {
        private readonly ICompilerExtension m_Ext;

        public ObservableCodeBlockRenderer(ICompilerExtension ext)
        {
            m_Ext = ext;
        }

        protected override void Write(HtmlRenderer renderer, CodeBlock obj)
        {
            if (obj is FencedCodeBlock)
            {
                var fencedCodeBlock = obj as FencedCodeBlock;

                var lang = fencedCodeBlock.Info;
                var code = fencedCodeBlock.Lines.ToString();
                var args = fencedCodeBlock.Arguments;

                var codeOut = new StringBuilder();
                using (var strWriter = new StringWriter(codeOut))
                {
                    base.Write(new HtmlRenderer(strWriter), obj);
                }

                m_Ext.RenderCodeBlock(code, lang, args, codeOut);

                renderer.Write(codeOut.ToString());
            }
            else
            {
                base.Write(renderer, obj);
            }
        }
    }
}
