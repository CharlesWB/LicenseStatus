// <copyright file="HostAddedEventArgs.cs" company="Charles W. Bozarth">
// Copyright (C) 2009-2012 Charles W. Bozarth
// Refer to License.cs for the full copyright notice.
// </copyright>

namespace CWBozarth.LicenseManager
{
    using System;

    /// <summary>
    /// Provides data for the <see cref="KnownHostSet.HostAdded"/> event.
    /// </summary>
    internal class HostAddedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the HostAddedEventArgs class.
        /// </summary>
        /// <param name="host">The host that has been added to the known hosts.</param>
        public HostAddedEventArgs(string host)
        {
            this.Host = host;
        }

        /// <summary>
        /// Gets the host name that was added to the known hosts.
        /// </summary>
        public string Host { get; private set; }
    }
}
