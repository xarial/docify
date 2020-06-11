//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using Xarial.Docify.Core.Exceptions;
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
            where T : new()
        {
            try
            {
                if (!string.IsNullOrEmpty(data))
                {
                    return m_YamlSerializer.Deserialize<T>(data);
                }
                else
                {
                    return new T();
                }
            }
            catch (YamlDotNet.Core.SemanticErrorException ex) 
            {
                throw new MetadataDeserializationException(ex.Message, ex);
            }
        }
    }
}
