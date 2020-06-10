using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Xarial.Docify.Base.Data;

namespace Tests.Common.Mocks
{
    public class MetadataMock : Dictionary<string, object>, IMetadata
    {
        public MetadataMock()
        {
        }

        public MetadataMock(IDictionary<string, object> data) : base(data)
        {
        }

        public IMetadata Copy(IDictionary<string, object> data)
        {
            return new MetadataMock(data);
        }
    }
}
