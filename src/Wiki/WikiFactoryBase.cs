using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Wiki.Configuration;

namespace Wiki
{
    public abstract class WikiFactoryBase : IWikiFactory
    {
        protected internal WikiConfiguration Config { get; }

        protected WikiFactoryBase(Configuration.WikiConfiguration config)
        {
            Config = config;
        }

        #region External Interface

        /// <summary>
        /// Closes down the wiki.
        /// </summary>
        /// <param name="wiki">The wiki to close, it is worthless hereafter.</param>
        /// <param name="options">Options on closing the wiki.</param>
        /// <returns>An async handle.</returns>
        public async Task CloseWikiAsync(IWiki wiki, WikiCloseOptions options)
        {
            if(IsMine(wiki))
            {
                var done = false;
                var retry = Config.RetryPolicy;
                var info = new RetryInfo();
                while(!done)
                {
                    try
                    {
                        await CloseAsync(wiki);
                        done = true;
                    }
                    catch {
                        info.PriorAttempts++;
                        info = retry.ShouldRetry(info);
                        if(info.Done)
                        {
                            break;
                        }
                        else
                        {
                            await Task.Delay(info.Delay);
                        }
                        //TODO: Log here.
                    }
                }
                if(!done && options.ThrowOnFailureToClose)
                {
                    throw new WikiUnopenedException(wiki.Moniker, this);
                }
            }
            else if (options.ThrowOnInvalid)
            {
                throw new InvalidOperationException(Resources.Text.WikiFactoryBaseCloseWikiNotMine);
            }
        }


        /// <summary>
        /// Opens a given wiki up.  
        /// </summary>
        /// <param name="moniker">
        /// The name of the wiki to open.  
        /// This is a path, name or URI.  Different implementations can handle different types.
        /// </param>
        /// <param name="options">Options to the open operation.</param>
        /// <returns>The opened wiki.</returns>
        public async Task<IWiki> OpenWikiAsync(string moniker, WikiOpenOptions options)
        {
            IWiki wiki = null;
            if(IsValidMoniker(moniker))
            {
                if (await FoundAsync(moniker))
                {
                    var done = false;
                    var retry = Config.RetryPolicy;
                    var info = new RetryInfo();
                    while (!done)
                    {
                        try
                        {
                            wiki = await OpenAsync(moniker);
                            done = true;
                        }
                        catch
                        {
                            info.PriorAttempts++;
                            info = retry.ShouldRetry(info);
                            if (info.Done)
                            {
                                break;
                            }
                            else
                            {
                                await Task.Delay(info.Delay);
                            }
                            //TODO: Log here.
                        }
                    }
                    if (!done && options.ThrowOnFailureToOpen)
                    {
                        throw new WikiUnopenedException(wiki.Moniker, this);
                    }
                }
                else
                {
                    switch (options.NotFound)
                    {
                        case WikiMissingBehavior.Create:
                            wiki = await CreateAsync(moniker);
                            break;
                        case WikiMissingBehavior.Nothing:
                            wiki = null;
                            break;
                        case WikiMissingBehavior.Throw:
                            throw new WikiNotFoundException(moniker);
                    };
                }
            }
            else if(options.ThrowOnInvalid)
            {
                throw new InvalidMonikerException(moniker, this);
            }
            return wiki;
        }
        #endregion

        #region Child Interface
        protected abstract Task<IWiki> CreateAsync(string moniker);
        protected abstract Task<IWiki> OpenAsync(string moniker);
        protected abstract Task<bool> FoundAsync(string moniker);
        protected abstract bool IsMine(IWiki wiki);
        protected abstract Task CloseAsync(IWiki wiki);


        /// <summary>
        /// Determines if this moniker is a valid that could be opened by this factory.
        /// </summary>
        /// <param name="moniker">The moniker to check.</param>
        /// <returns><see langword="true"/> if it's valid, otherwise, <see langword="false"/>.</returns>
        protected abstract bool IsValidMoniker(string moniker);
        #endregion
    }
}
