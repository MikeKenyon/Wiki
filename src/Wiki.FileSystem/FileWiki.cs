using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Wiki.FileSystem
{
    internal sealed class FileWiki : WikiBase
    {
        public FileWiki(FileInfo file, FileWikiFactory factory) : base(file.FullName, factory)
        {
            File = file;
        }

        public FileInfo File { get; }

        protected override Task StoreAsync(Article article)
        {
            throw new NotImplementedException();
        }

        internal Task LoadAsync()
        {
            throw new NotImplementedException();
        }
    }
}
