using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.Docify.Core.Exceptions
{
    public class SiteHostingFailedException : UserMessageException
    {
        public SiteHostingFailedException(Exception ex) 
            : base($"Failed to host a site: {ex.Message}", ex) 
        {
        }
    }
}
