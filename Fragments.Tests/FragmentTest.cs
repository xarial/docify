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
using Xarial.Docify.Core.Compiler.Context;
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

            var fragDir = Path.Combine(solDir, "Fragments\\Lib");

            return Path.Combine(fragDir, relPath);
        }

        public static string Normalize(string content) 
        {
            var lines = content.Split('\n');

            return string.Join(Environment.NewLine, lines.Select(l => l.Trim()).Where(l => !string.IsNullOrEmpty(l)));
        }

        public static Site NewSite(Metadata pageData = null, Configuration siteConfig = null)
        {
            var page = new Page(Location.FromPath("index.html"), "", pageData);

            var site = new Site("www.example.com", page, siteConfig);

            return site;
        }

        public static async Task<string> RenderIncludeNormalize(string relPath, Metadata param, Site site, Page page)
        {
            var includesHandler = new DocifyEngine("", "", "", Environment_e.Test).Resove<IIncludesHandler>();

            LoadInclude(relPath, site);

            var name = Path.GetFileNameWithoutExtension(relPath);

            var res = await includesHandler.Render(name, param, site, page);

            res = Normalize(res);

            return res;
        }

        public static async Task<string> TransformContentNormalize(string includeRelPath, string content, Site site, Page page)
        {
            var transformer = new DocifyEngine("", "", "", Environment_e.Test).Resove<IContentTransformer>();

            LoadInclude(includeRelPath, site);

            var res = await transformer.Transform(content, Guid.NewGuid().ToString(), new ContextModel(site, page));

            res = Normalize(res);

            return res;
        }

        private static void LoadInclude(string includeRelPath, Site site)
        {
            Metadata data;
            string rawContent;
            var path = GetPath(includeRelPath);
            new TextSourceFile(Location.FromPath(path), File.ReadAllText(path)).Parse(out rawContent, out data);

            var name = Path.GetFileNameWithoutExtension(path);

            site.Includes.Add(new Template(name, rawContent, data));
        }
    }
}
