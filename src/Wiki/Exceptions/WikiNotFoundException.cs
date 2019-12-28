using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;

namespace Wiki
{
    public sealed class WikiNotFoundException : Exception
    {
        public WikiNotFoundException() : base(Resources.Text.WikiNotFoundExceptionDefault)
        {
        }

        public WikiNotFoundException(string moniker) : base(
            string.Format(CultureInfo.InvariantCulture, 
                Resources.Text.WikiNotFoundExceptionMoniker, moniker))
        {
        }

        public WikiNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected WikiNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
