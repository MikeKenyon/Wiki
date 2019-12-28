using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wiki.Configuration
{
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Adds basic configuration for a wiki system.  You should fluently call additional
        /// configuration methods that modify the configuration.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static WikiConfiguration AddWiki(this IServiceCollection services)
        {
            var config = new WikiConfiguration(services);
            services.AddSingleton<WikiConfiguration>(config);
            return config;
        }

        /// <summary>
        /// Adds a retry policy with a fixed delay mechanism.
        /// </summary>
        /// <param name="config">Configuration to modify.</param>
        /// <param name="numberOfRetries">Number of times to try.</param>
        /// <param name="delayBetween">Delay between tries (default 1 second).</param>
        /// <returns>Same configuration.</returns>
        public static WikiConfiguration WithSimpleRetry(this WikiConfiguration config,
            uint numberOfRetries = 5, TimeSpan? delayBetween = null)
        {
            delayBetween = delayBetween ?? TimeSpan.FromSeconds(1);
            config.RetryPolicy = new SimpleRetryPolicy
            {
                NumberOfRetries = numberOfRetries,
                Delay = delayBetween.Value
            };
            return config;
        }
        /// <summary>
        /// Adds a retry policy with a delay mechanism that slows down as retries proceed.
        /// </summary>
        /// <param name="config">Configuration to modify.</param>
        /// <param name="numberOfRetries">Number of times to try.</param>
        /// <param name="baseDelay">Delay between tries (default 1/2 second).</param>
        /// <param name="multipliers">Set of numbers multiplied by the <see cref="baseDelay"/>
        /// to calculate the actual delay.  Defaults to the Fibonacci sequence.</param>
        /// <returns>Same configuration.</returns>
        public static WikiConfiguration WithSlowingRetry(this WikiConfiguration config,
            uint numberOfRetries = 10, TimeSpan? baseDelay = null,
            IEnumerable<uint> multipliers = null)
        {
            baseDelay = baseDelay ?? TimeSpan.FromSeconds(0.5);
            config.RetryPolicy = new SlowDownRetryPolicy
            {
                NumberOfRetries = numberOfRetries,
                Delay = baseDelay.Value,
                Multipliers = multipliers ?? new Wiki.Internal.FibonacciSequence()
            };
            return config;
        }
    }
}
