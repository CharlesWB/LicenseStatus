// <copyright file="Program.cs" company="Charles W. Bozarth">
// Copyright (C) 2009-2012 Charles W. Bozarth
// Refer to License.cs for the full copyright notice.
// </copyright>

namespace LicenseManagerTests
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Windows.Forms;
    using CWBozarth.LicenseManager;
    using System.Xml.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;

    /// <summary>
    /// Some general tests of LicenseManager.
    /// </summary>
    class Program
    {
        // Test files and lmutil are stored in the solution folder which is expected to be three folders
        // above the executable. GetFullPath is used to translate the relative path to an absolute path
        // only to make the string a little shorter on the console.
        private static string testFilesPath = Path.GetFullPath(Application.StartupPath + @"\..\..\..\");

        static void Main(string[] args)
        {
            ConsoleTraceListener listener = new ConsoleTraceListener();
            Trace.Listeners.Add(listener);
            Trace.WriteLine("Testing of LicenseManager");
            Trace.WriteLine("");

            LicenseSingleton();

            LicenseConstructor();

            // To simulate creating an uninstantiated License set the singleton value to its default
            // value of null before running the next test.
            UtilityProgram.Instance.Executable = null;

            LicenseDefaultConfigurationProperties();
            FeatureDefaultConfigurationProperties();
            UserDefaultConfigurationProperties();

            LicenseSerialize();

            LicenseGetStatusCanExecuteProperty();

            LicenseConfigurationProperties();

            LicenseProperties();

            LicenseAsynchronous();

            Console.WriteLine();
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        /// <summary>
        /// Tests the singleton aspects of License.
        /// </summary>
        private static void LicenseSingleton()
        {
            Trace.WriteLine("Singleton Tests");
            Trace.Indent();

            Trace.WriteLine("Get singleton value prior to initialization", "Singleton");
            Trace.Indent();
            Trace.WriteLine(String.Format("lmutil program: {0}", UtilityProgram.Instance.Executable), "Singleton");
            Trace.WriteLine(String.Format("lmutil version: {0}", UtilityProgram.Instance.Version), "Singleton");
            Trace.Unindent();

            Trace.WriteLine("Get singleton value after initialization with property", "Singleton");
            UtilityProgram.Instance.Executable = new FileInfo(testFilesPath + "lmutil.exe");
            Trace.Indent();
            Trace.WriteLine(String.Format("lmutil program: {0}", UtilityProgram.Instance.Executable), "Singleton");
            Trace.WriteLine(String.Format("lmutil version: {0}", UtilityProgram.Instance.Version), "Singleton");
            Trace.Unindent();

            // Set utility program to null to simulate a non-initialized singleton.
            UtilityProgram.Instance.Executable = null;

            Trace.WriteLine("Set static value to invalid file", "Singleton");
            Trace.Indent();
            UtilityProgram.Instance.Executable = new FileInfo(testFilesPath + "invalid file.exe");
            Trace.Unindent();

            // Set utility program to null to simulate a non-initialized singleton.
            UtilityProgram.Instance.Executable = null;

            Trace.WriteLine("");
            Trace.Unindent();
        }

        /// <summary>
        /// Tests the constructor methods of License.
        /// </summary>
        private static void LicenseConstructor()
        {
            Trace.WriteLine("Constructor Tests");
            Trace.Indent();

            Trace.WriteLine("License default constructor", "Constructor");
            License defaultLicense = new License();

            Trace.WriteLine("Feature default constructor", "Constructor");
            Feature defaultFeature = new Feature();

            Trace.WriteLine("User default constructor", "Constructor");
            User defaultUser = new User();

            Trace.WriteLine("");
            Trace.Unindent();
        }

        /// <summary>
        /// Tests the getting of the default properties used to configure License.
        /// </summary>
        private static void LicenseDefaultConfigurationProperties()
        {
            Trace.WriteLine("Default Configuration Properties Tests");
            Trace.Indent();

            Trace.WriteLine("Default License constructor get values", "Configuration");
            Trace.Indent();
            License defaultLicense = new License();
            Trace.WriteLine(String.Format("Name:   {0}", defaultLicense.Name), "Configuration");
            Trace.WriteLine(String.Format("Port:   {0}", defaultLicense.Port), "Configuration");
            Trace.WriteLine(String.Format("Host:   {0}", defaultLicense.Host), "Configuration");
            Trace.Unindent();

            Trace.WriteLine("");
            Trace.Unindent();
        }

        /// <summary>
        /// Tests the getting of the default properties of a Feature.
        /// </summary>
        private static void FeatureDefaultConfigurationProperties()
        {
            Trace.WriteLine("Default Configuration Properties Tests");
            Trace.Indent();

            // At this time not all properties are tested. This currently only checks a few types to verify the empty defaults.
            Trace.WriteLine("Default Feature constructor get values", "Configuration");
            Trace.Indent();
            Feature defaultFeature = new Feature();
            Trace.WriteLine(String.Format("Name:           {0}", defaultFeature.Name), "Configuration");
            Trace.WriteLine(String.Format("QuantityIssued: {0}", defaultFeature.QuantityIssued), "Configuration");
            Trace.WriteLine(String.Format("IsError:        {0}", defaultFeature.HasError), "Configuration");
            Trace.Unindent();

            Trace.WriteLine("");
            Trace.Unindent();
        }

        /// <summary>
        /// Tests the getting of the default properties of a User.
        /// </summary>
        private static void UserDefaultConfigurationProperties()
        {
            Trace.WriteLine("Default Configuration Properties Tests");
            Trace.Indent();

            // At this time not all properties are tested. This currently only checks a few types to verify the empty defaults.
            Trace.WriteLine("Default User constructor get values", "Configuration");
            Trace.Indent();
            User defaultUser = new User();
            Trace.WriteLine(String.Format("Name:       {0}", defaultUser.Name), "Configuration");
            Trace.WriteLine(String.Format("Port:       {0}", defaultUser.Port), "Configuration");
            Trace.WriteLine(String.Format("IsBorrowed: {0}", defaultUser.IsBorrowed), "Configuration");
            Trace.WriteLine(String.Format("Time:       {0}", defaultUser.Time), "Configuration");
            Trace.WriteLine(String.Format("Linger:     {0}", defaultUser.Linger), "Configuration");

            Trace.Unindent();

            Trace.WriteLine("");
            Trace.Unindent();
        }

        /// <summary>
        /// Tests serialization and deserialization.
        /// </summary>
        private static void LicenseSerialize()
        {
            Trace.WriteLine("Serialize Tests");
            Trace.Indent();

            License testLicense = new License();
            testLicense.Port = 11000;
            testLicense.Host = "TestLicense";
            testLicense.Name = "Test License Name";

            Trace.WriteLine("Write XML file", "Serialize");
            XmlSerializer xmlSerializeLicense = new XmlSerializer(typeof(License));
            Stream xmlWriteFile = new FileStream("SerializedTestLicense.xml", FileMode.Create, FileAccess.Write);
            xmlSerializeLicense.Serialize(xmlWriteFile, testLicense);
            xmlWriteFile.Close();

            Trace.WriteLine("Write binary file", "Serialize");
            BinaryFormatter binarySerializeLicense = new BinaryFormatter();
            Stream binaryWriteFile = new FileStream("SerializedTestLicense.bin", FileMode.Create, FileAccess.Write);
            binarySerializeLicense.Serialize(binaryWriteFile, testLicense);
            binaryWriteFile.Close();

            Trace.WriteLine("Read XML file", "Deserialize");
            Stream xmlReadFile = new FileStream("SerializedTestLicense.xml", FileMode.Open, FileAccess.Read);
            License xmlLicense = (License)xmlSerializeLicense.Deserialize(xmlReadFile);
            xmlReadFile.Close();

            Trace.WriteLineIf(xmlLicense.Port != 11000, String.Format("Port is incorrect. Value returned: {0}", xmlLicense.Port), "Deserialize");
            Trace.WriteLineIf(xmlLicense.Host != "TestLicense", String.Format("Host is incorrect. Value returned: {0}", xmlLicense.Host), "Deserialize");
            Trace.WriteLineIf(xmlLicense.Name != "Test License Name", String.Format("Name is incorrect. Value returned: {0}", xmlLicense.Name), "Deserialize");

            Trace.WriteLine("Read binary file", "Deserialize");
            Stream binaryReadFile = new FileStream("SerializedTestLicense.bin", FileMode.Open, FileAccess.Read);
            License binaryLicense = (License)binarySerializeLicense.Deserialize(binaryReadFile);
            binaryReadFile.Close();

            Trace.WriteLineIf(binaryLicense.Port != 11000, String.Format("Port is incorrect. Value returned: {0}", binaryLicense.Port), "Deserialize");
            Trace.WriteLineIf(binaryLicense.Host != "TestLicense", String.Format("Host is incorrect. Value returned: {0}", binaryLicense.Host), "Deserialize");
            Trace.WriteLineIf(binaryLicense.Name != "Test License Name", String.Format("Name is incorrect. Value returned: {0}", binaryLicense.Name), "Deserialize");

            File.Delete("SerializedTestLicense.xml");
            File.Delete("SerializedTestLicense.bin");

            Trace.WriteLine("");
            Trace.Unindent();
        }

        /// <summary>
        /// Tests that GetStatusCanExecute returns the correct value.
        /// </summary>
        private static void LicenseGetStatusCanExecuteProperty()
        {
            Trace.WriteLine("GetStatusCanExecute Property Tests (only errors are shown)");
            Trace.Indent();

            License testLicense = new License();
            Trace.WriteLineIf(testLicense.GetStatusCanExecute, "CanExecute is incorrectly true when all properties are invalid.");

            testLicense.Port = 1;
            Trace.WriteLineIf(testLicense.GetStatusCanExecute, "CanExecute is incorrectly true when only Port is valid.");

            testLicense.Port = 0;
            testLicense.Host = "localhost";
            Trace.WriteLineIf(testLicense.GetStatusCanExecute, "CanExecute is incorrectly true when only Host is valid.");

            testLicense.Host = null;
            UtilityProgram.Instance.Executable = new FileInfo(testFilesPath + "lmutil.exe");
            Trace.WriteLineIf(testLicense.GetStatusCanExecute, "CanExecute is incorrectly true when only Executable is valid.");

            testLicense.Port = 1;
            testLicense.Host = "localhost";
            Trace.WriteLineIf(!testLicense.GetStatusCanExecute, "CanExecute is incorrectly false when all properties are valid.");

            UtilityProgram.Instance.Executable = new FileInfo(testFilesPath + "invalid file.exe");
            Trace.WriteLineIf(testLicense.GetStatusCanExecute, "CanExecute is incorrectly true when only Executable is invalid.");

            Trace.WriteLine("");
            Trace.Unindent();
        }

        /// <summary>
        /// Tests the setting of the properties used to configure License.
        /// </summary>
        private static void LicenseConfigurationProperties()
        {
            Trace.WriteLine("Set Configuration Properties Tests");
            Trace.Indent();

            Trace.WriteLine("Set configuration values (only errors are shown)", "Configuration");
            Trace.Indent();

            // To simulate creating an uninstantiated License set the singleton value to its default
            // value of null before running the next test.
            UtilityProgram.Instance.Executable = null;
            UtilityProgram.Instance.Executable = new FileInfo(testFilesPath + "lmutil.exe");
            Trace.WriteLineIf(UtilityProgram.Instance.Executable.FullName != new FileInfo(testFilesPath + "lmutil.exe").FullName, String.Format("LMUtilProgram was not set correctly. Value returned: {0}", UtilityProgram.Instance.Executable), "Configuration");

            License defaultLicense = new License();
            defaultLicense.Name = "Set License Name";
            Trace.WriteLineIf(defaultLicense.Name != "Set License Name", String.Format("Name was not set correctly. Value returned: {0}", defaultLicense.Name), "Configuration");
            defaultLicense.Port = 54321;
            Trace.WriteLineIf(defaultLicense.Port != 54321, String.Format("Port was not set correctly. Value returned: {0}", defaultLicense.Port), "Configuration");
            defaultLicense.Host = "SetLicenseHost";
            Trace.WriteLineIf(defaultLicense.Host != "SetLicenseHost", String.Format("Host was not set correctly. Value returned: {0}", defaultLicense.Host), "Configuration");
            Trace.Unindent();

            Trace.WriteLine("");
            Trace.Unindent();
        }

        /// <summary>
        /// Tests the getting of License properties before GetStatus is used. The status is read from existing files.
        /// </summary>
        private static void LicenseProperties()
        {
            Trace.WriteLine("Get Properties Tests");
            Trace.Indent();

            UtilityProgram.Instance.Executable = new FileInfo(testFilesPath + "lmutil.exe");
            License testLicense = new License();

            // Features.Count is used as a rough check that the Features property is working.
            Trace.WriteLine("Get properties before GetStatus", "Properties");
            Trace.Indent();
            Trace.WriteLine(String.Format("Time:                {0}", testLicense.Time.ToString()), "Properties");
            Trace.WriteLine(String.Format("InUse:               {0}", testLicense.InUse), "Properties");
            Trace.WriteLine(String.Format("Feature Count:       {0}", testLicense.Features.Count), "Properties");
            Trace.WriteLine(String.Format("InUseCount:          {0}", testLicense.InUseCount), "Properties");
            Trace.WriteLine(String.Format("UserCount:           {0}", testLicense.UserCount), "Properties");
            Trace.WriteLine(String.Format("IsVendorDaemonUp:    {0}", testLicense.IsVendorDaemonUp), "Properties");
            Trace.WriteLine(String.Format("VendorDaemonName:    {0}", testLicense.VendorDaemonName), "Properties");
            Trace.WriteLine(String.Format("VendorDaemonStatus:  {0}", testLicense.VendorDaemonStatus), "Properties");
            Trace.WriteLine(String.Format("VendorDaemonVersion: {0}", testLicense.VendorDaemonVersion), "Properties");
            Trace.WriteLine(String.Format("Report:              {0}", testLicense.Report), "Properties");
            Trace.Unindent();

            // Report is not checked.
            Trace.WriteLine("Get properties after GetStatus (only errors are shown)", "Properties");
            Trace.Indent();
            testLicense.GetStatus(new FileInfo(testFilesPath + "lmstat-nx.log"));
            Trace.WriteLineIf(testLicense.Time != new DateTime(2008,6,16,10,43,0), String.Format("Time is incorrect. Value returned: {0}", testLicense.Time.ToString()), "Properties");
            Trace.WriteLineIf(testLicense.InUse != true, String.Format("InUse is incorrect. Value returned: {0}", testLicense.InUse), "Properties");
            Trace.WriteLineIf(testLicense.Features.Count != 135, String.Format("Features.Count is incorrect. Value returned: {0}", testLicense.Features.Count), "Properties");
            Trace.WriteLineIf(testLicense.InUseCount != 9, String.Format("InUseCount is incorrect. Value returned: {0}", testLicense.InUseCount), "Properties");
            Trace.WriteLineIf(testLicense.UserCount != 21, String.Format("UserCount is incorrect. Value returned: {0}", testLicense.UserCount), "Properties");
            Trace.WriteLineIf(testLicense.IsVendorDaemonUp != true, String.Format("IsVendorDaemonUp is incorrect. Value returned: {0}", testLicense.IsVendorDaemonUp), "Properties");
            Trace.WriteLineIf(testLicense.VendorDaemonName != "uglmd", String.Format("VendorDaemonName is incorrect. Value returned: {0}", testLicense.VendorDaemonName), "Properties");
            Trace.WriteLineIf(testLicense.VendorDaemonStatus != "UP", String.Format("VendorDaemonStatus is incorrect. Value returned: {0}", testLicense.VendorDaemonStatus), "Properties");
            Trace.WriteLineIf(testLicense.VendorDaemonVersion != "v10.8", String.Format("VendorDaemonVersion is incorrect. Value returned: {0}", testLicense.VendorDaemonVersion), "Properties");
            Trace.Unindent();

            Trace.WriteLine("Get properties after second GetStatus (only errors are shown)", "Properties");
            Trace.Indent();
            testLicense.GetStatus(new FileInfo(testFilesPath + "lmstat-acad.log"));
            Trace.WriteLineIf(testLicense.Time != new DateTime(2008, 6, 16, 10, 43, 0), String.Format("Time is incorrect. Value returned: {0}", testLicense.Time.ToString()), "Properties");
            Trace.WriteLineIf(testLicense.InUse != true, String.Format("InUse is incorrect. Value returned: {0}", testLicense.InUse), "Properties");
            Trace.WriteLineIf(testLicense.Features.Count != 22, String.Format("Features.Count is incorrect. Value returned: {0}", testLicense.Features.Count), "Properties");
            Trace.WriteLineIf(testLicense.InUseCount != 7, String.Format("InUseCount is incorrect. Value returned: {0}", testLicense.InUseCount), "Properties");
            Trace.WriteLineIf(testLicense.UserCount != 112, String.Format("UserCount is incorrect. Value returned: {0}", testLicense.UserCount), "Properties");
            Trace.WriteLineIf(testLicense.IsVendorDaemonUp != true, String.Format("IsVendorDaemonUp is incorrect. Value returned: {0}", testLicense.IsVendorDaemonUp), "Properties");
            Trace.WriteLineIf(testLicense.VendorDaemonName != "adskflex", String.Format("VendorDaemonName is incorrect. Value returned: {0}", testLicense.VendorDaemonName), "Properties");
            Trace.WriteLineIf(testLicense.VendorDaemonStatus != "UP", String.Format("VendorDaemonStatus is incorrect. Value returned: {0}", testLicense.VendorDaemonStatus), "Properties");
            Trace.WriteLineIf(testLicense.VendorDaemonVersion != "v10.8", String.Format("VendorDaemonVersion is incorrect. Value returned: {0}", testLicense.VendorDaemonVersion), "Properties");
            Trace.Unindent();

            Trace.WriteLine("Get properties after GetStatus with log errors (only errors are shown)", "Properties");
            Trace.Indent();
            testLicense.GetStatus(new FileInfo(testFilesPath + "lmstat-errors.log"));
            Trace.WriteLineIf(testLicense.Time != new DateTime(2008, 11, 20, 15, 42, 0), String.Format("Time is incorrect. Value returned: {0}", testLicense.Time.ToString()), "Properties");
            Trace.WriteLineIf(testLicense.InUse != false, String.Format("InUse is incorrect. Value returned: {0}", testLicense.InUse), "Properties");
            Trace.WriteLineIf(testLicense.Features.Count != 11, String.Format("Features.Count is incorrect. Value returned: {0}", testLicense.Features.Count), "Properties");
            Trace.WriteLineIf(testLicense.InUseCount != 0, String.Format("InUseCount is incorrect. Value returned: {0}", testLicense.InUseCount), "Properties");
            Trace.WriteLineIf(testLicense.UserCount != 0, String.Format("UserCount is incorrect. Value returned: {0}", testLicense.UserCount), "Properties");
            Trace.WriteLineIf(testLicense.IsVendorDaemonUp != false, String.Format("IsVendorDaemonUp is incorrect. Value returned: {0}", testLicense.IsVendorDaemonUp), "Properties");
            Trace.WriteLineIf(testLicense.VendorDaemonName != "theorem", String.Format("VendorDaemonName is incorrect. Value returned: {0}", testLicense.VendorDaemonName), "Properties");
            Trace.WriteLineIf(testLicense.VendorDaemonStatus != "The desired vendor daemon is down. (-97,121)", String.Format("VendorDaemonStatus is incorrect. Value returned: {0}", testLicense.VendorDaemonStatus), "Properties");
            Trace.WriteLineIf(testLicense.VendorDaemonVersion != "", String.Format("VendorDaemonVersion is incorrect. Value returned: {0}", testLicense.VendorDaemonVersion), "Properties");
            Trace.Unindent();

            Trace.WriteLine("");
            Trace.Unindent();
        }

        /// <summary>
        /// Tests the asynchronous method.
        /// </summary>
        private static void LicenseAsynchronous()
        {
            Trace.WriteLine("Asynchronous Status Tests");
            Trace.Indent();

            License testLicense = new License();
            testLicense.Port = 27000;
            testLicense.Host = "localhost";
            testLicense.GetStatusCompleted += new System.ComponentModel.AsyncCompletedEventHandler(TestAsync_GetStatusCompleted);
            Trace.WriteLine("Before executing GetStatusAsync", "Asynchronous");
            testLicense.GetStatusAsync();
            Trace.WriteLine("After executing GetStatusAsync", "Asynchronous");

            // Pause with the intent to have the completion occur before exiting this method.
            System.Threading.Thread.Sleep(5000);

            Trace.WriteLine("");
            Trace.Unindent();
        }

        /// <summary>
        /// Event handler that occurs when GetStatusAsync completes.
        /// </summary>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="e">The event data.</param>
        private static void TestAsync_GetStatusCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            Trace.WriteLine("GetStatusAsync Completed", "Asynchronous");
        }
    }
}
