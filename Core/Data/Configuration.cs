﻿using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Core.Data
{
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

        public override IMetadata Copy(IDictionary<string, object> data)
        {
            return new Configuration(data);
        }
    }
}