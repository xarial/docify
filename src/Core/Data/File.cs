//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Core.Data
{
    public class File : IFile
    {
        public byte[] Content { get; }
        public ILocation Location { get; }
        public string Id { get; }

        public File(ILocation path, byte[] content, string id)
        {
            Location = path;
            Content = content;
            Id = id;
        }

        public override string ToString()
        {
            return Location.ToString();
        }
    }
}
