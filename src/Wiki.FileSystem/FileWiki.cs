using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using SharpFileSystem;
using SharpFileSystem.FileSystems;
using SharpFileSystem.SharpZipLib;

namespace Wiki.FileSystem
{
    internal sealed class FileWiki : WikiBase
    {
        public FileWiki(FileInfo file, FileWikiFactory factory) : base(file.FullName, factory)
        {
            File = file;
        }

        public FileInfo File { get; }
        private SharpZipLibFileSystem Store { get; set; }

        protected override Task ConnectAsync()
        {
            var stream = File.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            Store = SharpZipLibFileSystem.Create(stream);
            return Task.CompletedTask;
        }

        protected override Task DisconnectAsync()
        {
            Store.Dispose();
            Store = null;
            return Task.CompletedTask;
        }

        internal async Task LoadAsync()
        {
            foreach(var entity in Store.GetEntitiesRecursive(FileSystemPath.Root))
            {
                var article = await GetAsync(entity.EntityName);
                await Index(article);
            }
        }

        public override Task SaveAsync()
        {
            Store.ZipFile.CommitUpdate();
            return Task.CompletedTask;
        }

        protected override Task<Stream> GetStreamForKeyAsync(string key, FileAccess access)
        {
            var path = KeyToFilePath(key);
            return Task.FromResult(Store.OpenFile(path, access));
        }

        protected async override Task StoreAsync(Article article)
        {
            var stream = await GetStreamForKeyAsync(article.Key, FileAccess.Write);
            stream.Position = 0;
            await WriteArticleToStream(article, stream);
        }
        /// <summary>
        /// Removes a record from the underlying stoe.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected override Task RemoveAsync(string key)
        {
            Store.Delete(KeyToFilePath(key));
            return Task.CompletedTask;
        }

        #region Helpers
        private Task Index(Article article)
        {
            //TODO: Need to index the article.
            return Task.CompletedTask;
        }
        private FileSystemPath KeyToFilePath(string key)
        {
            var path = key.Length switch
            {
                1 => $"{key}/{key}",
                2 => $"{key[0]}/{key[1]}/{key}",
                _ => $"{key[0]}/{key[1]}/{key[2]}/{key}"
            };
            return FileSystemPath.Parse(path);
        }


        #endregion

    }
}
