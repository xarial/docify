using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.Docify.Base.Context
{
    public interface IContextSite
    {
        IContextConfiguration Configuration { get; }
        string BaseUrl { get; }
        IContextPage MainPage { get; }
    }
}
