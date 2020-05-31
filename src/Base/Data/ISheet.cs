//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

namespace Xarial.Docify.Base.Data
{
    public interface ISheet : IResource
    {
        string RawContent { get; }
        ITemplate Layout { get; }
        IMetadata Data { get; }
    }
}
