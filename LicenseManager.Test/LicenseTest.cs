// <copyright file="LicenseTest.cs" company="Charles W. Bozarth">
// Copyright (C) 2009-2012 Charles W. Bozarth
// Refer to LicenseManager's License.cs for the full copyright notice.
// </copyright>

// Possible Enhancements
//
// Is there a better way to test all cultures?
//
// When testing cultures can the result say which culture failed?

namespace LicenseManager.Test
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Threading;
    using System.Xml.Serialization;
    using CWBozarth.LicenseManager;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for LicenseTest and is intended
    /// to contain all LicenseTest Unit Tests
    /// </summary>
    [TestClass]
    public class LicenseTest
    {
        private static TestFiles testFiles = new TestFiles();

        private static CultureInfo originalCulture = Thread.CurrentThread.CurrentCulture;

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

        //// Use ClassInitialize to run code before running the first test in the class
        ////[ClassInitialize]
        ////public static void MyClassInitialize(TestContext testContext)
        ////{
        ////}
        
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
        public void License_DefaultConfigurationPropertyValues_ValuesAreDefault()
        {
            License target = new License();
            Assert.IsNull(target.Port, "Port property is not null.");
            Assert.IsNull(target.Host, "Host property is not null.");
            Assert.IsNull(target.Name, "Name property is not null.");
        }

        [TestMethod]
        public void License_SerializeDeserialize_SettingsRestored()
        {
            string xmlFile = "SerializedTestLicense.xml";
            string binaryFile = "SerializedTestLicense.bin";

            File.Delete(xmlFile);
            File.Delete(binaryFile);

            int? expectedPort = 11000;
            string expectedHost = "TestLicense";
            string expectedName = "Test License Name";

            License target = new License();
            target.Port = expectedPort;
            target.Host = expectedHost;
            target.Name = expectedName;

            XmlSerializer xmlSerializeLicense = new XmlSerializer(typeof(License));
            Stream xmlWriteFile = new FileStream(xmlFile, FileMode.Create, FileAccess.Write);
            xmlSerializeLicense.Serialize(xmlWriteFile, target);
            xmlWriteFile.Close();

            BinaryFormatter binarySerializeLicense = new BinaryFormatter();
            Stream binaryWriteFile = new FileStream(binaryFile, FileMode.Create, FileAccess.Write);
            binarySerializeLicense.Serialize(binaryWriteFile, target);
            binaryWriteFile.Close();

            Stream xmlReadFile = new FileStream(xmlFile, FileMode.Open, FileAccess.Read);
            License xmlLicense = (License)xmlSerializeLicense.Deserialize(xmlReadFile);
            xmlReadFile.Close();

            Stream binaryReadFile = new FileStream("SerializedTestLicense.bin", FileMode.Open, FileAccess.Read);
            License binaryLicense = (License)binarySerializeLicense.Deserialize(binaryReadFile);
            binaryReadFile.Close();

            Assert.AreEqual(expectedPort, xmlLicense.Port, "Port has changed after xml serialization.");
            Assert.AreEqual(expectedHost, xmlLicense.Host, "Host has changed after xml serialization.");
            Assert.AreEqual(expectedName, xmlLicense.Name, "Name has changed after xml serialization.");

            Assert.AreEqual(expectedPort, binaryLicense.Port, "Port has changed after binary serialization.");
            Assert.AreEqual(expectedHost, binaryLicense.Host, "Host has changed after binary serialization.");
            Assert.AreEqual(expectedName, binaryLicense.Name, "Name has changed after binary serialization.");
        }

        [TestMethod]
        public void License_GetStatusCanExecute_DefinedAsPropertiesChanged()
        {
            License target = new License();

            Assert.IsFalse(target.GetStatusCanExecute, "GetStatusCanExecute incorrect when all properties are invalid.");

            target.Port = 1;
            Assert.IsFalse(target.GetStatusCanExecute, "GetStatusCanExecute incorrect when only Port is valid.");

            target.Port = 0;
            target.Host = "localhost";
            Assert.IsFalse(target.GetStatusCanExecute, "GetStatusCanExecute incorrect when only Host is valid.");

            target.Host = null;
            UtilityProgram.Instance.Executable = new FileInfo(Path.Combine(testFiles.Path, "lmutil.exe"));
            Assert.IsFalse(target.GetStatusCanExecute, "GetStatusCanExecute incorrect when only Executable is valid.");

            target.Port = 1;
            target.Host = "localhost";
            Assert.IsTrue(target.GetStatusCanExecute, "GetStatusCanExecute incorrect when all properties are valid.");

            UtilityProgram.Instance.Executable = new FileInfo(Path.Combine(testFiles.Path, "Abc123_lmutil.exe"));
            Assert.IsFalse(target.GetStatusCanExecute, "GetStatusCanExecute incorrect when only Executable is invalid.");
        }

        [TestMethod]
        public void License_SetProperties_GetReturnsAreCorrect()
        {
            int? expectedPort = 54321;
            string expectedHost = "SetLicenseHost";
            string expectedNameAutomatic = "54321@SetLicenseHost";
            string expectedNameManual = "Set License Name";

            License target = new License();

            target.Port = expectedPort;
            Assert.AreEqual(expectedPort, target.Port, "Port is incorrect.");

            target.Host = expectedHost;
            Assert.AreEqual(expectedHost, target.Host, "Host is incorrect.");

            Assert.AreEqual(expectedNameAutomatic, target.ToString(), "ToString is incorrect.");

            target.Name = expectedNameManual;
            Assert.AreEqual(expectedNameManual, target.Name, "Name is incorrect.");

            Assert.AreEqual(expectedNameManual, target.ToString(), "ToString is incorrect.");

            target.Port = 11000;
            Assert.AreEqual(expectedNameManual, target.ToString(), "ToString is incorrect.");
        }

        [TestMethod]
        public void License_Initialized_PropertyReturnsAreCorrect()
        {
            License target = new License();

            Assert.AreEqual(DateTime.MinValue.ToString(), target.Time.ToString());
            Assert.IsFalse(target.InUse);

            // Features.Count is used as a rough check that the Features property is working.
            Assert.AreEqual(0, target.Features.Count);

            Assert.AreEqual(0, target.InUseCount);
            Assert.AreEqual(0, target.UserCount);
            Assert.IsFalse(target.IsVendorDaemonUp);
            Assert.IsNull(target.VendorDaemonName);
            Assert.IsNull(target.VendorDaemonStatus);
            Assert.IsNull(target.VendorDaemonVersion);
            Assert.IsNull(target.Report);
        }

        [TestMethod]
        public void License_GetStatusFromFile_CompletesWithoutErrors()
        {
            License target = new License();

            foreach (var file in Directory.EnumerateFiles(testFiles.Path, "lmstat-*.log", SearchOption.TopDirectoryOnly))
            {
                target.GetStatus(new FileInfo(file));
            }
        }

        [TestMethod]
        public void License_GetStatusFromTestFileUsingDifferentCultures_CompletesWithoutErrors()
        {
            foreach (CultureInfo culture in CultureInfo.GetCultures(CultureTypes.AllCultures))
            {
                Thread.CurrentThread.CurrentCulture = culture;

                License target = new License();
                target.GetStatus(new FileInfo(Path.Combine(testFiles.Path, "lmstat-test.log")));
            }
        }

        [TestMethod]
        public void License_GetStatusFromTestFile_PropertyReturnsAreCorrect()
        {
            License target = new License();
            target.GetStatus(new FileInfo(Path.Combine(testFiles.Path, "lmstat-test.log")));

            DateTime expectedTime = DateTime.Today.AddHours(10).AddMinutes(43);
            Assert.AreEqual(expectedTime, target.Time);

            Assert.IsTrue(target.InUse);
            Assert.AreEqual(7, target.Features.Count);
            Assert.AreEqual(4, target.InUseCount);
            Assert.AreEqual(35, target.UserCount);
            Assert.IsTrue(target.IsVendorDaemonUp);
            Assert.AreEqual("testdaemon", target.VendorDaemonName);
            Assert.AreEqual("UP", target.VendorDaemonStatus);
            Assert.AreEqual("v10.1", target.VendorDaemonVersion);
            Assert.AreEqual(string.Empty, target.ErrorMessage);
            Assert.IsFalse(target.HasError);
            Assert.IsFalse(target.IsBusy);
            Assert.IsTrue(target.IsFeatureError);
            Assert.IsTrue(target.IsVendorDaemonUp);
            Assert.AreEqual(@"C:\License Servers\Test\Test.lic", target.ServerFile);
        }

        [TestMethod]
        public void License_GetStatusFromTestFileUsingDifferentCultures_PropertyReturnsAreCorrect()
        {
            foreach (CultureInfo culture in CultureInfo.GetCultures(CultureTypes.AllCultures))
            {
                Thread.CurrentThread.CurrentCulture = culture;

                License target = new License();
                target.GetStatus(new FileInfo(Path.Combine(testFiles.Path, "lmstat-test.log")));

                DateTime expectedTime = DateTime.Today.AddHours(10).AddMinutes(43);
                Assert.AreEqual(expectedTime, target.Time);

                Assert.IsTrue(target.InUse);
                Assert.AreEqual(7, target.Features.Count);
                Assert.AreEqual(4, target.InUseCount);
                Assert.AreEqual(35, target.UserCount);
                Assert.IsTrue(target.IsVendorDaemonUp);
                Assert.AreEqual("testdaemon", target.VendorDaemonName);
                Assert.AreEqual("UP", target.VendorDaemonStatus);
                Assert.AreEqual("v10.1", target.VendorDaemonVersion);
                Assert.AreEqual(string.Empty, target.ErrorMessage);
                Assert.IsFalse(target.HasError);
                Assert.IsFalse(target.IsBusy);
                Assert.IsTrue(target.IsFeatureError);
                Assert.IsTrue(target.IsVendorDaemonUp);
                Assert.AreEqual(@"C:\License Servers\Test\Test.lic", target.ServerFile);
            }
        }

        [TestMethod]
        public void License_GetStatusFromFile_PropertyReturnsAreCorrect()
        {
            License target = new License();
            target.GetStatus(new FileInfo(Path.Combine(testFiles.Path, "lmstat-nx.log")));

            DateTime expectedTime = DateTime.Today.AddHours(10).AddMinutes(43);
            Assert.AreEqual(expectedTime, target.Time);

            Assert.IsTrue(target.InUse);
            Assert.AreEqual(135, target.Features.Count);
            Assert.AreEqual(9, target.InUseCount);
            Assert.AreEqual(21, target.UserCount);
            Assert.IsTrue(target.IsVendorDaemonUp);
            Assert.AreEqual("uglmd", target.VendorDaemonName);
            Assert.AreEqual("UP", target.VendorDaemonStatus);
            Assert.AreEqual("v10.8", target.VendorDaemonVersion);
            Assert.AreEqual(string.Empty, target.ErrorMessage);
            Assert.IsFalse(target.HasError);
            Assert.IsFalse(target.IsBusy);
            Assert.IsFalse(target.IsFeatureError);
            Assert.IsTrue(target.IsVendorDaemonUp);
            Assert.AreEqual(@"C:\License Servers\UGNX\UGNX.dat", target.ServerFile);
        }

        [TestMethod]
        public void License_SecondGetStatusFromFile_PropertyReturnsAreCorrect()
        {
            License target = new License();
            target.GetStatus(new FileInfo(Path.Combine(testFiles.Path, "lmstat-nx.log")));
            target.GetStatus(new FileInfo(Path.Combine(testFiles.Path, "lmstat-acad.log")));

            DateTime expectedTime = DateTime.Today.AddHours(10).AddMinutes(43);
            Assert.AreEqual(expectedTime, target.Time);

            Assert.IsTrue(target.InUse);
            Assert.AreEqual(22, target.Features.Count);
            Assert.AreEqual(7, target.InUseCount);
            Assert.AreEqual(112, target.UserCount);
            Assert.IsTrue(target.IsVendorDaemonUp);
            Assert.AreEqual("adskflex", target.VendorDaemonName);
            Assert.AreEqual("UP", target.VendorDaemonStatus);
            Assert.AreEqual("v10.8", target.VendorDaemonVersion);
            Assert.AreEqual(string.Empty, target.ErrorMessage);
            Assert.IsFalse(target.HasError);
            Assert.IsFalse(target.IsBusy);
            Assert.IsFalse(target.IsFeatureError);
            Assert.IsTrue(target.IsVendorDaemonUp);
            Assert.AreEqual(@"C:\License Servers\Autodesk\Autodesk.dat", target.ServerFile);
        }

        [TestMethod]
        public void License_GetStatusFromErrorsFile_PropertyReturnsAreCorrect()
        {
            License target = new License();
            target.GetStatus(new FileInfo(Path.Combine(testFiles.Path, "lmstat-errors.log")));

            DateTime expectedTime = new DateTime(2008, 11, 20, 15, 42, 0);
            Assert.AreEqual(expectedTime, target.Time);

            Assert.IsFalse(target.InUse);
            Assert.AreEqual(11, target.Features.Count);
            Assert.AreEqual(0, target.InUseCount);
            Assert.AreEqual(0, target.UserCount);
            Assert.IsFalse(target.IsVendorDaemonUp);
            Assert.AreEqual("theorem", target.VendorDaemonName);
            Assert.AreEqual("The desired vendor daemon is down. (-97,121)", target.VendorDaemonStatus);
            Assert.AreEqual(string.Empty, target.VendorDaemonVersion);
            Assert.AreEqual(string.Empty, target.ErrorMessage);
            Assert.IsFalse(target.HasError);
            Assert.IsFalse(target.IsBusy);
            Assert.IsTrue(target.IsFeatureError);
            Assert.IsFalse(target.IsVendorDaemonUp);
            Assert.AreEqual(@"D:\License Servers\Theorem\theorem.dat", target.ServerFile);
        }

        [TestMethod]
        public void License_GetStatusFromConnectFile_PropertyReturnsAreCorrect()
        {
            License target = new License();
            target.GetStatus(new FileInfo(Path.Combine(testFiles.Path, "lmstat-connect.log")));

            DateTime expectedTime = DateTime.Today.AddHours(14).AddMinutes(25);
            Assert.AreEqual(expectedTime, target.Time);

            Assert.IsFalse(target.InUse);
            Assert.AreEqual(0, target.Features.Count);
            Assert.AreEqual(0, target.InUseCount);
            Assert.AreEqual(0, target.UserCount);
            Assert.IsFalse(target.IsVendorDaemonUp);
            Assert.AreEqual(string.Empty, target.VendorDaemonName);
            Assert.AreEqual(string.Empty, target.VendorDaemonStatus);
            Assert.AreEqual(string.Empty, target.VendorDaemonVersion);
            Assert.AreEqual("Cannot connect to license server system. (-15,10:10061 \"WinSock: Connection refused\")", target.ErrorMessage);
            Assert.IsTrue(target.HasError);
            Assert.IsFalse(target.IsBusy);
            Assert.IsFalse(target.IsFeatureError);
            Assert.IsFalse(target.IsVendorDaemonUp);
            Assert.IsNull(target.ServerFile);
        }

        [TestMethod]
        public void License_GetStatusAsyncFromFile_CompletesWithoutErrors()
        {
            //// http://stackoverflow.com/questions/1174702/is-there-a-way-to-unit-test-an-async-method

            License target = new License();

            using (AutoResetEvent waitHandle = new AutoResetEvent(false))
            {
                target.GetStatusCompleted += (s, e) =>
                {
                    waitHandle.Set();
                };

                target.GetStatusAsync(new FileInfo(Path.Combine(testFiles.Path, "lmstat-test.log")), 2500);

                Assert.IsTrue(target.IsBusy);
                Assert.IsFalse(target.GetStatusCanExecute);

                if (!waitHandle.WaitOne(5000, false))
                {
                    Assert.Fail("Test timed out.");
                }
            }

            DateTime expectedTime = DateTime.Today.AddHours(10).AddMinutes(43);
            Assert.AreEqual(expectedTime, target.Time);
        }

        [TestMethod]
        public void License_GetStatusAsync_CompletesWithoutErrors()
        {
            License target = new License();
            target.Port = 27000;
            target.Host = "localhost";
            UtilityProgram.Instance.Executable = new FileInfo(Path.Combine(testFiles.Path, "lmutil.exe"));

            using (AutoResetEvent waitHandle = new AutoResetEvent(false))
            {
                target.GetStatusCompleted += (s, e) =>
                {
                    waitHandle.Set();
                };

                target.GetStatusAsync();

                Assert.IsTrue(target.IsBusy);
                Assert.IsFalse(target.GetStatusCanExecute);

                if (!waitHandle.WaitOne(10000, false))
                {
                    Assert.Fail("Test timed out.");
                }
            }

            Assert.AreEqual("Cannot connect to license server system. (-15,10:10061 \"WinSock: Connection refused\")", target.ErrorMessage);
        }
    }
}
