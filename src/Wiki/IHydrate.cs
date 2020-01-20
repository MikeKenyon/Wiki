using System;
using System.Collections.Generic;
using System.Text;

namespace Wiki
{
    /// <summary>
    /// Used by content in the wiki to mark pre/post serialization opportunities.
    /// </summary>
    interface IHydrate
    {
        /// <summary>
        /// Called right before content is serialized to stablize it.
        /// </summary>
        void Dehydrate();
        /// <summary>
        /// Called right after content has been deserialized to allow it to "pop" out.
        /// </summary>
        void Rehydrate();
    }
}
