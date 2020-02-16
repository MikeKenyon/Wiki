using System;
using System.Collections.Generic;
using System.Text;

namespace Wiki.Configuration
{
    /// <summary>
    /// This implements a retry policy that slows down as the number of retries extends.
    /// </summary>
    public sealed class SlowDownRetryPolicy : IRetryPolicy
    {
        public uint NumberOfRetries { get; set; } = 10;
        public TimeSpan Delay { get; set; } = TimeSpan.FromSeconds(0.5);

        public IEnumerable<uint> Multipliers { get; set; } = new Internal.FibonacciSequence();

        public RetryInfo ShouldRetry(RetryInfo currentState)
        {
            if(currentState.PriorAttempts >= NumberOfRetries)
            {
                currentState.Done = true;
            }
            else
            {
                if(currentState.State == null)
                {
                    currentState.State = Multipliers.GetEnumerator();
                }
                if(currentState.State is IEnumerator<uint> seq)
                {
                    seq.MoveNext();
                    var multiple = seq.Current;
                    //TODO: Std2.1 currentState.Delay = Delay * multiple;
                    currentState.Delay = TimeSpan.FromMilliseconds( Delay.TotalMilliseconds * multiple);
                }
                else
                {
                    throw new ArgumentException("Dont' screw with the internal state of the retry.", "State");
                }
            }

            return currentState;
        }
    }
}
