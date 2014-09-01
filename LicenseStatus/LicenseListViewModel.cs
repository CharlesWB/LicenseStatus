// <copyright file="LicenseListViewModel.cs" company="Charles W. Bozarth">
// Copyright (C) 2009-2014 Charles W. Bozarth
// Refer to MainWindow.xaml.cs for the full copyright notice.
// </copyright>

// Possible Enhancements
//
// Review the use of SelectedLicense property versus the selected item in the CollectionView.

namespace LicenseStatus
{
    using System;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Windows;
    using System.Windows.Input;
    using LicenseStatus.Properties;

    /// <summary>
    /// Represents the view model of a collection of licenses.
    /// </summary>
    public class LicenseListViewModel : System.ComponentModel.INotifyPropertyChanged
    {
        /// <summary>
        /// Stores the currently selected license.
        /// </summary>
        private LicenseViewModel selectedLicense;

        /// <summary>
        /// Initializes a new instance of the LicenseListViewModel class.
        /// </summary>
        public LicenseListViewModel()
        {
            this.Licenses = new ObservableCollection<LicenseViewModel>();

            // These commands include the input gestures to simplify associating them with the command. Even
            // though this is probably inappropriate if the program were to be globalized.
            GetStatusCommand = new RoutedCommand("GetStatusCommand", typeof(LicenseListViewModel), new InputGestureCollection(new KeyGesture[] { new KeyGesture(Key.F5) }));
            AddLicenseCommand = new RoutedCommand("AddLicenseCommand", typeof(LicenseListViewModel), new InputGestureCollection(new KeyGesture[] { new KeyGesture(Key.L, ModifierKeys.Control) }));
            RemoveLicenseCommand = new RoutedCommand("RemoveLicenseCommand", typeof(LicenseListViewModel));

            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                this.CreateTestData();
            }

            this.RestoreSettings();
            
            // The first time the program is used there won't be any licenses defined.
            // Rather than have the user select the Add License command just do it for them.
            if (this.Licenses.Count == 0)
            {
                this.AddLicense();
            }
        }

        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the command for updating the status of licenses.
        /// </summary>
        public static RoutedCommand GetStatusCommand { get; private set; }

        /// <summary>
        /// Gets the command for adding a license to the collection.
        /// </summary>
        public static RoutedCommand AddLicenseCommand { get; private set; }

        /// <summary>
        /// Gets the command for removing the currently selected license from the collection.
        /// </summary>
        public static RoutedCommand RemoveLicenseCommand { get; private set; }

        /// <summary>
        /// Gets the list of licenses.
        /// </summary>
        public ObservableCollection<LicenseViewModel> Licenses { get; private set; }

        /// <summary>
        /// Gets or sets the currently selected license.
        /// </summary>
        /// <remarks>
        /// This was implemented to allow the view model to control which license is selected. This is
        /// used when adding or removing a license to the collection.
        /// </remarks>
        public LicenseViewModel SelectedLicense
        {
            get
            { 
                return this.selectedLicense;
            }

            set
            {
                if (value != this.selectedLicense)
                {
                    this.selectedLicense = value;
                    this.NotifyPropertyChanged("SelectedLicense");
                }
            }
        }

        /// <summary>
        /// Creates command bindings between the window and the commands provided by this view model.
        /// </summary>
        /// <param name="element">The element to bind the commands to.</param>
        public void BindCommandsToWindow(UIElement element)
        {
            element.CommandBindings.Add(new CommandBinding(GetStatusCommand, this.GetStatusCommand_Executed, this.GetStatusCommand_CanExecute));
            element.CommandBindings.Add(new CommandBinding(AddLicenseCommand, this.AddLicenseCommand_Executed, this.AddLicenseCommand_CanExecute));
            element.CommandBindings.Add(new CommandBinding(RemoveLicenseCommand, this.RemoveLicenseCommand_Executed, this.RemoveLicenseCommand_CanExecute));
        }

