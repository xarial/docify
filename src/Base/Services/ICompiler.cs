//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System.Collections.Generic;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Base.Services
{
    /// <summary>
    /// Services responsible for compiling the site to the target publishable files
    /// </summary>
    /// <remarks>This service will employ <see cref="IStaticContentTransformer"/>, <see cref="IDynamicContentTransformer"/>, <see cref="IIncludesHandler"/>, <see cref="ILayoutParser"/> services to compile the raw content into the target data</remarks>
    public interface ICompiler
    {
        /// <summary>
        /// Compiles the site
        /// </summary>
        /// <param name="site">Input site</param>
        /// <returns>Compiled files</returns>
        IAsyncEnumerable<IFile> Compile(ISite site);
    }
}
