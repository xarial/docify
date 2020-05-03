//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Base.Data
{
    public interface IFile
    {
        byte[] Content { get; }
        Location Location { get; }
    }

    public static class FileExtension
    {
        public static string AsTextContent(this IFile writable)
        {
            return DataConverter.ToText(writable.Content);
        }

        public static byte[] ToByteArray(string content) 
        {
            return DataConverter.ToByteArray(content);
        }
    }
}
