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
using Xarial.Docify.Base.Content;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Services;

namespace Xarial.Docify.Core.Compiler
{
    public class LayoutParser : ILayoutParser
    {
        private const string CONTENT_PLACEHOLDER_REGEX = "{{ *content *}}";

        private readonly IContentTransformer m_Transformer;

        public LayoutParser(IContentTransformer transformer) 
        {
            m_Transformer = transformer;
        }

        public bool ContainsPlaceholder(string content)
        {
            return Regex.IsMatch(content, CONTENT_PLACEHOLDER_REGEX);
        }

        public async Task<string> InsertContent(Template layout, string content, IContextModel model)
        {
            if (layout == null) 
            {
                throw new ArgumentNullException(nameof(layout));
            }

            while (layout != null)
            {
                var layoutContent = await m_Transformer.Transform(layout.RawContent, layout.Key, model);

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
