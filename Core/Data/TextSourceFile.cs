//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Core.Exceptions;
using YamlDotNet.Serialization;

namespace Xarial.Docify.Core.Data
{
    public class TextSourceFile : ITextSourceFile
    {
        public Location Location { get; }
        public string Content { get; }

        public TextSourceFile(Location path, string content)
        {
            Location = path;
            Content = content;
        }

        public override string ToString()
        {
            return Location.ToString();
        }
    }

    public static class ITextSourceFileExtension 
    {
        private const string FRONT_MATTER_HEADER = "---";

        public static void Parse(this ITextSourceFile src, out string rawContent,
            out Metadata data)
        {
            bool isStart = true;
            bool readingFrontMatter = false;

            rawContent = "";
            var frontMatter = new StringBuilder();

            using (var strReader = new StringReader(src.Content))
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
                var yamlDeserializer = new DeserializerBuilder().Build();

                data = yamlDeserializer.Deserialize<Metadata>(frontMatter.ToString());
            }
            else 
            {
                data = new Metadata();
            }
        }
    }
}
