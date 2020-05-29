using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Core;
using Xarial.Docify.Core.Data;

namespace Tests.Common.Mocks
{
    public class FileMock : File
    {
        public FileMock(Location location, string content)
            : this(location, ContentExtension.ToByteArray(content))
        {
        }

        public FileMock(string location, string content)
            : this(Xarial.Docify.Core.Location.FromPath(location), ContentExtension.ToByteArray(content))
        {
        }

        public FileMock(Location location, byte[] content)
            : base(location, content, Guid.NewGuid().ToString())
        {
        }
    }
}
