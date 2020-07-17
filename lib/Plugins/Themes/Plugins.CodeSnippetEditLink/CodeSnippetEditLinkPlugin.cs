using Plugins.Themes.UserGuide.CodeSnippetEditLink;
using System;
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Base.Context;
using Xarial.Docify.Base.Plugins;

namespace Plugins.CodeSnippetEditLink
{
    public class CodeSnippetEditLinkPlugin : IPlugin<CodeSnippetEditLinkPluginSettings>
    {
        private IDocifyApplication m_App;
        private CodeSnippetEditLinkPluginSettings m_Setts;

        public void Init(IDocifyApplication app, CodeSnippetEditLinkPluginSettings setts)
        {
            m_App = app;
            m_Setts = setts;

            if (m_Setts.Enable) 
            {
                m_App.Includes.PostResolveInclude += OnPostResolveInclude;
            }
        }

        private Task OnPostResolveInclude(string includeName, IContextModel model, StringBuilder html)
        {
            return Task.CompletedTask;
        }
    }
}
