//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System.Threading.Tasks;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Base.Services
{
    /// <summary>
    /// Service loads job configuration
    /// </summary>
    public interface IConfigurationLoader
    {
        /// <summary>
        /// Loads configuration from the input locations
        /// </summary>
        /// <param name="locations">Locations to load configurations from</param>
        /// <returns>Composed job configuration</returns>
        Task<IConfiguration> Load(ILocation[] locations);
    }
}
