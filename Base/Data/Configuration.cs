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
        Environment_e Environment { get; }
    }

    public class Configuration : Metadata, IConfiguration
    {
        public Environment_e Environment { get; }

        public Configuration(Environment_e env) : this(new Dictionary<string, dynamic>(), env)
        {
        }

        public Configuration(IDictionary<string, dynamic> parameters, Environment_e env) : base(parameters) 
        {
            Environment = env;
        }
    }
}
