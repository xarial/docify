//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

namespace Xarial.Docify.Lib.Plugins.ImageOptimizer
{
    public class ImageOptimizerSettings
    {
        public string[] MatchPattern { get; set; } = new string[]
        {
            "\\.png$", "\\.jpg$", "\\.jpeg$", "\\.bmp$", "\\.tif$", "\\.tiff$"
        };

        public bool IgnoreMatchCase { get; set; } = true;

        public bool ImageTagConvertSvgToPng { get; set; } = true;

        /// <remarks>
        /// 0 to maintain aspect ration
        /// </remarks>
        public int SvgPngWidth { get; set; } = 1200;

        /// <remarks>
        /// 0 to maintain aspect ration
        /// </remarks>
        public int SvgPngHeight { get; set; } = 0;

        public bool GenerateFavIcon { get; set; } = false;
    }
}
