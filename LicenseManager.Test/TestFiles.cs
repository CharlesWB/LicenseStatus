// <copyright file="TestFiles.cs" company="Charles W. Bozarth">
// Copyright (C) 2009-2012 Charles W. Bozarth
// Refer to LicenseManager's License.cs for the full copyright notice.
// </copyright>

// TODO This should be improved.

namespace LicenseManager.Test
{
    using System;
    using System.IO;
    using CWBozarth.LicenseManager;

    /// <summary>
    /// Used by the test classes to manage the test files.
    /// </summary>
    public class TestFiles
    {
        /// <summary>
        /// Initializes a new instance of the TestFiles class.
        /// </summary>
        public TestFiles()
        {
            // Assumes test files are in the Solution folder and assumes this is three folders above the assembly.
            this.Path = System.IO.Path.GetDirectoryName(typeof(UtilityProgramTest).Assembly.Location);
            this.Path = System.IO.Path.GetFullPath(this.Path + @"\..\..\..\");

            // lmutil.exe is not included in the solution by default. It must be manually placed in the Solution folder.
            if (!File.Exists(System.IO.Path.Combine(this.Path, "lmutil.exe")))
            {
                throw new FileNotFoundException("The test file lmutil.exe was not found at " + this.Path, this.Path);
            }

            // Ensure the test files are updated with today's date. Assumes License.Time works. Probably should use a different technique.
            License checkDateLicense = new License();
            checkDateLicense.GetStatus(new FileInfo(System.IO.Path.Combine(this.Path, "lmstat-test.log")));
            if (checkDateLicense.Time.Date != DateTime.Today)
            {
                LmStatReportGenerator.LmStatGenerator.WriteAllTestFiles(this.Path);
            }
        }

        public string Path { get; private set; }
    }
}
