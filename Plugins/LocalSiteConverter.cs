using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Plugins;

namespace Xarial.Docify.Lib.Plugins
{
    [Plugin("local-site-converter")]
    public class LocalSiteConverter : IPrePublishTextAssetPlugin
    {
        private const string REGEX_LINK = "(?<=src=\"|href=\")[^\"]*(?=\")";

        public void PrePublishTextAsset(ref Location loc, ref string content, out bool cancel)
        {
            cancel = false;
            content = Regex.Replace(content, REGEX_LINK, m =>
            {
                var link = m.Value.Trim('/');
                if (!Path.HasExtension(link)) 
                {
                    link = $"{link}/index.html";
                }

                return link;
            });
        }
    }
}
