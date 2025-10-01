// <copyright file="NullValueException.cs" company="IlyaSotnikov">
// Copyright (c) IlyaSotnikov. All rights reserved.
// </copyright>

namespace Lazy;

/// <summary>
/// Exception for null values.
/// </summary>
public class NullValueException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NullValueException>" /> class.
    /// </summary>
    /// <param name="message">Message.</param>
    public NullValueException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NullValueException>" /> class.
    /// </summary>
    /// <param name="message">Message.</param>
    /// <param name="innerException">The inner exception.</param>
    public NullValueException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}