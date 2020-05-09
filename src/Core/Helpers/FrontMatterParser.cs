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
using Xarial.Docify.Core.Data;
using Xarial.Docify.Core.Exceptions;
using YamlDotNet.Serialization;

namespace Xarial.Docify.Core.Helpers
{
    public static class FrontMatterParser
    {
        private const string FRONT_MATTER_HEADER = "---";

        public static void Parse(string content, out string rawContent,
            out IMetadata data)
        {
            bool isStart = true;
            bool readingFrontMatter = false;

            rawContent = "";
            var frontMatter = new StringBuilder();

            using (var strReader = new StringReader(content))
            {
                var line = strReader.ReadLine();

                while (line != null)
                {
                    if (line == FRONT_MATTER_HEADER)
                    {
                        if (isStart)
                        {
                            readingFrontMatter = true;
                        }
                        else
                        {
                            readingFrontMatter = false;
                            rawContent = strReader.ReadToEnd();
                        }
                    }
                    else if (readingFrontMatter)
                    {
                        frontMatter.AppendLine(line);
                    }
                    else
                    {
                        rawContent = strReader.ReadToEnd();

                        rawContent = line + (!string.IsNullOrEmpty(rawContent) ? Environment.NewLine : "") + rawContent;
                    }

                    isStart = false;
                    line = strReader.ReadLine();
                }
            }

            if (readingFrontMatter)
            {
                throw new FrontMatterErrorException("Front matter closing tag is not found");
            }

            if (frontMatter.Length > 0)
            {
                var yamlDeserializer = new MetadataSerializer();

                data = yamlDeserializer.Deserialize<Metadata>(frontMatter.ToString());
            }
            else
            {
                data = new Metadata();
            }
        }
    }
}
