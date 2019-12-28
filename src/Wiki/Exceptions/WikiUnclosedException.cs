using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;

namespace Wiki
{
    public sealed class WikiUnclosedException : Exception
    {
        public WikiUnclosedException() : this(Resources.Text.WikiUnclosedExceptionDefault)
        {
        }

        public WikiUnclosedException(string message) : base(message)
        {
        }

        public WikiUnclosedException(string moniker, IWikiFactory factory) : 
            this(string.Format(CultureInfo.InvariantCulture, 
                Resources.Text.WikiUnclosedExceptionFromMonikerAndFactory, 
                moniker, factory.GetType().Name))
        {
        }

        public WikiUnclosedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected WikiUnclosedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
