// <copyright file="UserTest.cs" company="Charles W. Bozarth">
// Copyright (C) 2009-2012 Charles W. Bozarth
// Refer to LicenseManager's License.cs for the full copyright notice.
// </copyright>

namespace LicenseManager.Test
{
    using System;
    using System.IO;
    using System.Linq;
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

        //// Use TestInitialize to run code before running each test
        ////[TestInitialize]
        ////public void MyTestInitialize()
        ////{
        ////}

        //// Use TestCleanup to run code after each test has run
        ////[TestCleanup]
        ////public void MyTestCleanup()
        ////{
        ////}
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

            Assert.AreEqual(TimeSpan.Zero, target.Linger);
            Assert.AreEqual(DateTime.MinValue, target.BorrowEndTime);
            Assert.IsFalse(target.IsBorrowed);

            Assert.AreEqual(1169 + indexOffset * 2, target.EntryIndex);
            Assert.AreEqual(76 + indexOffset, target.EntryLength);
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

            TimeSpan expectedLinger = TimeSpan.FromSeconds(14437140);
            Assert.AreEqual(expectedLinger, target.Linger);

            DateTime expectedBorrowEndTime = expectedTime.AddSeconds(expectedLinger.TotalSeconds);
            Assert.AreEqual(expectedBorrowEndTime, target.BorrowEndTime);

            Assert.IsTrue(target.IsBorrowed);

            Assert.AreEqual(1429 + indexOffset * 5, target.EntryIndex);
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

            Assert.AreEqual(TimeSpan.Zero, target.Linger);
            Assert.AreEqual(DateTime.MinValue, target.BorrowEndTime);
            Assert.IsFalse(target.IsBorrowed);

            Assert.AreEqual(81 + indexOffset, target.EntryLength);
        }
    }
}
