//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Base.Content;

namespace Xarial.Docify.Base.Services
{
    public interface IPublisher
    {
        Task Write(IEnumerable<IWritable> writables);
    }
}
