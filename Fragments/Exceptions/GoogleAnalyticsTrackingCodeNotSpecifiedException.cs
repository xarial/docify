//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fragments.Exceptions
{
    public class GoogleAnalyticsTrackingCodeNotSpecifiedException : Exception
    {
        public GoogleAnalyticsTrackingCodeNotSpecifiedException() : base("Specify the tracking code for google analytics via 'google_analytics' parameter in the configuration file") 
        {
        }
    }
}
