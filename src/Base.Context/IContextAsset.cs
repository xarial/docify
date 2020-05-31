//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

namespace Xarial.Docify.Base.Context
{
    public interface IContextAsset
    {
        byte[] Content { get; }
        string Name { get; }
    }
}
