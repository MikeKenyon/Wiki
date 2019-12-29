using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private bool IsFileNew { get; set; }
        private ZipFile Store { get; set; }

        protected override Task ConnectAsync()
        {
            if(File.Exists)
            {
                Store = new ZipFile(File.FullName, Encoding.UTF8);
            }
            else
            {
                IsFileNew = true;
                Store = new ZipFile();
            }
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
            foreach(var entry in Store)
            {
                if(entry.IsDirectory)
                {
                    continue;
                }
                var article = await GetAsync(entry.FileName);
                await Index(article);
            }
        }

        public override Task SaveAsync()
        {
            Store.Save(File.FullName);
            return Task.CompletedTask;
        }

        protected override Task<Stream> GetStreamForKeyAsync(string key, FileAccess access)
        {
            Stream stream = null;
            var path = KeyToFilePath(key);
            var entry = (from e in Store.Entries
                         where e.FileName == key
                         select e).FirstOrDefault();
            if(entry != null)
            {
                stream = entry.OpenReader();
            }
            return Task.FromResult(stream);
        }
        /// <summary>
        /// Updates or inserts an article based off of whether or not it's 
        /// <see cref="Article.Key"/> already exists.
        /// </summary>
        /// <param name="article">The article to upsert.</param>
        /// <returns>Async key.</returns>
        public override async Task UpsertAsync(Article article)
        {
            Stablize(article);
            var json = ConvertArticleToJson(article);
            var path = KeyToFilePath(article.Key);
            //TODO: Test next line, not sure if the entry contains path.
            if(Store.ContainsEntry(path))
            {
                Store.UpdateEntry(path, json);
            }
            else
            {
                Store.AddEntry(path, json);
            }
            if (Autosave)
            {
                await SaveAsync();
            }
        }

        protected override Task StoreAsync(Article article)
        {
            throw new NotImplementedException("StoreAsync() isn't implemented, use UpsertAsync()");
        }
        /// <summary>
        /// Removes a record from the underlying stoe.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected override Task RemoveAsync(string key)
        {
            Store.RemoveEntry(KeyToFilePath(key));
            return Task.CompletedTask;
        }

        #region Helpers
        private Task Index(Article article)
        {
            //TODO: Need to index the article.
            return Task.CompletedTask;
        }

        private string KeyToFilePath(string key)
        {
            return key.Length switch
            {
                1 => $"{key}/{key}",
                2 => $"{key[0]}/{key[1]}/{key}",
                _ => $"{key[0]}/{key[1]}/{key[2]}/{key}"
            };
        }
        #endregion

    }
}
