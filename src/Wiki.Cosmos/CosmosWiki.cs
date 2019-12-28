using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Wiki.Configuration;

namespace Wiki.Cosmos
{
    class CosmosWiki : WikiBase
    {
        internal CosmosWiki(string moniker, CosmosWikiFactory factory) : base(moniker, factory) { }
        protected override Task StoreAsync(Article article)
        {
            throw new NotImplementedException();
        }
    }
}
