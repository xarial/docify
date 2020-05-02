using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Xarial.Docify.Lib.Plugins
{
    internal static class Helper
    {
        internal const string CSS_LINK_TEMPLATE = "<link rel=\"stylesheet\" type=\"text/css\" href=\"/{0}\" />\r\n";

        internal static void InjectDataIntoHtmlHead(ref string html, string data) 
        {
            var headInd = html.IndexOf("</head>");

            if (headInd != -1)
            {
                html = html.Insert(headInd, data);
            }
            else
            {
                throw new Exception("Failed to find </head> tag in the html document");
            }
        }

        internal static string ByteArrayContentToText(byte[] content)
        {
            using (var memStr = new MemoryStream(content))
            {
                memStr.Seek(0, SeekOrigin.Begin);

                using (var streamReader = new StreamReader(memStr))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }

        internal static byte[] TextContentToByteArray(string content)
        {
            using (var memStr = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(memStr))
                {
                    streamWriter.Write(content);
                }

                memStr.Seek(0, SeekOrigin.Begin);
                return memStr.ToArray();
            }
        }
    }
}
