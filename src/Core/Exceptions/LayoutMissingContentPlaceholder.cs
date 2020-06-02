using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.Docify.Core.Exceptions
{
    public class LayoutMissingContentPlaceholder : UserMessageException
    {
        public LayoutMissingContentPlaceholder() : base("Layout doesn't doesn't contain the placeholder: {{ content }}") 
        {
        }
    }
}
