﻿namespace AngleSharp.Dom
{
    using AngleSharp.Attributes;

    /// <summary>
    /// Enumeration with possible values for the adjacent position insertion.
    /// </summary>
    public enum AdjacentPosition : System.Byte
    {
        /// <summary>
        /// Before the element itself.
        /// </summary>
        [DomName("beforebegin")]
        BeforeBegin,
        /// <summary>
        /// Just inside the element, before its first child.
        /// </summary>
        [DomName("afterbegin")]
        AfterBegin,
        /// <summary>
        /// Just inside the element, after its last child.
        /// </summary>
        [DomName("beforeend")]
        BeforeEnd,
        /// <summary>
        /// After the element itself.
        /// </summary>
        [DomName("afterend")]
        AfterEnd
    }
}
