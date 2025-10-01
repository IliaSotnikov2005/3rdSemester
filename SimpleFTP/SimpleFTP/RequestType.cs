// <copyright file="RequestType.cs" company="IlyaSotnikov">
// Copyright (c) IlyaSotnikov. All rights reserved.
// </copyright>

namespace SimpleFTP;

/// <summary>
/// Request types for the server.
/// </summary>
public enum RequestType
    {
        /// <summary>
        /// List request type.
        /// </summary>
        List = 1,

        /// <summary>
        /// Get request type.
        /// </summary>
        Get = 2,
    }