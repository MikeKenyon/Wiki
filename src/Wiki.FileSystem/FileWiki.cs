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
        private ZipFile _store;

        public ZipFile Store
        {
            get {
                if(_store == null)
                {
                    throw new InvalidOperationException("Have not called OpenAsync() yet. ");
                }
                return _store; }
            set { _store = value; }
        }

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

        /// <summary>
        /// File-based wikis should cache their contents.
        /// </summary>
        /// <returns><see langword="true"/>, indicating we should cache.</returns>
        protected override bool CachesContent()
        {
            return true;
        }

        public override Task SaveAsync()
        {
            Store.Save(File.FullName);
            return Task.CompletedTask;
        }

        protected override Task<Stream> GetStreamForKeyAsync(WikiEntryType type,
            string key, FileAccess access)
        {
            Stream stream = null;
            string path = null;
            switch (type)
            {
                case WikiEntryType.Metadata:
                    path = @$"Metadata/{key}";
                    break;
                case WikiEntryType.Article:
                    path = KeyToFilePath(key);
                    break;
                default:
                    throw new NotImplementedException($"Entry type {type} is not handled.");
            }
            var entry = (from e in Store.Entries
                         where string.Equals(e.FileName, path, StringComparison.OrdinalIgnoreCase)
                         select e).FirstOrDefault();
            if (entry != null)
            {
                stream = entry.OpenReader();
            }
            return Task.FromResult(stream);
        }

        protected override Task StoreAsync(Article article)
        {
            var json = ConvertEntryToJson(article);
            var path = KeyToFilePath(article.Key);
            //TODO: Test next line, not sure if the entry contains path.
            if (Store.ContainsEntry(path))
            {
                Store.UpdateEntry(path, json);
            }
            else
            {
                Store.AddEntry(path, json);
            }
            return Task.CompletedTask;
        }
        /// <summary>
        /// File-based wikis shouldn't need to have case sensitive keys.
        /// </summary>
        /// <returns></returns>
        protected override bool KeysCaseSensitive()
        {
            return false;
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
            var route = key.ToLowerInvariant();
            return key.Length switch
            {
                1 => $"{route}/{key}",
                2 => $"{route[0]}/{route[1]}/{key}",
                _ => $"{route[0]}/{route[1]}/{route[2]}/{key}"
            };
        }

        protected override Task StoreAsync(Metadata metadata)
        {
            var json = ConvertEntryToJson(metadata);
            var path = @$"Metadata/{metadata.GetType().Name}";
            //TODO: Test next line, not sure if the entry contains path.
            if (Store.ContainsEntry(path))
            {
                Store.UpdateEntry(path, json);
            }
            else
            {
                Store.AddEntry(path, json);
            }
            return Task.CompletedTask;
        }
        #endregion

    }
}
