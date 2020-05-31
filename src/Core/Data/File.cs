//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
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
