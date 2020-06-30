//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Plugins;
using Xarial.Docify.Lib.Plugins.Common.Exceptions;
using Xarial.Docify.Lib.Plugins.Common.Helpers;
using Xarial.Docify.Lib.Plugins.ResponsiveImage.Properties;

namespace Xarial.Docify.Lib.Plugins.ResponsiveImage
{
    public class ResponsiveImagePlugin : IPlugin
    {
        private const string CSS_FILE_PATH = "/_assets/styles/responsive-image.css";
        private const string CLASS_NAME = "responsive";
        private const string RESP_IMG_ATT = "responsive-image";

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

            var respImgAtt = doc.Root.Attributes().FirstOrDefault(
                a => string.Equals(a.Name.ToString(), RESP_IMG_ATT, StringComparison.CurrentCultureIgnoreCase));

            var processImage = true;

            if (respImgAtt != null) 
            {
                processImage = bool.Parse(respImgAtt.Value);
                respImgAtt.Remove();
            }

            if (processImage)
            {
                if (classAtt != null)
                {
                    classAtt.Value = $"{classAtt.Value} {CLASS_NAME}";
                }
                else
                {
                    classAtt = new XAttribute("class", CLASS_NAME);
                    doc.Root.Add(classAtt);
                }
            }

            img = doc.ToString();

            html.Clear();

            if (processImage) 
            {
                img = string.Format(Resources.img_figure, img, imgSrc, imgAlt);
            }

            html.Append(img);
        }

        private Task OnWritePageContent(StringBuilder html, IMetadata data, string url)
        {
            if (html.Length > 0)
            {
                try
                {
                    var htmlWriter = new HtmlHeadWriter(html);
                    htmlWriter.AddStyleSheets(CSS_FILE_PATH);

                    return Task.CompletedTask;
                }
                catch (Exception ex)
                {
                    throw new HeadAssetLinkFailedException(CSS_FILE_PATH, url, ex);
                }
            }
            else
            {
                return Task.FromResult(html);
            }
        }
    }
}
