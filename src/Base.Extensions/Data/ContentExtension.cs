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

namespace Xarial.Docify.Base.Data
{
    public static class ContentExtension
    {
        public static string AsTextContent(this IContent file)
        {
            var buffer = file.Content;

            if (buffer != null && buffer.Length > 0)
            {
                using (var memStr = new MemoryStream(buffer))
                {
                    memStr.Seek(0, SeekOrigin.Begin);

                    using (var streamReader = new StreamReader(memStr))
                    {
                        return streamReader.ReadToEnd();
                    }
                }
            }
            else
            {
                return "";
            }
        }

        public static byte[] ToByteArray(string content)
        {
            byte[] buffer = new byte[0];

            if (!string.IsNullOrEmpty(content))
            {
                using (var memStr = new MemoryStream())
                {
                    using (var streamWriter = new StreamWriter(memStr))
                    {
                        streamWriter.Write(content);
                        streamWriter.Flush();
                        memStr.Seek(0, SeekOrigin.Begin);
                        buffer = memStr.ToArray();
                    }
                }
            }

            return buffer;
        }
    }
}
