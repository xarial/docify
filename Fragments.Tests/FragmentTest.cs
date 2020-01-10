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

namespace Fragments.Tests
{
    public static class FragmentTest
    {
        public static string GetPath(string relPath) 
        {
            const string SOLUTION_FILE_NAME = "docify.sln";

            var solDir = Path.GetDirectoryName(typeof(FragmentTest).Assembly.Location);

            while (!File.Exists(Path.Combine(solDir, SOLUTION_FILE_NAME)))
            {
                solDir = Path.GetDirectoryName(solDir);
            }

            var fragDir = Path.Combine(solDir, "Fragments");

            return Path.Combine(fragDir, relPath);
        }

        public static string Normalize(string content) 
        {
            var lines = content.Split('\n');

            return string.Join(Environment.NewLine, lines.Select(l => l.Trim()));
        }
    }
}
