using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Plugins;
using Xarial.Docify.Lib.Plugins.Common.Data;

namespace Xarial.Docify.Lib.Plugins.FilesPromoter
{
    public class FilesPromoterPlugin : IPlugin<FilesPromoterPluginSettings>
    {
        private IDocifyApplication m_App;
        private FilesPromoterPluginSettings m_Setts;

        private ILocation[] m_PromoteLocations;

        public void Init(IDocifyApplication app, FilesPromoterPluginSettings setts)
        {
            m_App = app;
            m_Setts = setts;

            m_App.Loader.PreLoadFile += OnPreLoadFile;

            m_PromoteLocations = setts.Folders?.Select(f => PluginLocation.FromPath(f)).ToArray();
        }

        private Task OnPreLoadFile(PreLoadFileArgs args)
        {
            var contentLoc = m_PromoteLocations?.FirstOrDefault(l => args.File.Location.IsInLocation(l));

            if (contentLoc != null) 
            {
                args.File = new PluginFile(args.File.Content, args.File.Location.GetRelative(contentLoc), args.File.Id);
            }

            return Task.CompletedTask;
        }
    }
}
