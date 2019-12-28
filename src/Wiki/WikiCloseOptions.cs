namespace Wiki
{
    public class WikiCloseOptions
    {
        /// <summary>
        /// If you attempt to close a wiki with the wrong factory, should an 
        /// <see cref="System.InvalidOperationException"/> be thrown.
        /// </summary>
        public bool ThrowOnInvalid { get; set; } = true;

        /// <summary>
        /// If the system cannot close the instance, should an error be raised to surface the
        /// issue.
        /// </summary>
        public bool ThrowOnFailureToClose { get; set; } = true;
    }
}