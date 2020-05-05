//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using NUnit.Framework;
using Xarial.Docify.Lib.Plugins;

namespace Plugins.Tests
{
    public class CodeSnippetHelperTest
    {
        [Test]
        public void SelectSingleRegion()
        {
            var res = CodeSnippetHelper.Select("line1\r\nline2\r\n    //--- Reg1\r\nline3\r\nline4\r\n    //---\r\nline5", "cs", new CodeSelectorOptions() 
            {
                Regions = new string[] { "Reg1" }
            });

            Assert.AreEqual(1, res.Length);
            Assert.AreEqual("line3\r\nline4", res[0].Code);
        }

        [Test]
        public void SelectMultiRegions()
        {
            var res = CodeSnippetHelper.Select("line1\r\nline2\r\n    //--- Reg1\r\nline3\r\nline4\r\n    //---\r\nline5\r\n    //--- Reg2\r\nline6\r\nline7\r\n    //---\r\nline8", "cs", new CodeSelectorOptions()
            {
                Regions = new string[] { "Reg1", "Reg2" }
            });

            Assert.AreEqual(2, res.Length);
            Assert.AreEqual("line3\r\nline4", res[0].Code);
            Assert.AreEqual("line6\r\nline7", res[1].Code);
        }

        [Test]
        public void ExcludeSingleRegions()
        {
            var res = CodeSnippetHelper.Select("line1\r\nline2\r\n    //--- Reg1\r\nline3\r\nline4\r\n    //---\r\nline5\r\n    //--- Reg2\r\nline6\r\nline7\r\n    //---\r\nline8", "cs", new CodeSelectorOptions()
            {
                ExcludeRegions = new string[] { "Reg1" }
            });

            Assert.AreEqual(2, res.Length);
            Assert.AreEqual("line1\r\nline2", res[0].Code);
            Assert.AreEqual("line5\r\nline6\r\nline7\r\nline8", res[1].Code);
        }

        [Test]
        public void ExcludeMultiRegions()
        {
            var res = CodeSnippetHelper.Select("line1\r\nline2\r\n    //--- Reg1\r\nline3\r\nline4\r\n    //---\r\nline5\r\n    //--- Reg2\r\nline6\r\nline7\r\n    //---\r\nline8", "cs", new CodeSelectorOptions()
            {
                ExcludeRegions = new string[] { "Reg1", "Reg2" }
            });

            Assert.AreEqual(3, res.Length);
            Assert.AreEqual("line1\r\nline2", res[0].Code);
            Assert.AreEqual("line5", res[1].Code);
            Assert.AreEqual("line8", res[2].Code);
        }

        [Test]
        public void HideRegions() 
        {
            var res = CodeSnippetHelper.Select("line1\r\nline2\r\n    //--- Reg1\r\nline3\r\nline4\r\n    //---\r\nline5", "cs",
                new CodeSelectorOptions());

            Assert.AreEqual(1, res.Length);
            Assert.AreEqual("line1\r\nline2\r\nline3\r\nline4\r\nline5", res[0].Code);
        }

        [Test]
        public void LeftAlignCode()
        {
            var res = CodeSnippetHelper.Select("line1\r\nline2\r\n    //--- Reg1\r\n    line3\r\n    line4\r\n    //---\r\nline5", "cs", new CodeSelectorOptions()
            {
                LeftAlign = true,
                Regions = new string[] { "Reg1" }
            });

            Assert.AreEqual(1, res.Length);
            Assert.AreEqual("line3\r\nline4", res[0].Code);
        }

        [Test]
        public void SelectSingleRegionVB()
        {
            var res = CodeSnippetHelper.Select("line1\r\nline2\r\n    '--- Reg1\r\n    line3\r\n    line4\r\n    '---\r\nline5", "vb", new CodeSelectorOptions()
            {
                Regions = new string[] { "Reg1" }
            });

            Assert.AreEqual(1, res.Length);
            Assert.AreEqual("    line3\r\n    line4", res[0].Code);
        }

