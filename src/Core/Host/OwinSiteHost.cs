//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Services;

namespace Xarial.Docify.Core.Host
{
    public class HostSettings
    {
        public int HttpPort { get; }
        public int HttpsPort { get; }

        public HostSettings(int httpPort, int httpsPort) 
        {
            HttpPort = httpPort;
            HttpsPort = httpsPort;
        }
    }

    public class OwinSiteHost : ISiteHost
    {
        private readonly string m_HttpUrl;
        private readonly string m_HttpsUrl;

        private readonly Base.Services.ILogger m_Logger;

        public OwinSiteHost(HostSettings setts, Base.Services.ILogger logger) 
        {
            m_HttpUrl = "http://localhost:" + setts.HttpPort;
            m_HttpsUrl = "https://localhost:" + setts.HttpsPort;

            m_Logger = logger;
        }

        public async Task Host(ILocation siteLoc, Func<Task> hostCalback)
        {
            var sitePath = siteLoc.ToPath();

            var hostBuilder = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.SuppressStatusMessages(true);
                    webBuilder.ConfigureLogging((context, logging) =>
                    {
                        logging.ClearProviders();
                    });
                    webBuilder.UseUrls(m_HttpUrl, m_HttpsUrl);
                    webBuilder.Configure(app =>
                    {
                        var opts = new FileServerOptions()
                        {
                            FileProvider = new PhysicalFileProvider(sitePath),
                            EnableDefaultFiles = true,
                        };

                        opts.StaticFileOptions.ServeUnknownFileTypes = true;

                        app.UseFileServer(opts);
                    });
                });

            var host = hostBuilder.Build();
            
            await host.StartAsync();

            m_Logger.LogInformation($"'{sitePath}' is served at {m_HttpUrl} and {m_HttpsUrl}");

            await hostCalback.Invoke();

            await host.StopAsync();

            host.Dispose();

            m_Logger.LogInformation("Host is closed");
        }
    }
}
