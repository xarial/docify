//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
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
