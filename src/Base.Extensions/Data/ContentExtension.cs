//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System.IO;

namespace Xarial.Docify.Base.Data
{
    /// <summary>
    /// Provides extension methods for <see cref="IContent"/>
    /// </summary>
    public static class ContentExtension
    {
        /// <summary>
        /// Reads the content as text
        /// </summary>
        /// <param name="file">File to read a content from</param>
        /// <returns>Text content</returns>
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

        /// <summary>
        /// Transforms the text into the byte array suitable for the <see cref="IContent.Content"/>
        /// </summary>
        /// <param name="content">Text content</param>
        /// <returns>Byte array representation of the text content</returns>
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
