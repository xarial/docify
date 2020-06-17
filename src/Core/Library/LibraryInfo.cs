using System;

namespace Xarial.Docify.Core.Library
{
    public class LibraryInfo
    {
        public Version MinimumAppVersion { get; set; }
        public Version MaximumAppVersion { get; set; }
        public Version Version { get; set; }
        public string DownloadUrl { get; set; }
        public byte[] Signature { get; set; }
    }
}
