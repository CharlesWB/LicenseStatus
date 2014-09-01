// <copyright file="LmStatTest.cs" company="Charles W. Bozarth">
// Copyright (C) 2009-2014 Charles W. Bozarth
// Refer to LicenseManager's License.cs for the full copyright notice.
// </copyright>

namespace MockUtil
{
    using System;
    using System.IO;

    /// <summary>
    /// Generates a test lmstat report.
    /// </summary>
    public class LmStatTest : StatusWriter
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
        /// If ReportDate is not specified then today's date will be used.
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
            this.WriteLine();

            // An example was reported where the user ended with ", 2 licenses". This text also appears
            // in lmutil.exe. Based on the text within lmutil.exe the word "licenses" is always plural.
            // This is also an example where the quantity used does not match the unique user/display count.
            // I'm assuming multiple licenses can be borrowed this way.
            this.WriteFeature("User_Multiple_Checkouts", 9, 8);
            this.WriteLine("    user011 comp011 comp011 (v22.0) (SERVER001/{0} 2209), start {1:ddd M/d} 13:21, 2 licenses", this.ServerPort, this.ReportDate);
            this.WriteLine("    user012 comp012 comp012 (v22.0) (SERVER001/{0} 2201), start {1:ddd M/d} 9:31", this.ServerPort, this.ReportDate);
            this.WriteLine("    user013 comp013 comp013 (v22.0) (SERVER001/{0} 2200), start {1:ddd M/d} 11:23, 3 licenses", this.ServerPort, this.ReportDate);
            this.WriteLine("    user014 comp014 comp014 (v22.0) (SERVER001/{0} 2219), start {1:ddd M/d} 10:15, 2 licenses (linger: 14437140)", this.ServerPort, this.ReportDate);
            this.WriteLine();

            // Users and/or displays that contain spaces. All of these would be parsed
            // correctly during the first attempt.
            // Unit testing is done by expecting the first letter to be u, h or d.
            this.WriteFeature("Users_With_Spaces_Auto", 15, 15);
            this.WriteUser("user100 host100 display100", 100, "10:21");
            this.WriteUser("user101 host100 display101", 100, "10:21");

            this.WriteUser("user 110 host110 display110", 100, "10:21");
            this.WriteUser("user111 host111 display 111", 100, "10:21");
            this.WriteUser("user112 host112 display112 A", 100, "10:21");
            this.WriteUser("user 113 host113 display113 A", 100, "10:21");
            this.WriteUser("user 114 host114 display 114A", 100, "10:21");
            this.WriteUser("user A B 115 host115 display115", 100, "10:21");

            this.WriteUser("user200 host200 display200", 100, "10:21");
            this.WriteUser("user201 host200 display201", 100, "10:21");
            this.WriteUser("user 202 host200 display202", 100, "10:21");
            this.WriteUser("user203 host200 display A203", 100, "10:21");
            this.WriteUser("user 204 host200 display A204", 100, "10:21");
            this.WriteUser("user A B205 host200 display A B205", 100, "10:21");
            this.WriteUser("user  ABC  DE F206 host206 display206", 100, "10:21");
            this.WriteLine();

            // Additional user identities with spaces that will all parse correctly
            // but cannot easily be automatically tested.
            this.WriteFeature("Users_With_Spaces_Other", 8, 8);
            this.WriteUser("user300 host300 host300", 100, "10:21");
            this.WriteUser("user 301 host300 host300", 100, "10:21");
            this.WriteUser("user 302 host300 host3000.0", 100, "10:21");
            this.WriteUser("user 303 host303 host300", 100, "10:21");
            this.WriteUser("host300 host304 host 304", 100, "10:21");
            this.WriteUser("host300 host300 host 305", 100, "10:21");
            this.WriteUser("host300 A host300 B host300", 100, "10:21");
            this.WriteUser("host300 host300 host300 host300", 100, "10:21");
            this.WriteLine();

            // User identities which are known to not parse correctly.
            this.WriteFeature("Users_With_Spaces_Fail", 1, 1);
            this.WriteUser("user 400 host400 display AB400", 100, "10:21");
            this.WriteLine();

            // Users identities which will originally be incorrect, but will
            // become correct when the host becomes known.
            this.WriteFeature("Users_With_Spaces_ChangedEvent", 4, 4);
            this.WriteUser("user 500 host500 display500", 100, "10:21");
            this.WriteUser("user501 host500 display A501", 100, "10:21");
            this.WriteUser("user 502 host500 display A502", 100, "10:21");
            this.WriteUser("user503 host500 display503", 100, "10:21");
            this.WriteLine();
            
            // User identity which is originally incorrect, but then changed when
            // lmstat-nx is parsed. The host is from lmstat-nx.
            this.WriteFeature("Users_With_Spaces_ChangedEvent_Other", 1, 1);
            this.WriteUser("user 504 CAD9695D display A504", 100, "10:21");
            this.WriteLine();
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
