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
using Xarial.Docify.Core.Data;
using Xarial.Docify.Core.Helpers;

namespace Tests.Common
{
    public class LibTestsHelper
    {
        private string m_Name;

        public LibTestsHelper(string name) 
        {
            m_Name = name;
        }

        public string GetPath(string relPath)
        {
            const string SOLUTION_FILE_NAME = "docify.sln";

            var solDir = Path.GetDirectoryName(typeof(LibTestsHelper).Assembly.Location);

            while (!System.IO.File.Exists(Path.Combine(solDir, SOLUTION_FILE_NAME)))
            {
                solDir = Path.GetDirectoryName(solDir);
            }

            var compDir = Path.Combine(solDir, $"lib\\{m_Name}\\Lib");

            return Path.Combine(compDir, relPath);
        }

        public T GetData<T>(string paramStr)
            where T : Metadata
        {
            var yamlDeserializer = new MetadataSerializer();

            return yamlDeserializer.Deserialize<T>(paramStr);
        }

        public string Normalize(string content)
        {
            var lines = content.Split('\n');

            return string.Join(Environment.NewLine, lines.Select(l => l.Trim()).Where(l => !string.IsNullOrEmpty(l)));
        }

        public Site NewSite(string pageContent, string includePath, Metadata pageData = null, Configuration siteConfig = null)
        {
            var page = new PageMock("index", pageContent, pageData);
            var site = new Site("www.example.com", page, siteConfig);

            LoadInclude(includePath, site);

            return site;
        }

        public async Task<string> CompileMainPageNormalize(Site site)
        {
            var compiler = new DocifyEngineMock().Resove<ICompiler>();

            var files = await compiler.Compile(site).ToListAsync();

            var res = files.First(f => f.Location.ToId() == "index.html").AsTextContent();

            res = Normalize(res);

            return res;
        }

        private void LoadInclude(string includeRelPath, Site site)
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
