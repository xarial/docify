﻿//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Xarial.Docify.Core.Helpers
{
    public class MetadataSerializer
    {
        private class DictionaryTypeResolver : INodeTypeResolver
        {
            public bool Resolve(NodeEvent nodeEvent, ref Type currentType)
            {
                if (currentType == typeof(object) && nodeEvent is MappingStart)
                {
                    currentType = typeof(Dictionary<string, object>);
                    return true;
                }

                return false;
            }
        }

        private readonly IDeserializer m_YamlSerializer;

        public MetadataSerializer()
        {
            m_YamlSerializer = new DeserializerBuilder()
                .WithNodeTypeResolver(new DictionaryTypeResolver())
                .Build();
        }

        public T Deserialize<T>(string data)
        {
            return m_YamlSerializer.Deserialize<T>(data);
        }
    }
}
