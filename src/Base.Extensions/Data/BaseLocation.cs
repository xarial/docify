using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xarial.Docify.Base.Data
{
    internal class BaseLocation : ILocation
    {
        public IReadOnlyList<string> Segments { get; }
        public string Root { get; }
        public string FileName { get; }

        internal BaseLocation(string path) 
        {
            LocationExtension.Parse(path, out string root, out string fileName, out string[] segments);
            Root = root;
            FileName = fileName;
            Segments = segments.ToList();
        }

        private BaseLocation(string root, string fileName, IEnumerable<string> segments) 
        {
            Root = root;
            FileName = fileName;
            Segments = segments.ToList();
        }
        
        public ILocation Create(string root, string fileName, IEnumerable<string> segments)
        {
            return new BaseLocation(root, fileName, segments);
        }
    }
}
