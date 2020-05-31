//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using Components.Tests.Properties;
using NUnit.Framework;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using System.Text.RegularExpressions;

namespace Components.Tests
{
    public class SocialShareTest
    {
        private const string INCLUDE_PATH = @"social-share\_includes\social-share.cshtml";
        
        [Test]
        public async Task DefaultTest()
        {
            var site = ComponentsTest.Instance.NewSite("<div>\r\n{% social-share %}\r\n</div>", INCLUDE_PATH);

            var res = await ComponentsTest.Instance.CompileMainPageNormalize(site);

            Assert.AreEqual(Resources.social_share1, res);
        }

        [Test]
        public async Task LinkedInTest()
        {
            var site = ComponentsTest.Instance.NewSite("<div>\r\n{% social-share { linkedin: true, facebook: false, pinterest: false, reddit: false } %}\r\n</div>", INCLUDE_PATH);

            var res = await ComponentsTest.Instance.CompileMainPageNormalize(site);

            Assert.AreEqual(Resources.social_share2, res);
        }

        [Test]
        public async Task FacebookTest()
        {
            var site = ComponentsTest.Instance.NewSite("<div>\r\n{% social-share { linkedin: false, facebook: true, pinterest: false, reddit: false } %}\r\n</div>", INCLUDE_PATH);

            var res = await ComponentsTest.Instance.CompileMainPageNormalize(site);

            Assert.AreEqual(Resources.social_share3, res);
        }

        [Test]
        public async Task PinterestTest()
        {
            var site = ComponentsTest.Instance.NewSite("<div>\r\n{% social-share { linkedin: false, facebook: false, pinterest: true, reddit: false } %}\r\n</div>", INCLUDE_PATH);

            var res = await ComponentsTest.Instance.CompileMainPageNormalize(site);

            Assert.AreEqual(Resources.social_share4, res);
        }

        [Test]
        public async Task RedditTest()
        {
            var site = ComponentsTest.Instance.NewSite("<div>\r\n{% social-share { linkedin: false, facebook: false, pinterest: false, reddit: true } %}\r\n</div>", INCLUDE_PATH);

            var res = await ComponentsTest.Instance.CompileMainPageNormalize(site);

            Assert.AreEqual(Resources.social_share5, res);
        }

        [Test]
        public async Task ColorTest()
        {
            var site = ComponentsTest.Instance.NewSite("<div>\r\n{% social-share { color: green } %}\r\n</div>", INCLUDE_PATH);

            var res = await ComponentsTest.Instance.CompileMainPageNormalize(site);

            Assert.AreEqual(Resources.social_share6, res);
        }
    }
}