        /// <summary>
        /// Filter event handler that can be used to filter out features which are not in use.
        /// </summary>
        /// <remarks>
        /// <para>This only sets Accepted to false to allow other filters to be applied at the same time.</para>
        /// <para>Reference: http://bea.stollnitz.com/blog/?p=32. </para>
        /// </remarks>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="e">The event data.</param>
        public void UnusedFeatures_Filter(object sender, System.Windows.Data.FilterEventArgs e)
        {
            FeatureUserItem item = (FeatureUserItem)e.Item;
            if (!item.Feature.InUse)
            {
                e.Accepted = false;
            }
        }

        /// <summary>
        /// Save the application settings.
        /// </summary>
        /// <remarks>
        /// <para>This must be manually called.</para>
        /// <para>I'd like to have a more automatic method, possibly through binding.</para>
        /// </remarks>
        public void SaveSettings()
        {
            if (CWBozarth.LicenseManager.UtilityProgram.Instance.Executable == null)
            {
                Settings.Default.UtilityProgramExecutable = string.Empty;
            }
            else
            {
                Settings.Default.UtilityProgramExecutable = CWBozarth.LicenseManager.UtilityProgram.Instance.Executable.FullName;
            }

            // This implementation will create a list even if it will be empty after processing the licenses.
            // This means the settings file will contain the empty list object instead of no object, but this
            // is not significant.
            if (Settings.Default.Licenses == null)
            {
                Settings.Default.Licenses = new System.Collections.Generic.List<CWBozarth.LicenseManager.License>();
            }

            Settings.Default.Licenses.Clear();
            foreach (LicenseViewModel license in this.Licenses)
            {
                if (string.IsNullOrEmpty(license.License["Port"]) && string.IsNullOrEmpty(license.License["Host"]))
                {
                    Settings.Default.Licenses.Add(license.License);
                }
            }
        }

        /// <summary>
        /// Restore the application settings.
        /// </summary>
        private void RestoreSettings()
        {
            if (!string.IsNullOrEmpty(Settings.Default.UtilityProgramExecutable))
            {
                CWBozarth.LicenseManager.UtilityProgram.Instance.Executable = new FileInfo(Settings.Default.UtilityProgramExecutable);
            }

            // This is both a hack and a solution. This is a hack in that if LicenseViewModel were serializable the
            // ObservableCollection could be sent directly to the settings. This is a solution in that storing the
            // License means the settings don't need to be updated if the UI aspects get redesigned. Although that
            // is only a minor benefit. Another benefit is that a license can be checked to see if the port and
            // host are defined before saving it.
            if (Settings.Default.Licenses != null)
            {
                foreach (CWBozarth.LicenseManager.License license in Settings.Default.Licenses)
                {
                    LicenseViewModel licenseViewModel = new LicenseViewModel(license);
                    this.Licenses.Add(licenseViewModel);

                    if (licenseViewModel.License.GetStatusCanExecute)
                    {
                        licenseViewModel.GetStatus();
                    }
                }
            }
        }

        /// <summary>
        /// Event handler that determines if the get status command can be executed.
        /// </summary>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="e">The event data.</param>
        private void GetStatusCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            bool result = false;

            foreach (LicenseViewModel license in this.Licenses)
            {
                if (license.License.GetStatusCanExecute == true)
                {
                    result = true;
                }
            }

