using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Plugins;
using Xarial.Docify.Lib.Plugins.Properties;

namespace Xarial.Docify.Lib.Plugins
{
    [Plugin("responsive-image")]
    public class ResponsiveImagePlugin : IPreCompilePlugin, IPrePublishFilePlugin, IRenderImagePlugin
    {
        private const string CSS_FILE_NAME = "responsive-image.css";
        private readonly string[] CSS_FILE_PATH = new string[] { "assets", "styles" };
        private const string CLASS_NAME = "responsive";

        public void PreCompile(ISite site)
        {
            site.MainPage.Assets.Add(new PluginDataFile(Resources.responsive_image, new PluginDataFileLocation(CSS_FILE_NAME, CSS_FILE_PATH)));
        }

        public void PrePublishFile(ILocation outLoc, ref IFile file, out bool skip)
        {
            if (string.Equals(Path.GetExtension(file.Location.FileName), ".html", StringComparison.InvariantCultureIgnoreCase))
            {
                var pageContent = file.AsTextContent();
                Helper.InjectDataIntoHtmlHead(ref pageContent,
                    string.Format(Helper.CSS_LINK_TEMPLATE, string.Join('/', CSS_FILE_PATH) + "/" + CSS_FILE_NAME));
                file = new PluginDataFile(pageContent, file.Location);
            }

            skip = false;
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
    }
}
