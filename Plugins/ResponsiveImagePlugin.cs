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
using Xarial.Docify.Lib.Plugins.Helpers;
using Xarial.Docify.Lib.Plugins.Properties;

namespace Xarial.Docify.Lib.Plugins
{
    [Plugin("responsive-image")]
    public class ResponsiveImagePlugin : IPreCompilePlugin, IPageContentWriterPlugin, IRenderImagePlugin
    {
        private readonly string CSS_FILE_PATH = "assets/styles/responsive-image.css";
        private const string CLASS_NAME = "responsive";

        public Task PreCompile(ISite site)
        {
            AssetsHelper.AddTextAsset(Resources.responsive_image, site.MainPage, CSS_FILE_PATH);

            return Task.CompletedTask;
        }
        
        public void RenderImage(StringBuilder html)
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

        public Task<string> WritePageContent(string content, string url)
        {
            content = this.WriteToPageHead(content, w => w.AddStyleSheets(CSS_FILE_PATH));
            return Task.FromResult(content);
        }
    }
}
