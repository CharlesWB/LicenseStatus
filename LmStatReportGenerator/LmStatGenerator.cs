// <copyright file="LmStatGenerator.cs" company="Charles W. Bozarth">
// Copyright (C) 2009-2012 Charles W. Bozarth
// Refer to LicenseManager's License.cs for the full copyright notice.
// </copyright>

namespace LmStatReportGenerator
{
    using System;
    using System.IO;

    /// <summary>
    /// Create all of the test lmstat reports with an appropriate date.
    /// </summary>
    public static class LmStatGenerator
    {
        /// <summary>
        /// Write all of the test files to a folder.
        /// </summary>
        /// <param name="outputFolder">The folder to write the files to.</param>
        public static void WriteAllTestFiles(string outputFolder)
        {
            WriteReportToFile(new LmStatTest(), Path.Combine(outputFolder, "lmstat-test.log"));
            WriteReportToFile(new LmStatInvalid(), Path.Combine(outputFolder, "lmstat-invalid.log"));
            WriteReportToFile(new LmStatConnect(), Path.Combine(outputFolder, "lmstat-connect.log"));
            WriteReportToFile(new LmStatErrors() { ReportDate = new DateTime(2008, 11, 20) }, Path.Combine(outputFolder, "lmstat-errors.log"));
            WriteReportToFile(new LmStatLarge(), Path.Combine(outputFolder, "lmstat-large.log"));
            WriteReportToFile(new LmStatNX(), Path.Combine(outputFolder, "lmstat-nx.log"));
            WriteReportToFile(new LmStatAcad(), Path.Combine(outputFolder, "lmstat-acad.log"));
        }

        /// <summary>
        /// Write the lmstat report to a file.
        /// </summary>
        /// <param name="reportGenerator">The lmstat report generator.</param>
        /// <param name="fileName">The file name of the file to save the report to.</param>
        private static void WriteReportToFile(LmStatWriterBase reportGenerator, string fileName)
        {
            using (StreamWriter sw = new StreamWriter(fileName, false))
            {
                reportGenerator.OutputFile = sw;
                reportGenerator.CreateReport();
            }
        }
    }
}
