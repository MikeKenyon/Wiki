using System;
using System.Collections.Generic;
using System.Text;

namespace Wiki
{
    /// <summary>
    /// Describes a chunk of content associatd with an article.
    /// </summary>
    public abstract class Content 
    {

        /// <summary>
        /// Prepares the content to be saved.  
        /// </summary>
        /// <param name="article">The article this is being saved as part of.</param>
        protected internal virtual void Stablize(Article article)
        {

        }
    }
}
