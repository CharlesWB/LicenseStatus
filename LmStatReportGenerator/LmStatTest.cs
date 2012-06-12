// <copyright file="LmStatTest.cs" company="Charles W. Bozarth">
// Copyright (C) 2009-2012 Charles W. Bozarth
// Refer to LicenseManager's License.cs for the full copyright notice.
// </copyright>

namespace LmStatReportGenerator
{
    using System;
    using System.IO;

    /// <summary>
    /// Generates a test lmstat report.
    /// </summary>
    public class LmStatTest : LmStatWriterBase
    {
        /// <summary>
        /// Initializes a new instance of the LmStatTest class.
        /// </summary>
        public LmStatTest() : base()
        {
            this.ServerName = "SERVER001";
            this.ServerPort = 27001;
            this.Vendor = "testdaemon";
            this.Version = "v22.0";
        }

        /// <summary>
        /// Writes the lmstat test report.
        /// </summary>
        /// <remarks>
        /// <para>If OutputFile is not specified the output will only write to the console.</para>
        /// <para>If ReportDate is not specified the today's date will be used.</para>
        /// </remarks>
        public override void CreateReport()
        {
            this.WriteLine("lmutil - Copyright (c) 1989-2006 Macrovision Europe Ltd. and/or Macrovision Corporation. All Rights Reserved.");
            this.WriteLine("Flexible License Manager status on {0:ddd M/d/yyyy} 10:43", this.ReportDate);
            this.WriteLine();
            this.WriteLine("[Detecting lmgrd processes...]");
            this.WriteLine("License server status: {0}@{1}", this.ServerPort, this.ServerName);
            this.WriteLine(@"    License file(s) on {0}: C:\License Servers\Test\Test.lic:", this.ServerName);
            this.WriteLine();
            this.WriteLine("{0}: license server UP (MASTER) v10.8", this.ServerName);
            this.WriteLine();
            this.WriteLine("Vendor daemon status (on {0}):", this.ServerName);
            this.WriteLine();
            this.WriteLine("     {0}: UP v10.1", this.Vendor);
            this.WriteLine();
            this.WriteLine("Feature usage info:");
            this.WriteLine();

            this.WriteFeature("Empty_Feature_One_License", 1);

            this.WriteFeature("Used_Feature_One_License", 1, 1);
            this.WriteUser("user001 comp001 comp001", 3861, "7:13");
            this.WriteLine();

            this.WriteLine("Users of Feature_With_Error: Cannot get users of Feature_With_Error: No such feature exists. (-5,222)");
            this.WriteLine();

            this.WriteLine("Users of Feature_With_Other_Error:  (Error: 21 licenses, unsupported by licensed server)");
            this.WriteLine();

            this.WriteFeature("Feature_With_Borrow", 16, 4);
            this.WriteUser("user003 comp003 comp003", 2009, "10:21");
            this.WriteUser("user005 comp005 comp005", 601, "10:18", this.ReportDate, "14437140");
            this.WriteUser("user004 comp004 comp004", 6118, "9:55");
            this.WriteUser("user006 comp006 comp006", 6301, "11:28", this.ReportDate.AddDays(-15), "14437140");
            this.WriteLine();

            // The total in use does not automatically update to match the created user count.
            this.WriteFeature(string.Format("Feature_With_Various_Dates_{0:M_dd_yyyy}", this.ReportDate), 90, 27);

            int userNumber = 1;
            this.WriteUserWithTestDate(ref userNumber, this.ReportDate, "10:06"); // Current day, normal times.
            this.WriteUserWithTestDate(ref userNumber, this.ReportDate, "10:21");
            this.WriteUserWithTestDate(ref userNumber, this.ReportDate, "9:55");
            this.WriteUserWithTestDate(ref userNumber, this.ReportDate, "10:18");
            this.WriteUserWithTestDate(ref userNumber, this.ReportDate, "0:00"); // Beginning of current day.
            this.WriteUserWithTestDate(ref userNumber, this.ReportDate, "12:00"); // Noon of current day.
            this.WriteUserWithTestDate(ref userNumber, this.ReportDate, "23:59"); // End of current day, which would be a future time.
            this.WriteUserWithTestDate(ref userNumber, this.ReportDate.AddDays(-1), "15:01"); // Yesterday.
            this.WriteUserWithTestDate(ref userNumber, this.ReportDate.AddDays(-1), "0:00"); // Beginning of yesterday.
            this.WriteUserWithTestDate(ref userNumber, this.ReportDate.AddDays(-1), "23:59"); // End of yesterday.
            this.WriteUserWithTestDate(ref userNumber, this.ReportDate.AddDays(-2), "6:18"); // Day before yesterday.
            this.WriteUserWithTestDate(ref userNumber, this.ReportDate.AddDays(-5), "13:00"); // A few days ago.
            this.WriteUserWithTestDate(ref userNumber, this.ReportDate.AddDays(-7), "15:01"); // One week ago.
            this.WriteUserWithTestDate(ref userNumber, new DateTime(this.ReportDate.Year, this.ReportDate.Month, 1), "0:00"); // Beginning of first day of current month.
            this.WriteUserWithTestDate(ref userNumber, new DateTime(this.ReportDate.Year, this.ReportDate.Month, 1).AddDays(-1), "23:59"); // End of last day of previous month.
            this.WriteUserWithTestDate(ref userNumber, new DateTime(this.ReportDate.Year, 1, 1), "0:00"); // Beginning of first day of current year.
            this.WriteUserWithTestDate(ref userNumber, new DateTime(this.ReportDate.Year, 1, 1).AddDays(-1), "23:59"); // End of last day of previous year.
            this.WriteUserWithTestDate(ref userNumber, ReportDate.AddYears(-1).AddDays(-1), "7:00"); // One year and one day previously.

            this.WriteUserWithTestDate(ref userNumber, this.ReportDate.AddDays(1), "8:06"); // Tomorrow.
            this.WriteUserWithTestDate(ref userNumber, this.ReportDate.AddDays(1), "0:00"); // Beginning of tomorrow.
            this.WriteUserWithTestDate(ref userNumber, this.ReportDate.AddDays(1), "23:59"); // End of tomorrow.
            this.WriteUserWithTestDate(ref userNumber, this.ReportDate.AddDays(2), "10:55"); // Day after tomorrow.
            this.WriteUserWithTestDate(ref userNumber, this.ReportDate.AddDays(7), "6:18"); // One week from now.
            this.WriteUserWithTestDate(ref userNumber, new DateTime(this.ReportDate.Year, this.ReportDate.Month, 1).AddMonths(1).AddDays(-2), "23:59"); // End of next to last day of current month.
            this.WriteUserWithTestDate(ref userNumber, new DateTime(this.ReportDate.Year, this.ReportDate.Month, 1).AddMonths(1).AddDays(-1), "0:00"); // Beginning of last day of current month.
            this.WriteUserWithTestDate(ref userNumber, new DateTime(this.ReportDate.Year, this.ReportDate.Month, 1).AddMonths(1).AddDays(-1), "23:59"); // End of last day of current month.
            this.WriteUserWithTestDate(ref userNumber, new DateTime(this.ReportDate.Year, this.ReportDate.Month, 1).AddMonths(1), "0:00"); // Beginning of first day of next month.
            this.WriteLine();

            this.WriteFeature("Feature_Non-Word.Characters", 16, 6);
            this.WriteLine("    user-001 comp-001 comp-001 (v1.000) (SERVER.001/{0} 2009), start {1:ddd M/d} 10:21", this.ServerPort, this.ReportDate);
            this.WriteLine("    user-001 comp-003 comp-003 (v1.000) (SERVER.001/{0} 2001), start {1:ddd M/d} 10:31", this.ServerPort, this.ReportDate);
            this.WriteLine("    user-001 comp-002 comp-002 (v1.000) (SERVER.001/{0} 2000), start {1:ddd M/d} 11:20", this.ServerPort, this.ReportDate);
            this.WriteLine("    user-001 comp-004 comp-004 (v1.000) (SERVER.001/{0} 2004), start {1:ddd M/d} 10:12", this.ServerPort, this.ReportDate);
            this.WriteUser("user.005 comp.005 comp.005", 662, "10:10");
            this.WriteUser("user.004 comp.004 comp.004", 6101, "9:52");
        }

        /// <summary>
        /// Writes an incrementing user line to the output.
        /// </summary>
        /// <remarks>Used for creating DateTime variations.</remarks>
        /// <param name="userNumber">A number used to create unique user name, host, display and handle values. This is automatically incremented at each call.</param>
        /// <param name="date">The date to use for checkout.</param>
        /// <param name="time">The time to use for checkout.</param>
        private void WriteUserWithTestDate(ref int userNumber, DateTime date, string time)
        {
            this.WriteUser(string.Format("user_d{0:0#} comp_d{0:0#} comp_d{0:0#}", userNumber), 3001 + (userNumber * 10), time, date, null);
            userNumber++;
        }
    }
}
