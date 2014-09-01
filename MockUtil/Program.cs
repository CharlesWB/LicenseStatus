// <copyright file="Program.cs" company="Charles W. Bozarth">
// Copyright (C) 2009-2014 Charles W. Bozarth
// Refer to LicenseManager's License.cs for the full copyright notice.
// </copyright>

// The primary purpose of these classes is to generate test lmstat output with the current date.
//
// This replaces the LmStatReportGenerator project which saved the test lmstat to a file for
// reading later.

// TODO Add a -nodelay option?

namespace MockUtil
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Console application which outputs the test lmstat reports.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// A port number that can be used to skip the built in delay.
        /// </summary>
        public static readonly int NoDelayPort = 60999;

        /// <summary>
        /// Stores the list of available lmstat reports.
        /// </summary>
        private static readonly List<StatusWriter> StatusWriters = new List<StatusWriter>()
        {
            new LmStatTest(),
            new LmStatNX(),
            new LmStatAcad(),
            new LmStatConnect(),
            new LmStatErrors(),
            new LmStatInvalid(),
            new LmStatCombined(),
            new LmStatLarge(),
        };

        /// <summary>
        /// Main entry point.
        /// </summary>
        /// <param name="args">Application arguments.</param>
        private static void Main(string[] args)
        {
            bool invalidParameter = true;

            if (args.Length != 0 && args[0] == "lmstat")
            {
                if (args.Length == 4 && args[1] == "-a" && args[2] == "-c")
                {
                    invalidParameter = false;

                    string licenseName = args[3];
                    int port = 0;
                    string host = licenseName;

                    // The license name can be in the form port@host. This only requires the host portion.
                    if (licenseName.IndexOf('@') != -1)
                    {
                        if (licenseName.IndexOf('@') != licenseName.Length - 1)
                        {
                            host = licenseName.Substring(licenseName.IndexOf('@') + 1);
                        }

                        if (licenseName.IndexOf('@') != 0)
                        {
                            int.TryParse(licenseName.Substring(0, licenseName.IndexOf('@')), out port);
                        }
                    }

                    // Ignore any dashes in the host name. Only because I'm used to entering
                    // it as lmstat-test from when it was written to a file. And I didn't want
                    // to put dashes in the class name.
                    host = host.Replace("-", string.Empty);

                    StatusWriter writer = StatusWriters.FirstOrDefault(s => s.GetType().Name.Equals(host, StringComparison.InvariantCultureIgnoreCase));
                    if (writer != null)
                    {
                        if (port != NoDelayPort)
                        {
                            Random delay = new Random((int)(DateTime.Now.Ticks % int.MaxValue));
                            System.Threading.Thread.Sleep(delay.Next(100, 5000));
                        }

                        writer.CreateReport();
                    }
                }
            }

            if (invalidParameter)
            {
                WriteHelp();
            }
        }

        /// <summary>
        /// Writes the help information.
        /// </summary>
        private static void WriteHelp()
        {
            Console.WriteLine("MockUtil - Copyright (c) 2012-2014 Charles W. Bozarth");
            Console.WriteLine("Emulates the lmutil program to generate test lmstat reports.");
            Console.WriteLine();
            Console.WriteLine("usage:  MockUtil lmstat -a -c license_file");
            Console.WriteLine();
            Console.WriteLine("   license_file represents one of the available test reports.");
            Console.WriteLine("   It can be written as the test's class name alone or in the form port@host");
            Console.WriteLine("   where host is the test's class name. Dashes within the name are ignored.");
            Console.WriteLine();
            Console.WriteLine("   By default there is a random delay before writing the report. If the");
            Console.WriteLine("   port number is " + NoDelayPort + " the delay is disabled.");
            Console.WriteLine();
            Console.WriteLine("   The available test classes are:");

            foreach (var item in StatusWriters)
            {
                Console.WriteLine("      {0}", item.GetType().Name);
            }
        }
    }
}
