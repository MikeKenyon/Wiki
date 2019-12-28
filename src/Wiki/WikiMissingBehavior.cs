namespace Wiki
{
    public enum WikiMissingBehavior
    {
        /// <summary>
        /// Returns <see langword="null"/> when not found.
        /// </summary>
        Nothing,
        /// <summary>
        /// Creates the missing wiki.
        /// </summary>
        Create,
        /// <summary>
        /// Throws a <see cref="WikiNotFoundException"/> when the wiki wasn't present.
        /// </summary>
        Throw
    }
}