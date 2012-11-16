// <copyright file="UserTest.cs" company="Charles W. Bozarth">
// Copyright (C) 2009-2012 Charles W. Bozarth
// Refer to LicenseManager's License.cs for the full copyright notice.
// </copyright>

#region Notes
// Clearing KnownHostSet:
//
// User_IdentityChangesWhenNewHostAdded_IdentityChangesAreCorrect requires that the
// NX host not be in KnownHostSet. But other tests will read an NX license so the host
// will already be added. Code using PrivateObject and PrivateType will clear the
// set for the test. Technically this should probably be in MyTestInitialize,
// but I'm not convinced this is the best option. For now it's left as is to make
// a test that works.
//
// This also required adding [assembly: InternalsVisibleTo("LicenseManager.Test")] to
// LicenseManager's AssemblyInfo.cs to allow the unit test access to the internal class.
#endregion

namespace LicenseManager.Test
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using CWBozarth.LicenseManager;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for UserTest and is intended
    /// to contain all UserTest Unit Tests
    /// </summary>
    [TestClass]
    public class UserTest
    {
        private static CultureInfo originalCulture = Thread.CurrentThread.CurrentCulture;

        private static int indexOffset;

        private TestContext testContextInstance;

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext
        {
            get
            {
                return this.testContextInstance;
            }

            set
            {
                this.testContextInstance = value;
            }
        }

        #region Additional test attributes
        // You can use the following additional attributes as you write your tests:

        // Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            // The EntryIndex value will change depending on whether today's month or day are one or two digits.
            // The indexOffset is used to compensate for this.
            if (DateTime.Today.Month > 9)
            {
                indexOffset++;
            }

            if (DateTime.Today.Day > 9)
            {
                indexOffset++;
            }
        }
        
        //// Use ClassCleanup to run code after all tests in a class have run
        ////[ClassCleanup]
        ////public static void MyClassCleanup()
        ////{
        ////}

        // Use TestInitialize to run code before running each test
        [TestInitialize]
        public void MyTestInitialize()
        {
            Thread.CurrentThread.CurrentCulture = originalCulture;
        }

        // Use TestCleanup to run code after each test has run
        [TestCleanup]
        public void MyTestCleanup()
        {
            Thread.CurrentThread.CurrentCulture = originalCulture;
        }
        #endregion

        [TestMethod]
        public void User_Typical_PropertyReturnsAreCorrect()
        {
            UtilityProgram.Instance.Executable = new FileInfo("MockUtil.exe");
            License license = new License() { Port = MockUtil.Program.NoDelayPort, Host = "LmStatTest" };
            license.GetStatus();

            Feature feature = license.Features.First(f => f.Name == "Feature_With_Borrow");

            string expectedName = "user003";
            User target = feature.Users.First(u => u.Name == expectedName);

            PropertiesComparer.AssertUserPropertiesAreEqual(
                actual: target,
                name: expectedName,
                host: "comp003",
                display: "comp003",
                version: "v22.0",
                server: "SERVER001",
                port: 27001,
                handle: 2009,
                time: DateTime.Today.AddHours(10).AddMinutes(21),
                quantityused: 1,
                linger: TimeSpan.Zero,
                isBorrowed: false,
                borrowEndTime: DateTime.MinValue,
                entryIndex: 1169 + (2 * indexOffset),
                entryLength: 76 + indexOffset);
        }

        [TestMethod]
        public void User_TypicalUsingDifferentCultures_PropertyReturnsAreCorrect()
        {
            UtilityProgram.Instance.Executable = new FileInfo("MockUtil.exe");

            foreach (CultureInfo culture in CultureInfo.GetCultures(CultureTypes.AllCultures))
            {
                Thread.CurrentThread.CurrentCulture = culture;

                License license = new License() { Port = MockUtil.Program.NoDelayPort, Host = "LmStatTest" };
                license.GetStatus();

                Feature feature = license.Features.First(f => f.Name == "Feature_With_Borrow");

                string expectedName = "user003";
                User target = feature.Users.First(u => u.Name == expectedName);

                PropertiesComparer.AssertUserPropertiesAreEqual(
                    actual: target,
                    name: expectedName,
                    host: "comp003",
                    display: "comp003",
                    version: "v22.0",
                    server: "SERVER001",
                    port: 27001,
                    handle: 2009,
                    time: DateTime.Today.AddHours(10).AddMinutes(21),
                    quantityused: 1,
                    linger: TimeSpan.Zero,
                    isBorrowed: false,
                    borrowEndTime: DateTime.MinValue,
                    entryIndex: 1169 + (2 * indexOffset),
                    entryLength: 76 + indexOffset);
            }
        }

        [TestMethod]
        public void User_Borrowed_PropertyReturnsAreCorrect()
        {
            UtilityProgram.Instance.Executable = new FileInfo("MockUtil.exe");
            License license = new License() { Port = MockUtil.Program.NoDelayPort, Host = "LmStatTest" };
            license.GetStatus();

            Feature feature = license.Features.First(f => f.Name == "Feature_With_Borrow");

            string expectedName = "user006";
            User target = feature.Users.First(u => u.Name == expectedName);

            DateTime expectedTime = DateTime.Today.AddDays(-15).AddHours(11).AddMinutes(28);
            TimeSpan expectedLinger = TimeSpan.FromSeconds(14437140);

            PropertiesComparer.AssertUserPropertiesAreEqual(
                actual: target,
                name: expectedName,
                host: "comp006",
                display: "comp006",
                version: "v22.0",
                server: "SERVER001",
                port: 27001,
                handle: 6301,
                time: expectedTime,
                quantityused: 1,
                linger: expectedLinger,
                isBorrowed: true,
                borrowEndTime: expectedTime.AddSeconds(expectedLinger.TotalSeconds),
                entryIndex: 1429 + (5 * indexOffset),
                entryLength: target.EntryLength);
        }

        [TestMethod]
        public void User_NonWordCharacters_PropertyReturnsAreCorrect()
        {
            UtilityProgram.Instance.Executable = new FileInfo("MockUtil.exe");
            License license = new License() { Port = MockUtil.Program.NoDelayPort, Host = "LmStatTest" };
            license.GetStatus();

            Feature feature = license.Features.First(f => f.Name == "Feature_Non-Word.Characters");

            string expectedName = "user-001";
            User target = feature.Users.First(u => u.Name == expectedName);

            PropertiesComparer.AssertUserPropertiesAreEqual(
                actual: target,
                name: expectedName,
                host: "comp-001",
                display: "comp-001",
                version: "v1.000",
                server: "SERVER.001",
                port: 27001,
                handle: 2009,
                time: DateTime.Today.AddHours(10).AddMinutes(21),
                quantityused: 1,
                linger: TimeSpan.Zero,
                isBorrowed: false,
                borrowEndTime: DateTime.MinValue,
                entryIndex: target.EntryIndex,
                entryLength: 81 + indexOffset);
        }

        [TestMethod]
        public void User_OtherFormats_PropertyReturnsAreCorrect()
        {
            UtilityProgram.Instance.Executable = new FileInfo("MockUtil.exe");
            License license = new License() { Port = MockUtil.Program.NoDelayPort, Host = "LmStatTest" };
            license.GetStatus();

            Feature feature = license.Features.First(f => f.Name == "User_Multiple_Checkouts");

            string expectedName = "user011";
            User target = feature.Users.First(u => u.Name == expectedName);

            PropertiesComparer.AssertUserPropertiesAreEqual(
                actual: target,
                name: expectedName,
                host: "comp011",
                display: "comp011",
                version: "v22.0",
                server: "SERVER001",
                port: 27001,
                handle: 2209,
                time: DateTime.Today.AddHours(13).AddMinutes(21),
                quantityused: 2,
                linger: TimeSpan.Zero,
                isBorrowed: false,
                borrowEndTime: DateTime.MinValue,
                entryIndex: target.EntryIndex,
                entryLength: 88 + indexOffset);
        }

        [TestMethod]
        public void User_NameDisplayWithSpaces_PropertyReturnsAreCorrect()
        {
            UtilityProgram.Instance.Executable = new FileInfo("MockUtil.exe");
            License license = new License() { Port = MockUtil.Program.NoDelayPort, Host = "LmStatTest" };
            license.GetStatus();

            Feature feature = license.Features.First(f => f.Name == "Users_With_Spaces_Auto");

            foreach (User user in feature.Users)
            {
                bool expectedIdentity = user.Name.First() == 'u' && user.Host.First() == 'h' && user.Display.First() == 'd';
                Assert.IsTrue(expectedIdentity, "Name, Host and/or Display are incorrect.");

                PropertiesComparer.AssertUserPropertiesAreEqual(
                    actual: user,
                    name: user.Name,
                    host: user.Host,
                    display: user.Display,
                    version: "v22.0",
                    server: "SERVER001",
                    port: 27001,
                    handle: 100,
                    time: DateTime.Today.AddHours(10).AddMinutes(21),
                    quantityused: 1,
                    linger: TimeSpan.Zero,
                    isBorrowed: false,
                    borrowEndTime: DateTime.MinValue,
                    entryIndex: user.EntryIndex,
                    entryLength: user.EntryLength);
            }
        }

        [TestMethod]
        public void User_NameDisplayWithSpacesOther_PropertyReturnsAreCorrect()
        {
            UtilityProgram.Instance.Executable = new FileInfo("MockUtil.exe");
            License license = new License() { Port = MockUtil.Program.NoDelayPort, Host = "LmStatTest" };
            license.GetStatus();

            Feature feature = license.Features.First(f => f.Name == "Users_With_Spaces_Other");

            Assert.AreEqual(feature.Users.Count, 8, "Users.Count property.");

            Assert.AreEqual(feature.Users[0].Name, "user300", "Name property.");
            Assert.AreEqual(feature.Users[0].Host, "host300", "Host property.");
            Assert.AreEqual(feature.Users[0].Display, "host300", "Display property.");

            Assert.AreEqual(feature.Users[1].Name, "user 301", "Name property.");
            Assert.AreEqual(feature.Users[1].Host, "host300", "Host property.");
            Assert.AreEqual(feature.Users[1].Display, "host300", "Display property.");

            Assert.AreEqual(feature.Users[2].Name, "user 302", "Name property.");
            Assert.AreEqual(feature.Users[2].Host, "host300", "Host property.");
            Assert.AreEqual(feature.Users[2].Display, "host3000.0", "Display property.");

            Assert.AreEqual(feature.Users[3].Name, "user 303", "Name property.");
            Assert.AreEqual(feature.Users[3].Host, "host303", "Host property.");
            Assert.AreEqual(feature.Users[3].Display, "host300", "Display property.");

            Assert.AreEqual(feature.Users[4].Name, "host300", "Name property.");
            Assert.AreEqual(feature.Users[4].Host, "host304", "Host property.");
            Assert.AreEqual(feature.Users[4].Display, "host 304", "Display property.");

            Assert.AreEqual(feature.Users[5].Name, "host300", "Name property.");
            Assert.AreEqual(feature.Users[5].Host, "host300", "Host property.");
            Assert.AreEqual(feature.Users[5].Display, "host 305", "Display property.");

            Assert.AreEqual(feature.Users[6].Name, "host300 A", "Name property.");
            Assert.AreEqual(feature.Users[6].Host, "host300", "Host property.");
            Assert.AreEqual(feature.Users[6].Display, "B host300", "Display property.");

            Assert.AreEqual(feature.Users[7].Name, "host300 host300", "Name property.");
            Assert.AreEqual(feature.Users[7].Host, "host300", "Host property.");
            Assert.AreEqual(feature.Users[7].Display, "host300", "Display property.");
        }

        [TestMethod]
        public void User_NameDisplayWithSpacesFail_PropertyReturnsAreCorrect()
        {
            UtilityProgram.Instance.Executable = new FileInfo("MockUtil.exe");
            License license = new License() { Port = MockUtil.Program.NoDelayPort, Host = "LmStatTest" };
            license.GetStatus();

            Feature feature = license.Features.First(f => f.Name == "Users_With_Spaces_Fail");

            Assert.AreEqual(feature.Users.Count, 1, "Users.Count property.");

            Assert.AreEqual(feature.Users[0].Name, "user 400 host400", "Name property.");
            Assert.AreEqual(feature.Users[0].Host, "display", "Host property.");
            Assert.AreEqual(feature.Users[0].Display, "AB400", "Display property.");
        }

        [TestMethod]
        public void User_IdentityChangedAfterFirstParse_PropertyReturnsAreCorrect()
        {
            // Currently the threading is such that by the time GetStatus returns
            // the identity will be correct. This may not be true in the future.

            UtilityProgram.Instance.Executable = new FileInfo("MockUtil.exe");
            License license = new License() { Port = MockUtil.Program.NoDelayPort, Host = "LmStatTest" };
            license.GetStatus();

            Feature feature = license.Features.First(f => f.Name == "Users_With_Spaces_ChangedEvent");

            foreach (User user in feature.Users)
            {
                bool expectedIdentity = user.Name.First() == 'u' && user.Host.First() == 'h' && user.Display.First() == 'd';
                Assert.IsTrue(expectedIdentity, "Name, Host and/or Display are incorrect.");
            }
        }

        [TestMethod]
        public void User_IdentityChangesWhenNewHostAdded_IdentityChangesAreCorrect()
        {
            // Clear the KnownHostSet.
            PrivateType privateKnownHostSet = new PrivateType(typeof(KnownHostSet));
            PrivateObject privateInstance = new PrivateObject(privateKnownHostSet.GetStaticProperty("Instance", null));
            ((HashSet<string>)privateInstance.GetField("knownHosts")).Clear();

            UtilityProgram.Instance.Executable = new FileInfo("MockUtil.exe");
            License testLicense = new License() { Port = 27001, Host = "LmStatTest" };

            // Using GetStatusAsync may be overkill for testing the property changes, but it
            // throws in some threading.
            using (AutoResetEvent waitHandle = new AutoResetEvent(false))
            {
                testLicense.GetStatusCompleted += (s, e) => waitHandle.Set();

                testLicense.GetStatusAsync();

                if (!waitHandle.WaitOne(5100, false))
                {
                    Assert.Fail("Test timed out.");
                }
            }

            Feature testFeature = testLicense.Features.First(f => f.Name == "Users_With_Spaces_ChangedEvent_Other");
            User target = testFeature.Users.First();

            Assert.AreEqual(target.Name, "user 504 CAD9695D", "Name property.");
            Assert.AreEqual(target.Host, "display", "Host property.");
            Assert.AreEqual(target.Display, "A504", "Display property.");

            List<string> propertiesChanged = new List<string>();
            target.PropertyChanged += (s, e) => propertiesChanged.Add(e.PropertyName);

            License nxLicense = new License() { Port = 27000, Host = "LmStatNX" };

            using (AutoResetEvent waitHandle = new AutoResetEvent(false))
            {
                nxLicense.GetStatusCompleted += (s, e) => waitHandle.Set();

                nxLicense.GetStatusAsync();

                if (!waitHandle.WaitOne(5100, false))
                {
                    Assert.Fail("Test timed out.");
                }
            }

            Assert.IsTrue(propertiesChanged.Contains("Name"));
            Assert.IsTrue(propertiesChanged.Contains("Host"));
            Assert.IsTrue(propertiesChanged.Contains("Display"));

            Assert.AreEqual(target.Name, "user 504", "Name property.");
            Assert.AreEqual(target.Host, "CAD9695D", "Host property.");
            Assert.AreEqual(target.Display, "display A504", "Display property.");
        }
    }
}
