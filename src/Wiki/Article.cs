using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Wiki.Resources;

namespace Wiki
{
    /// <summary>
    /// An article in a wiki.
    /// </summary>
    public sealed class Article : IHydrate
    {
        #region Data
        [JsonProperty("Content")]
        internal ObservableCollection<Content> _content = new ObservableCollection<Content>();
        #endregion
        #region Construction
        public Article()
        {
            Content = new ReadOnlyObservableCollection<Content>(_content);   
        }
        #endregion

        #region Properties

        /// <summary>
        /// The key for this article.
        /// </summary>
        /// <remarks>This should be null/empty for a new article.</remarks>
        public string Key { get; set; } = string.Empty;
        /// <summary>
        /// The title of this article.
        /// </summary>
        public string Title { get; set; } = string.Empty;
        /// <summary>
        /// When this article was initially created.
        /// </summary>
        public DateTimeOffset Created { get; set; } = DateTimeOffset.Now;
        /// <summary>
        /// When this was last modified.
        /// </summary>
        public DateTimeOffset Modified { get; set; } = DateTimeOffset.Now;
        /// <summary>
        /// The contents of this article
        /// </summary>
        [JsonIgnore]
        public ReadOnlyObservableCollection<Content> Content { get; }

        #endregion

        #region Methods
        /// <summary>
        /// Sets a content type for this record.  An article can contain each content type once.
        /// </summary>
        /// <typeparam name="TContentType">The type of content to set.</typeparam>
        /// <param name="content">The content to set.</param>
        /// <returns>This article (for fluent purposes)</returns>
        /// <remarks>
        /// Setting a content type to <see langword="null"/> effectively removes that 
        /// content type from the record.
        /// </remarks>
        public Article Set<TContentType>(TContentType content) where TContentType : Content
        {
            var existing = _content
                .FirstOrDefault(c => c.GetType() == typeof(TContentType));
            if(existing != null)
            {
                _content.Remove(existing);
            }
            if (content != null)
            {
                _content.Add(content);
            }

            return this;
        }
        #endregion

        #region Hydration
        /// <summary>
        /// Called right before content is serialized to stablize it.
        /// </summary>
        public void Dehydrate()
        {
            Content.Apply(c => c.Dehydrate(this));
        }

        /// <summary>
        /// Called right after content has been deserialized to allow it to "pop" out.
        /// </summary>
        public void Rehydrate()
        {
            Content.Apply(c => c.Rehydrate(this));
        }

        #endregion

        #region Overrides
        #endregion

        #region Helpers
        #endregion
    }
}