        [Test]
        public void SelectMultiRegionsNested()
        {
            var res = CodeSnippetHelper.Select("line1\r\nline2\r\n    //--- Reg1\r\nline3\r\nline4\r\n    //---\r\nline5\r\n//--- Reg2\r\nl1\r\n    //--- Reg3\r\nline6\r\nline7\r\n    //---\r\n//---\r\nline8", "cs", new CodeSelectorOptions()
            {
                Regions = new string[] { "Reg1", "Reg3" }
            });

            Assert.AreEqual(2, res.Length);
            Assert.AreEqual("line3\r\nline4", res[0].Code);
            Assert.AreEqual("line6\r\nline7", res[1].Code);
        }

        [Test]
        public void SelectMultiRegionsAndExclude()
        {
            var res = CodeSnippetHelper.Select("line1\r\nline2\r\n    //--- Reg1\r\nline3\r\nline4\r\n    //---\r\nline5\r\n//--- Reg2\r\nl1\r\n    //--- Reg3\r\nline6\r\nline7\r\n    //---\r\n//---\r\nline8", "cs", new CodeSelectorOptions()
            {
                Regions = new string[] { "Reg2" },
                ExcludeRegions = new string[] { "Reg3" }
            });

            Assert.AreEqual(1, res.Length);
            Assert.AreEqual("l1", res[0].Code);
        }

        [Test]
        public void SelectSingleOnlyRegion()
        {
            var res = CodeSnippetHelper.Select("//--- Reg1\r\nline3\r\nline4\r\n    //---", "cs", new CodeSelectorOptions()
            {
                Regions = new string[] { "Reg1" }
            });

            Assert.AreEqual(1, res.Length);
            Assert.AreEqual("line3\r\nline4", res[0].Code);
        }

        [Test]
        public void ExcludeSingleOnlyRegion()
        {
            var res = CodeSnippetHelper.Select("//--- Reg1\r\nline3\r\nline4\r\n    //---", "cs", new CodeSelectorOptions()
            {
                ExcludeRegions = new string[] { "Reg1" }
            });

            Assert.AreEqual(0, res.Length);
        }

        [Test]
        public void FullSnippetInfoTest() 
        {
            var res1 = CodeSnippetHelper.Select("line1\r\nline2", "cs", new CodeSelectorOptions());
            var res2 = CodeSnippetHelper.Select("//---reg1\r\nline1\r\nline2\r\n//---", "cs", new CodeSelectorOptions());

            Assert.AreEqual(SnippetInfo_e.Full, res1[0].Info);
            Assert.AreEqual(SnippetInfo_e.Full, res2[0].Info);
        }

        [Test]
        public void TopJaggedSnippetInfoTest() 
        {
            var res1 = CodeSnippetHelper.Select("//---reg1\r\nline1\r\nline2\r\n//---\r\nline3", "cs",
                new CodeSelectorOptions()
                {
                    ExcludeRegions = new string[] { "reg1" }
                });

            var res2 = CodeSnippetHelper.Select("line1\r\n//---reg1\r\nline2\r\nline3\r\n//---", "cs", 
                new CodeSelectorOptions()
                {
                    Regions = new string[] { "reg1" }
                });

            Assert.AreEqual(SnippetInfo_e.BottomBoundary, res1[0].Info);
            Assert.AreEqual(SnippetInfo_e.BottomBoundary, res2[0].Info);
        }

        [Test]
        public void BottomJaggedSnippetInfoTest()
        {
            var res1 = CodeSnippetHelper.Select("//---reg1\r\nline1\r\nline2\r\n//---\r\nline3", "cs",
                new CodeSelectorOptions()
                {
                    Regions = new string[] { "reg1" }
                });

            var res2 = CodeSnippetHelper.Select("line1\r\n//---reg1\r\nline2\r\nline3\r\n//---", "cs",
                new CodeSelectorOptions()
                {
                    ExcludeRegions = new string[] { "reg1" }
                });

            Assert.AreEqual(SnippetInfo_e.TopBoundary, res1[0].Info);
            Assert.AreEqual(SnippetInfo_e.TopBoundary, res2[0].Info);
        }

