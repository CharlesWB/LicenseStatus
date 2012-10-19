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
        private static TestFiles testFiles = new TestFiles();

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
            License license = new License();
            license.GetStatus(new FileInfo(Path.Combine(testFiles.Path, "lmstat-test.log")));

            Feature feature = license.Features.First(f => f.Name == "Feature_With_Borrow");

            string expectedName = "user003";
            User target = feature.Users.First(u => u.Name == expectedName);

            Assert.AreEqual(expectedName, target.Name);
            Assert.AreEqual("comp003", target.Host);
            Assert.AreEqual("comp003", target.Display);
            Assert.AreEqual("v22.0", target.Version);
            Assert.AreEqual("SERVER001", target.Server);
            Assert.AreEqual(27001, target.Port);
            Assert.AreEqual(2009, target.Handle);

            DateTime expectedTime = DateTime.Today.AddHours(10).AddMinutes(21);
            Assert.AreEqual(expectedTime, target.Time);

            Assert.AreEqual(1, target.QuantityUsed);

            Assert.AreEqual(TimeSpan.Zero, target.Linger);
            Assert.AreEqual(DateTime.MinValue, target.BorrowEndTime);
            Assert.IsFalse(target.IsBorrowed);

            Assert.AreEqual(1169 + (indexOffset * 2), target.EntryIndex);
            Assert.AreEqual(76 + indexOffset, target.EntryLength);
        }

        [TestMethod]
        public void User_TypicalUsingDifferentCultures_PropertyReturnsAreCorrect()
        {
            foreach (CultureInfo culture in CultureInfo.GetCultures(CultureTypes.AllCultures))
            {
                Thread.CurrentThread.CurrentCulture = culture;

                License license = new License();
                license.GetStatus(new FileInfo(Path.Combine(testFiles.Path, "lmstat-test.log")));

                Feature feature = license.Features.First(f => f.Name == "Feature_With_Borrow");

                string expectedName = "user003";
                User target = feature.Users.First(u => u.Name == expectedName);

                Assert.AreEqual(expectedName, target.Name);
                Assert.AreEqual("comp003", target.Host);
                Assert.AreEqual("comp003", target.Display);
                Assert.AreEqual("v22.0", target.Version);
                Assert.AreEqual("SERVER001", target.Server);
                Assert.AreEqual(27001, target.Port);
                Assert.AreEqual(2009, target.Handle);

                DateTime expectedTime = DateTime.Today.AddHours(10).AddMinutes(21);
                Assert.AreEqual(expectedTime, target.Time);

                Assert.AreEqual(1, target.QuantityUsed);

                Assert.AreEqual(TimeSpan.Zero, target.Linger);
                Assert.AreEqual(DateTime.MinValue, target.BorrowEndTime);
                Assert.IsFalse(target.IsBorrowed);

                Assert.AreEqual(1169 + (indexOffset * 2), target.EntryIndex);
                Assert.AreEqual(76 + indexOffset, target.EntryLength);
            }
        }

        [TestMethod]
        public void User_Borrowed_PropertyReturnsAreCorrect()
        {
            License license = new License();
            license.GetStatus(new FileInfo(Path.Combine(testFiles.Path, "lmstat-test.log")));

            Feature feature = license.Features.First(f => f.Name == "Feature_With_Borrow");

            string expectedName = "user006";
            User target = feature.Users.First(u => u.Name == expectedName);

            Assert.AreEqual(expectedName, target.Name);
            Assert.AreEqual("comp006", target.Host);
            Assert.AreEqual("comp006", target.Display);
            Assert.AreEqual("v22.0", target.Version);
            Assert.AreEqual("SERVER001", target.Server);
            Assert.AreEqual(27001, target.Port);
            Assert.AreEqual(6301, target.Handle);

            DateTime expectedTime = DateTime.Today.AddDays(-15).AddHours(11).AddMinutes(28);
            Assert.AreEqual(expectedTime, target.Time);

            Assert.AreEqual(1, target.QuantityUsed);

            TimeSpan expectedLinger = TimeSpan.FromSeconds(14437140);
            Assert.AreEqual(expectedLinger, target.Linger);

            DateTime expectedBorrowEndTime = expectedTime.AddSeconds(expectedLinger.TotalSeconds);
            Assert.AreEqual(expectedBorrowEndTime, target.BorrowEndTime);

            Assert.IsTrue(target.IsBorrowed);

            Assert.AreEqual(1429 + (indexOffset * 5), target.EntryIndex);
        }

        [TestMethod]
        public void User_NonWordCharacters_PropertyReturnsAreCorrect()
        {
            License license = new License();
            license.GetStatus(new FileInfo(Path.Combine(testFiles.Path, "lmstat-test.log")));

            Feature feature = license.Features.First(f => f.Name == "Feature_Non-Word.Characters");

            string expectedName = "user-001";
            User target = feature.Users.First(u => u.Name == expectedName);

            Assert.AreEqual(expectedName, target.Name);
            Assert.AreEqual("comp-001", target.Host);
            Assert.AreEqual("comp-001", target.Display);
            Assert.AreEqual("v1.000", target.Version);
            Assert.AreEqual("SERVER.001", target.Server);
            Assert.AreEqual(27001, target.Port);
            Assert.AreEqual(2009, target.Handle);

            DateTime expectedTime = DateTime.Today.AddHours(10).AddMinutes(21);
            Assert.AreEqual(expectedTime, target.Time);

            Assert.AreEqual(1, target.QuantityUsed);

            Assert.AreEqual(TimeSpan.Zero, target.Linger);
            Assert.AreEqual(DateTime.MinValue, target.BorrowEndTime);
            Assert.IsFalse(target.IsBorrowed);

            Assert.AreEqual(81 + indexOffset, target.EntryLength);
        }

        [TestMethod]
        public void User_OtherFormats_PropertyReturnsAreCorrect()
        {
            License license = new License();
            license.GetStatus(new FileInfo(Path.Combine(testFiles.Path, "lmstat-test.log")));

            Feature feature = license.Features.First(f => f.Name == "User_Multiple_Checkouts");

            string expectedName = "user011";
            User target = feature.Users.First(u => u.Name == expectedName);

            Assert.AreEqual(expectedName, target.Name);
            Assert.AreEqual("comp011", target.Host);
            Assert.AreEqual("comp011", target.Display);
            Assert.AreEqual("v22.0", target.Version);
            Assert.AreEqual("SERVER001", target.Server);
            Assert.AreEqual(27001, target.Port);
            Assert.AreEqual(2209, target.Handle);

            DateTime expectedTime = DateTime.Today.AddHours(13).AddMinutes(21);
            Assert.AreEqual(expectedTime, target.Time);

            Assert.AreEqual(2, target.QuantityUsed);

            Assert.AreEqual(TimeSpan.Zero, target.Linger);
            Assert.AreEqual(DateTime.MinValue, target.BorrowEndTime);
            Assert.IsFalse(target.IsBorrowed);

            Assert.AreEqual(88 + indexOffset, target.EntryLength);
        }

        [TestMethod]
        public void User_NameDisplayWithSpaces_PropertyReturnsAreCorrect()
        {
            License license = new License();
            license.GetStatus(new FileInfo(Path.Combine(testFiles.Path, "lmstat-test.log")));

            Feature feature = license.Features.First(f => f.Name == "Users_With_Spaces_Auto");

            foreach (User user in feature.Users)
            {
                bool expectedIdentity = user.Name.First() == 'u' && user.Host.First() == 'h' && user.Display.First() == 'd';
                Assert.IsTrue(expectedIdentity);

                Assert.AreEqual("v22.0", user.Version);
                Assert.AreEqual("SERVER001", user.Server);
                Assert.AreEqual(27001, user.Port);
                Assert.AreEqual(100, user.Handle);

                DateTime expectedTime = DateTime.Today.AddHours(10).AddMinutes(21);
                Assert.AreEqual(expectedTime, user.Time);

                Assert.AreEqual(1, user.QuantityUsed);

                Assert.AreEqual(TimeSpan.Zero, user.Linger);
                Assert.AreEqual(DateTime.MinValue, user.BorrowEndTime);
                Assert.IsFalse(user.IsBorrowed);
            }
        }

        [TestMethod]
        public void User_NameDisplayWithSpacesOther_PropertyReturnsAreCorrect()
        {
            License license = new License();
            license.GetStatus(new FileInfo(Path.Combine(testFiles.Path, "lmstat-test.log")));

            Feature feature = license.Features.First(f => f.Name == "Users_With_Spaces_Other");

            Assert.AreEqual(feature.Users.Count, 8);

            Assert.AreEqual(feature.Users[0].Name, "user300");
            Assert.AreEqual(feature.Users[0].Host, "host300");
            Assert.AreEqual(feature.Users[0].Display, "host300");

            Assert.AreEqual(feature.Users[1].Name, "user 301");
            Assert.AreEqual(feature.Users[1].Host, "host300");
            Assert.AreEqual(feature.Users[1].Display, "host300");

            Assert.AreEqual(feature.Users[2].Name, "user 302");
            Assert.AreEqual(feature.Users[2].Host, "host300");
            Assert.AreEqual(feature.Users[2].Display, "host3000.0");

            Assert.AreEqual(feature.Users[3].Name, "user 303");
            Assert.AreEqual(feature.Users[3].Host, "host303");
            Assert.AreEqual(feature.Users[3].Display, "host300");

            Assert.AreEqual(feature.Users[4].Name, "host300");
            Assert.AreEqual(feature.Users[4].Host, "host304");
            Assert.AreEqual(feature.Users[4].Display, "host 304");

            Assert.AreEqual(feature.Users[5].Name, "host300");
            Assert.AreEqual(feature.Users[5].Host, "host300");
            Assert.AreEqual(feature.Users[5].Display, "host 305");

            Assert.AreEqual(feature.Users[6].Name, "host300 A");
            Assert.AreEqual(feature.Users[6].Host, "host300");
            Assert.AreEqual(feature.Users[6].Display, "B host300");

            Assert.AreEqual(feature.Users[7].Name, "host300 host300");
            Assert.AreEqual(feature.Users[7].Host, "host300");
            Assert.AreEqual(feature.Users[7].Display, "host300");
        }

        [TestMethod]
        public void User_NameDisplayWithSpacesFail_PropertyReturnsAreCorrect()
        {
            License license = new License();
            license.GetStatus(new FileInfo(Path.Combine(testFiles.Path, "lmstat-test.log")));

            Feature feature = license.Features.First(f => f.Name == "Users_With_Spaces_Fail");

            Assert.AreEqual(feature.Users.Count, 1);

            Assert.AreEqual(feature.Users[0].Name, "user 400 host400");
            Assert.AreEqual(feature.Users[0].Host, "display");
            Assert.AreEqual(feature.Users[0].Display, "AB400");
        }

        [TestMethod]
        public void User_IdentityChangedAfterFirstParse_PropertyReturnsAreCorrect()
        {
            // Currently the threading is such that by the time GetStatus returns
            // the identity will be correct. This may not be true in the future.

            License license = new License();
            license.GetStatus(new FileInfo(Path.Combine(testFiles.Path, "lmstat-test.log")));

            Feature feature = license.Features.First(f => f.Name == "Users_With_Spaces_ChangedEvent");

            foreach (User user in feature.Users)
            {
                bool expectedIdentity = user.Name.First() == 'u' && user.Host.First() == 'h' && user.Display.First() == 'd';
                Assert.IsTrue(expectedIdentity);
            }
        }

        [TestMethod]
        public void User_IdentityChangesWhenNewHostAdded_IdentityChangesAreCorrect()
        {
            // Clear the KnownHostSet.
            PrivateType privateKnownHostSet = new PrivateType(typeof(KnownHostSet));
            PrivateObject privateInstance = new PrivateObject(privateKnownHostSet.GetStaticProperty("Instance", null));
            ((HashSet<string>)privateInstance.GetField("knownHosts")).Clear();

            License testLicense = new License();

            // Using GetStatusAsync may be overkill for testing the property changes, but it
            // throws in some threading.
            using (AutoResetEvent waitHandle = new AutoResetEvent(false))
            {
                testLicense.GetStatusCompleted += (s, e) => waitHandle.Set();

                testLicense.GetStatusAsync(new FileInfo(Path.Combine(testFiles.Path, "lmstat-test.log")), 500);

                if (!waitHandle.WaitOne(5000, false))
                {
                    Assert.Fail("Test timed out.");
                }
            }

            Feature testFeature = testLicense.Features.First(f => f.Name == "Users_With_Spaces_ChangedEvent_Other");
            User target = testFeature.Users.First();

            Assert.AreEqual(target.Name, "user 504 CAD9695D");
            Assert.AreEqual(target.Host, "display");
            Assert.AreEqual(target.Display, "A504");

            List<string> propertiesChanged = new List<string>();
            target.PropertyChanged += (s, e) => propertiesChanged.Add(e.PropertyName);

            License nxLicense = new License();

            using (AutoResetEvent waitHandle = new AutoResetEvent(false))
            {
                nxLicense.GetStatusCompleted += (s, e) => waitHandle.Set();

                nxLicense.GetStatusAsync(new FileInfo(Path.Combine(testFiles.Path, "lmstat-nx.log")), 500);

                if (!waitHandle.WaitOne(5000, false))
                {
                    Assert.Fail("Test timed out.");
                }
            }

            Assert.IsTrue(propertiesChanged.Contains("Name"));
            Assert.IsTrue(propertiesChanged.Contains("Host"));
            Assert.IsTrue(propertiesChanged.Contains("Display"));

            Assert.AreEqual(target.Name, "user 504");
            Assert.AreEqual(target.Host, "CAD9695D");
            Assert.AreEqual(target.Display, "display A504");
        }
    }
}
