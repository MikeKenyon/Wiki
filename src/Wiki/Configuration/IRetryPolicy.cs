using System;
using System.Collections.Generic;
using System.Text;

namespace Wiki.Configuration
{
    public interface IRetryPolicy
    {
        /// <summary>
        /// Updates a <see cref="RetryInfo"/> object to reflect current state.
        /// </summary>
        /// <param name="currentState">The return from the last check.</param>
        /// <returns>Current state of the retry cycle.</returns>
        RetryInfo ShouldRetry(RetryInfo currentState);
    }
}
