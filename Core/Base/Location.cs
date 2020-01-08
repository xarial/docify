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
    //public class LocationBlock : IEquatable<LocationBlock>
    //{
    //    public string Value { get; }
    //    public bool IsRoot { get; }
    //    public bool IsFileName { get; }

    //    internal LocationBlock(string value, bool isRoot, bool isFileName) 
    //    {
    //        Value = value;
    //        IsRoot = isRoot;
    //        IsFileName = isFileName;
    //    }

    //    public bool Equals([AllowNull] LocationBlock other)
    //    {
    //        return Equals(other, StringComparison.CurrentCultureIgnoreCase);
    //    }

    //    public bool Equals(LocationBlock other, StringComparison comparisonType) 
    //    {
    //        if (other == null)
    //        {
    //            return false;
    //        }
    //        else
    //        {
    //            return string.Equals(Value, other.Value, comparisonType)
    //                && IsRoot == other.IsRoot 
    //                && IsFileName == other.IsFileName;
    //        }
    //    }

    //    public override string ToString()
    //    {
    //        return Value;
    //    }

    //    public static bool operator ==(LocationBlock obj1, LocationBlock obj2)
    //    {
    //        if (ReferenceEquals(obj1, obj2))
    //        {
    //            return true;
    //        }

    //        if (ReferenceEquals(obj1, null))
    //        {
    //            return false;
    //        }

    //        if (ReferenceEquals(obj2, null))
    //        {
    //            return false;
    //        }

    //        return obj1.Equals(obj2);
    //    }

    //    public static bool operator !=(LocationBlock obj1, LocationBlock obj2)
    //    {
    //        return !(obj1 == obj2);
    //    }

    //    public override bool Equals(object obj)
    //    {
    //        if (ReferenceEquals(null, obj))
    //        {
    //            return false;
    //        }
    //        if (ReferenceEquals(this, obj))
    //        {
    //            return true;
    //        }

    //        return obj.GetType() == GetType() && Equals((LocationBlock)obj);
    //    }

    //    public override int GetHashCode()
    //    {
    //        return 0;
    //    }
    //}

    public class Location
    {
        private const string PATH_SEP = "\\";
        private const string URL_SEP = "/";
        private const string ID_SEP = "-";

        //public static Location Empty() 
        //{
        //    return new Location("");
        //}

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
        
        //public bool Equals([AllowNull]Location other)
        //{
        //    return Equals(other, StringComparison.CurrentCultureIgnoreCase);
        //}

        //public bool Equals(Location other, StringComparison comparisonType)
        //{
        //    if (other == null)
        //    {
        //        return false;
        //    }
        //    else
        //    {
        //        if (other.Path.Count == Path.Count)
        //        {
        //            for (int i = 0; i < Path.Count; i++)
        //            {
        //                if (!string.Equals(Path[i], other.Path[i], comparisonType)) 
        //                {
        //                    return false;
        //                }
        //            }

        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //}

        public IReadOnlyList<string> Path { get; }

        public int TotalLevel 
        {
            get 
            {
                return Path.Count;
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
        
        //public static bool operator ==(Location obj1, Location obj2)
        //{
        //    if (ReferenceEquals(obj1, obj2))
        //    {
        //        return true;
        //    }

        //    if (ReferenceEquals(obj1, null))
        //    {
        //        return false;
        //    }

        //    if (ReferenceEquals(obj2, null))
        //    {
        //        return false;
        //    }

        //    return obj1.Equals(obj2);
        //}

        //public static bool operator !=(Location obj1, Location obj2)
        //{
        //    return !(obj1 == obj2);
        //}

        //public override bool Equals(object obj)
        //{
        //    if (ReferenceEquals(null, obj))
        //    {
        //        return false;
        //    }
        //    if (ReferenceEquals(this, obj))
        //    {
        //        return true;
        //    }

        //    return obj.GetType() == GetType() && Equals((Location)obj);
        //}

        //public override int GetHashCode()
        //{
        //    return 0;
        //}

        public override string ToString()
        {
            return ToId();
        }

        //public static Location operator +(Location obj1, Location obj2)
        //{
        //    return new Location(obj2.FileName, obj1.Path.ToArray());
        //}

        //public static Location operator +(Location obj1, string obj2)
        //{
        //    return new Location("", obj1.Path
        //        .Concat(new string[] { obj2 }).ToArray());
        //}
    }
}
