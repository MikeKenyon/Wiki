using System;
using System.Collections.Generic;
using System.Text;

namespace Wiki.Configuration
{
    /// <summary>
    /// Tries a fixed number of times, with a static delay between them.
    /// </summary>
    public sealed class SimpleRetryPolicy : IRetryPolicy
    {
        public uint NumberOfRetries { get; set; } = 5;
        public TimeSpan Delay { get; set; } = TimeSpan.FromSeconds(1);
        public RetryInfo ShouldRetry(RetryInfo currentState)
        {
            if (currentState.PriorAttempts >= NumberOfRetries)
            {
                currentState.Done = true;
            }
            else
            {
                currentState.Delay = Delay;
            }

            return currentState;
        }
    }
}
