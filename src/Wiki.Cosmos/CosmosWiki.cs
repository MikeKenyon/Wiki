using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Wiki.Configuration;

namespace Wiki.Cosmos
{
    internal class CosmosWiki : WikiBase
    {
        internal CosmosWiki(string moniker, CosmosWikiFactory factory) : base(moniker, factory) { }

        public override Task SaveAsync()
        {
            throw new NotImplementedException();
        }

        protected override Task<Stream> GetStreamForKeyAsync(string key, FileAccess access)
        {
            throw new NotImplementedException();
        }

        protected override Task RemoveAsync(string key)
        {
            throw new NotImplementedException();
        }

        protected override Task StoreAsync(Article article)
        {
            throw new NotImplementedException();
        }
    }
}