        [Test]
        public void JaggedSnippetInfoTest()
        {
            var res1 = CodeSnippetHelper.Select("line1\r\n//---reg1\r\nline2\r\nline3\r\n//---\r\nline4", "cs",
                new CodeSelectorOptions()
                {
                    Regions = new string[] { "reg1" }
                });

            var res2 = CodeSnippetHelper.Select("//---reg1\r\nline1\r\nline2\r\n//---\r\nline3\r\n//---reg2\r\nline4\r\nline5\r\n//---", "cs",
                new CodeSelectorOptions()
                {
                    ExcludeRegions = new string[] { "reg1", "reg2" }
                });

            Assert.AreEqual(SnippetInfo_e.Jagged, res1[0].Info);
            Assert.AreEqual(SnippetInfo_e.Jagged, res2[0].Info);
        }

        [Test]
        public void MultipleJaggedSnippetInfoTest()
        {
            var res1 = CodeSnippetHelper.Select("line1\r\n//---reg1\r\nline1\r\nline2\r\n//---\r\nline3\r\n//---reg2\r\nline4\r\nline5\r\n//---", "cs",
                new CodeSelectorOptions()
                {
                    Regions = new string[] { "reg1", "reg2" }
                });

            var res2 = CodeSnippetHelper.Select("line1\r\n//---reg1\r\nline1\r\nline2\r\n//---\r\nline3\r\n//---reg2\r\nline4\r\nline5\r\n//---\r\nline6", "cs",
                new CodeSelectorOptions()
                {
                    ExcludeRegions = new string[] { "reg2" }
                });

            Assert.AreEqual(SnippetInfo_e.Jagged, res1[0].Info);
            Assert.AreEqual(SnippetInfo_e.BottomBoundary, res1[1].Info);
            Assert.AreEqual(SnippetInfo_e.TopBoundary, res2[0].Info);
            Assert.AreEqual(SnippetInfo_e.BottomBoundary, res2[1].Info);
        }

        [Test]
        public void FullRegionsJaggedTest() 
        {
            var res1 = CodeSnippetHelper.Select("//---reg1\r\nline1\r\nline2\r\n//---", "cs",
                new CodeSelectorOptions()
                {
                    Regions = new string[] { "reg1" }
                });

            Assert.AreEqual(SnippetInfo_e.Full, res1[0].Info);
        }

        [Test]
        public void BoundaryBottomNestedRegionsJaggedTest()
        {
            var res1 = CodeSnippetHelper.Select("line1\r\n//---reg1\r\nline1\r\n//---reg2\r\nline2\r\n//---\r\n//---", "cs",
                new CodeSelectorOptions()
                {
                    Regions = new string[] { "reg2" }
                });

            Assert.AreEqual(SnippetInfo_e.BottomBoundary, res1[0].Info);
        }

        [Test]
        public void BoundaryTopNestedRegionsJaggedTest()
        {
            var res1 = CodeSnippetHelper.Select("//---reg1\r\n//---reg2\r\nline2\r\n//---\r\nline1\r\n//---\r\nline3", "cs",
                new CodeSelectorOptions()
                {
                    Regions = new string[] { "reg2" }
                });

            Assert.AreEqual(SnippetInfo_e.TopBoundary, res1[0].Info);
        }

        [Test]
        public void IncludeExcludeRegionsJaggedTest()
        {
            var res1 = CodeSnippetHelper.Select("line1\r\n//---reg1\r\n//---reg2\r\nline2\r\n//---\r\nline1\r\n//---\r\nline3", "cs",
                new CodeSelectorOptions()
                {
                    Regions = new string[] { "reg1" },
                    ExcludeRegions = new string[] { "reg2" }
                });

            Assert.AreEqual(SnippetInfo_e.Jagged, res1[0].Info);
        }
    }
}