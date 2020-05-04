//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Xarial.Docify.Base.Context;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Lib.Tools
{
    public static class DataDeserializer
    {
        public static T Deserialize<T>(IContextMetadata data)
        {
            return MetadataExtension.ToObject<T>(new Dictionary<string, object>(data));
        }
    }
}
