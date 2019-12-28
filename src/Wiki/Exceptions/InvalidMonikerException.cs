using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;

namespace Wiki
{
    public sealed class InvalidMonikerException : Exception
    {
        public InvalidMonikerException() : this(Resources.Text.InvalidMonikerExceptionDefault)
        {
        }

        public InvalidMonikerException(string message) : base(message)
        {
        }
        public InvalidMonikerException(string moniker, IWikiFactory factory) : 
            this(string.Format(CultureInfo.InvariantCulture,
                Resources.Text.InvalidMonikerExceptionFromMonikerAndFactory,
                moniker, factory.GetType().Name))
        {

        }

        public InvalidMonikerException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidMonikerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
