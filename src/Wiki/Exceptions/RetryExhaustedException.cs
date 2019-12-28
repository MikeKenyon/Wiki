using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Wiki
{
    public sealed class RetryExhaustedException : Exception
    {
        public RetryExhaustedException() : base(Resources.Text.RetryExhaustedExceptionDefault)
        {
        }
        public RetryExhaustedException(uint failures) : base(
            string.Format(Resources.Text.RetryExhaustedExceptionFromFailures, failures))
        { 
        }

        public RetryExhaustedException(string message) : base(message)
        {
        }

        public RetryExhaustedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RetryExhaustedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
