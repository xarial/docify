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
using Xarial.Docify.Base.Services;

namespace Xarial.Docify.Core.Compiler
{
    public class LayoutParser : ILayoutParser
    {
        private const string CONTENT_PLACEHOLDER_REGEX = "{{ *content *}}";

        public bool ContainsPlaceholder(string content)
        {
            return Regex.IsMatch(content, CONTENT_PLACEHOLDER_REGEX);
        }

        public string InsertContent(string content, string insertContent)
        {
            return Regex.Replace(content, CONTENT_PLACEHOLDER_REGEX, insertContent);
        }
    }
}
