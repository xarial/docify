//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.Docify.Core.Compiler
{
    public class BaseCompilerConfig
    {
        public enum ParallelPartitions_e
        {
            Infinite = -1,
            AutoDetect = 0,
            NoParallelism = 1
        }
        
        /// <summary>
        /// Number of partitions for parallel job. See <see cref="ParallelPartitions_e"/> for options
        /// </summary>
        public int ParallelPartitionsCount { get; set; }

        public BaseCompilerConfig()
        {
            ParallelPartitionsCount = (int)ParallelPartitions_e.NoParallelism;
        }
    }
}
