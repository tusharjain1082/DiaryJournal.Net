﻿using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using static LiteDB.Constants;

namespace LiteDB.Engine
{
    /// <summary>
    /// Interface factory to provider new Stream instances for datafile/walfile resources. It's useful to multiple threads can read same datafile
    /// </summary>
    internal interface IStreamFactory
    {
        /// <summary>
        /// Get Stream name (filename)
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Get new Stream instance
        /// </summary>
        Stream GetStream(bool canWrite, bool sequencial);

        /// <summary>
        /// Get file length
        /// </summary>
        /// <returns></returns>
        long GetLength();

        /// <summary>
        /// Checks if file exists
        /// </summary>
        bool Exists();

        /// <summary>
        /// Delete physical file on disk
        /// </summary>
        void Delete();

        /// <summary>
        /// Test if this file are used by another process
        /// </summary>
        bool IsLocked();

        /// <summary>
        /// Indicate that factory must be dispose on finish
        /// </summary>
        bool CloseOnDispose { get; }
    }
}