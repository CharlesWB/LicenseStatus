// <copyright file="LicenseTest.cs" company="Charles W. Bozarth">
// Copyright (C) 2009-2014 Charles W. Bozarth
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
            UtilityProgram.Instance.Executable = new FileInfo("MockUtil.exe");
            Assert.IsFalse(target.GetStatusCanExecute, "GetStatusCanExecute incorrect when only Executable is valid.");

            target.Port = 1;
            target.Host = "localhost";
            Assert.IsTrue(target.GetStatusCanExecute, "GetStatusCanExecute incorrect when all properties are valid.");

            UtilityProgram.Instance.Executable = new FileInfo("Abc123_lmutil.exe");
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
            Assert.AreEqual(expectedPort, target.Port, "Port property.");

            target.Host = expectedHost;
            Assert.AreEqual(expectedHost, target.Host, "Host property.");

            Assert.AreEqual(expectedNameAutomatic, target.ToString(), "ToString is incorrect.");

            target.Name = expectedNameManual;
            Assert.AreEqual(expectedNameManual, target.Name, "Name property.");

            Assert.AreEqual(expectedNameManual, target.ToString(), "ToString is incorrect.");

            target.Port = 11000;
            Assert.AreEqual(expectedNameManual, target.ToString(), "ToString is incorrect.");
        }

        [TestMethod]
        public void License_Initialized_PropertyReturnsAreCorrect()
        {
            License target = new License();

            PropertiesComparer.AssertLicensePropertiesAreEqual(
                actual: target,
                time: DateTime.MinValue,
                serverFile: null,
                vendorDaemonName: null,
                vendorDaemonStatus: null,
                vendorDaemonVersion: null,
                isVendorDaemonUp: false,
                featuresCount: 0,
                isFeatureError: false,
                userCount: 0,
                inUse: false,
                inUseCount: 0,
                hasError: false,
                errorMessage: null,
                isBusy: false);

            Assert.IsNull(target.Report, "Report property");
        }

        [TestMethod]
        public void License_GetStatusFromTestUsingDifferentCultures_CompletesWithoutErrors()
        {
            UtilityProgram.Instance.Executable = new FileInfo("MockUtil.exe");

            foreach (CultureInfo culture in CultureInfo.GetCultures(CultureTypes.AllCultures))
            {
                Thread.CurrentThread.CurrentCulture = culture;

                License target = new License() { Port = MockUtil.Program.NoDelayPort, Host = "LmStatTest" };
                target.GetStatus();
            }
        }

        [TestMethod]
        public void License_GetStatusFromTest_PropertyReturnsAreCorrect()
        {
            UtilityProgram.Instance.Executable = new FileInfo("MockUtil.exe");
            License target = new License() { Port = 27001, Host = "LmStatTest" };
            target.GetStatus();

            PropertiesComparer.AssertLicensePropertiesAreEqual(
                actual: target,
                time: DateTime.Today.AddHours(10).AddMinutes(43),
                serverFile: @"C:\License Servers\Test\Test.lic",
                vendorDaemonName: "testdaemon",
                vendorDaemonStatus: "UP",
                vendorDaemonVersion: "v10.1",
                isVendorDaemonUp: true,
                featuresCount: 13,
                isFeatureError: true,
                userCount: 67,
                inUse: true,
                inUseCount: 10,
                hasError: false,
                errorMessage: string.Empty,
                isBusy: false);
        }

        [TestMethod]
        public void License_GetStatusFromTestUsingDifferentCultures_PropertyReturnsAreCorrect()
        {
            UtilityProgram.Instance.Executable = new FileInfo("MockUtil.exe");

            foreach (CultureInfo culture in CultureInfo.GetCultures(CultureTypes.AllCultures))
            {
                Thread.CurrentThread.CurrentCulture = culture;

                License target = new License() { Port = MockUtil.Program.NoDelayPort, Host = "LmStatTest" };
                target.GetStatus();

                PropertiesComparer.AssertLicensePropertiesAreEqual(
                    actual: target,
                    time: DateTime.Today.AddHours(10).AddMinutes(43),
                    serverFile: @"C:\License Servers\Test\Test.lic",
                    vendorDaemonName: "testdaemon",
                    vendorDaemonStatus: "UP",
                    vendorDaemonVersion: "v10.1",
                    isVendorDaemonUp: true,
                    featuresCount: 13,
                    isFeatureError: true,
                    userCount: 67,
                    inUse: true,
                    inUseCount: 10,
                    hasError: false,
                    errorMessage: string.Empty,
                    isBusy: false);
            }
        }

        [TestMethod]
        public void License_GetStatusFromNX_PropertyReturnsAreCorrect()
        {
            UtilityProgram.Instance.Executable = new FileInfo("MockUtil.exe");
            License target = new License() { Port = 27001, Host = "LmStatNX" };
            target.GetStatus();

            PropertiesComparer.AssertLicensePropertiesAreEqual(
                actual: target,
                time: DateTime.Today.AddHours(10).AddMinutes(43),
                serverFile: @"C:\License Servers\UGNX\UGNX.dat",
                vendorDaemonName: "uglmd",
                vendorDaemonStatus: "UP",
                vendorDaemonVersion: "v10.8",
                isVendorDaemonUp: true,
                featuresCount: 135,
                isFeatureError: false,
                userCount: 21,
                inUse: true,
                inUseCount: 9,
                hasError: false,
                errorMessage: string.Empty,
                isBusy: false);
        }

        [TestMethod]
        public void License_SecondGetStatus_PropertyReturnsAreCorrect()
        {
            UtilityProgram.Instance.Executable = new FileInfo("MockUtil.exe");
            License target = new License() { Port = 27001, Host = "LmStatNX" };
            target.GetStatus();
            target.Port = 27005;
            target.Host = "LmStatAcad";
            target.GetStatus();

            PropertiesComparer.AssertLicensePropertiesAreEqual(
                actual: target,
                time: DateTime.Today.AddHours(10).AddMinutes(43),
                serverFile: @"C:\License Servers\Autodesk\Autodesk.dat",
                vendorDaemonName: "adskflex",
                vendorDaemonStatus: "UP",
                vendorDaemonVersion: "v10.8",
                isVendorDaemonUp: true,
                featuresCount: 22,
                isFeatureError: false,
                userCount: 112,
                inUse: true,
                inUseCount: 7,
                hasError: false,
                errorMessage: string.Empty,
                isBusy: false);
        }

        [TestMethod]
        public void License_GetStatusFromErrors_PropertyReturnsAreCorrect()
        {
            UtilityProgram.Instance.Executable = new FileInfo("MockUtil.exe");
            License target = new License() { Port = 7601, Host = "LmStatErrors" };
            target.GetStatus();

            PropertiesComparer.AssertLicensePropertiesAreEqual(
                actual: target,
                time: new DateTime(2008, 11, 20, 15, 42, 0),
                serverFile: @"D:\License Servers\Theorem\theorem.dat",
                vendorDaemonName: "theorem",
                vendorDaemonStatus: "The desired vendor daemon is down. (-97,121)",
                vendorDaemonVersion: string.Empty,
                isVendorDaemonUp: false,
                featuresCount: 11,
                isFeatureError: true,
                userCount: 0,
                inUse: false,
                inUseCount: 0,
                hasError: false,
                errorMessage: string.Empty,
                isBusy: false);
        }

        [TestMethod]
        public void License_GetStatusFromConnect_PropertyReturnsAreCorrect()
        {
            UtilityProgram.Instance.Executable = new FileInfo("MockUtil.exe");
            License target = new License() { Port = 7601, Host = "LmStatConnect" };
            target.GetStatus();

            PropertiesComparer.AssertLicensePropertiesAreEqual(
                actual: target,
                time: DateTime.Today.AddHours(14).AddMinutes(25),
                serverFile: null,
                vendorDaemonName: string.Empty,
                vendorDaemonStatus: string.Empty,
                vendorDaemonVersion: string.Empty,
                isVendorDaemonUp: false,
                featuresCount: 0,
                isFeatureError: false,
                userCount: 0,
                inUse: false,
                inUseCount: 0,
                hasError: true,
                errorMessage: "Cannot connect to license server system. (-15,10:10061 \"WinSock: Connection refused\")",
                isBusy: false);
        }

        [TestMethod]
        public void License_GetStatusAsyncMock_CompletesWithoutErrors()
        {
            //// http://stackoverflow.com/questions/1174702/is-there-a-way-to-unit-test-an-async-method

            UtilityProgram.Instance.Executable = new FileInfo("MockUtil.exe");
            License target = new License() { Port = 27001, Host = "LmStatTest" };

            using (AutoResetEvent waitHandle = new AutoResetEvent(false))
            {
                target.GetStatusCompleted += (s, e) => waitHandle.Set();

                target.GetStatusAsync();

                Assert.IsTrue(target.IsBusy);
                Assert.IsFalse(target.GetStatusCanExecute);

                if (!waitHandle.WaitOne(5100, false))
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
            // This code also exists in UtilityProgramtest.MyClassInitialize.
            // Assumes the lmutil is in the Solution folder and assumes this is three folders above the assembly.
            // lmutil.exe is not included in the solution by default. It must be manually placed in the Solution folder.
            string path = System.IO.Path.GetDirectoryName(typeof(UtilityProgramTest).Assembly.Location);
            path = System.IO.Path.GetFullPath(path + @"\..\..\..\");
            UtilityProgram.Instance.Executable = new FileInfo(System.IO.Path.Combine(path, "lmutil.exe"));

            License target = new License();
            target.Port = 27000;
            target.Host = "localhost";

            using (AutoResetEvent waitHandle = new AutoResetEvent(false))
            {
                target.GetStatusCompleted += (s, e) => waitHandle.Set();

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
