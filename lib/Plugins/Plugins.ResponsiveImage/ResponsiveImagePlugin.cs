//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Plugins;
using Xarial.Docify.Lib.Plugins.Data;
using Xarial.Docify.Lib.Plugins.Exceptions;
using Xarial.Docify.Lib.Plugins.Helpers;
using Xarial.Docify.Lib.Plugins.ResponsiveImage.Properties;

namespace Xarial.Docify.Lib.Plugins.ResponsiveImage
{
    [Plugin("responsive-image")]
    public class ResponsiveImagePlugin : IPlugin
    {
        private const string CSS_FILE_PATH = "/assets/styles/responsive-image.css";
        private const string CLASS_NAME = "responsive";

        private IDocifyApplication m_App;

        public void Init(IDocifyApplication app)
        {
            m_App = app;

            m_App.Compiler.PreCompile += OnPreCompile;
            m_App.Compiler.RenderImage += OnRenderImage;
            m_App.Compiler.WritePageContent += OnWritePageContent;
        }
        
        private Task OnPreCompile(ISite site)
        {
            AssetsHelper.AddTextAsset(Resources.responsive_image, site.MainPage, CSS_FILE_PATH);

            return Task.CompletedTask;
        }
        
        private void OnRenderImage(StringBuilder html)
        {
            var img = html.ToString();
            
            var doc = XDocument.Parse(img);

            var imgSrc = doc.Root.Attributes().FirstOrDefault(
                a => string.Equals(a.Name.ToString(), "src", StringComparison.CurrentCultureIgnoreCase))?.Value;

            var imgAlt = doc.Root.Attributes().FirstOrDefault(
                a => string.Equals(a.Name.ToString(), "alt", StringComparison.CurrentCultureIgnoreCase))?.Value;

            var classAtt = doc.Root.Attributes().FirstOrDefault(
                a => string.Equals(a.Name.ToString(), "class", StringComparison.CurrentCultureIgnoreCase));

            if (classAtt != null)
            {
                classAtt.Value = $"{classAtt.Value} {CLASS_NAME}";
            }
            else 
            {
                classAtt = new XAttribute("class", CLASS_NAME);
                doc.Root.Add(classAtt);
            }

            img = doc.ToString();

            html.Clear();
            html.Append(string.Format(Resources.img_figure, img, imgSrc, imgAlt));
        }

        private Task<string> OnWritePageContent(string content, IMetadata data, string url)
        {
            if (!string.IsNullOrEmpty(content))
            {
                try
                {
                    var htmlWriter = new HtmlHeadWriter(content);
                    htmlWriter.AddStyleSheets(CSS_FILE_PATH);

                    return Task.FromResult(htmlWriter.Content);
                }
                catch (Exception ex)
                {
                    throw new HeadAssetLinkFailedException(CSS_FILE_PATH, url, ex);
                }
            }
            else 
            {
                return Task.FromResult(content);
            }
        }
    }
}
