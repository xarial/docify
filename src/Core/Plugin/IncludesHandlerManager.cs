//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Context;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Plugins;
using Xarial.Docify.Base.Services;
using Xarial.Docify.Core.Exceptions;
using Xarial.Docify.Core.Plugin.Extensions;

namespace Xarial.Docify.Core.Plugin
{
    public class IncludesHandlerManager : IIncludesHandlerManager
    {
        public IIncludesHandler Instance { get; }

        private readonly Dictionary<string, ResolveCustomIncludeDelegate> m_CustomIncludesHandlers;
        private readonly IncludesHandlerExtension m_Ext;

        public event PreResolveIncludeDelegate PreResolveInclude;
        public event PostResolveIncludeDelegate PostResolveInclude;

        public IncludesHandlerManager(IIncludesHandler instance, IncludesHandlerExtension ext)
        {
            Instance = instance;
            m_Ext = ext;

            m_Ext.RequestResolveInclude += OnRequestResolveInclude;
            m_Ext.RequestPreResolveInclude += OnRequestPreResolveInclude;
            m_Ext.RequestPostResolveInclude += OnRequestPostResolveInclude;

            m_CustomIncludesHandlers = new Dictionary<string, ResolveCustomIncludeDelegate>(
                StringComparer.CurrentCultureIgnoreCase);
        }

        private async Task<string> OnRequestResolveInclude(string includeName, IMetadata metadata, IPage page)
        {
            if (m_CustomIncludesHandlers.TryGetValue(includeName, out ResolveCustomIncludeDelegate handler))
            {
                return await handler.Invoke(metadata, page);
            }
            else
            {
                throw new MissingIncludeException(includeName);
            }
        }

        private async Task OnRequestPreResolveInclude(string includeName, IContextModel model)
        {
            if (PreResolveInclude != null)
            {
                foreach (PreResolveIncludeDelegate del in PreResolveInclude.GetInvocationList())
                {
                    await del.Invoke(includeName, model);
                }
            }
        }

        private async Task OnRequestPostResolveInclude(string includeName, IContextModel model, StringBuilder html)
        {
            if (PostResolveInclude != null)
            {
                foreach (PostResolveIncludeDelegate del in PostResolveInclude.GetInvocationList())
                {
                    await del.Invoke(includeName, model, html);
                }
            }
        }

        public void RegisterCustomIncludeHandler(string includeName, ResolveCustomIncludeDelegate handler)
        {
            if (!m_CustomIncludesHandlers.ContainsKey(includeName))
            {
                m_CustomIncludesHandlers.Add(includeName, handler);
            }
            else
            {
                throw new UserMessageException($"Include '{includeName}' already registered with other plugin");
            }
        }
    }
}
