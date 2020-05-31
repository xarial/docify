//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

namespace Xarial.Docify.Base.Data
{
    public interface IFile : IContent, IResource
    {
        ILocation Location { get; }
    }
}
