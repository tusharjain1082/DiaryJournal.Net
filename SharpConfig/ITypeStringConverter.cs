﻿// Copyright (c) 2013-2018 Cemalettin Dervis, MIT License.
// https://github.com/cemdervis/SharpConfig

using System;

namespace SharpConfig
{
  /// <summary>
  /// Defines a type-to-string and string-to-type converter
  /// that is used for the conversion of setting values.
  /// </summary>
  public interface ITypeStringConverter
  {
    /// <summary>
    /// Converts an object to its string representation.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>The object's string representation.</returns>
    string ConvertToString(object value);

    /// <summary>
    /// The type that this converter is able to convert to and from a string.
    /// </summary>
    Type ConvertibleType { get; }

    /// <summary>
    /// Tries to convert a string value to an object of this converter's type.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="hint">
    ///     A type hint. This is used rarely, such as in the enum converter.
    ///     The enum converter's official type is Enum, whereas the type hint
    ///     represents the underlying enum type.
    ///     This parameter can be safely ignored for custom converters.
    /// </param>
    /// <returns>The converted object, or null if conversion is not possible.</returns>
    object TryConvertFromString(string value, Type hint);
  }

  /// <summary>
  /// Represents a type-to-string and string-to-type converter
  /// that is used for the conversion of setting values.
  /// </summary>
  /// <typeparam name="T">The type that this converter is able to convert.</typeparam>
  public abstract class TypeStringConverter<T> : ITypeStringConverter
  {
    /// <summary>
    /// Converts an object to its string representation.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>The object's string representation.</returns>
    public abstract string ConvertToString(object value);

    /// <summary>
    /// The type that this converter is able to convert to and from a string.
    /// </summary>
    public Type ConvertibleType => typeof(T);

    /// <summary>
    /// Tries to convert a string value to an object of this converter's type.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="hint">
    ///     A type hint. This is used rarely, such as in the enum converter.
    ///     The enum converter's official type is Enum, whereas the type hint
    ///     represents the underlying enum type.
    ///     This parameter can be safely ignored for custom converters.
    /// </param>
    /// <returns>The converted object, or null if conversion is not possible.</returns>
    public abstract object TryConvertFromString(string value, Type hint);
  }
}
