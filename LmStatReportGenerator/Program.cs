// <copyright file="Program.cs" company="Charles W. Bozarth">
// Copyright (C) 2009-2012 Charles W. Bozarth
// Refer to LicenseManager's License.cs for the full copyright notice.
// </copyright>

// The primary purpose of these classes is to create test lmstat report files with the current date.

namespace LmStatReportGenerator
{
    using System;
    using System.IO;

    /// <summary>
    /// Console application which writes the test files.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main entry point.
        /// </summary>
        /// <param name="args">Application arguments.</param>
        public static void Main(string[] args)
        {
            // Assumes test files are in the Solution folder and assumes this is three folders above the assembly.
            string outputFolder = Path.GetDirectoryName(typeof(LmStatReportGenerator.Program).Assembly.Location);
            outputFolder = Path.GetFullPath(outputFolder + @"\..\..\..\");

            Console.WriteLine("Writing all reports.");

            LmStatGenerator.WriteAllTestFiles(outputFolder);

            Console.WriteLine();
            Console.WriteLine("Press any key");
            Console.ReadKey();
        }
    }
}
