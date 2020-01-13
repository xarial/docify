//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using NUnit.Framework;
using Xarial.Docify.Lib.Tools;

namespace Tools.Tests
{
    public class CodeSnippetHelperTest
    {
        [Test]
        public void SelectSingleRegion()
        {
            var res = CodeSnippetHelper.Select("line1\r\nline2\r\n    #region Reg1\r\nline3\r\nline4\r\n    #endregion\r\nline5", "cs", new CodeSelectorOptions() 
            {
                Regions = new string[] { "Reg1" }
            });

            Assert.AreEqual(1, res.Length);
            Assert.AreEqual("line3\r\nline4", res[0]);
        }

        [Test]
        public void SelectMultiRegions()
        {
            var res = CodeSnippetHelper.Select("line1\r\nline2\r\n    #region Reg1\r\nline3\r\nline4\r\n    #endregion\r\nline5\r\n    #region Reg2\r\nline6\r\nline7\r\n    #endregion\r\nline8", "cs", new CodeSelectorOptions()
            {
                Regions = new string[] { "Reg1", "Reg2" }
            });

            Assert.AreEqual(2, res.Length);
            Assert.AreEqual("line3\r\nline4", res[0]);
            Assert.AreEqual("line6\r\nline7", res[1]);
        }

        [Test]
        public void ExcludeSingleRegions()
        {
            var res = CodeSnippetHelper.Select("line1\r\nline2\r\n    #region Reg1\r\nline3\r\nline4\r\n    #endregion\r\nline5\r\n    #region Reg2\r\nline6\r\nline7\r\n    #endregion\r\nline8", "cs", new CodeSelectorOptions()
            {
                ExcludeRegions = new string[] { "Reg1" }
            });

            Assert.AreEqual(2, res.Length);
            Assert.AreEqual("line1\r\nline2", res[0]);
            Assert.AreEqual("line5\r\n    #region Reg2\r\nline6\r\nline7\r\n    #endregion\r\nline8", res[1]);
        }

        [Test]
        public void ExcludeMultiRegions()
        {
            var res = CodeSnippetHelper.Select("line1\r\nline2\r\n    #region Reg1\r\nline3\r\nline4\r\n    #endregion\r\nline5\r\n    #region Reg2\r\nline6\r\nline7\r\n    #endregion\r\nline8", "cs", new CodeSelectorOptions()
            {
                ExcludeRegions = new string[] { "Reg1", "Reg2" }
            });

            Assert.AreEqual(3, res.Length);
            Assert.AreEqual("line1\r\nline2", res[0]);
            Assert.AreEqual("line5", res[1]);
            Assert.AreEqual("line8", res[2]);
        }

        [Test]
        public void HideRegions() 
        {
            var res = CodeSnippetHelper.Select("line1\r\nline2\r\n    #region Reg1\r\nline3\r\nline4\r\n    #endregion\r\nline5", "cs", new CodeSelectorOptions()
            {
                HideRegions = true
            });

            Assert.AreEqual(1, res.Length);
            Assert.AreEqual("line1\r\nline2\r\nline3\r\nline4\r\nline5", res[0]);
        }

        [Test]
        public void LeftAlignCode()
        {
            var res = CodeSnippetHelper.Select("line1\r\nline2\r\n    #region Reg1\r\n    line3\r\n    line4\r\n    #endregion\r\nline5", "cs", new CodeSelectorOptions()
            {
                LeftAlign = true,
                Regions = new string[] { "Reg1" }
            });

            Assert.AreEqual(1, res.Length);
            Assert.AreEqual("line3\r\nline4", res[0]);
        }

        [Test]
        public void SelectSingleRegionVB()
        {
            var res = CodeSnippetHelper.Select("line1\r\nline2\r\n    #Region \"Reg1\"\r\n    line3\r\n    line4\r\n    #End Region\r\nline5", "vb", new CodeSelectorOptions()
            {
                Regions = new string[] { "Reg1" }
            });

            Assert.AreEqual(1, res.Length);
            Assert.AreEqual("    line3\r\n    line4", res[0]);
        }

        [Test]
        public void SelectMultiRegionsNested()
        {
            var res = CodeSnippetHelper.Select("line1\r\nline2\r\n    #region Reg1\r\nline3\r\nline4\r\n    #endregion\r\nline5\r\n#region Reg2\r\nl1\r\n    #region Reg3\r\nline6\r\nline7\r\n    #endregion\r\n#endregion\r\nline8", "cs", new CodeSelectorOptions()
            {
                Regions = new string[] { "Reg1", "Reg3" }
            });

            Assert.AreEqual(2, res.Length);
            Assert.AreEqual("line3\r\nline4", res[0]);
            Assert.AreEqual("line6\r\nline7", res[1]);
        }

        [Test]
        public void SelectMultiRegionsAndExclude()
        {
            var res = CodeSnippetHelper.Select("line1\r\nline2\r\n    #region Reg1\r\nline3\r\nline4\r\n    #endregion\r\nline5\r\n#region Reg2\r\nl1\r\n    #region Reg3\r\nline6\r\nline7\r\n    #endregion\r\n#endregion\r\nline8", "cs", new CodeSelectorOptions()
            {
                Regions = new string[] { "Reg2" },
                ExcludeRegions = new string[] { "Reg3" }
            });

            Assert.AreEqual(1, res.Length);
            Assert.AreEqual("l1", res[0]);
        }

        [Test]
        public void SelectSingleOnlyRegion()
        {
            var res = CodeSnippetHelper.Select("#region Reg1\r\nline3\r\nline4\r\n    #endregion", "cs", new CodeSelectorOptions()
            {
                Regions = new string[] { "Reg1" }
            });

            Assert.AreEqual(1, res.Length);
            Assert.AreEqual("line3\r\nline4", res[0]);
        }

        [Test]
        public void ExcludeSingleOnlyRegion()
        {
            var res = CodeSnippetHelper.Select("#region Reg1\r\nline3\r\nline4\r\n    #endregion", "cs", new CodeSelectorOptions()
            {
                ExcludeRegions = new string[] { "Reg1" }
            });

            Assert.AreEqual(0, res.Length);
        }
    }
}