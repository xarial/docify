using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Xarial.Docify.Base.Services
{
    public interface ITargetDirectoryCleaner
    {
        Task ClearDirectory(ILocation outDir);
    }
}
