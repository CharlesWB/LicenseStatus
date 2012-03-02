// <copyright file="License.cs" company="Charles W. Bozarth">
// Copyright (C) 2012 Charles W. Bozarth
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see http://www.gnu.org/licenses/.
// </copyright>
// <author>Charles W. Bozarth</author>
// <date>2009-08-17</date>
// <site>http://licensestatus.sourceforge.net</site>
// <summary>Provides access to FlexLM license information.</summary>

// Revisions:
//
// 2009-09-10 CWB
// The regular expressions for finding the feature name and the vendor daemon name have been
// changed from matching a word character (\w) to matching non-white space character (\S).
// A license was found where the feature name included a period which is not a word character.
// It was decided that a vendor daemon name could have the same issue and was also updated.

#region Notes
// Event-based Asynchronous Pattern:
//
// This class implements asynchronous operations using events.
// http://msdn.microsoft.com/en-us/library/wewwczdw.aspx
//
// AsyncOperationManager:
//
// The GetStatusCompleted event handler uses the AsyncOperationManager so that
// it will be raised on the thread of the calling application. This was done
// instead of using a dispatcher within the calling application.
//
// The PropertyChanged event handler does not do this. A WPF binding handles
// the thread on its own.
//
// This may be considered inconsistent and may not be the best design.
//
// Unique Users:
//
// A property to return a list of unique users of this license was considered.
// The problem is that only the user name is meaningful. The full user
// definition refers to the feature the user is associated to. For example,
// the user's Handle is different for each instance of the user. This list
// would only return the first instance of each user. The use for this information
// was to find a count of unique users. So rather than return a special case
// list the user count property was implemented on its own.
//
// For User and Feature implementing IEquatable was tested to support
// List.Contains for the unique user count. This based testing for equals
// on the name. This made IndexOf invalid since it only found the first user
// with the same name rather than the full reference object.
//
// Namespace Name:
//
// There is a name conflict with the LicenseManager class in the System.ComponentModel namespace.
//
// As it's currently designed LicenseManager is not really accurate. LicenseReader would probably
// be a better assembly and namespace name. But the idea for using LicenseManager was that
// functionality other than reading would be added at some point.
//
// Possible Enhancements:
//
// Instead of clearing the Feature and User collection when the status is updated, reuse them
// if the feature or user hasn't changed. This might improve UI performance.
//
// The sample error status log returns "The desired vendor daemon is down. (-97,121)". Should
// this be processed in some way?
//
// Would it be useful to report the process ExitCode?
//
// Currently only License uses the PropertyChanged event. Should it be applied to Feature and User?
// This would only be useful if Feature and User objects were reused when the license file is reparsed.
//
// What would it take to make UtilityProgram serializable? Would that be useful?
//
// No special attempt has been made to ensure GetStatusAsyncCancel does not cause an exception. This violates the best practices.
//
// No special attempt has been made to catch exceptions during GetStatusAsync. This violates the best practices.
//
// Record line numbers of Features and Users to allow for highlighting of lines in the report.
#endregion

