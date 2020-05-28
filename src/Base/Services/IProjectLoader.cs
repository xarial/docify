using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Base.Services
{
    public interface IProjectLoader
    {
        IAsyncEnumerable<IFile> Load(ILocation[] locations);
    }
}
