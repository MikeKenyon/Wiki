using System;
using System.Collections.Generic;
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
        public WikiBase(string moniker, WikiFactoryBase factory)
        {
            if (string.IsNullOrWhiteSpace(moniker))
            {
                throw new ArgumentException("No moniker provided.", nameof(moniker));
            }

            Moniker = moniker;
            Factory = factory;
        }

        public string Moniker { get; }
        protected WikiFactoryBase Factory { get; }
        public WikiConnectivityStatus Status { get; private set; }

        public async Task CloseAsync()
        {
            if (Status == WikiConnectivityStatus.Connected)
            {
                Status = WikiConnectivityStatus.Disconnecting;
                await DisconnectAsync();
                Status = WikiConnectivityStatus.Disconnected;
            }
        }

        public Task DeleteAsync(string key)
        {
            throw new NotImplementedException();
        }

        public ValueTask DisposeAsync()
        {
            return new ValueTask(CloseAsync());
        }

        public Task<Article> GetAsync(string key)
        {
            throw new NotImplementedException();
        }

        public async Task OpenAsync()
        {
            if (Status == WikiConnectivityStatus.Disconnected)
            {
                Status = WikiConnectivityStatus.Connecting;
                await ConnectAsync();
                Status = WikiConnectivityStatus.Connected;
            }
        }

        public async Task UpsertAsync(Article article)
        {
            Stablize(article);
            await StoreAsync(article);
        }

        #region Child interface
        /// <summary>
        /// Stores a pristine article in the wiki.
        /// </summary>
        /// <param name="article">The article to store, no checks required.</param>
        /// <returns>Async handle.</returns>
        protected abstract Task StoreAsync(Article article);

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
        #endregion


        #region Helpers
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
        private void Stablize(Article article)
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
                    article.Key = article.Title.Sanatize();
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
            article.Body.Apply(content => content.Stablize(article));
            // Modified
            article.Modified = DateTimeOffset.Now;
        }

        #endregion
    }
}
