//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;

namespace Xarial.Docify.Core.Base
{
    public class Location
    {
        private const string PATH_SEP = "\\";
        private const string URL_SEP = "/";
        private const string ID_SEP = "-";

        public static Location FromPath(string path, string relTo = "") 
        {
            //TODO: check if path is valid

            if (!string.IsNullOrEmpty(relTo)) 
            {
                if (path.StartsWith(relTo, StringComparison.CurrentCultureIgnoreCase))
                {
                    path = System.IO.Path.GetRelativePath(relTo, path);
                }
            }

            var fileName = System.IO.Path.GetFileName(path);
            var dir = System.IO.Path.GetDirectoryName(path);

            string[] blocks = null;

            if (!string.IsNullOrEmpty(dir))
            {
                blocks = dir.Split(PATH_SEP).ToArray();
            }
            else 
            {
                blocks = new string[0];
            }
            
            return new Location(fileName, blocks);
        }

        public string ToPath(string root = "") 
        {
            return FormFullLocation(root, PATH_SEP);
        }

        public string ToId()
        {
            return FormFullLocation("", ID_SEP);
        }

        public bool IsEmpty
        {
            get 
            {
                return !Path.Any() && string.IsNullOrEmpty(FileName);
            }
        }

        public string ToUrl(string baseUrl = "") 
        {
            return FormFullLocation(baseUrl, URL_SEP);
        }

        private string FormFullLocation(string basePart, string sep) 
        {
            var fullLoc = new StringBuilder();

            if (!string.IsNullOrEmpty(basePart)) 
            {
                fullLoc.Append(basePart);
            }

            foreach (var block in Path) 
            {
                if (fullLoc.Length > 0) 
                {
                    fullLoc.Append(sep);
                }

                fullLoc.Append(block);
            }

            if (!string.IsNullOrEmpty(FileName)) 
            {
                if (fullLoc.Length > 0)
                {
                    fullLoc.Append(sep);
                }

                fullLoc.Append(FileName);
            }

            return fullLoc.ToString();
        }
        
        public IReadOnlyList<string> Path { get; }

        public int TotalLevel 
        {
            get 
            {
                return Path.Count;
            }
        }

        public bool IsRoot 
        {
            get
            {
                return !Path.Any();
            }
        }

        public string Root 
        {
            get 
            {
                return Path.FirstOrDefault();
            }
        }

        public string FileName  { get; }

        public Location(string fileName, params string[] path) 
        {
            FileName = fileName;
            Path = new List<string>(path);
        }
        
        public override string ToString()
        {
            return ToId();
        }
    }
}
