//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using Markdig.Renderers;
using Markdig.Renderers.Html;
using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Base.Services;

namespace Xarial.Docify.Core.Compiler.MarkdigMarkdownParser
{
    public class IncludeRenderer : HtmlObjectRenderer<IncludeData>
    {
        private readonly IIncludesHandler m_IncludesHandler;

        public IncludeRenderer(IIncludesHandler includesHandler)
        {
            m_IncludesHandler = includesHandler;
        }

        protected override void Write(HtmlRenderer renderer, IncludeData data)
        {
            if (renderer.EnableHtmlForInline)
            {
                var res = m_IncludesHandler.Insert(data.Name, data.Parameters, data.Site, data.Page).Result;

                renderer.Write(res);
            }
            else
            {
                //TODO: implement plain writing
            }
        }
    }
}
