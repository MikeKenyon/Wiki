using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Wiki.FileSystem
{
    public class FileWikiFactory : WikiFactoryBase
    {
        public FileWikiFactory(Configuration.WikiConfiguration config) : base(config)
        {

        }
        protected override async Task<IWiki> OpenAsync(string moniker)
        {
            var wiki = new FileWiki(new FileInfo(moniker), this);
            await wiki.LoadAsync();
            return wiki;
        }

        protected override Task<IWiki> CreateAsync(string moniker)
        {
            var wiki = new FileWiki(new FileInfo(moniker), this);
            return Task.FromResult<IWiki>(wiki);
        }

        protected override Task<bool> FoundAsync(string moniker)
        {
            return Task.FromResult(File.Exists(moniker));
        }

        protected override bool IsMine(IWiki wiki)
        {
            return wiki is FileWiki;
        }

        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        protected override bool IsValidMoniker(string moniker)
        {
            try
            {
                Path.GetFullPath(moniker);
                return !Directory.Exists(moniker); // Must therefore be a legal file name.
            }
            catch // Throws exceptions on invalid paths.
            {
                return false;
            }
        }

    }
}
