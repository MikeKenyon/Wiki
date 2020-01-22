using System;
using System.Collections.Generic;
using System.Text;

namespace Wiki
{
    /// <summary>
    /// A schema-less metadata object.
    /// </summary>
    public class GenericMetadata : Metadata
    {
        /// <summary>
        /// Generic data.
        /// </summary>
        public Dictionary<string, string> Data { get; }
            = new Dictionary<string, string>();

        public void FlagUpdate()
        {
            OnNotifyPropertyChanged();
        }
    }
}
