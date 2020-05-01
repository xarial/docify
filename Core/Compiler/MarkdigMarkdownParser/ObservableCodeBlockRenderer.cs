//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using Markdig.Renderers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Xarial.Docify.Base.Plugins;
using Xarial.Docify.Core.Plugin;
using Markdig.Renderers.Html;
using System.IO;

namespace Xarial.Docify.Core.Compiler.MarkdigMarkdownParser
{
    public class ObservableCodeBlockRenderer : CodeBlockRenderer
    {
        private readonly IEnumerable<IRenderCodeBlockPlugin> m_Plugins;

        public ObservableCodeBlockRenderer(IEnumerable<IRenderCodeBlockPlugin> plugins) 
        {
            m_Plugins = plugins;
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
                
                m_Plugins.InvokePluginsIfAny(p => p.RenderCodeBlock(code, lang, args, codeOut));

                renderer.Write(codeOut.ToString());
            }
            else
            {
                base.Write(renderer, obj);
            }
        }
    }
}
