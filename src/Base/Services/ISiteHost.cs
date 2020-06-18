using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Xarial.Docify.Base.Services
{
    public interface ISiteHost
    {
        Task Host(ILocation site, Func<Task> hostCalback);
    }
}
