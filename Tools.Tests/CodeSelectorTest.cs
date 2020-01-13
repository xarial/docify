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
    public class CodeSelectorTest
    {
        [Test]
        public void SelectSingleRegion()
        {
            var res = CodeSnippetHelper.Select("line1\r\nline2\r\n    #region Reg1\r\nline3\r\nline4\r\n    #endregion\r\nline5", "cs", new CodeSelectorOptions() 
            {
                Regions = new string[] { "Reg1" }
            });
        }

        [Test]
        public void SelectMultiRegions()
        {
            var res = CodeSnippetHelper.Select("line1\r\nline2\r\n    #region Reg1\r\nline3\r\nline4\r\n    #endregion\r\nline5\r\n    #region Reg2\r\nline6\r\nline7\r\n    #endregion\r\nline8", "cs", new CodeSelectorOptions()
            {
                Regions = new string[] { "Reg1", "Reg2" }
            });
        }

        [Test]
        public void ExcludeSingleRegions()
        {
            var res = CodeSnippetHelper.Select("line1\r\nline2\r\n    #region Reg1\r\nline3\r\nline4\r\n    #endregion\r\nline5\r\n    #region Reg2\r\nline6\r\nline7\r\n    #endregion\r\nline8", "cs", new CodeSelectorOptions()
            {
                ExcludeRegions = new string[] { "Reg1" }
            });
        }

        [Test]
        public void ExcludeMultiRegions()
        {
            var res = CodeSnippetHelper.Select("line1\r\nline2\r\n    #region Reg1\r\nline3\r\nline4\r\n    #endregion\r\nline5\r\n    #region Reg2\r\nline6\r\nline7\r\n    #endregion\r\nline8", "cs", new CodeSelectorOptions()
            {
                ExcludeRegions = new string[] { "Reg1", "Reg2" }
            });
        }

        [Test]
        public void HideRegions() 
        {
            var res = CodeSnippetHelper.Select("line1\r\nline2\r\n    #region Reg1\r\nline3\r\nline4\r\n    #endregion\r\nline5", "cs", new CodeSelectorOptions()
            {
                HideRegions = true
            });
        }

        [Test]
        public void LeftAlignCode()
        {
            var res = CodeSnippetHelper.Select("line1\r\nline2\r\n    #region Reg1\r\n    line3\r\n    line4\r\n    #endregion\r\nline5", "cs", new CodeSelectorOptions()
            {
                LeftAlign = true,
                Regions = new string[] { "Reg1" }
            });
        }

        [Test]
        public void SelectSingleRegionVB()
        {
            var res = CodeSnippetHelper.Select("line1\r\nline2\r\n    #Region \"Reg1\"\r\n    line3\r\n    line4\r\n    #End Region\r\nline5", "vb", new CodeSelectorOptions()
            {
                Regions = new string[] { "Reg1" }
            });
        }
    }
}