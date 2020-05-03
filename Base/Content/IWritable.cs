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

namespace Xarial.Docify.Base.Content
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
    }

    public class Writable : IFile
    {
        public static Writable FromTextContent(string content, Location loc) 
        {
            var buffer = DataConverter.ToByteArray(content);

            return new Writable(buffer, loc);
        }

        public byte[] Content { get; }

        public Location Location { get; }

        public Writable(byte[] content, Location loc) 
        {
            Content = content;
            Location = loc;
        }

        public Writable(string content, Location loc) 
            : this(DataConverter.ToByteArray(content), loc)
        {
        }
    }
}
