using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Tests.Common.Mocks;
using Xarial.Docify.Core.Data;
using Xarial.Docify.Base;

namespace Core.Tests
{
    public class SiteTest
    {
        [Test]
        public void GetFullUrl_NoHostNoBaseTest() 
        {
            var site = new Site("", "", new PageMock("p1", ""), null);

            var u1 = site.GetFullUrl("");
            var u2 = site.GetFullUrl("/");
            var u3 = site.GetFullUrl("/abc");
            var u4 = site.GetFullUrl("/abc/xyz");
            var u5 = site.GetFullUrl("abc");

            Assert.AreEqual("", u1);
            Assert.AreEqual("/", u2);
            Assert.AreEqual("/abc", u3);
            Assert.AreEqual("/abc/xyz", u4);
            Assert.AreEqual("/abc", u5);
        }

        [Test]
        public void GetFullUrl_HostNoBaseTest()
        {
            var site = new Site("https://www.example.com", "", new PageMock("p1", ""), null);

            var u1 = site.GetFullUrl("");
            var u2 = site.GetFullUrl("/");
            var u3 = site.GetFullUrl("/abc");
            var u4 = site.GetFullUrl("/abc/xyz");
            var u5 = site.GetFullUrl("abc");

            Assert.AreEqual("https://www.example.com", u1);
            Assert.AreEqual("https://www.example.com", u2);
            Assert.AreEqual("https://www.example.com/abc", u3);
            Assert.AreEqual("https://www.example.com/abc/xyz", u4);
            Assert.AreEqual("https://www.example.com/abc", u5);
        }

        [Test]
        public void GetFullUrl_HostBaseTest()
        {
            var site = new Site("https://www.example.com", "root", new PageMock("p1", ""), null);

            var u1 = site.GetFullUrl("");
            var u2 = site.GetFullUrl("/");
            var u3 = site.GetFullUrl("/abc");
            var u4 = site.GetFullUrl("/abc/xyz");
            var u5 = site.GetFullUrl("abc");

            Assert.AreEqual("https://www.example.com/root", u1);
            Assert.AreEqual("https://www.example.com/root", u2);
            Assert.AreEqual("https://www.example.com/root/abc", u3);
            Assert.AreEqual("https://www.example.com/root/abc/xyz", u4);
            Assert.AreEqual("https://www.example.com/root/abc", u5);
        }

        [Test]
        public void GetFullUrl_HostBase1Test()
        {
            var site = new Site("https://www.example.com", "/root", new PageMock("p1", ""), null);

            var u1 = site.GetFullUrl("");
            var u2 = site.GetFullUrl("/");
            var u3 = site.GetFullUrl("/abc");
            var u4 = site.GetFullUrl("/abc/xyz");
            var u5 = site.GetFullUrl("abc");

            Assert.AreEqual("https://www.example.com/root", u1);
            Assert.AreEqual("https://www.example.com/root", u2);
            Assert.AreEqual("https://www.example.com/root/abc", u3);
            Assert.AreEqual("https://www.example.com/root/abc/xyz", u4);
            Assert.AreEqual("https://www.example.com/root/abc", u5);
        }

        [Test]
        public void GetFullUrl_HostBase2Test()
        {
            var site = new Site("https://www.example.com", "/root/", new PageMock("p1", ""), null);

            var u1 = site.GetFullUrl("");
            var u2 = site.GetFullUrl("/");
            var u3 = site.GetFullUrl("/abc");
            var u4 = site.GetFullUrl("/abc/xyz");
            var u5 = site.GetFullUrl("abc");

            Assert.AreEqual("https://www.example.com/root", u1);
            Assert.AreEqual("https://www.example.com/root", u2);
            Assert.AreEqual("https://www.example.com/root/abc", u3);
            Assert.AreEqual("https://www.example.com/root/abc/xyz", u4);
            Assert.AreEqual("https://www.example.com/root/abc", u5);
        }
    }
}
