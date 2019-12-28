using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;

namespace Wiki
{
    public sealed class WikiUnopenedException : Exception
    {
        public WikiUnopenedException() : this(Resources.Text.WikiUnopenedExceptionDefault)
        {
        }

        public WikiUnopenedException(string message) : base(message)
        {
        }

        public WikiUnopenedException(string moniker, IWikiFactory factory) : 
            this(string.Format(CultureInfo.InvariantCulture, 
                Resources.Text.WikiUnopenedExceptionFromMonikerAndFactory, 
                moniker, factory.GetType().Name))
        {
        }

        public WikiUnopenedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected WikiUnopenedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
