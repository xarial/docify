//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

namespace Xarial.Docify.Base.Context
{
    public interface IContextAsset
    {
        byte[] Content { get; }
        string Name { get; }
    }
}
