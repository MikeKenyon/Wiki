using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Wiki
{
    /// <summary>
    /// Configuration is settings for how to get to and open/close a wiki.  Metadata are
    /// settings about the wiki stored _in_ the wiki.  This is the base class that you
    /// should derive off of to have the wiki store content about itself.
    /// </summary>
    public abstract class Metadata : IHydrate, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        #region Hydration
        /// <summary>
        /// Called right before content is serialized to stablize it.
        /// </summary>
        public virtual void Dehydrate()
        {
        }

        /// <summary>
        /// Called right after content has been deserialized to allow it to "pop" out.
        /// </summary>
        public virtual void Rehydrate()
        {
        }
        /// <summary>
        /// Flags that this has been changed.  If the containing wiki is marked as 
        /// <see cref="IWiki.Autosave"/>, this will result in a save operation.
        /// </summary>
        /// <param name="property">The property being updated, or leave blank for all.</param>
        protected void OnNotifyPropertyChanged([CallerMemberName]string property = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property ?? string.Empty));
        }
        #endregion
    }
}
