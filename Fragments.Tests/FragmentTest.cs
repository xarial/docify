//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Content;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Services;
using Xarial.Docify.CLI;
using Xarial.Docify.Core.Compiler;
using Xarial.Docify.Core.Data;

namespace Fragments.Tests
{
    public static class FragmentTest
    {
        public static string GetPath(string relPath) 
        {
            const string SOLUTION_FILE_NAME = "docify.sln";

            var solDir = Path.GetDirectoryName(typeof(FragmentTest).Assembly.Location);

            while (!File.Exists(Path.Combine(solDir, SOLUTION_FILE_NAME)))
            {
                solDir = Path.GetDirectoryName(solDir);
            }

            var fragDir = Path.Combine(solDir, "Fragments");

            return Path.Combine(fragDir, relPath);
        }

        public static string Normalize(string content) 
        {
            var lines = content.Split('\n');

            return string.Join(Environment.NewLine, lines.Select(l => l.Trim()).Where(l => !string.IsNullOrEmpty(l)));
        }

        public static async Task<string> InsertIncludeToPageNormalize(string relPath, Metadata param, Site site, Page page)
        {
            var includesHandler = new DocifyEngine("", "", "", Environment_e.Test).Resove<IIncludesHandler>();

            Metadata data;
            string rawContent;
            var path = GetPath(relPath);
            new TextSourceFile(Location.FromPath(path), File.ReadAllText(path)).Parse(out rawContent, out data);

            var name = Path.GetFileNameWithoutExtension(path);

            site.Includes.Add(new Template(name, rawContent, data));

            var res = await includesHandler.Insert(name, param, site, page);

            res = Normalize(res);

            return res;
        }
    }
}
