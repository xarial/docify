//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System.Collections.Generic;

namespace Xarial.Docify.Base
{
    public interface ILocation
    {
        IReadOnlyList<string> Path { get; }
        string FileName { get; }

        ILocation Copy(string fileName, IEnumerable<string> path);
    }
}
