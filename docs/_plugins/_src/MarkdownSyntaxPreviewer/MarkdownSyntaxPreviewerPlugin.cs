using System;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Plugins;

namespace Xarial.Docify.Documentation
{
    public class MarkdownSyntaxPreviewerPlugin : IPlugin
    {
        private IDocifyApplication m_App;
        private List<string> m_SnippetIds;

        public void Init(IDocifyApplication app)
        {            
            m_App = app;
            m_SnippetIds = new List<string>();

            m_App.Includes.RegisterCustomIncludeHandler("markdown-snippet", OnResolveMarkdownSnippet);
            m_App.Publisher.PrePublishFile += OnPrePublishFile;
        }

        private async Task<string> OnResolveMarkdownSnippet(IMetadata data, IPage page)
        {
            var assetName = data.GetParameterOrDefault<string>("name");
            
            var snippetAsset = page.FindAsset(assetName);

            m_SnippetIds.Add(snippetAsset.Id);
            
            var markdown = snippetAsset.AsTextContent();

            var html = await m_App.Compiler.StaticContentTransformer.Transform(markdown);

            var htmlCode = await m_App.Compiler.StaticContentTransformer.Transform("``` html" + Environment.NewLine + html + Environment.NewLine + "```");
            var mdCode = await m_App.Compiler.StaticContentTransformer.Transform("``` md" + Environment.NewLine + markdown + Environment.NewLine + "```");

            var tab = new StringBuilder();
            var tabId = System.IO.Path.GetFileNameWithoutExtension(assetName) + "-tab";
            
            tab.AppendLine($"<div id=\"{tabId}\">");
            tab.AppendLine("<div class=\"code-tab\">");
            tab.AppendLine($"<button class=\"tablinks active\" onclick=\"openTab(event, '{tabId}', '{tabId}-md')\">Markdown</button>");
            tab.AppendLine($"<button class=\"tablinks\" onclick=\"openTab(event, '{tabId}', '{tabId}-html')\">HTML</button>");
            tab.AppendLine($"<button class=\"tablinks\" onclick=\"openTab(event, '{tabId}', '{tabId}-preview')\">Preview</button>");
            tab.AppendLine("</div>");
            
            tab.AppendLine($"<div id=\"{tabId}-md\" class=\"tabcontent\" style=\"display: block\">");
            tab.AppendLine(mdCode);
            tab.AppendLine("</div>");

            tab.AppendLine($"<div id=\"{tabId}-html\" class=\"tabcontent\" style=\"display: none\">");
            tab.AppendLine(htmlCode);
            tab.AppendLine("</div>");

            tab.AppendLine($"<div id=\"{tabId}-preview\" class=\"tabcontent\" style=\"display: none\">");
            tab.AppendLine("<div style=\"background: #eff0f1; margin: 1em 0px\">" + html + "</div>");
            tab.AppendLine("</div>");

            tab.AppendLine("</div>");

            return tab.ToString();
        }

        private Task OnPrePublishFile(ILocation outLoc, PrePublishFileArgs args)
        {
            args.SkipFile = m_SnippetIds.Contains(args.File.Id);
            return Task.CompletedTask;
        }
    }
}
