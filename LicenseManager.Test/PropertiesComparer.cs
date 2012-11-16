// <copyright file="PropertiesComparer.cs" company="Charles W. Bozarth">
// Copyright (C) 2009-2012 Charles W. Bozarth
// Refer to LicenseManager's License.cs for the full copyright notice.
// </copyright>

namespace LicenseManager.Test
{
    using System;
    using CWBozarth.LicenseManager;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Provides methods for comparing the properties of the License, Feature and
    /// User objects.
    /// </summary>
    /// <remarks>
    /// Some consideration was given to finding and comparing properties using reflection
    /// and a mock object with expected values. For now, manual control over the
    /// properties to compare was chosen. The number of method parameters is a bit of
    /// an annoyance.
    /// </remarks>
    public static class PropertiesComparer
    {
        /// <summary>
        /// Verifies that the properties of the License match the expected values.
        /// </summary>
        /// <remarks>
        /// Does not verify Port, Host, Name, Report, GetStatusCanExecute, Error, this[columnName].
        /// </remarks>
        /// <param name="actual">The actual License object the unit test produced.</param>
        /// <param name="time">Expected value of <see cref="License.Time"/>.</param>
        /// <param name="serverFile">Expected value of <see cref="License.ServerFile"/>.</param>
        /// <param name="vendorDaemonName">Expected value of <see cref="License.VendorDaemonName"/>.</param>
        /// <param name="vendorDaemonStatus">Expected value of <see cref="License.VendorDaemonStatus"/>.</param>
        /// <param name="vendorDaemonVersion">Expected value of <see cref="License.VendorDaemonVersion"/>.</param>
        /// <param name="isVendorDaemonUp">Expected value of <see cref="License.IsVendorDaemonUp"/>.</param>
        /// <param name="featuresCount">Expected value of <see cref="License.Features.Count"/>.</param>
        /// <param name="isFeatureError">Expected value of <see cref="License.IsFeatureError"/>.</param>
        /// <param name="userCount">Expected value of <see cref="License.UserCount"/>.</param>
        /// <param name="inUse">Expected value of <see cref="License.InUse"/>.</param>
        /// <param name="inUseCount">Expected value of <see cref="License.InUseCount"/>.</param>
        /// <param name="hasError">Expected value of <see cref="License.HasError"/>.</param>
        /// <param name="errorMessage">Expected value of <see cref="License.ErrorMessage"/>.</param>
        /// <param name="isBusy">Expected value of <see cref="License.IsBusy"/>.</param>
        public static void AssertLicensePropertiesAreEqual(
            License actual,
            DateTime time,
            string serverFile,
            string vendorDaemonName,
            string vendorDaemonStatus,
            string vendorDaemonVersion,
            bool isVendorDaemonUp,
            int featuresCount,
            bool isFeatureError,
            int userCount,
            bool inUse,
            int inUseCount,
            bool hasError,
            string errorMessage,
            bool isBusy)
        {
            Assert.AreEqual(time, actual.Time, "Time property.");
            Assert.AreEqual(serverFile, actual.ServerFile, "ServerFile property.");

            Assert.AreEqual(vendorDaemonName, actual.VendorDaemonName, "VendorDaemonName property.");
            Assert.AreEqual(vendorDaemonStatus, actual.VendorDaemonStatus, "VendorDaemonStatus property.");
            Assert.AreEqual(vendorDaemonVersion, actual.VendorDaemonVersion, "VendorDaemonVersion property.");
            Assert.AreEqual(isVendorDaemonUp, actual.IsVendorDaemonUp, "IsVendorDaemonUp property.");

            // Features.Count is used as a rough check that the Features property is working.
            Assert.AreEqual(featuresCount, actual.Features.Count, "Features.Count property.");
            Assert.AreEqual(isFeatureError, actual.IsFeatureError, "IsFeatureError property.");
            Assert.AreEqual(userCount, actual.UserCount, "UserCount property.");
            Assert.AreEqual(inUse, actual.InUse, "InUse property.");
            Assert.AreEqual(inUseCount, actual.InUseCount, "InUseCount property.");
            
            Assert.AreEqual(hasError, actual.HasError, "HasError property.");
            Assert.AreEqual(errorMessage, actual.ErrorMessage, "ErrorMessage property.");
            Assert.AreEqual(isBusy, actual.IsBusy, "IsBusy property.");
        }

