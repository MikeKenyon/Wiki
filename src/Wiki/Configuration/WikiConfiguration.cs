using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wiki.Configuration
{
    /// <summary>
    /// The configuration of the wiki.
    /// </summary>
    public sealed class WikiConfiguration : IAddWikiContext
    {
        internal WikiConfiguration(IServiceCollection services)
        {
            _services = services;
        }

        /// <summary>
        /// Some providers support autosave, some do not.  When autosave is turned off, 
        /// <see cref="IWiki.SaveAsync"/> needs to be called expressly.
        /// </summary>
        public bool PreferAutosave { get; set; }

        /// <summary>
        /// Defines how and when we should attempt to retry operations.
        /// </summary>
        public IRetryPolicy RetryPolicy { get; set; } =
            new SlowDownRetryPolicy { Delay = TimeSpan.FromSeconds(0.5), NumberOfRetries = 10 };

        /// <summary>
        /// Parameters getting passed around for configuration.
        /// </summary>
        Dictionary<string, object> IAddWikiContext.Parameters { get; } = new Dictionary<string, object>();

        /// <summary>
        /// Service collection to modify.
        /// </summary>
        IServiceCollection IAddWikiContext.Services { get; }

        private IServiceCollection _services;
        void IAddWikiContext.SetWikiFactory<T>() 
        {
            _services.AddSingleton<IWikiFactory,T>();
        }
    }
}
