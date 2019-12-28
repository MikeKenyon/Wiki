using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Wiki
{
    /// <summary>
    /// The interface for a wiki.  
    /// </summary>
    public interface IWiki : IAsyncDisposable
    {
        /// <summary>
        /// The moniker for this wiki.
        /// </summary>
        string Moniker { get; }
        /// <summary>
        /// Determines whether the wiki is connected or not.
        /// </summary>
        WikiConnectivityStatus Status { get; }
        /// <summary>
        /// Connects to the wiki if it wasn't already.
        /// </summary>
        /// <returns>Async handle.</returns>
        Task OpenAsync();
        /// <summary>
        /// Disconnects from the wiki (if connected).
        /// </summary>
        /// <returns>Async Handle.</returns>
        Task CloseAsync();
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
    }
}
