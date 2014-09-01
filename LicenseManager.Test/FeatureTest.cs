// <copyright file="FeatureTest.cs" company="Charles W. Bozarth">
// Copyright (C) 2009-2014 Charles W. Bozarth
// Refer to LicenseManager's License.cs for the full copyright notice.
// </copyright>

// TODO Need better ways to test EntryIndex.

namespace LicenseManager.Test
{
    using System;
    using System.IO;
    using System.Linq;
    using CWBozarth.LicenseManager;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for FeatureTest and is intended
    /// to contain all FeatureTest Unit Tests
    /// </summary>
    [TestClass]
    public class FeatureTest
    {
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
        public void Feature_Empty_PropertyReturnsAreCorrect()
        {
            UtilityProgram.Instance.Executable = new FileInfo("MockUtil.exe");
            License license = new License() { Port = MockUtil.Program.NoDelayPort, Host = "LmStatTest" };
            license.GetStatus();

            string expectedName = "Empty_Feature_One_License";
            Feature target = license.Features.First(f => f.Name == expectedName);

            PropertiesComparer.AssertFeaturePropertiesAreEqual(
                actual: target,
                name: expectedName,
                quantityIssued: 1,
                quantityUsed: 0,
                quantityAvailable: 1,
                quantityBorrowed: 0,
                usersCount: 0,
                vendor: null,
                version: null,
                type: null,
                inUse: false,
                hasError: false,
                errorMessage: null,
                entryIndex: 450 + indexOffset,
                entryLength: 97);
        }

        [TestMethod]
        public void Feature_Used_PropertyReturnsAreCorrect()
        {
            UtilityProgram.Instance.Executable = new FileInfo("MockUtil.exe");
            License license = new License() { Port = MockUtil.Program.NoDelayPort, Host = "LmStatTest" };
            license.GetStatus();

            string expectedName = "Used_Feature_One_License";
            Feature target = license.Features.First(f => f.Name == expectedName);

            PropertiesComparer.AssertFeaturePropertiesAreEqual(
                actual: target,
                name: expectedName,
                quantityIssued: 1,
                quantityUsed: 1,
                quantityAvailable: 0,
                quantityBorrowed: 0,
                usersCount: 1,
                vendor: "testdaemon",
                version: "v22.0",
                type: "floating license",
                inUse: true,
                hasError: false,
                errorMessage: null,
                entryIndex: 547 + indexOffset,
                entryLength: 255 + indexOffset);
        }

        [TestMethod]
        public void Feature_Borrowed_PropertyReturnsAreCorrect()
        {
            UtilityProgram.Instance.Executable = new FileInfo("MockUtil.exe");
            License license = new License() { Port = MockUtil.Program.NoDelayPort, Host = "LmStatTest" };
            license.GetStatus();

            string expectedName = "Feature_With_Borrow";
            Feature target = license.Features.First(f => f.Name == expectedName);

            PropertiesComparer.AssertFeaturePropertiesAreEqual(
                actual: target,
                name: expectedName,
                quantityIssued: 16,
                quantityUsed: 4,
                quantityAvailable: 12,
                quantityBorrowed: 2,
                usersCount: 4,
                vendor: "testdaemon",
                version: "v22.0",
                type: "floating license",
                inUse: true,
                hasError: false,
                errorMessage: null,
                entryIndex: 999 + (2 * indexOffset),
                entryLength: target.EntryLength);
        }

        [TestMethod]
        public void Feature_Error_PropertyReturnsAreCorrect()
        {
            UtilityProgram.Instance.Executable = new FileInfo("MockUtil.exe");
            License license = new License() { Port = MockUtil.Program.NoDelayPort, Host = "LmStatTest" };
            license.GetStatus();

            string expectedName = "Feature_With_Error";
            Feature target = license.Features.First(f => f.Name == expectedName);

            PropertiesComparer.AssertFeaturePropertiesAreEqual(
                actual: target,
                name: expectedName,
                quantityIssued: 0,
                quantityUsed: 0,
                quantityAvailable: 0,
                quantityBorrowed: 0,
                usersCount: 0,
                vendor: null,
                version: null,
                type: null,
                inUse: false,
                hasError: true,
                errorMessage: "Cannot get users of Feature_With_Error: No such feature exists. (-5,222)",
                entryIndex: 802 + (2 * indexOffset),
                entryLength: 105);
        }

        [TestMethod]
        public void Feature_NonWordCharacters_PropertyReturnsAreCorrect()
        {
            UtilityProgram.Instance.Executable = new FileInfo("MockUtil.exe");
            License license = new License() { Port = MockUtil.Program.NoDelayPort, Host = "LmStatTest" };
            license.GetStatus();

            string expectedName = "Feature_Non-Word.Characters";
            Feature target = license.Features.First(f => f.Name == expectedName);

            PropertiesComparer.AssertFeaturePropertiesAreEqual(
                actual: target,
                name: expectedName,
                quantityIssued: 16,
                quantityUsed: 6,
                quantityAvailable: 10,
                quantityBorrowed: 0,
                usersCount: 6,
                vendor: "testdaemon",
                version: "v22.0",
                type: "floating license",
                inUse: true,
                hasError: false,
                errorMessage: null,
                entryIndex: target.EntryIndex,
                entryLength: 694 + (6 * indexOffset));
        }

        [TestMethod]
        public void Feature_UserMultipleCheckouts_PropertyReturnsAreCorrect()
        {
            UtilityProgram.Instance.Executable = new FileInfo("MockUtil.exe");
            License license = new License() { Port = MockUtil.Program.NoDelayPort, Host = "LmStatTest" };
            license.GetStatus();

            string expectedName = "User_Multiple_Checkouts";
            Feature target = license.Features.First(f => f.Name == expectedName);

            PropertiesComparer.AssertFeaturePropertiesAreEqual(
                actual: target,
                name: expectedName,
                quantityIssued: 9,
                quantityUsed: 8,
                quantityAvailable: 1,
                quantityBorrowed: 2,
                usersCount: 4,
                vendor: "testdaemon",
                version: "v22.0",
                type: "floating license",
                inUse: true,
                hasError: false,
                errorMessage: null,
                entryIndex: target.EntryIndex,
                entryLength: 553 + (4 * indexOffset));
        }
    }
}
