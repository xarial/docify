//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using Markdig.Renderers;
using Markdig.Syntax.Inlines;
using System.IO;
using System.Text;
using Markdig.Renderers.Html.Inlines;
using Xarial.Docify.Core.Plugin.Extensions;

namespace Xarial.Docify.Core.Compiler.MarkdigMarkdownParser
{
    public class ObservableLinkInlineRenderer : LinkInlineRenderer
    {
        private readonly ICompilerExtension m_Ext;

        public ObservableLinkInlineRenderer(ICompilerExtension ext)
        {
            m_Ext = ext;
        }

        protected override void Write(HtmlRenderer renderer, LinkInline link)
        {
            var linkOut = new StringBuilder();

            using (var strWriter = new StringWriter(linkOut))
            {
                base.Write(new HtmlRenderer(strWriter), link);
            }

            if (!link.IsImage)
            {
                m_Ext.RenderUrl(linkOut);
            }
            else
            {
                m_Ext.RenderImage(linkOut);
            }

            renderer.Write(linkOut.ToString());
        }
    }
}
