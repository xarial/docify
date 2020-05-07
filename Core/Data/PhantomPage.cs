using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xarial.Docify.Base;

namespace Xarial.Docify.Core.Data
{
    [DebuggerDisplay("~{" + nameof(Key) + "}")]
    public class PhantomPage : Page
    {
        public PhantomPage(ILocation url) : base(url, null)
        {
        }
    }
}
