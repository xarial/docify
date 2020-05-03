//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;

namespace Xarial.Docify.Base.Data
{
    public interface IConfiguration 
    {
        Environment_e Environment { get; set; }
    }

    public class Configuration : Metadata, IConfiguration
    {
        public Environment_e Environment { get; set; }
        public string WorkingFolder { get; set; }
        public ILocation ComponentsFolder { get; set; }
        public List<string> Components { get; set; }
        public ILocation PluginsFolder { get; set; }
        public List<string> Plugins { get; set; }
        public ILocation ThemesFolder { get; set; }
        public List<string> ThemesHierarchy { get; }
        
        public Configuration() : this(new Dictionary<string, dynamic>())
        {
        }

        public Configuration(IDictionary<string, dynamic> parameters) : base(parameters) 
        {
            Environment = Environment_e.Test;
            ThemesHierarchy = new List<string>();
        }
    }
}
