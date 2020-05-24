//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Context;
using Xarial.Docify.Base.Services;
using Xarial.Docify.Base;
using Xarial.Docify.Core.Compiler.Context;

namespace Xarial.Docify.Core.Compiler
{
    public class LayoutParser : ILayoutParser
    {
        private const string CONTENT_PLACEHOLDER_REGEX = "{{ *content *}}";

        private readonly IDynamicContentTransformer m_Transformer;

        public LayoutParser(IDynamicContentTransformer transformer) 
        {
            m_Transformer = transformer;
        }

        public bool ContainsPlaceholder(string content)
        {
            return Regex.IsMatch(content, CONTENT_PLACEHOLDER_REGEX);
        }

        public async Task<string> InsertContent(ITemplate layout, string content, ISite site, IPage page, string url)
        {
            if (layout == null) 
            {
                throw new ArgumentNullException(nameof(layout));
            }

            while (layout != null)
            {
                var model = new ContextModel(site, page, layout.Data, url);

                var layoutContent = await m_Transformer.Transform(layout.RawContent, layout.Id, model);

                content = ReplaceContent(layoutContent, content);

                layout = layout.Layout;
            }

            return content;
        }

        private string ReplaceContent(string content, string insertContent) 
        {
            return Regex.Replace(content, CONTENT_PLACEHOLDER_REGEX, insertContent);
        }
    }
}
