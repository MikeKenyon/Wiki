using System;

namespace Wiki.Configuration
{
    public class RetryInfo
    {
        /// <summary>
        /// Whether this attempt is done and we should fail if not connected.
        /// </summary>
        public bool Done { get; set; }

        /// <summary>
        /// How long we should wait before we try next.
        /// </summary>
        public TimeSpan Delay { get; set; } = TimeSpan.Zero;

        /// <summary>
        /// How many times we've tried previously.
        /// </summary>
        public uint PriorAttempts { get; set; } = 0;

        /// <summary>
        /// A state object for use by the retry policy to store state on the attempt.
        /// </summary>
        /// <remarks>You should not manually edit this outside of implementing a policy.</remarks>
        public object State { get; set; }
    }
}