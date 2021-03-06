﻿using System;
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
        protected internal virtual void Dehydrate(Article article)
        {

        }
        /// <summary>
        /// Acknowledges that the content has been restored from being serialized.
        /// </summary>
        /// <param name="article">The article this content is part of.</param>
        protected internal virtual void Rehydrate(Article article)
        { 
        }
    }
}
