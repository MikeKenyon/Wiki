using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Wiki
{
    /// <summary>
    /// The interface for a wiki.  
    /// </summary>
    public interface IWiki : IDisposable
    {
        #region Connectivity
        /// <summary>
        /// The moniker that uniquely defines this wiki (usually, but not always a URI).
        /// </summary>
        string Moniker { get; }
        /// <summary>
        /// Determines whether the wiki is connected or not.
        /// </summary>
        WikiConnectivityStatus Status { get; }
        /// <summary>
        /// Checks if we're in autosave mode, and/or requests that mode be changed.  If we're 
        /// not in autosave mode, <see cref="SaveAsync"/> needs to be called before close.
        /// </summary>
        bool Autosave { get; set; }
        /// <summary>
        /// Connects to the wiki if it wasn't already.
        /// </summary>
        /// <returns>Async handle.</returns>
        Task OpenAsync();
        /// <summary>
        /// Saves all outstanding changes.
        /// </summary>
        /// <returns>Async handle.</returns>
        Task SaveAsync();
        /// <summary>
        /// Disconnects from the wiki (if connected).
        /// </summary>
        /// <returns>Async Handle.</returns>
        Task CloseAsync();
        #endregion

        #region Articles
        /// <summary>
        /// Gets an article by it's key.
        /// </summary>
        /// <param name="key">The key to look for.</param>
        /// <returns>The article for that key.</returns>
        Task<Article> GetAsync(string key);

        /// <summary>
        /// Updates or inserts an article based off of whether or not it's 
        /// <see cref="Article.Key"/> already exists.
        /// </summary>
        /// <param name="article">The article to upsert.</param>
        /// <returns>Async key.</returns>
        Task UpsertAsync(Article article);

        /// <summary>
        /// Delets the article with the given key.
        /// </summary>
        /// <param name="key">The key to delete.</param>
        /// <returns>Async key.</returns>
        Task DeleteAsync(string key);
        #endregion

        #region Metadata
        /// <summary>
        /// Obtain a specific type of metadata about this wiki.  If it doesn't exist,
        /// it does now.
        /// </summary>
        /// <typeparam name="TMetadata">Type of metadata you're interested in.</typeparam>
        /// <returns>The one instance of that metadata for this wiki.</returns>
        Task<TMetadata> Metadata<TMetadata>()
            where TMetadata : Metadata, new();
        #endregion

        #region Std 2.1 Future Compatibility
        ValueTask DisposeAsync();
        #endregion
    }
}
