using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.Docify.Core.Exceptions
{
    public class FilePublishOverwriteForbiddenException : Exception
    {
        public FilePublishOverwriteForbiddenException(string filePath)
            : base($"Cannot publish file to {filePath} as overwriting is forbidden")
        {
        }
    }
}
