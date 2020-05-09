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
using Tests.Common.Mocks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Services;
using Xarial.Docify.CLI;
using Xarial.Docify.Core;
using Xarial.Docify.Core.Compiler;
using Xarial.Docify.Core.Compiler.Context;
using Xarial.Docify.Core.Data;
using Xarial.Docify.Core.Helpers;
using YamlDotNet.Serialization;

namespace Components.Tests
{
    public static class ComponentsTest
    {
        public static string GetPath(string relPath) 
        {
            const string SOLUTION_FILE_NAME = "docify.sln";

            var solDir = Path.GetDirectoryName(typeof(ComponentsTest).Assembly.Location);

            while (!System.IO.File.Exists(Path.Combine(solDir, SOLUTION_FILE_NAME)))
            {
                solDir = Path.GetDirectoryName(solDir);
            }

            var compDir = Path.Combine(solDir, "Components\\Lib");

            return Path.Combine(compDir, relPath);
        }

        public static T GetData<T>(string paramStr)
            where T : Metadata
        {
            var yamlDeserializer = new MetadataSerializer();

            return yamlDeserializer.Deserialize<T>(paramStr);
        }

        public static string Normalize(string content) 
        {
            var lines = content.Split('\n');

            return string.Join(Environment.NewLine, lines.Select(l => l.Trim()).Where(l => !string.IsNullOrEmpty(l)));
        }

        public static Site NewSite(string pageContent, string includePath, Metadata pageData = null, Configuration siteConfig = null)
        {
            var page = new PageMock("index", pageContent, pageData);
            var site = new Site("www.example.com", page, siteConfig);

            LoadInclude(includePath, site);

            return site;
        }

        public static async Task<string> CompileMainPageNormalize(Site site)
        {
            var compiler = new DocifyEngine("", "", "", Environment_e.Test).Resove<ICompiler>();

            var files = await compiler.Compile(site).ToListAsync();

            var res = files.First(f => f.Location.ToId() == "index.html").AsTextContent();

            res = Normalize(res);

            return res;
        }

        private static void LoadInclude(string includeRelPath, Site site)
        {
            IMetadata data;
            string rawContent;
            var path = GetPath(includeRelPath);

            FrontMatterParser.Parse(System.IO.File.ReadAllText(path), out rawContent, out data);

            var name = Path.GetFileNameWithoutExtension(path);

            site.Includes.Add(new TemplateMock(name, rawContent, data));
        }
    }
}
