//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

//#define ENABLE_PERFORMANCE_USER_TEST

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Content;
using Xarial.Docify.Base.Services;
using Xarial.Docify.CLI;
using Xarial.Docify.Core;
using Xarial.Docify.Core.Compiler;

namespace Core.Tests
{
    //not real unit test, but supervised user test
    public class _MarkdownRazorCompilerPerformanceTest
    {
        private ICompiler m_Compiler;

        [SetUp]
        public void Setup()
        {
            m_Compiler = new DocifyEngine("", "", "").Resove<ICompiler>();
        }

#if ENABLE_PERFORMANCE_USER_TEST
        [Test]
        public async Task Compile_PerformanceAutoPartitionsTest()
        {
            await CompileMeasurePerformance((int)BaseCompilerConfig.ParallelPartitions_e.AutoDetect, true);
        }

        [Test]
        public async Task Compile_PerformanceNoParallelismTest()
        {
            await CompileMeasurePerformance(1, true);
        }

        [Test]
        public async Task Compile_PerformanceInfinitePartitionsTest()
        {
            await CompileMeasurePerformance((int)BaseCompilerConfig.ParallelPartitions_e.Infinite, true);
        }
#endif

        public async Task CompileMeasurePerformance(int partCount, bool useLayout)
        {
            const string CONTENT_TEMPLATE = "<div>@Model.Site.MainPage.SubPages.Count <a href=\"@Model.Page.Location.FileName\">{0}</a></div>\r\n\r\n*{0}*\r\n\r\n**{0}**";
            
            const int NUMBER_OF_PAGES = 200;

            Template layout = null;

            if (useLayout) 
            {
                layout = new Template("t1", "<div> @Model.Page.Location.FileName </div>\r\n\r\n**Some Text**\r\n\r\n{{ content }}");
            }

            var rootPage = new Page(new Location("page.html"),
                string.Format(CONTENT_TEMPLATE, Guid.NewGuid()), layout);

            for (int i = 0; i < NUMBER_OF_PAGES; i++)
            {
                rootPage.SubPages.Add(new Page(new Location($"page{i}.html"),
                    string.Format(CONTENT_TEMPLATE, Guid.NewGuid()), layout));
            }

            var site = new Site("", rootPage);

            var config = new BaseCompilerConfig();
            config.ParallelPartitionsCount = partCount;
            
            var start = DateTime.Now;
            await m_Compiler.Compile(site);
            var elapsed = DateTime.Now - start;

            TestContext.WriteLine($"Compilation time of {NUMBER_OF_PAGES} with {partCount} partitions is {elapsed.TotalSeconds} seconds");
        }
    }
}
