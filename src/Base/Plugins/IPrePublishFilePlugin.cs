﻿//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Base.Plugins
{
    public struct PrePublishResult 
    {
        public IFile File { get; set; }
        public bool SkipFile { get; set; }
    }

    public interface IPrePublishFilePlugin : IPlugin
    {
        Task<PrePublishResult> PrePublishFile(ILocation outLoc, IFile file);
    }
}