        /// <summary>
        /// Verifies that the properties of the Feature match the expected values.
        /// </summary>
        /// <remarks>
        /// Does not verify Report.
        /// </remarks>
        /// <param name="actual">The actual Feature object the unit test produced.</param>
        /// <param name="name">Expected value of <see cref="Feature.Name"/>.</param>
        /// <param name="quantityIssued">Expected value of <see cref="Feature.QuantityIssued"/>.</param>
        /// <param name="quantityUsed">Expected value of <see cref="Feature.QuantityUsed"/>.</param>
        /// <param name="quantityAvailable">Expected value of <see cref="Feature.QuantityAvailable"/>.</param>
        /// <param name="quantityBorrowed">Expected value of <see cref="Feature.QuantityBorrowed"/>.</param>
        /// <param name="usersCount">Expected value of <see cref="Feature.Users.Count"/>.</param>
        /// <param name="vendor">Expected value of <see cref="Feature.Vendor"/>.</param>
        /// <param name="version">Expected value of <see cref="Feature.Version"/>.</param>
        /// <param name="type">Expected value of <see cref="Feature.Type"/>.</param>
        /// <param name="inUse">Expected value of <see cref="Feature.InUse"/>.</param>
        /// <param name="hasError">Expected value of <see cref="Feature.HasError"/>.</param>
        /// <param name="errorMessage">Expected value of <see cref="Feature.ErrorMessasge"/>.</param>
        /// <param name="entryIndex">Expected value of <see cref="Feature.EntryIndex"/>.</param>
        /// <param name="entryLength">Expected value of <see cref="Feature.EntryLength"/>.</param>
        public static void AssertFeaturePropertiesAreEqual(
            Feature actual,
            string name,
            int quantityIssued,
            int quantityUsed,
            int quantityAvailable,
            int quantityBorrowed,
            int usersCount,
            string vendor,
            string version,
            string type,
            bool inUse,
            bool hasError,
            string errorMessage,
            int entryIndex,
            int entryLength)
        {
            Assert.AreEqual(name, actual.Name, "Name property.");

            Assert.AreEqual(quantityIssued, actual.QuantityIssued, "QuantityIssued property.");
            Assert.AreEqual(quantityUsed, actual.QuantityUsed, "QuantityUsed property.");
            Assert.AreEqual(quantityAvailable, actual.QuantityAvailable, "QuantityAvailable property.");
            Assert.AreEqual(quantityBorrowed, actual.QuantityBorrowed, "QuantityBorrowed property.");

            // Users.Count is used as a rough check that the Features property is working.
            Assert.AreEqual(usersCount, actual.Users.Count, "Users.Count property.");

            Assert.AreEqual(vendor, actual.Vendor, "Vendor property.");
            Assert.AreEqual(version, actual.Version, "Version property.");
            Assert.AreEqual(type, actual.Type, "Type property.");

            Assert.AreEqual(inUse, actual.InUse, "InUse property.");
            Assert.AreEqual(hasError, actual.HasError, "HasError property.");
            Assert.AreEqual(errorMessage, actual.ErrorMessage, "ErrorMessage property.");

            Assert.AreEqual(entryIndex, actual.EntryIndex, "EntryIndex property.");
            Assert.AreEqual(entryLength, actual.EntryLength, "EntryLength property.");
        }

        /// <summary>
        /// Verifies that the properties of the User match the expected values.
        /// </summary>
        /// <remarks>
        /// Does not verify Report.
        /// </remarks>
        /// <param name="actual">The actual User object the unit test produced.</param>
        /// <param name="name">Expected value of <see cref="User.Name"/>.</param>
        /// <param name="host">Expected value of <see cref="User.Host"/>.</param>
        /// <param name="display">Expected value of <see cref="User.Display"/>.</param>
        /// <param name="version">Expected value of <see cref="User.Version"/>.</param>
        /// <param name="server">Expected value of <see cref="User.Server"/>.</param>
        /// <param name="port">Expected value of <see cref="User.Port"/>.</param>
        /// <param name="handle">Expected value of <see cref="User.Handle"/>.</param>
        /// <param name="time">Expected value of <see cref="User.Time"/>.</param>
        /// <param name="quantityused">Expected value of <see cref="User.QuantityUsed"/>.</param>
        /// <param name="linger">Expected value of <see cref="User.Linger"/>.</param>
        /// <param name="isBorrowed">Expected value of <see cref="User.IsBorrowed"/>.</param>
        /// <param name="borrowEndTime">Expected value of <see cref="User.BorrowEndTime"/>.</param>
        /// <param name="entryIndex">Expected value of <see cref="User.EntryIndex"/>.</param>
        /// <param name="entryLength">Expected value of <see cref="User.EntryLength"/>.</param>
        public static void AssertUserPropertiesAreEqual(
            User actual,
            string name,
            string host,
            string display,
            string version,
            string server,
            int port,
            int handle,
            DateTime time,
            int quantityused,
            TimeSpan linger,
            bool isBorrowed,
            DateTime borrowEndTime,
            int entryIndex,
            int entryLength)
        {
            Assert.AreEqual(name, actual.Name, "Name property.");
            Assert.AreEqual(host, actual.Host, "Host property.");
            Assert.AreEqual(display, actual.Display, "Display property.");
            Assert.AreEqual(version, actual.Version, "Version property.");

            Assert.AreEqual(server, actual.Server, "Server property.");
            Assert.AreEqual(port, actual.Port, "Port property.");

            Assert.AreEqual(handle, actual.Handle, "Handle property.");
            Assert.AreEqual(time, actual.Time, "Time property.");
            Assert.AreEqual(quantityused, actual.QuantityUsed, "QuantityUsed property.");

            Assert.AreEqual(linger, actual.Linger, "Linger property.");
            Assert.AreEqual(isBorrowed, actual.IsBorrowed, "IsBorrowed property.");
            Assert.AreEqual(borrowEndTime, actual.BorrowEndTime, "BorrowEndTime property.");

            Assert.AreEqual(entryIndex, actual.EntryIndex, "EntryIndex property.");
            Assert.AreEqual(entryLength, actual.EntryLength, "EntryLength property.");
        }
    }
}
