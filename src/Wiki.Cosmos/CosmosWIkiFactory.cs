using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Wiki.Configuration;

namespace Wiki.Cosmos
{
    internal class CosmosWikiFactory : WikiFactoryBase
    {
        public CosmosWikiFactory(WikiConfiguration config) : base(config)
        {
        }

        protected override Task CloseAsync(IWiki wiki)
        {
            throw new NotImplementedException();
        }

        protected override Task<IWiki> CreateAsync(string moniker)
        {
            throw new NotImplementedException();
        }

        protected override Task<bool> FoundAsync(string moniker)
        {
            throw new NotImplementedException();
        }

        protected override bool IsMine(IWiki wiki)
        {
            throw new NotImplementedException();
        }

        protected override bool IsValidMoniker(string moniker)
        {
            throw new NotImplementedException();
        }

        protected override Task<IWiki> OpenAsync(string moniker)
        {
            throw new NotImplementedException();
        }
    }
}
