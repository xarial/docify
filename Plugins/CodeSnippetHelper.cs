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

namespace Xarial.Docify.Lib.Plugins
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
        private const string REGION_BLOCK = "---";
        
        private static string GetCommentLineSymbol(string codeLang) 
        {
            switch (codeLang.ToLower()) 
            {
                case "cs":
                case "csharp":
                case "c#":
                case "js":
                case "javascript":
                case "java":
                    return "//";

                case "vb":
                case "vbnet":
                case "vb.net":
                case "vba":
                    return "'";
                    
                default:
                    return "";
            }
        }
        
        public static string[] Select(string rawCode, string codeLang, CodeSelectorOptions opts)
        {
            var srcLines = Regex.Split(rawCode, "\r\n|\r|\n");

            var result = new List<string[]>();
            result.Add(srcLines);

            var commentSymbol = GetCommentLineSymbol(codeLang);

            void ProcessRegions(string[] regs, bool inner) 
            {
                if (regs?.Any() == true)
                {
                    var newResult = new List<string[]>();

                    for (int i = 0; i < result.Count; i++)
                    {
                        var lineGroups = SelectRegionLines(result[i], commentSymbol, regs, inner);
                        newResult.AddRange(lineGroups);
                    }

                    result = newResult;
                }
            }

            ProcessRegions(opts.Regions, true);
            ProcessRegions(opts.ExcludeRegions, false);
            
            if (opts.HideRegions)
            {
                for (int i = 0; i < result.Count; i++)
                {
                    var lines = result[i];
                    lines = result[i].Where(l =>
                    {
                        string regName;
                        return !(IsRegionEnd(l, commentSymbol) || IsRegionStart(l, commentSymbol, out regName));
                     }).ToArray();

                    if (result[i].Length != lines.Length)
                    {
                        result[i] = lines;
                    }
                }
            }

            if (opts.LeftAlign)
            {
                for (int i = 0; i < result.Count; i++)
                {
                    var lines = result[i];
                    var indent = lines.Min(l => string.IsNullOrEmpty(l) ? int.MaxValue : l.TakeWhile(Char.IsWhiteSpace).Count());

                    if (indent > 0)
                    {
                        lines = lines.Select(l => string.IsNullOrEmpty(l) ? l : l.Substring(indent)).ToArray();
                        result[i] = lines;
                    }
                }
            }

            return result.Select(lines => string.Join(Environment.NewLine, lines)).ToArray();
        }

        private static IEnumerable<string[]> SelectRegionLines(string[] lines, string commentSymbol, 
            string[] regions, bool inner)
        {
            var result = new List<string[]>();
            
            var curGroup = new List<string>();

            var foundRegions = new List<string>();

            bool isRecordingRegion = false;
            int relRegLevel = 0;

            void FlushCurrentGroup() 
            {
                if (curGroup.Any())
                {
                    result.Add(curGroup.ToArray());
                    curGroup.Clear();
                }
            }

            for (int i = 0; i < lines.Length; i++)
            {
                string regName = "";
                var isRegStart = IsRegionStart(lines[i], commentSymbol, out regName);
                var isRegEnd = IsRegionEnd(lines[i], commentSymbol);

                var isStart = isRegStart
                    && regions.Contains(regName, StringComparer.CurrentCultureIgnoreCase)
                    && !isRecordingRegion;

                var isEnd = isRegEnd && relRegLevel == 1;

                if (isStart)
                {
                    if (!inner)
                    {
                        FlushCurrentGroup();
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
                    if (!isEnd)
                    {   
                        if (inner)
                        {
                            curGroup.Add(lines[i]);
                        }
                    }

                    if (isRegEnd)
                    {
                        relRegLevel--;
                    }

                    if (isEnd)
                    {
                        isRecordingRegion = false;
                        FlushCurrentGroup();
                    }

                    if (isRegStart)
                    {
                        relRegLevel++;
                    }
                }
                else if(!inner)
                {
                    curGroup.Add(lines[i]);
                }
            }

            if (!inner) 
            {
                FlushCurrentGroup();
            }

            var missingRegs = regions.Except(foundRegions, StringComparer.CurrentCultureIgnoreCase);

            if (missingRegs.Any())
            {
                throw new Exception($"Missing regions: {string.Join(',', missingRegs)}");
            }

            return result;
        }

        private static bool IsRegionStart(string line, string commentSymbol, out string name)
        {
            line = line.Trim();
            var regBlock = commentSymbol + REGION_BLOCK;

            if (line.StartsWith(regBlock)) 
            {
                name = line.Substring(regBlock.Length).Trim();
                return !string.IsNullOrEmpty(name);
            }
            else
            {
                name = "";
                return false;
            }
        }

        private static bool IsRegionEnd(string line, string commentSymbol)
        {
            line = line.Trim();
            var regBlock = commentSymbol + REGION_BLOCK;
            return line == regBlock;
        }
    }
}
