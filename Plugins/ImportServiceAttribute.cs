using System;
using System.Collections.Generic;
using System.Composition;
using System.Text;

namespace Xarial.Docify.Lib.Plugins
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ImportServiceAttribute : ImportAttribute
    {
    }
}
