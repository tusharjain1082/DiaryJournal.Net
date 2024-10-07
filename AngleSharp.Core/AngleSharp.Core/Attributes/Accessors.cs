﻿namespace AngleSharp.Attributes
{
    using System;

    /// <summary>
    /// The various DOM object accessor types.
    /// </summary>
    [Flags]
    public enum Accessors : Byte
    {
        /// <summary>
        /// Specifies that the property does not have any special meaning.
        /// </summary>
        None = 0x0,
        /// <summary>
        /// Specifies that the property or method should be handled as a getter.
        /// </summary>
        Getter = 0x1,
        /// <summary>
        /// Specifies that the property or method should be handled as a setter.
        /// </summary>
        Setter = 0x2,
        /// <summary>
        /// Specifies that the property or method should be handled by delete.
        /// </summary>
        Deleter = 0x4,
        /// <summary>
        /// Specifies that the method should be handled as an event adder.
        /// </summary>
        Adder = 0x8,
        /// <summary>
        /// Specifies that the method should be handled by an event remover.
        /// </summary>
        Remover = 0x10
    }
}
