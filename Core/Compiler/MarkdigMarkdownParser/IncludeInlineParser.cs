//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using Markdig.Helpers;
using Markdig.Parsers;
using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Base.Services;
using Xarial.Docify.Core.Exceptions;

namespace Xarial.Docify.Core.Compiler.MarkdigMarkdownParser
{
    public class IncludeInlineParser : InlineParser
    {
        private readonly IIncludesHandler m_ParamsParser;

        private const string START_TAG = "{%";
        private const string END_TAG = "%}";

        public IncludeInlineParser(IIncludesHandler paramsParser)
        {
            m_ParamsParser = paramsParser;
            OpeningCharacters = new char[] { START_TAG[0] };
        }

        public override bool Match(InlineProcessor processor, ref StringSlice slice)
        {
            if (!slice.Match(START_TAG))
            {
                return false;
            }

            var rawContent = new StringBuilder();

            slice.Start = slice.Start + START_TAG.Length - 1;

            var current = slice.NextChar();

            while (current != END_TAG[0] && slice.PeekChar(1) != END_TAG[1])
            {
                if (slice.IsEmpty)
                {
                    throw new NotClosedIncludeException(rawContent.ToString(), END_TAG);
                }

                rawContent.Append(current);
                current = slice.NextChar();
            }

            slice.Start = slice.Start + END_TAG.Length;

            string name;
            Dictionary<string, dynamic> param;
            m_ParamsParser.ParseParameters(rawContent.ToString(), out name, out param);

            var model = (ContextModel)processor.Context.Properties[MarkdigMarkdownContentTransformer.CONTEXT_MODEL_PARAM_NAME];
            processor.Inline = new IncludeData(name, param, model.Site.BaseSite, model.Page.BasePage);

            return true;
        }
    }
}
