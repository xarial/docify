//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Services;
using Xarial.Docify.Base;
using Xarial.Docify.Core.Compiler.Context;
using Xarial.Docify.Core.Exceptions;

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

        public void ValidateLayout(string content)
        {
            if (!Regex.IsMatch(content, CONTENT_PLACEHOLDER_REGEX))
            {
                throw new LayoutMissingContentPlaceholder();
            }
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
