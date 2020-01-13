//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Xarial.Docify.Lib.Tools
{
    public class CodeSelectorOptions
    {
        public bool LeftAlign { get; set; }
        public string[] Regions { get; set; }
        public string[] ExcludeRegions { get; set; }
        public bool HideRegions { get; set; }
    }

    public static class CodeSnippetHelper
    {
        private interface IRegionParser
        {
            string[] CodeTokens { get; }
            string StartRegEx { get; }
            string EndRegEx { get; }
        }

        private class CSharpRegionParser : IRegionParser
        {
            public string[] CodeTokens => new string[] { "cs", "csharp", "c#" };
            public string StartRegEx => "\\#region (.*)";
            public string EndRegEx => "\\#endregion";
        }

        private class VisualBasicRegionParser : IRegionParser
        {
            public string[] CodeTokens => new string[] { "vb", "vbnet", "vb.net", "vba" };
            public string StartRegEx => "\\#Region \"(.*)\"";
            public string EndRegEx => "\\#End Region";
        }

        private class DefaultRegionParser : IRegionParser
        {
            public string[] CodeTokens => null;
            public string StartRegEx => "\\---\"(.*)\"---";
            public string EndRegEx => "\\---";
        }

        private static readonly IRegionParser[] m_Parsers = new IRegionParser[]
        {
            new CSharpRegionParser(),
            new VisualBasicRegionParser(),
            new DefaultRegionParser()
        };
        
        public static string[] Select(string rawCode, string codeLang, CodeSelectorOptions opts)
        {
            var srcLines = Regex.Split(rawCode, "\r\n|\r|\n");

            var result = new List<string[]>();
            result.Add(srcLines);

            var parser = SelectParser(codeLang);

            if (opts.Regions?.Any() == true)
            {
                var newResult = new List<string[]>();

                for (int i = 0; i < result.Count; i++)
                {
                    var lines = result[i];
                    var indices = SelectRegionLines(lines, parser, opts.Regions, false);
                    
                    var curGroup = new List<string>();

                    for (int j = 0; j < indices.Length; j++) 
                    {
                        if (!(j == 0 || indices[j] - indices[j - 1] == 1))
                        {
                            newResult.Add(curGroup.ToArray());
                            curGroup.Clear();
                        }

                        curGroup.Add(lines[indices[j]]);
                    }

                    if (curGroup.Any())
                    {
                        newResult.Add(curGroup.ToArray());
                    }
                }

                result = newResult;
            }

            if (opts.ExcludeRegions?.Any() == true)
            {
                //var indices = SelectRegionLines(lines, parser, opts.ExcludeRegions, true);
                //lines = lines.Where((l, i) => !indices.Contains(i)).ToArray();
            }

            if (opts.HideRegions)
            {
                //lines = lines.Where(l =>
                //{
                //    string regName;
                //    return !(IsRegionEnd(l, parser) || IsRegionStart(l, parser, out regName));
                //}).ToArray();
            }

            if (opts.LeftAlign)
            {
                //var indent = lines.Min(l => string.IsNullOrEmpty(l) ? int.MaxValue : l.TakeWhile(Char.IsWhiteSpace).Count());

                //if (indent > 0)
                //{
                //    lines = lines.Select(l => string.IsNullOrEmpty(l) ? l : l.Substring(indent)).ToArray();
                //}
            }

            return result.Select(lines => string.Join(Environment.NewLine, lines)).ToArray();
        }

        private static IRegionParser SelectParser(string codeLang) 
        {
            return m_Parsers.First(p => p.CodeTokens == null || p.CodeTokens.Contains(codeLang, StringComparer.CurrentCultureIgnoreCase));
        }

        private static int[] SelectRegionLines(string[] lines, IRegionParser parser, string[] regions, bool inclusive)
        {
            var foundRegions = new List<string>();

            var resLinesIndices = new List<int>();

            bool isRecordingRegion = false;
            int relRegLevel = 0;

            for (int i = 0; i < lines.Length; i++)
            {
                string regName = "";
                var isRegStart = IsRegionStart(lines[i], parser, out regName);
                var isRegEnd = IsRegionEnd(lines[i], parser);

                var isStart = isRegStart
                    && regions.Contains(regName, StringComparer.CurrentCultureIgnoreCase)
                    && !isRecordingRegion;

                var isEnd = isRegEnd && relRegLevel == 1;

                if (isStart)
                {
                    if (inclusive)
                    {
                        resLinesIndices.Add(i);
                    }

                    if (!foundRegions.Contains(regName, StringComparer.CurrentCultureIgnoreCase))
                    {
                        foundRegions.Add(regName);
                    }

                    isRecordingRegion = true;
                    relRegLevel = 1;
                }
                else if (isRecordingRegion)
                {
                    if (!isEnd || inclusive)
                    {
                        resLinesIndices.Add(i);
                    }

                    if (isRegEnd)
                    {
                        relRegLevel--;
                    }

                    if (isEnd)
                    {
                        isRecordingRegion = false;
                    }

                    if (isRegStart)
                    {
                        relRegLevel++;
                    }
                }
            }

            var missingRegs = regions.Except(foundRegions, StringComparer.CurrentCultureIgnoreCase);

            if (missingRegs.Any())
            {
                throw new Exception($"Missing regions: {string.Join(',', missingRegs)}");
            }

            return resLinesIndices.ToArray();
        }

        private static bool IsRegionStart(string line, IRegionParser parser, out string name)
        {
            var regEx = parser.StartRegEx;
            
            if (Regex.IsMatch(line, regEx))
            {
                //TODO: replace with non-capturing groups
                name = Regex.Match(line, regEx).Groups[1].Value;
                return true;
            }
            else
            {
                name = "";
                return false;
            }
        }

        private static bool IsRegionEnd(string line, IRegionParser parser)
        {
            return Regex.IsMatch(line, parser.EndRegEx);
        }
    }
}
