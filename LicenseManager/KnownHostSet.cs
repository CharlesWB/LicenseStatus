// <copyright file="KnownHostSet.cs" company="Charles W. Bozarth">
// Copyright (C) 2009-2012 Charles W. Bozarth
// Refer to License.cs for the full copyright notice.
// </copyright>

#region Notes
// Functionality Options:
//
// To provide the correct functionality this class had to provide:
// single instance; add non-duplicate host; test for containing a host;
// read only collection; thread safe; and raising an event when a new host is added.
//
// A static collection was considered but threading was an issue and as well as
// emulating some form of event raising (such as static methods to review existing users).
//
// A custom read only version of HashSet was considered but it would have
// required implementing interface methods that were not really needed.
// At the moment there is no need to access the collection outside of this
// class.
//
// Possible Enhancements:
//
// Consider changing to a non-singleton and use dependency injection. Partly
// to simplify the unit test.
#endregion

namespace CWBozarth.LicenseManager
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the set of user host names which have been confirmed as hosts.
    /// </summary>
    /// <remarks>
    /// The <see cref="User"/> class uses this set to store confirmed hosts.
    /// When a user is parsed that has spaces in the name or display then the possible
    /// host can be checked against known hosts and the user information can then be
    /// parsed more accurately.
    /// </remarks>
    internal sealed class KnownHostSet
    {
        /// <summary>
        /// Stores the instance of this singleton class.
        /// </summary>
        private static volatile KnownHostSet instance;

        /// <summary>
        /// Stores the lock object used to creating the singleton instance.
        /// </summary>
        private static object syncRoot = new object();

        /// <summary>
        /// Stores the lock object used when modifying the collection.
        /// </summary>
        private static object syncCollection = new object();

        /// <summary>
        /// Stores the collection of known hosts.
        /// </summary>
        private HashSet<string> knownHosts;

        /// <summary>
        /// Prevents a default instance of the <see cref="KnownHostSet" /> class from being created.
        /// </summary>
        private KnownHostSet()
        {
            this.knownHosts = new HashSet<string>();
        }

        /// <summary>
        /// Raised when a new host has been added to the set.
        /// </summary>
        public event EventHandler<HostAddedEventArgs> HostAdded;

        /// <summary>
        /// Gets the instance of this singleton class.
        /// </summary>
        public static KnownHostSet Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new KnownHostSet();
                        }
                    }
                }

                return instance;
            }
        }

        /// <summary>
        /// Adds the host to the set.
        /// </summary>
        /// <remarks>
        /// If the host has not previously been added then the <see cref="HostAdded"/> event is raised.
        /// </remarks>
        /// <param name="host">The host name.</param>
        public void Add(string host)
        {
            lock (syncCollection)
            {
                if (this.knownHosts.Add(host))
                {
                    this.OnHostAdded(new HostAddedEventArgs(host));
                }
            }
        }

        /// <summary>
        /// Returns a value indicating whether the host is confirmed as a known host.
        /// </summary>
        /// <param name="host">The host name.</param>
        /// <returns>True if the host is a known host.</returns>
        public bool IsKnown(string host)
        {
            return this.knownHosts.Contains(host);
        }

        /// <summary>
        /// Raises this <see cref="HostAdded"/> event.
        /// </summary>
        /// <param name="e">The event data.</param>
        private void OnHostAdded(HostAddedEventArgs e)
        {
            EventHandler<HostAddedEventArgs> handler = this.HostAdded;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
