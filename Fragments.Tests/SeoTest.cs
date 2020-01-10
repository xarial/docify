//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Content;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Core.Compiler;
using Xarial.Docify.Core.Data;

namespace Fragments.Tests
{
    public class SeoTest
    {
        [Test]
        public async Task DefaultParamsTest() 
        {
            IncludesHandler includesHandler = null;
            new MarkdigRazorLightTransformer(t => includesHandler = new IncludesHandler(t));

            var p1 = new Page(Location.FromPath("index.html"), "");
            var p2 = new Page(Location.FromPath("p2\\index.html"), "");
            p1.SubPages.Add(p2);

            var site = new Site("www.example.com", null, new Configuration()
            { { "title", "t1" }, { "description", "d1" } });

            Metadata data;
            string rawContent;
            var path = @"D:\Projects\Xarial\open-source\docify\Fragments\seo\_includes\seo.cshtml";
            new TextSourceFile(Location.FromPath(path), File.ReadAllText(path)).Parse(out rawContent, out data);
            
            site.Includes.Add(new Template("seo", rawContent, data));

            var res = await includesHandler.Insert("seo", null, site, p2);
        }
    }
}
