using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xarial.Docify.Base;

namespace Xarial.Docify.Core.Data
{
    [DebuggerDisplay("~{" + nameof(Name) + "}")]
    public class PhantomPage : Page
    {
        public PhantomPage(string name) 
            : base(name, "", new Metadata(), Guid.NewGuid().ToString(), null)
        {
        }
    }
}
