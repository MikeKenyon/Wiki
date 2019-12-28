using Microsoft.Extensions.DependencyInjection;
using System;
using Wiki.Configuration;

namespace Wiki.Cosmos
{
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Setups CosmosDB as being the backing store for wikis
        /// </summary>
        /// <param name="config"></param>
        /// <param name="url"></param>
        /// <param name="accessToken"></param>
        public static void WithCosmos(this IAddWikiContext config,
            Uri url, string accessToken)
        {
            config.SetWikiFactory<CosmosWikiFactory>();

            config.Parameters[Constants.Url] = url;
            config.Parameters[Constants.PAT] = accessToken;
        }
    }
}
