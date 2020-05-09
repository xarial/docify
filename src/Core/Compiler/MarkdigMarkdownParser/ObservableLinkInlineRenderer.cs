//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using Markdig.Renderers;
using Markdig.Syntax.Inlines;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xarial.Docify.Base.Plugins;
using Xarial.Docify.Core.Plugin;
using Markdig.Renderers.Html.Inlines;

namespace Xarial.Docify.Core.Compiler.MarkdigMarkdownParser
{
    public class ObservableLinkInlineRenderer : LinkInlineRenderer
    {
        private readonly IEnumerable<IRenderUrlPlugin> m_UrlPlugins;
        private readonly IEnumerable<IRenderImagePlugin> m_ImagePlugins;

        public ObservableLinkInlineRenderer(IEnumerable<IRenderUrlPlugin> urlPlugins,
            IEnumerable<IRenderImagePlugin> imagePlugins)
        {
            m_UrlPlugins = urlPlugins;
            m_ImagePlugins = imagePlugins;
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
                m_UrlPlugins.InvokePluginsIfAny(p => p.RenderUrl(linkOut));
            }
            else 
            {
                m_ImagePlugins.InvokePluginsIfAny(p => p.RenderImage(linkOut));
            }

            renderer.Write(linkOut.ToString());
        }
    }
}
