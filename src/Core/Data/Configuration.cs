﻿//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System.Collections.Generic;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Core.Data
{
    public class Configuration : Metadata, IConfiguration
    {
        public string Environment { get; set; }
        public List<string> Components { get; set; }
        public List<string> Plugins { get; set; }
        public List<string> ThemesHierarchy { get; }

        public Configuration() : this(new Dictionary<string, dynamic>())
        {
        }

        public Configuration(IDictionary<string, dynamic> parameters) : base(parameters)
        {
            Environment = "";
            ThemesHierarchy = new List<string>();
        }

        public override IMetadata Copy(IDictionary<string, object> data)
        {
            return new Configuration(data);
        }
    }
}
