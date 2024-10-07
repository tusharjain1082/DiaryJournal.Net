namespace AngleSharp.Dom
{
    using AngleSharp.Attributes;

    /// <summary>
    /// Defines the document readiness.
    /// </summary>
    public enum DocumentReadyState : System.Byte
    {
        /// <summary>
        /// The document is still loading.
        /// </summary>
        [DomName("loading")]
        Loading,
        /// <summary>
        /// The document is interactive, i.e. interaction possible.
        /// </summary>
        [DomName("interactive")]
        Interactive,
        /// <summary>
        /// Loading is complete.
        /// </summary>
        [DomName("complete")]
        Complete
    }
}