namespace CWBozarth.LicenseManager
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml.Serialization;

    /// <summary>
    /// Represents a FlexLM based license.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The License class provides methods to retrieve license information from the lmstat log file.
    /// Given a license the License class parses the lmstat log file and creates related Feature and User objects.
    /// </para>
    /// <para>
    /// The status can be retrieved asynchronously using GetStatusAsync. As noted by the Port and Host
    /// properties the asynchronous process will automatically be cancelled if they are changed while the
    /// process is running. What also needs to be noted is that changing the UtilityProgram's Executable
    /// property will also cancel a running process.
    /// </para>
    /// </remarks>
    [Serializable]
    public class License : INotifyPropertyChanged, IDataErrorInfo
    {
        /// <summary>
        /// Stores the name.
        /// </summary>
        private string name;

        /// <summary>
        /// Stores the port number.
        /// </summary>
        private int? port;

        /// <summary>
        /// Stores the host name.
        /// </summary>
        private string host;

        /// <summary>
        /// Stores the time the status was retrieved.
        /// </summary>
        [NonSerialized]
        private DateTime time;

        /// <summary>
        /// Stores the path of the license file on the server.
        /// </summary>
        [NonSerialized]
        private string serverFile;

        /// <summary>
        /// Stores the vendor daemon name.
        /// </summary>
        [NonSerialized]
        private string vendorDaemonName;

        /// <summary>
        /// Stores the vendor daemon status.
        /// </summary>
        [NonSerialized]
        private string vendorDaemonStatus;

        /// <summary>
        /// Stores the vendor daemon version.
        /// </summary>
        [NonSerialized]
        private string vendorDaemonVersion;

        /// <summary>
        /// Stores the vendor daemon state.
        /// </summary>
        [NonSerialized]
        private bool isVendorDaemonUp;

        /// <summary>
        /// Stores the list of features.
        /// </summary>
        [NonSerialized]
        private List<Feature> features;

        /// <summary>
        /// Stores error state of the license.
        /// </summary>
        [NonSerialized]
        private bool hasError;

        /// <summary>
        /// Stores the message.
        /// </summary>
        private string errorMessage;

        /// <summary>
        /// Stores the full status report.
        /// </summary>
        [NonSerialized]
        private string report;

        /// <summary>
        /// Stores the busy state of the asynchronous operation.
        /// </summary>
        [NonSerialized]
        private bool isBusy;

        /// <summary>
        /// Stores the output from the lmutil process.
        /// </summary>
        [NonSerialized]
        private StringBuilder statusOutput;

        /// <summary>
        /// Stores the lmutil process.
        /// </summary>
        [NonSerialized]
        private System.Diagnostics.Process process;

        /// <summary>
        /// Stores the delay timer used for testing.
        /// </summary>
        [NonSerialized]
        private System.Timers.Timer delayTimer;

        /// <summary>
        /// Stores the asynchronous operation object.
        /// </summary>
        [NonSerialized]
        private AsyncOperation asyncOperation;

        /// <summary>
        /// Initializes a new instance of the License class.
        /// </summary>
        public License()
        {
            this.features = new List<Feature>();

            // By connecting to the UtilityProgram's PropertyChanged event we can cancel an asynchronous process
            // if the executable changes.
            UtilityProgram.Instance.PropertyChanged += new PropertyChangedEventHandler(this.UtilityProgramPropertyChanged);
        }

        /// <summary>
        /// Occurs when the asynchronous get status operation is completed, has been cancelled, or has raised an exception.
        /// </summary>
        /// <remarks>This event will be raised on the thread of the calling application.</remarks>
        [field: NonSerialized]
        public event AsyncCompletedEventHandler GetStatusCompleted;

        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the port number.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is the port number used to communicate with the license on the server. For the address
        /// '27000@SERVER001' the port number is '27000'.
        /// </para>
        /// <para>
        /// If the port number is changed while GetStatusAsync is running then it will be cancelled.
        /// </para>
        /// </remarks>
        public int? Port
        {
            get
            {
                return this.port;
            }

            set
            {
                if (value != this.port)
                {
                    if (this.IsBusy)
                    {
                        this.GetStatusAsyncCancel();
                    }

                    this.port = value;
                    this.NotifyPropertyChanged("Port");

                    if (this.name == null)
                    {
                        this.NotifyPropertyChanged("Name");
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the host name.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The host name is the name of the server on which the license exists. For the address
        /// '27000@SERVER001' the host name is 'SERVER001'.
        /// </para>
        /// <para>
        /// If the host name is changed while GetStatusAsync is running then it will be cancelled.
        /// </para>
        /// </remarks>
        public string Host
        {
            get
            {
                return this.host;
            }

            set
            {
                if (value != this.host)
                {
                    if (this.IsBusy)
                    {
                        this.GetStatusAsyncCancel();
                    }

                    this.host = value;
                    this.NotifyPropertyChanged("Host");

                    if (this.name == null)
                    {
                        this.NotifyPropertyChanged("Name");
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the descriptive name of the license.
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                if (value != this.name)
                {
                    this.name = value;
                    this.NotifyPropertyChanged("Name");
                }
            }
        }

        /// <summary>
        /// Gets the time the status was retrieved.
        /// </summary>
        [XmlIgnore]
        public DateTime Time
        {
            get
            {
                return this.time;
            }

            private set
            {
                if (value != this.time)
                {
                    this.time = value;
                    this.NotifyPropertyChanged("Time");
                }
            }
        }

        /// <summary>
        /// Gets the path of the license file on the server.
        /// </summary>
        [XmlIgnore]
        public string ServerFile
        {
            get
            {
                return this.serverFile;
            }

            private set
            {
                if (value != this.serverFile)
                {
                    this.serverFile = value;
                    this.NotifyPropertyChanged("ServerFile");
                }
            }
        }

        /// <summary>
        /// Gets the name of the vendor daemon.
        /// </summary>
        [XmlIgnore]
        public string VendorDaemonName
        {
            get
            {
                return this.vendorDaemonName;
            }

            private set
            {
                if (value != this.vendorDaemonName)
                {
                    this.vendorDaemonName = value;
                    this.NotifyPropertyChanged("VendorDaemonName");
                }
            }
        }

        /// <summary>
        /// Gets the status of the vendor daemon.
        /// </summary>
        [XmlIgnore]
        public string VendorDaemonStatus
        {
            get
            {
                return this.vendorDaemonStatus;
            }

            private set
            {
                if (value != this.vendorDaemonStatus)
                {
                    this.vendorDaemonStatus = value;
                    this.NotifyPropertyChanged("VendorDaemonStatus");
                }
            }
        }

        /// <summary>
        /// Gets the version of the vendor daemon.
        /// </summary>
        [XmlIgnore]
        public string VendorDaemonVersion
        {
            get
            {
                return this.vendorDaemonVersion;
            }

            private set
            {
                if (value != this.vendorDaemonVersion)
                {
                    this.vendorDaemonVersion = value;
                    this.NotifyPropertyChanged("VendorDaemonVersion");
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the vendor daemon is up.
        /// </summary>
        [XmlIgnore]
        public bool IsVendorDaemonUp
        {
            get
            {
                return this.isVendorDaemonUp;
            }

            private set
            {
                if (value != this.isVendorDaemonUp)
                {
                    this.isVendorDaemonUp = value;
                    this.NotifyPropertyChanged("IsVendorDaemonUp");
                }
            }
        }

        /// <summary>
        /// Gets a list of the features found within the license.
        /// </summary>
        [XmlIgnore]
        public ReadOnlyCollection<Feature> Features
        {
            get { return new ReadOnlyCollection<Feature>(this.features); }
        }

        /// <summary>
        /// Gets a value indicating whether one or more features are reporting an error.
        /// </summary>
        public bool IsFeatureError
        {
            get
            {
                bool result = false;

                foreach (Feature feature in this.features)
                {
                    if (feature.HasError)
                    {
                        result = true;
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Gets a count of users of all features within the license file.
        /// </summary>
        /// <remarks>A user in mulitple features or hosts is only counted once.</remarks>
        public int UserCount
        {
            get
            {
                List<User> uniqueUsers = new List<User>();
                foreach (Feature feature in this.features)
                {
                    foreach (User featureUser in feature.Users)
                    {
                        bool userFound = false;

                        foreach (User user in uniqueUsers)
                        {
                            if (featureUser.Name == user.Name)
                            {
                                userFound = true;
                            }
                        }

                        if (!userFound)
                        {
                            uniqueUsers.Add(featureUser);
                        }
                    }
                }

                return uniqueUsers.Count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether any feature of the license is being used.
        /// </summary>
        public bool InUse
        {
            get
            {
                bool result = false;

                foreach (Feature feature in this.features)
                {
                    if (feature.InUse)
                    {
                        result = true;
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Gets a count of features that are in use.
        /// </summary>
        public int InUseCount
        {
            get
            {
                int inUseCount = 0;

                foreach (Feature feature in this.features)
                {
                    if (feature.InUse)
                    {
                        inUseCount++;
                    }
                }

                return inUseCount;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the license reported an error.
        /// </summary>
        [XmlIgnore]
        public bool HasError
        {
            get
            {
                return this.hasError;
            }

            private set
            {
                if (value != this.hasError)
                {
                    this.hasError = value;
                    this.NotifyPropertyChanged("HasError");
                }
            }
        }

        /// <summary>
        /// Gets the error message.
        /// </summary>
        [XmlIgnore]
        public string ErrorMessage
        {
            get
            {
                return this.errorMessage;
            }

            private set
            {
                if (value != this.errorMessage)
                {
                    this.errorMessage = value;
                    this.NotifyPropertyChanged("ErrorMessage");
                }
            }
        }

        /// <summary>
        /// Gets the full report as returned by lmstat.
        /// </summary>
        public string Report
        {
            get { return this.report; }
        }

        /// <summary>
        /// Gets a value indicating whether the asynchronous operation is working.
        /// </summary>
        [XmlIgnore]
        public bool IsBusy
        {
            get
            {
                return this.isBusy;
            }

            private set
            {
                if (value != this.isBusy)
                {
                    this.isBusy = value;
                    this.NotifyPropertyChanged("IsBusy");
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether all the properties are defined that are needed
        /// by GetStatus or GetStatusAsync.
        /// </summary>
        /// <remarks>
        /// <para>The Port property must be valid.</para>
        /// <para>The Host property must be valid.</para>
        /// <para>The UtilityProgram's Executable property must be valid.</para>
        /// <para>An existing asynchronous process must not be running, e.g. IsBusy must be false.</para>
        /// </remarks>
        public bool GetStatusCanExecute
        {
            get
            {
                bool result = false;

                if (String.IsNullOrEmpty(this["Port"]) &&
                    String.IsNullOrEmpty(this["Host"]) &&
                    String.IsNullOrEmpty(UtilityProgram.Instance["Executable"]) &&
                    !this.IsBusy)
                {
                    result = true;
                }

                return result;
            }
        }

        /// <summary>
        /// Gets an error message indicating what is wrong with this object.
        /// </summary>
        /// <remarks>
        /// Not implemented. From the IDataErrorInfo interface.
        /// </remarks>
        public string Error
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the error message for the property with the given name.
        /// </summary>
        /// <remarks>From the IDataErrorInfo interface.</remarks>
        /// <param name="columnName">The name of the property whose error message to get.</param>
        /// <returns>The error message for the property.</returns>
        public string this[string columnName]
        {
            get
            {
                string result = null;

                if (columnName == "Port")
                {
                    if (this.port == null)
                    {
                        result = "Port is a required entry.";
                    }
                    else if (this.port < 1 || this.port > 65535)
                    {
                        result = "Port must be between 1 and 65535.";
                    }
                }

                if (columnName == "Host")
                {
                    if (String.IsNullOrEmpty(this.host) || this.host.Trim().Length == 0)
                    {
                        result = "Host is a required entry.";
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the current status of the license.
        /// </summary>
        /// <remarks>
        /// If GetStatusCanExecute is false then this will return without updating the status.
        /// </remarks>
        public void GetStatus()
        {
            if (this.IsBusy)
            {
                throw new InvalidOperationException("A second GetStatus operation cannot be started while an asynchronous operation is running.");
            }

            if (this.GetStatusCanExecute)
            {
                this.IsBusy = true;

                this.ConfigureProcess();
                this.process.EnableRaisingEvents = false;
                this.process.Start();
                this.report = this.process.StandardOutput.ReadToEnd();
                this.process.WaitForExit();
                this.process.Close();
                this.process.Dispose();
                this.process = null;

                this.ParseReport();

                this.IsBusy = false;
            }
        }

        /// <summary>
        /// Gets the status of the license by reading from an existing lmstat output file.
        /// </summary>
        /// <remarks>
        /// This was developed primarily for testing when a license service is not available.
        /// Port, Host and UtilityProgram are not used to retrieve the status.
        /// </remarks>
        /// <param name="statusFile">A file containing the output of the lmstat command.</param>
        public void GetStatus(FileInfo statusFile)
        {
            if (this.IsBusy)
            {
                throw new InvalidOperationException("A second GetStatus operation cannot be started while an asynchronous operation is running.");
            }

            this.IsBusy = true;

            using (StreamReader fileReader = statusFile.OpenText())
            {
                this.report = fileReader.ReadToEnd();
            }

            this.ParseReport();

            this.IsBusy = false;
        }

        /// <summary>
        /// Gets the current status of the license asynchronously.
        /// </summary>
        /// <remarks>
        /// The Port, Host and UtilityProgram must be defined before the status can be retrieved.
        /// </remarks>
        public void GetStatusAsync()
        {
            if (this.IsBusy)
            {
                throw new InvalidOperationException("A second GetStatus operation cannot be started while an asynchronous operation is running.");
            }

            if (this.GetStatusCanExecute)
            {
                this.IsBusy = true;

                this.asyncOperation = AsyncOperationManager.CreateOperation(null);

                this.ConfigureProcess();
                this.statusOutput = new StringBuilder();
                this.process.Exited += new EventHandler(this.StatusProcessExited);
                this.process.OutputDataReceived += new System.Diagnostics.DataReceivedEventHandler(this.StatusOutputDataReceived);
                this.process.EnableRaisingEvents = true;
                this.process.Start();
                this.process.BeginOutputReadLine();
            }
        }

        /// <summary>
        /// Gets the status of the license by reading from an existing lmstat output file.
        /// </summary>
        /// <remarks>
        /// This was developed primarily for testing when a license service is not available.
        /// Port, Host and UtilityProgram are not used to retrieve the status.
        /// </remarks>
        /// <param name="statusFile">A file containing the output of the lmstat command.</param>
        /// <param name="delay">The amount of time to wait before the GetStatusCompleted event occurs.</param>
        public void GetStatusAsync(FileInfo statusFile, double delay)
        {
            if (this.IsBusy)
            {
                throw new InvalidOperationException("A second GetStatus operation cannot be started while an asynchronous operation is running.");
            }

            this.IsBusy = true;

            this.asyncOperation = AsyncOperationManager.CreateOperation(null);

            using (StreamReader fileReader = statusFile.OpenText())
            {
                this.statusOutput = new StringBuilder(fileReader.ReadToEnd());
            }

            this.delayTimer = new System.Timers.Timer(delay);
            this.delayTimer.Elapsed += new System.Timers.ElapsedEventHandler(this.StatusDelayTimerElapsed);
            this.delayTimer.Enabled = true;
        }

        /// <summary>
        /// Cancels the asynchronous process for getting the status.
        /// </summary>
        public void GetStatusAsyncCancel()
        {
            if (this.process != null && this.IsBusy)
            {
                // EnableRaisingEvents is set to false so that StatusExited is not called when the
                // process is killed.
                this.process.EnableRaisingEvents = false;
                this.process.Kill();
                this.process.CancelOutputRead();
                this.process.Dispose();
                this.process = null;
            }

            if (this.delayTimer != null && this.delayTimer.Enabled)
            {
                this.delayTimer.Stop();
                this.delayTimer.Dispose();
                this.delayTimer = null;
            }

            this.OnGetStatusCompleted(new AsyncCompletedEventArgs(null, true, null));
        }

        /// <summary>
        /// Gets the name of the license.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If a descriptive name has been set then Name is returned. Otherwise
        /// port@host, e.g. 27000@SERVER001, is returned.
        /// </para>
        /// <para>
        /// If either port or host are not defined then the text "license not specified" is returned.
        /// </para>
        /// </remarks>
        /// <returns>The name of the license.</returns>
        public override string ToString()
        {
            if (String.IsNullOrEmpty(this.name))
            {
                if (this.port == null || String.IsNullOrEmpty(this.host) || this.host.Trim().Length == 0)
                {
                    return "license not specified";
                }

                return String.Format(CultureInfo.InvariantCulture, "{0}@{1}", this.port, this.host);
            }
            else
            {
                return this.name;
            }
        }

        /// <summary>
        /// Raises the GetStatusCompleted event when the asynchronous operation ends.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected virtual void OnGetStatusCompleted(AsyncCompletedEventArgs e)
        {
            this.IsBusy = false;

            AsyncCompletedEventHandler handler = this.GetStatusCompleted;
            if (handler != null)
            {
                this.asyncOperation.PostOperationCompleted(delegate { handler(this, e); }, null);
            }
        }

        /// <summary>
        /// Occurs when the asynchronous lmstat process outputs data.
        /// </summary>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="e">The event data.</param>
        private void StatusOutputDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            this.statusOutput.Append(e.Data + Environment.NewLine);
        }

        /// <summary>
        /// Occurs when the delay timer elapses.
        /// </summary>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="e">The event data.</param>
        private void StatusDelayTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.delayTimer.Stop();
            this.delayTimer.Dispose();
            this.delayTimer = null;

            this.report = this.statusOutput.ToString();
            this.ParseReport();

            this.OnGetStatusCompleted(new AsyncCompletedEventArgs(null, false, null));
        }

        /// <summary>
        /// Occurs when the asynchronous lmstat process has exited.
        /// </summary>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="e">The event data.</param>
        private void StatusProcessExited(object sender, EventArgs e)
        {
            // Although the process has exited, the output has not finished. WaitForExit appears to
            // work around this.
            // http://social.msdn.microsoft.com/Forums/en-US/netfxbcl/thread/2c8946c4-90c9-42a0-8ea8-b24b738b2ec1
            this.process.WaitForExit();
            this.process.CancelOutputRead();
            this.process.Close();
            this.process.Dispose();
            this.process = null;

            this.report = this.statusOutput.ToString();
            this.ParseReport();

            this.OnGetStatusCompleted(new AsyncCompletedEventArgs(null, false, null));
        }

        /// <summary>
        /// Occurs when a property changes on the UtilityProgram object.
        /// </summary>
        /// <remarks>
        /// A GetStatusAsync is automatically cancelled when the Port or Host properties are
        /// changed. To be consistent changing the UtilityProgram Executable should do the same.
        /// This is done by attaching to the PropertyChanged event.
        /// </remarks>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="e">The event data.</param>
        private void UtilityProgramPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Executable" && this.IsBusy)
            {
                this.GetStatusAsyncCancel();
            }
        }

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Configure the process for the lmutil command with the common parameters for synchronous
        /// and asynchronous calls.
        /// </summary>
        private void ConfigureProcess()
        {
            if (this.process == null)
            {
                this.process = new System.Diagnostics.Process();
            }

            this.process.StartInfo.FileName = UtilityProgram.Instance.Executable.FullName;
            this.process.StartInfo.Arguments = "lmstat -a -c " + this.port.ToString() + "@" + this.host;
            this.process.StartInfo.CreateNoWindow = true;
            this.process.StartInfo.RedirectStandardOutput = true;
            this.process.StartInfo.UseShellExecute = false;
        }

        /// <summary>
        /// Parses the output of lmstat to populate the fields with the license's status.
        /// </summary>
        private void ParseReport()
        {
            // Store the last value of certain properties for change notification later.
            bool lastIsFeatureError = this.IsFeatureError;
            int lastUserCount = this.UserCount;
            bool lastInUse = this.InUse;
            int lastInUseCount = this.InUseCount;

            // Example:
            // Flexible License Manager status on Tue 3/17/2009 10:43
            Regex timeExpression = new Regex(@"^Flexible License Manager status on (?<time>.+?)\r?$", RegexOptions.Multiline);
            Match timeMatch = timeExpression.Match(this.report);
            if (timeMatch.Success)
            {
                this.Time = DateTime.ParseExact(timeMatch.Groups["time"].Value, "ddd M/d/yyyy HH:mm", null);
            }
            else
            {
                this.Time = DateTime.MinValue;
            }

            // Example:
            //      License file(s) on SERVER001: C:\License Servers\Test\Test.lic:
            Regex fileExpression = new Regex(@"License file\(s\) on .+: (?<file>.*):", RegexOptions.Multiline);
            Match fileMatch = fileExpression.Match(this.report);
            if (fileMatch.Success)
            {
                this.ServerFile = fileMatch.Groups["file"].Value;
            }

            // Example:
            // Vendor daemon status (on SERVER001):
            //
            //      testdaemon: UP v10.1
            Regex vendorExpression = new Regex(@"^Vendor daemon status.+$\n\s+(?<name>\S+): (?<status>UP) (?<version>v[0-9.]+)", RegexOptions.Multiline);
            Match vendorMatch = vendorExpression.Match(this.report);
            if (vendorMatch.Success)
            {
                this.VendorDaemonName = vendorMatch.Groups["name"].Value;
                this.VendorDaemonStatus = vendorMatch.Groups["status"].Value;
                this.VendorDaemonVersion = vendorMatch.Groups["version"].Value;
                this.IsVendorDaemonUp = true;
            }
            else
            {
                // Example:
                // Vendor daemon status (on SERVER001):
                //
                //     ugslmd: Cannot connect to license server system. (-15,578:10049 "WinSock: Invalid address")
                Regex vendorDownExpression = new Regex(@"^Vendor daemon status.+$\n\s+(?<name>\S+): (?<status>[\S ]+)", RegexOptions.Multiline);
                Match vendorDownMatch = vendorDownExpression.Match(this.report);
                if (vendorDownMatch.Success)
                {
                    this.VendorDaemonName = vendorDownMatch.Groups["name"].Value;
                    this.VendorDaemonStatus = vendorDownMatch.Groups["status"].Value;
                    this.VendorDaemonVersion = String.Empty;
                    this.IsVendorDaemonUp = false;
                }
                else
                {
                    this.VendorDaemonName = String.Empty;
                    this.VendorDaemonStatus = String.Empty;
                    this.VendorDaemonVersion = String.Empty;
                    this.IsVendorDaemonUp = false;
                }
            }

            // Example:
            // Error getting status: Cannot connect to license server system. (-15,10:10061 "WinSock: Connection refused")
            Regex errorStatusExpression = new Regex(@"^Error getting status: (?<message>[^\r]+)", RegexOptions.Multiline);
            Match errorStatusMatch = errorStatusExpression.Match(this.report);
            if (errorStatusMatch.Success)
            {
                this.HasError = true;
                this.ErrorMessage = errorStatusMatch.Groups["message"].Value;
            }
            else
            {
                this.HasError = false;
                this.ErrorMessage = String.Empty;
            }

            // Example:
            // Users of Used_Feature_One_License:  (Total of 1 license issued;  Total of 1 licenses in use)
            //
            //   "Used_Feature_One_License" v22.0, vendor: testdaemon
            //   floating license
            //
            //     user001 comp001 comp001 (v22.0) (SERVER001/27001 3861), start Tue 3/17 7:13
            this.features.Clear();
            Regex featureExpression = new Regex(@"^Users of \S+:.*?(?=^Users of \S+:)|^Users of \S+:.*", RegexOptions.Multiline | RegexOptions.Singleline);
            MatchCollection featureMatches = featureExpression.Matches(this.report);
            foreach (Match featureMatch in featureMatches)
            {
                this.features.Add(new Feature(featureMatch.Value));
            }

            // The following properties are calculated on demand so do not have a setter to check
            // for a PropertyChanged event. Instead they are checked here.
            if (this.IsFeatureError != lastIsFeatureError)
            {
                this.NotifyPropertyChanged("IsFeatureError");
            }

            if (this.UserCount != lastUserCount)
            {
                this.NotifyPropertyChanged("UserCount");
            }

            if (this.InUse != lastInUse)
            {
                this.NotifyPropertyChanged("InUse");
            }

            if (this.InUseCount != lastInUseCount)
            {
                this.NotifyPropertyChanged("InUseCount");
            }

            // The following properties do not have a check to determine if they have changed so
            // the PropertyChanged event is always raised against them.
            this.NotifyPropertyChanged("Report");
            this.NotifyPropertyChanged("Features");
        }
    }
}
