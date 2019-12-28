using System;
using System.IO;
using System.Threading.Tasks;

namespace Wiki.FileSystem
{
    public class FileWikiFactory : WikiFactoryBase
    {
        public FileWikiFactory(Configuration.WikiConfiguration config) : base(config)
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
