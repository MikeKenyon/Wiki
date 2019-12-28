using System;
using System.Collections.Generic;
using System.Text;

namespace Wiki
{
    /// <summary>
    /// The body content of an article.
    /// </summary>
    public sealed class Body : Content
    {
        /// <summary>
        /// The body content, as markdown.
        /// </summary>
        public string Markdown { get; set; }
    }
}
