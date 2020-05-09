using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Xarial.Docify.Base.Plugins
{
    public interface IPostPublishPlugin : IPlugin
    {
        Task PostPublish(ILocation loc);
    }
}
