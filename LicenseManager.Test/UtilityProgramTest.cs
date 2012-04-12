using CWBozarth.LicenseManager;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace LicenseManager.Test
{
    /// <summary>
    ///This is a test class for UtilityProgramTest and is intended
    ///to contain all UtilityProgramTest Unit Tests
    ///</summary>
    [TestClass()]
    public class UtilityProgramTest
    {
        private TestContext testContextInstance;

        private static string testFilesPath;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            // Assumes test files are in the Solution folder and assumes this is three folders above the assembly.
            testFilesPath = Path.GetDirectoryName(typeof(UtilityProgramTest).Assembly.Location);
            testFilesPath = Path.GetFullPath(testFilesPath + @"\..\..\..\");

            // lmutil.exe is not included in the solution by default. It must be manually placed in the Solution folder.
            if (!File.Exists(Path.Combine(testFilesPath, "lmutil.exe")))
            {
                throw new FileNotFoundException("The test file lmutil.exe was not found at " + testFilesPath, testFilesPath);
            }
        }
        
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        /// <summary>
        ///A test for Executable
        ///</summary>
        [TestMethod()]
        public void Executable_SetToValidFile_VersionIsAvailable()
        {
            UtilityProgram.Instance.Executable = new FileInfo(Path.Combine(testFilesPath, "lmutil.exe"));

            string expected = "11.4.100.0";
            string actual = UtilityProgram.Instance.Version;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void Executable_SetToInvalidFile_VersionIsEmpty()
        {
            UtilityProgram.Instance.Executable = new FileInfo(Path.Combine(testFilesPath, "Abc123_lmutil.exe"));

            string expected = string.Empty;
            string actual = UtilityProgram.Instance.Version;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void Executable_SetToNull_VersionIsEmpty()
        {
            UtilityProgram.Instance.Executable = null;

            string expected = string.Empty;
            string actual = UtilityProgram.Instance.Version;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void Executable_Set_CauseDataErrorChanges()
        {
            UtilityProgram.Instance.Executable = new FileInfo(Path.Combine(testFilesPath, "lmutil.exe"));
            Assert.IsTrue(string.IsNullOrEmpty(UtilityProgram.Instance["Executable"]), "A valid Executable caused an error.");

            UtilityProgram.Instance.Executable = new FileInfo(Path.Combine(testFilesPath, "Abc123_lmutil.exe"));
            Assert.IsFalse(string.IsNullOrEmpty(UtilityProgram.Instance["Executable"]), "An invalid Executable did not cause an error.");

            UtilityProgram.Instance.Executable = null;
            Assert.IsFalse(string.IsNullOrEmpty(UtilityProgram.Instance["Executable"]), "A null Executable did not cause an error.");
        }
    }
}
