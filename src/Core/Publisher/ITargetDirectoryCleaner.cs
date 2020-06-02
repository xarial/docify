//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System.Threading.Tasks;
using Xarial.Docify.Base;

namespace Xarial.Docify.Core.Publisher
{
    public interface ITargetDirectoryCleaner 
    {
        Task ClearDirectory(ILocation outDir);
    }
}
