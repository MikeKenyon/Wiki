using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Wiki
{
    public interface IWikiFactory
    {
        /// <summary>
        /// Opens a given wiki up.  
        /// </summary>
        /// <param name="moniker">
        /// The name of the wiki to open.  
        /// This is a path, name or URI.  Different implementations can handle different types.
        /// </param>
        /// <param name="options">Options to the open operation.</param>
        /// <returns>The opened wiki.</returns>
        Task<IWiki> OpenWikiAsync(string moniker, WikiOpenOptions options);

        /// <summary>
        /// Closes down the wiki.
        /// </summary>
        /// <param name="wiki">The wiki to close, it is worthless hereafter.</param>
        /// <param name="options">Options on closing the wiki.</param>
        /// <returns>An async handle.</returns>
        Task CloseWikiAsync(IWiki wiki, WikiCloseOptions options);
    }
}
