// <copyright file="FeatureTest.cs" company="Charles W. Bozarth">
// Copyright (C) 2009-2012 Charles W. Bozarth
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
        private static string testFilesPath;

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
            // Duplicated from UtilityProgramTest.

            // Assumes test files are in the Solution folder and assumes this is three folders above the assembly.
            testFilesPath = Path.GetDirectoryName(typeof(UtilityProgramTest).Assembly.Location);
            testFilesPath = Path.GetFullPath(testFilesPath + @"\..\..\..\");

            // lmutil.exe is not included in the solution by default. It must be manually placed in the Solution folder.
            if (!File.Exists(Path.Combine(testFilesPath, "lmutil.exe")))
            {
                throw new FileNotFoundException("The test file lmutil.exe was not found at " + testFilesPath, testFilesPath);
            }

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
            License license = new License();
            license.GetStatus(new FileInfo(Path.Combine(testFilesPath, "lmstat-test.log")));

            string expectedName = "Empty_Feature_One_License";
            Feature target = license.Features.First(f => f.Name == expectedName);

            Assert.AreEqual(expectedName, target.Name);
            Assert.AreEqual(1, target.QuantityAvailable);
            Assert.AreEqual(1, target.QuantityIssued);
            Assert.AreEqual(0, target.QuantityUsed);
            Assert.AreEqual(0, target.QuantityBorrowed);
            Assert.IsFalse(target.InUse);
            Assert.IsNull(target.Type);
            Assert.IsNull(target.Vendor);
            Assert.IsNull(target.Version);
            Assert.IsFalse(target.HasError);
            Assert.IsNull(target.ErrorMessage);
            Assert.AreEqual(0, target.Users.Count);
            Assert.AreEqual(450 + indexOffset, target.EntryIndex);
            Assert.AreEqual(97, target.EntryLength);
        }

        [TestMethod]
        public void Feature_Used_PropertyReturnsAreCorrect()
        {
            License license = new License();
            license.GetStatus(new FileInfo(Path.Combine(testFilesPath, "lmstat-test.log")));

            string expectedName = "Used_Feature_One_License";
            Feature target = license.Features.First(f => f.Name == expectedName);

            Assert.AreEqual(expectedName, target.Name);
            Assert.AreEqual(0, target.QuantityAvailable);
            Assert.AreEqual(1, target.QuantityIssued);
            Assert.AreEqual(1, target.QuantityUsed);
            Assert.AreEqual(0, target.QuantityBorrowed);
            Assert.IsTrue(target.InUse);
            Assert.AreEqual("floating license", target.Type);
            Assert.AreEqual("testdaemon", target.Vendor);
            Assert.AreEqual("v22.0", target.Version);
            Assert.IsFalse(target.HasError);
            Assert.IsNull(target.ErrorMessage);
            Assert.AreEqual(1, target.Users.Count);
            Assert.AreEqual(547 + indexOffset, target.EntryIndex);
            Assert.AreEqual(255+ indexOffset, target.EntryLength);
        }

        [TestMethod]
        public void Feature_Borrowed_PropertyReturnsAreCorrect()
        {
            License license = new License();
            license.GetStatus(new FileInfo(Path.Combine(testFilesPath, "lmstat-test.log")));

            string expectedName = "Feature_With_Borrow";
            Feature target = license.Features.First(f => f.Name == expectedName);

            Assert.AreEqual(expectedName, target.Name);
            Assert.AreEqual(12, target.QuantityAvailable);
            Assert.AreEqual(16, target.QuantityIssued);
            Assert.AreEqual(4, target.QuantityUsed);
            Assert.AreEqual(2, target.QuantityBorrowed);
            Assert.IsTrue(target.InUse);
            Assert.AreEqual("floating license", target.Type);
            Assert.AreEqual("testdaemon", target.Vendor);
            Assert.AreEqual("v22.0", target.Version);
            Assert.IsFalse(target.HasError);
            Assert.IsNull(target.ErrorMessage);
            Assert.AreEqual(4, target.Users.Count);
            Assert.AreEqual(999 + indexOffset * 2, target.EntryIndex);
        }

        [TestMethod]
        public void Feature_Error_PropertyReturnsAreCorrect()
        {
            License license = new License();
            license.GetStatus(new FileInfo(Path.Combine(testFilesPath, "lmstat-test.log")));

            string expectedName = "Feature_With_Error";
            Feature target = license.Features.First(f => f.Name == expectedName);

            Assert.AreEqual(expectedName, target.Name);
            Assert.AreEqual(0, target.QuantityAvailable);
            Assert.AreEqual(0, target.QuantityIssued);
            Assert.AreEqual(0, target.QuantityUsed);
            Assert.AreEqual(0, target.QuantityBorrowed);
            Assert.IsFalse(target.InUse);
            Assert.IsNull(target.Type);
            Assert.IsNull(target.Vendor);
            Assert.IsNull(target.Version);
            Assert.IsTrue(target.HasError);
            Assert.AreEqual("Cannot get users of Feature_With_Error: No such feature exists. (-5,222)", target.ErrorMessage);
            Assert.AreEqual(0, target.Users.Count);
            Assert.AreEqual(802 + indexOffset * 2, target.EntryIndex);
            Assert.AreEqual(105, target.EntryLength);
        }

        [TestMethod]
        public void Feature_NonWordCharacters_PropertyReturnsAreCorrect()
        {
            License license = new License();
            license.GetStatus(new FileInfo(Path.Combine(testFilesPath, "lmstat-test.log")));

            string expectedName = "Feature_Non-Word.Characters";
            Feature target = license.Features.First(f => f.Name == expectedName);

            Assert.AreEqual(expectedName, target.Name);
            Assert.AreEqual(10, target.QuantityAvailable);
            Assert.AreEqual(16, target.QuantityIssued);
            Assert.AreEqual(6, target.QuantityUsed);
            Assert.AreEqual(0, target.QuantityBorrowed);
            Assert.IsTrue(target.InUse);
            Assert.AreEqual("floating license", target.Type);
            Assert.AreEqual("testdaemon", target.Vendor);
            Assert.AreEqual("v22.0", target.Version);
            Assert.IsFalse(target.HasError);
            Assert.IsNull(target.ErrorMessage);
            Assert.AreEqual(6, target.Users.Count);
            Assert.AreEqual(692 + indexOffset * 6, target.EntryLength);
        }
    }
}
