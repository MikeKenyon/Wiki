using System;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Wiki.Resources;

namespace Wiki
{
    /// <summary>
    /// An article in a wiki.
    /// </summary>
    public sealed class Article 
    {
        #region Construction
        #endregion

        #region Properties

        /// <summary>
        /// The key for this article.
        /// </summary>
        /// <remarks>This should be null/empty for a new article.</remarks>
        public string Key { get; set; } = string.Empty;
        /// <summary>
        /// The title of this article.
        /// </summary>
        public string Title { get; set; } = string.Empty;
        /// <summary>
        /// When this article was initially created.
        /// </summary>
        public DateTimeOffset Created { get; set; } = DateTimeOffset.Now;
        /// <summary>
        /// When this was last modified.
        /// </summary>
        public DateTimeOffset Modified { get; set; } = DateTimeOffset.Now;
        /// <summary>
        /// The contents of this article
        /// </summary>
        public ObservableCollection<Content> Body { get; } = new ObservableCollection<Content>();
        #endregion

        #region Methods
        #endregion

        #region Overrides
        #endregion

        #region Helpers
        #endregion
    }
}
