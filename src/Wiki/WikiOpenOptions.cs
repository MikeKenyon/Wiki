namespace Wiki
{
    /// <summary>
    /// Options when opening a wiki.
    /// </summary>
    public class WikiOpenOptions
    {
        /// <summary>
        /// What should be done if the wiki requested is a valid address, but is not found.
        /// </summary>
        public WikiMissingBehavior NotFound { get; set; } = WikiMissingBehavior.Create;

        /// <summary>
        /// If the moniker passed is invalid, should we throw.
        /// </summary>
        public bool ThrowOnInvalid { get; set; } = true;
        public bool ThrowOnFailureToOpen { get; internal set; }
    }
}