            e.CanExecute = result;
        }

        /// <summary>
        /// Event handler that executes the get status command.
        /// </summary>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="e">The event data.</param>
        private void GetStatusCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            foreach (LicenseViewModel license in this.Licenses)
            {
                if (license.License.GetStatusCanExecute)
                {
                    license.GetStatus();
                }
            }
        }

        /// <summary>
        /// Event handler that determines if the add license command can be executed.
        /// </summary>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="e">The event data.</param>
        private void AddLicenseCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        /// <summary>
        /// Event handler that executes the add license command.
        /// </summary>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="e">The event data.</param>
        private void AddLicenseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.AddLicense();
        }

        /// <summary>
        /// Event handler that determines if the remove license command can be executed.
        /// </summary>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="e">The event data.</param>
        private void RemoveLicenseCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.SelectedLicense != null;
        }

        /// <summary>
        /// Event handler that executes the remove license command.
        /// </summary>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="e">The event data.</param>
        private void RemoveLicenseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.SelectedLicense.License.IsBusy)
            {
                this.SelectedLicense.License.GetStatusAsyncCancel();
            }

            this.Licenses.Remove(this.SelectedLicense);

            // If all the licenses are removed then the only useful thing the user can
            // then do is add a new license. Rather than have the user select the Add
            // License command just do it for them.
            if (this.Licenses.Count == 0)
            {
                this.AddLicense();
            }
        }

        /// <summary>
        /// Adds a new license to the collection and makes it the selected license.
        /// </summary>
        private void AddLicense()
        {
            LicenseViewModel newLicense = new LicenseViewModel(new CWBozarth.LicenseManager.License());
            this.Licenses.Add(newLicense);
            this.SelectedLicense = newLicense;
        }

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        private void NotifyPropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Create a set of licenses for development testing and design-time data.
        /// </summary>
        private void CreateTestData()
        {
            // The class won't be instantiated at design-time if StartupPath, GetExecutingAssembly or
            // Path.GetDirectoryName is used. Another option hasn't been found.
            string testFilesPath;
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                testFilesPath = @"C:\Users\Charles\Documents\Visual Studio 2012\Projects\License Status\";
                ////testFilesPath = @"C:\Documents and Settings\Charles\My Documents\Visual Studio 11\Projects\License Status\";
            }
            else
            {
                testFilesPath = System.IO.Path.GetFullPath(System.Reflection.Assembly.GetExecutingAssembly().Location + @"\..\..\..\..\");
            }

            CWBozarth.LicenseManager.UtilityProgram.Instance.Executable = new System.IO.FileInfo(testFilesPath + "lmutil.exe");

            CWBozarth.LicenseManager.License testLicense = new CWBozarth.LicenseManager.License();
            testLicense.Name = "Test License";
            testLicense.Port = 27001;
            testLicense.Host = "SERVER001";
            testLicense.GetStatus(new System.IO.FileInfo(testFilesPath + "lmstat-test.log"));

            CWBozarth.LicenseManager.License ugsLicense = new CWBozarth.LicenseManager.License();
            ugsLicense.Name = "NX";
            ugsLicense.Port = 27000;
            ugsLicense.Host = "SERVER001";
            ugsLicense.GetStatus(new System.IO.FileInfo(testFilesPath + "lmstat-nx.log"));

            CWBozarth.LicenseManager.License acadLicense = new CWBozarth.LicenseManager.License();
            acadLicense.Name = "Autodesk";
            acadLicense.Port = 27005;
            acadLicense.Host = "SERVER001";
            acadLicense.GetStatus(new System.IO.FileInfo(testFilesPath + "lmstat-acad.log"));

            CWBozarth.LicenseManager.License errorsLicense = new CWBozarth.LicenseManager.License();
            errorsLicense.Port = 7601;
            errorsLicense.Host = "server001";
            errorsLicense.GetStatus(new System.IO.FileInfo(testFilesPath + "lmstat-errors.log"));

            CWBozarth.LicenseManager.License invalidLicense = new CWBozarth.LicenseManager.License();
            invalidLicense.Name = "Invalid";
            invalidLicense.Port = 1001;
            invalidLicense.Host = "SERVER001";
            invalidLicense.GetStatus(new System.IO.FileInfo(testFilesPath + "lmstat-invalid.log"));

            this.Licenses.Add(new LicenseViewModel(ugsLicense));
            this.Licenses.Add(new LicenseViewModel(testLicense));
            this.Licenses.Add(new LicenseViewModel(acadLicense));
            this.Licenses.Add(new LicenseViewModel(errorsLicense));
            this.Licenses.Add(new LicenseViewModel(invalidLicense));
        }
    }
}
