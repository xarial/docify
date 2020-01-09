//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

//#define ENABLE_PERFORMANCE_USER_TEST

using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Core;
using Xarial.Docify.Core.Base;

namespace Core.Tests
{
    //not real unit test, but supervised user test
    public class _MarkdownRazorCompilerPerformanceTest
    {

#if ENABLE_PERFORMANCE_USER_TEST
        [Test]
        public async Task Compile_PerformanceAutoPartitionsTest()
        {
            await CompileMeasurePerformance((int)MarkdownRazorCompilerConfig.ParallelPartitions_e.AutoDetect, true);
        }

        [Test]
        public async Task Compile_PerformanceNoParallelismTest()
        {
            await CompileMeasurePerformance(1, true);
        }

        [Test]
        public async Task Compile_PerformanceInfinitePartitionsTest()
        {
            await CompileMeasurePerformance((int)MarkdownRazorCompilerConfig.ParallelPartitions_e.Infinite, true);
        }
#endif

        public async Task CompileMeasurePerformance(int partCount, bool useLayout)
        {
            const string CONTENT_TEMPLATE = "<div>@Model.Site.MainPage.Children.Count <a href=\"@Model.Page.Location.FileName\">{0}</a></div>\r\n\r\n*{0}*\r\n\r\n**{0}**";
            
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
                rootPage.Children.Add(new Page(new Location($"page{i}.html"),
                    string.Format(CONTENT_TEMPLATE, Guid.NewGuid()), layout));
            }

            var site = new Site("", rootPage);

            var config = new BaseCompilerConfig("");
            config.ParallelPartitionsCount = partCount;
            var comp = new BaseCompiler(config, new Mock<ILogger>().Object, null, 
                new LayoutParser(), new CompositionTransformer(new RazorLightEvaluator(), new MarkdigMarkdownParser()));

            var start = DateTime.Now;
            await comp.Compile(site);
            var elapsed = DateTime.Now - start;

            TestContext.WriteLine($"Compilation time of {NUMBER_OF_PAGES} with {partCount} partitions is {elapsed.TotalSeconds} seconds");
        }
    }
}
