using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Wiki.Configuration;

namespace Wiki
{
    /// <summary>
    /// Implementation helper for creating wikis.
    /// </summary>
    public abstract class WikiBase : IWiki
    {
        #region Creation
        static readonly JsonSerializerSettings Serialization = new JsonSerializerSettings
        {
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
            TypeNameHandling = TypeNameHandling.Auto
        };
        /// <summary>
        /// Creates a wiki.
        /// </summary>
        /// <param name="moniker">The address for this wiki.</param>
        /// <param name="factory">The factory that produced it.</param>
        public WikiBase(string moniker, WikiFactoryBase factory)
        {
            if (string.IsNullOrWhiteSpace(moniker))
            {
                throw new ArgumentException("No moniker provided.", nameof(moniker));
            }

            //TODO: Add Logger to this lazy init.
            CacheRepo = new Lazy<MemoryCache>(
                () => new MemoryCache(
                    new OptionsWrapper<MemoryCacheOptions>(
                        new MemoryCacheOptions())));

            Moniker = moniker;
            Factory = factory ?? throw new ArgumentNullException(nameof(factory));
            Autosave = factory.Config.PreferAutosave;
        }
        protected void InvalidCache(string key = null)
        {
            if(CachesContent())
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    CacheRepo.Value.Compact(1.00);
                }
                else
                {
                    CacheRepo.Value.Remove(key);
                }
            }
        }
        #endregion

        #region Data
        /// <summary>
        /// The factory that generated this wiki.
        /// </summary>
        protected WikiFactoryBase Factory { get; }
        #endregion

        #region Lifecycle
        /// <summary>
        /// The moniker that uniquely defines this wiki (usually, but not always a URI).
        /// </summary>
        public string Moniker { get; }
        /// <summary>
        /// Determines whether the wiki is connected or not.
        /// </summary>
        public WikiConnectivityStatus Status { get; private set; } = WikiConnectivityStatus.Disconnected;

        private bool _autosave = false;

        /// <summary>
        /// Checks if we're in autosave mode, and/or requests that mode be changed.  If we're 
        /// not in autosave mode, <see cref="SaveAsync"/> needs to be called before close.
        /// </summary>
        public bool Autosave
        {
            get
            {
                return CannotAutosave() ? false : (_autosave || MustAutosave());
            }
            set
            {
                _autosave = value;
            }
        }

        /// <summary>
        /// Disconnects from the wiki (if connected).
        /// </summary>
        /// <returns>Async Handle.</returns>
        public async Task CloseAsync()
        {
            if (Status == WikiConnectivityStatus.Connected)
            {
                Status = WikiConnectivityStatus.Disconnecting;
                await DisconnectAsync();
                Status = WikiConnectivityStatus.Disconnected;
            }
        }
        /// <summary>
        /// Saves all outstanding changes.
        /// </summary>
        /// <returns>Async handle.</returns>
        public abstract Task SaveAsync();

        /// <summary>
        /// Disposes of this reference to the wiki, commiting any changes.
        /// </summary>
        /// <returns></returns>
        public ValueTask DisposeAsync()
        {
            return new ValueTask(CloseAsync());
        }

        /// <summary>
        /// Connects to the wiki if it wasn't already.
        /// </summary>
        /// <returns>Async handle.</returns>
        public async Task OpenAsync()
        {
            if (Status == WikiConnectivityStatus.Disconnected)
            {
                Status = WikiConnectivityStatus.Connecting;
                await ConnectAsync();
                Status = WikiConnectivityStatus.Connected;
            }
        }
        #endregion

        #region Article Methods
        /// <summary>
        /// Gets an article by it's key.
        /// </summary>
        /// <param name="key">The key to look for.</param>
        /// <returns>The article for that key.</returns>
        public async virtual Task<Article> GetAsync(string key)
        {
            key = ConformKey(key);
            var article = CheckCache(key);
            if (article == null)
            {
                var stream = await GetStreamForKeyAsync(key, FileAccess.Read);
                if (stream != null)
                {
                    article = GetArticleFromStream(stream);
                    await stream.DisposeAsync();
                    Cache(article);
                }
            }
            return article;
        }

        /// <summary>
        /// Updates or inserts an article based off of whether or not it's 
        /// <see cref="Article.Key"/> already exists.
        /// </summary>
        /// <param name="article">The article to upsert.</param>
        /// <returns>Async key.</returns>
        public virtual async Task UpsertAsync(Article article)
        {
            Stablize(article);
            Cache(article);
            await StoreAsync(article);
            if(Autosave)
            {
                await SaveAsync();
            }
        }
        /// <summary>
        /// Delets the article with the given key.
        /// </summary>
        /// <param name="key">The key to delete.</param>
        /// <returns>Async key.</returns>
        public async Task DeleteAsync(string key)
        {
            await RemoveAsync(key);
            if(Autosave)
            {
                await SaveAsync();
            }
        }

        #endregion


        #region Child interface
        /// <summary>
        /// Gets the stream containing the guts of a stream.  Not required if you also override 
        /// <see cref="GetAsync(string)"/>.
        /// </summary>
        /// <param name="key">The key to get a stream for.</param>
        /// <param name="access">The way to open the stream.</param>
        /// <returns>The stream that contains the results, or null if the key doesn't exist.</returns>
        protected abstract Task<Stream> GetStreamForKeyAsync(string key, FileAccess access);
        /// <summary>
        /// Stores a pristine article in the wiki.
        /// </summary>
        /// <param name="article">The article to store, no checks required.</param>
        /// <returns>Async handle.</returns>
        protected abstract Task StoreAsync(Article article);

        /// <summary>
        /// Removes a record from the underlying stoe.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected abstract Task RemoveAsync(string key);


        /// <summary>
        /// Connects to a remote source (if required).
        /// </summary>
        /// <returns>Async handle</returns>
        protected virtual Task ConnectAsync()
        {
            return Task.CompletedTask;
        }
        /// <summary>
        /// Disconnects from a remote source (if required).
        /// </summary>
        /// <returns>Async handle</returns>
        protected virtual Task DisconnectAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Override this method to return <see langword="true"/> if this provider requires auto-save on.
        /// </summary>
        /// <returns>Whether or not to auto-save.</returns>
        protected virtual bool MustAutosave()
        {
            return false;
        }
        /// <summary>
        /// Override this method to return <see langword="true"/> if this provider cannot have auto-save on.
        /// </summary>
        /// <returns>Whether auto-save is supported.</returns>
        protected virtual bool CannotAutosave()
        {
            return false;
        }
        /// <summary>
        /// Indicates if a cache should be made of content loaded.
        /// </summary>
        /// <returns>Whether or not to create/maintain such a cache.</returns>
        protected virtual bool CachesContent()
        {
            return false;
        }
        protected virtual bool KeysCaseSensitive()
        {
            return true;
        }
        #endregion

        #region Helpers

        #region Caching
        /// <summary>
        /// Checks for the article in the internal cache.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected Article CheckCache(string key)
        {
            Article response = null;
            if(CachesContent())
            {
                if(!CacheRepo.Value.TryGetValue(key, out response))
                {
                    response = null;
                }
            }
            return response;
        }
        /// <summary>
        /// Caches the value.
        /// </summary>
        /// <param name="article">The article to cache.</param>
        protected void Cache(Article article)
        {
            if(CachesContent())
            {
                CacheRepo.Value.Set(ConformKey(article.Key), article);
            }
        }
        private Lazy<MemoryCache> CacheRepo { get; set; }
        #endregion

        #region Keys
        /// <summary>
        /// Conforms the key to rules.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string ConformKey(string key)
        {
            return KeysCaseSensitive() ? key : key.ToLowerInvariant();
        }
        #endregion

        /// <summary>
        /// Parses an article from a stream.
        /// </summary>
        /// <param name="stream">The stream to parse.</param>
        /// <returns>The contents of the stream.</returns>
        private Article GetArticleFromStream(Stream stream)
        {
            using var reader = new StreamReader(stream, Encoding.UTF8);
            var json = reader.ReadToEnd();
            return JsonConvert.DeserializeObject<Article>(json, Serialization);
        }

        /// <summary>
        /// Writes an article to a stream.
        /// </summary>
        /// <param name="article">The article to write.</param>
        /// <param name="stream">The stream to write it to.</param>
        protected async Task WriteArticleToStream(Article article, Stream stream)
        {
            var json = ConvertArticleToJson(article);
            var bytes = Encoding.UTF8.GetBytes(json);
            await stream.WriteAsync(bytes);
        }

        /// <summary>
        /// Converts an article to JSON.
        /// </summary>
        /// <param name="article">The article to convert.</param>
        /// <returns>The cardinal JSON form of it.</returns>
        protected string ConvertArticleToJson(Article article)
        {
            return JsonConvert.SerializeObject(article, Formatting.None, Serialization);
        }

        protected IRetryPolicy Retry { get { return Factory.Config.RetryPolicy; } }
        /// <summary>
        /// Confirms that we're connected when the method ends.
        /// </summary>
        /// <returns>Async handle.</returns>
        protected async Task EnsureConnected()
        {
            var info = new RetryInfo();
            while (!info.Done)
            {
                // Wait as long as we should.
                await Task.Delay(info.Delay);
                switch (Status)
                {
                    case WikiConnectivityStatus.Connected:
                        return;
                    case WikiConnectivityStatus.Disconnected:
                        await OpenAsync();
                        break;
                }
                info.PriorAttempts++;
            }
            if(Status != WikiConnectivityStatus.Connected)
            {
                throw new RetryExhaustedException(info.PriorAttempts);
            }
        }
        /// <summary>
        /// Ensures the article is ready to be saved.
        /// </summary>
        /// <param name="article"></param>
        protected void Stablize(Article article)
        {
            // Key
            if(string.IsNullOrWhiteSpace(article.Key))
            {
                if (string.IsNullOrWhiteSpace(article.Title))
                {
                    article.Key = Guid.NewGuid().ToString("N");
                }
                else
                {
                    article.Key = ConformKey(article.Title.Sanatize());
                }
            }
            article.Key = article.Key.Trim();
            // Title -- intentionally after key.
            if (string.IsNullOrWhiteSpace(article.Title))
            {
                article.Title = Resources.Text.DefaultArticleName;
            }
            article.Title = article.Title.Trim();
            // Body
            article.Content.Apply(content => content.Stablize(article));
            // Modified
            article.Modified = DateTimeOffset.Now;
        }

        #endregion
    }
}
