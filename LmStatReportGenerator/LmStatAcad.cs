// <copyright file="LmStatAcad.cs" company="Charles W. Bozarth">
// Copyright (C) 2009-2012 Charles W. Bozarth
// Refer to LicenseManager's License.cs for the full copyright notice.
// </copyright>

namespace LmStatReportGenerator
{
    using System;
    using System.IO;

    /// <summary>
    /// Generates a AutoCAD lmstat report.
    /// </summary>
    public class LmStatAcad : LmStatWriterBase
    {
        /// <summary>
        /// Initializes a new instance of the LmStatAcad class.
        /// </summary>
        public LmStatAcad() : base()
        {
            this.ServerName = "SERVER001";
            this.ServerPort = 27005;
            this.Vendor = "adskflex";
            this.Version = "v1.000";
        }

        /// <summary>
        /// Writes the lmstat AutoCAD report.
        /// </summary>
        public override void CreateReport()
        {
            this.WriteLine("lmutil - Copyright (c) 1989-2006 Macrovision Europe Ltd. and/or Macrovision Corporation. All Rights Reserved.");
            this.WriteLine("Flexible License Manager status on {0:ddd M/d/yyyy} 10:43", ReportDate);
            this.WriteLine();
            this.WriteLine("[Detecting lmgrd processes...]");
            this.WriteLine("License server status: {0}@{1},{0}@SERVER002,{0}@SERVER003", this.ServerPort, this.ServerName);
            this.WriteLine(@"    License file(s) on {0}: C:\License Servers\Autodesk\Autodesk.dat:", this.ServerName);
            this.WriteLine();
            this.WriteLine("{0}: license server UP (MASTER) v10.8", this.ServerName);
            this.WriteLine("SERVER002: license server UP v10.8");
            this.WriteLine("SERVER003: license server UP v10.8");
            this.WriteLine();
            this.WriteLine("Vendor daemon status (on {0}):", this.ServerName);
            this.WriteLine();
            this.WriteLine("  {0}: UP v10.8", this.Vendor);
            this.WriteLine();
            this.WriteLine("Feature usage info:");
            this.WriteLine();

            this.WriteFeature("42600ACD_2004_0F", 473);
            this.WriteFeature("47300ACAD_E_2004_0F", 50);
            this.WriteFeature("45900AMECH_PP_2004DX_0F", 12);
            this.WriteFeature("43400AMECH_PP_2004_0F", 10);
            this.WriteFeature("51200ACD_2007_0F", 323);
            this.WriteFeature("PLIST", 1);

            this.WriteFeature("48800ACD_2006_0F", 323, 4);
            this.WriteUser("ganger CAD9201D CAD9201D", 2931, "10:40");
            this.WriteUser("ghaeli CAD8651D CAD8651D", 5318, "7:45");
            this.WriteUser("rhodea CAD8689D CAD8689D", 101, "15:18", this.ReportDate.AddDays(-6), "15429540");
            this.WriteUser("stubbs CAD8979D CAD8979D", 3618, "10:35");
            this.WriteLine();

            this.WriteFeature("51900AMECH_PP_2007_0F", 250);

            this.WriteFeature("49400AMECH_PP_2006_0F", 250, 1);
            this.WriteUser("deboer CAD8508D CAD8508D", 10116, "9:18");
            this.WriteLine();

            this.WriteFeature("52100ACAD_E_2007_0F", 40);

            this.WriteFeature("49600ACAD_E_2006_0F", 40, 1);
            this.WriteUser("lupton CAD9320D CAD9320D", 12514, "9:58");
            this.WriteLine();

            this.WriteFeature("51300INVBUN_11_0F", 3);
            this.WriteFeature("48900INVBUN_10_0F", 3);
            this.WriteFeature("55000INVBUN_2008_0F", 3);
            this.WriteFeature("53500VIZ_2007_0F", 2);

            this.WriteFeature("54600ACD_2008_0F", 293, 82);
            this.WriteUser("amandp CAD8466D CAD8466D", 12813, "10:35");
            this.WriteUser("ambros CAD8500d CAD8500d", 13915, "10:15");
            this.WriteUser("baldwi CAD8573D CAD8573D", 13622, "8:59");
            this.WriteUser("barbur CAD9211d CAD9211d", 5409, "10:16", this.ReportDate.AddDays(-4), "567780");
            this.WriteUser("boudre CAD9022D CAD9022D", 5026, "9:18");
            this.WriteUser("bouwsk CAD8329D CAD8329D", 11710, "7:39");
            this.WriteUser("bowski CAD8647D CAD8647D", 4219, "7:07");
            this.WriteUser("brofft CAD9620d CAD9620d", 5613, "10:09");
            this.WriteUser("brumel CAD9396D CAD9396D", 601, "15:18", this.ReportDate.AddDays(-6), "14437140");
            this.WriteUser("buggad CAD8622D CAD8622D", 201, "15:18", this.ReportDate.AddDays(-6), "10749180");
            this.WriteUser("bukosk CAD8247D CAD8247D", 6718, "9:59");
            this.WriteUser("burnet CAD8341D CAD8341D", 13335, "8:45");
            this.WriteUser("calvin CAD9400D CAD9400D", 10721, "9:23");
            this.WriteUser("camero CAD9260D CAD9260D", 7825, "9:45");
            this.WriteUser("cianfa CAD9181d CAD9181d", 8815, "6:59");
            this.WriteUser("coolba CAD8590D CAD8590D", 10615, "7:09");
            this.WriteUser("delane CAD8509D CAD8509D", 3318, "10:30");
            this.WriteUser("ducsay CAD8326D CAD8326D", 2829, "7:45");
            this.WriteUser("edenlg CAD8224d CAD8224d", 301, "15:18", this.ReportDate.AddDays(-6), "7812720");
            this.WriteUser("elliot CAD9461d CAD9461d", 2311, "7:19");
            this.WriteUser("fleckc CAD9249d CAD9249d", 7121, "9:18");
            this.WriteUser("frosha CAD8226D CAD8226D", 1601, "15:18", this.ReportDate.AddDays(-6), "2464800");
            this.WriteUser("fulker CAD8784D CAD8784D", 7714, "8:23");
            this.WriteUser("gallat CAD9581D CAD9581D", 7413, "7:23");
            this.WriteUser("galmor CAD8473d CAD8473d", 5721, "9:32");
            this.WriteUser("garcia CAD9024D CAD9024D", 12314, "10:01");
            this.WriteUser("hagado CAD9631D CAD9631D", 7504, "13:37", this.ReportDate.AddDays(-5), "825720");
            this.WriteUser("harder CAD8623D CAD8623D", 1203, "13:07", this.ReportDate.AddDays(-3), "1248720");
            this.WriteUser("harmon CAD9552D CAD9552D", 13719, "8:00");
            this.WriteUser("heathk CAD9225D CAD9225D", 2015, "9:39");
            this.WriteUser("hirsch CAD9303D CAD9303D", 1401, "15:18", this.ReportDate.AddDays(-6), "10402560");
            this.WriteUser("hussm1 CAD8700D CAD8700D", 10023, "10:01");
            this.WriteUser("johnso CAD8950D CAD8950D", 901, "15:18", this.ReportDate.AddDays(-6), "2797680");
            this.WriteUser("kempsd CAD8619D CAD8619D", 4832, "8:59");
            this.WriteUser("kentra CAD8426D CAD8426D", 12611, "8:20");
            this.WriteUser("kossam CAD8339D CAD8339D", 8320, "8:27");
            this.WriteUser("kossen CAD8701d CAD8701d", 13117, "8:55");
            this.WriteUser("leiter CAD8483D CAD8483D", 9312, "10:14");
            this.WriteUser("lesarg CAD9318D CAD9318D", 6916, "10:38");
            this.WriteUser("lindse CAD8740D CAD8740D", 11908, "8:49");
            this.WriteUser("lirajb CAD9029D CAD9029D", 6819, "10:34");
            this.WriteUser("lohrdp CAD8865d CAD8865d", 4416, "9:14");
            this.WriteUser("lomont CAD9198D CAD9198D", 9516, "7:39");
            this.WriteUser("lutzjr CAD9221D CAD9221D", 2402, "9:48");
            this.WriteUser("mcgeec CAD8632D CAD8632D", 401, "15:18", this.ReportDate.AddDays(-6), "7373640");
            this.WriteUser("mellem CAD8781D CAD8781D", 4318, "7:32");
            this.WriteUser("mitche CAD9116D CAD9116D", 12014, "8:52");
            this.WriteUser("mooreb CAD8744D CAD8744D", 10212, "10:07");
            this.WriteUser("mziktj CAD8702D CAD8702D", 6314, "8:16");
            this.WriteUser("noomes CAD8485D CAD8485D", 9907, "7:20");
            this.WriteUser("pilarz CAD9398d CAD9398d", 8116, "9:20");
            this.WriteUser("pospyc CAD8468D CAD8468D", 7615, "8:08");
            this.WriteUser("pothrm CAD9599D CAD9599D", 1501, "15:18", this.ReportDate.AddDays(-6), "2642760");
            this.WriteUser("radoca CAD8650D CAD8650D", 5533, "8:25");
            this.WriteUser("ramonj CAD8338D CAD8338D", 11115, "10:22");
            this.WriteUser("ramsey CAD8745D CAD8745D", 3814, "7:58");
            this.WriteUser("ransch CAD8328D CAD8328D", 7319, "10:28");
            this.WriteUser("reisse CAD9199D CAD9199D", 8918, "8:06");
            this.WriteUser("saunde CAD9345d CAD9345d", 2201, "15:18", this.ReportDate.AddDays(-6), "990540");
            this.WriteUser("schmit CAD8968D CAD8968D", 12917, "7:51");
            this.WriteUser("schwit CAD9183D CAD9183D", 6514, "8:00");
            this.WriteUser("silvag CAD9178D CAD9178D", 13419, "10:11");
            this.WriteUser("solowa CAD8321D CAD8321D", 3517, "7:47");
            this.WriteUser("staals CAD8324D CAD8324D", 6420, "9:59");
            this.WriteUser("staats CAD8437D CAD8437D", 8714, "9:02");
            this.WriteUser("steine CAD8981D CAD8981D", 12112, "10:14");
            this.WriteUser("stelma CAD8709D CAD8709D", 10513, "6:53");
            this.WriteUser("szarow CAD8952D CAD8952D", 1001, "15:18", this.ReportDate.AddDays(-6), "2797080");
            this.WriteUser("terpst CAD8327D CAD8327D", 9023, "8:44");
            this.WriteUser("trabka CAD9366D CAD9366D", 4514, "7:50");
            this.WriteUser("vander CAD8755D CAD8755D", 12708, "7:06");
            this.WriteUser("vandyk CAD8756D CAD8756D", 6616, "7:04");
            this.WriteUser("vassel CAD8989d CAD8989d", 9718, "8:16");
            this.WriteUser("vaugha CAD9315d CAD9315d", 2611, "8:55");
            this.WriteUser("veithr CAD9161D CAD9161D", 1717, "8:06");
            this.WriteUser("wendla CAD8711d CAD8711d", 9821, "9:18");
            this.WriteUser("westbf CAD8765D CAD8765D", 11010, "9:58");
            this.WriteUser("wintv1 CAD8872D CAD8872D", 11411, "9:27");
            this.WriteUser("wright CAD8712d CAD8712d", 8514, "7:46");
            this.WriteUser("yacksj CAD8496D CAD8496D", 9224, "9:45");
            this.WriteUser("zalesk CAD9226D CAD9226D", 3920, "8:58");
            this.WriteUser("zimmer CAD8713d CAD8713d", 8213, "10:37");
            this.WriteLine();

            this.WriteFeature("56000ACAD_E_2008_0F", 50, 25);
            this.WriteUser("Venekl CAD8477D CAD8477D", 11526, "6:04");
            this.WriteUser("bawden CAD8714d CAD8714d", 13017, "9:26");
            this.WriteUser("blackj CAD9558D CAD9558D", 1901, "15:18", this.ReportDate.AddDays(-6), "1851600");
            this.WriteUser("brower CAD8293D CAD8293D", 2103, "7:08", this.ReportDate.AddDays(-2), "579060");
            this.WriteUser("burkit CAD9441D CAD9441D", 5118, "6:58");
            this.WriteUser("chalme CAD9263D CAD9263D", 6110, "7:11");
            this.WriteUser("consta CAD9268d CAD9268d", 8613, "16:59", this.ReportDate.AddDays(-4), "4780800");
            this.WriteUser("ehlert CAD8617D CAD8617D", 10318, "6:25");
            this.WriteUser("hekman CAD9632D CAD9632D", 4723, "8:37");
            this.WriteUser("hildeb CAD8931D CAD8931D", 4920, "10:30");
            this.WriteUser("hofste CAD9744D CAD9744D", 11620, "10:32");
            this.WriteUser("kempsd CAD8619D CAD8619D", 501, "15:18", this.ReportDate.AddDays(-6), "15444420");
            this.WriteUser("krysti CAD8782D CAD8782D", 1101, "15:18", this.ReportDate.AddDays(-6), "2479200");
            this.WriteUser("krysti CAD9437d CAD9437d", 9618, "7:29");
            this.WriteUser("lamphe CAD8703D CAD8703D", 3716, "7:13");
            this.WriteUser("marekj CAD9057D CAD9057D", 7023, "8:51");
            this.WriteUser("nguyen CAD8306D CAD8306D", 1801, "15:18", this.ReportDate.AddDays(-6), "2645760");
            this.WriteUser("ravell CAD8996D CAD8996D", 801, "15:18", this.ReportDate.AddDays(-6), "1862760");
            this.WriteUser("schuch CAD9348D CAD9348D", 2501, "15:18", this.ReportDate.AddDays(-6), "4530600");
            this.WriteUser("shaver CAD8310D CAD8310D", 1303, "8:52", this.ReportDate.AddDays(-3), "1091220");
            this.WriteUser("smithg CAD9058D CAD9058D", 5920, "8:29");
            this.WriteUser("spratk CAD9421D CAD9421D", 3216, "9:28");
            this.WriteUser("vigens CAD9376D CAD9376D", 10819, "9:49");
            this.WriteUser("webbdj CAD8993D CAD8993D", 703, "10:27", this.ReportDate.AddDays(-4), "2554320");
            this.WriteUser("woodwy CAD9381D CAD9381D", 6214, "9:21");
            this.WriteLine();

            this.WriteFeature("54900AMECH_PP_2008_0F", 250, 1);
            this.WriteUser("crawfo CAD8506D CAD8506D", 4125, "10:05");
            this.WriteLine();

            this.WriteFeature("64500INVBUN_F", 3);

            this.WriteFeature("64400AMECH_PP_F", 250, 2);
            this.WriteUser("crawfo CAD8506D CAD8506D", 10915, "10:05");
            this.WriteUser("deboer CAD8508D CAD8508D", 9125, "9:18");
            this.WriteLine();

            this.WriteFeature("59200AMECH_PP_2009_0F", 250);
            this.WriteFeature("59300INVBUN_2009_0F", 3);
        }
    }
}
