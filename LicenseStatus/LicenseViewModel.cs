// <copyright file="LicenseViewModel.cs" company="Charles W. Bozarth">
// Copyright (C) 2009-2012 Charles W. Bozarth
// Refer to MainWindow.xaml.cs for the full copyright notice.
// </copyright>

namespace LicenseStatus
{
    using System;
    using System.Collections.ObjectModel;
    using CWBozarth.LicenseManager;

    /// <summary>
    /// Represents a feature and one of its users.
    /// </summary>
    /// <remarks>
    /// <para>A feature and a user are combined in this structure to enable showing them as a single item.</para>
    /// <para>This was implemented as a structure since there did not seem to be a need for a full class definition.</para>
    /// </remarks>
    public struct FeatureUserItem
    {
        /// <summary>
        /// Stores the feature.
        /// </summary>
        private Feature feature;

        /// <summary>
        /// Stores the user.
        /// </summary>
        private User user;

        /// <summary>
        /// Initializes a new instance of the FeatureUserItem structure.
        /// </summary>
        /// <param name="feature">A feature.</param>
        /// <param name="user">A user of the feature.</param>
        public FeatureUserItem(Feature feature, User user)
        {
            this.feature = feature;
            this.user = user;
        }

        /// <summary>
        /// Gets the feature of this item.
        /// </summary>
        public Feature Feature
        {
            get { return this.feature; }
        }

        /// <summary>
        /// Gets the user of this item.
        /// </summary>
        public User User
        {
            get { return this.user; }
        }
    }

    /// <summary>
    /// Represents the view model of a License.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The primary purpose of this view model is to provide a collection that merges a Feature and one
    /// of its users into a single item.
    /// </para>
    /// <para>
    /// This class uses the License class although it could have inherited it. The choice was made to
    /// follow some existing M-V-VM examples and to simulate using a class that couldn't be inherited from.
    /// </para>
    /// </remarks>
    public class LicenseViewModel : System.ComponentModel.INotifyPropertyChanged, System.ComponentModel.IDataErrorInfo
    {
        /// <summary>
        /// Stores an empty instance of the User class.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The original intent was to set the user to null when a feature does not have any users.
        /// This caused the default sort method to fail because it does not support null values for comparison.
        /// Rather than create a custom sort method this blank user is used with empty and minimum value properties.
        /// The problem with this is that those properties with zero or minimum values should be treated as non-values.
        /// This was resolved with value converters.
        /// </para>
        /// <para>
        /// If a custom sort is implemented that can support null values then this will not be needed. The User class
        /// can then also be updated to remove the default constructor.
        /// </para>
        /// </remarks>
        private static User blankUser;

        /// <summary>
        /// Stores the text version of the port number.
        /// </summary>
        private string portText;

        /// <summary>
        /// Stores the collection of the combined feature and user items.
        /// </summary>
        private ObservableCollection<FeatureUserItem> featuresUsers;

        /// <summary>
        /// Initializes a new instance of the LicenseViewModel class for a License.
        /// </summary>
        /// <param name="license">The license that this is a view model of.</param>
        public LicenseViewModel(License license)
        {
            this.License = license;
            this.portText = license.Port.ToString();

            this.featuresUsers = new ObservableCollection<FeatureUserItem>();
            this.UpdateFeaturesUsers();

            if (blankUser == null)
            {
                blankUser = new User();
            }

            license.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(this.License_PropertyChanged);
            license.GetStatusCompleted += new System.ComponentModel.AsyncCompletedEventHandler(this.License_GetStatusCompleted);
        }

        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the license of this view model.
        /// </summary>
        public License License { get; private set; }

        /// <summary>
        /// Gets the collection of the combined feature and user items.
        /// </summary>
        /// <remarks>This might be something that Linq could be used for.</remarks>
        public ObservableCollection<FeatureUserItem> FeaturesUsers
        {
            get { return this.featuresUsers; }
        }

        /// <summary>
        /// Gets or sets the port number from the view in a string format.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This property is a recreation of the License Port property except as a string type.
        /// That is so that when a non-numeric value is entered in the view the binding conversion
        /// error is not ignored.
        /// </para>
        /// <para>
        /// This is taken directly from the example at
        /// http://joshsmithonwpf.wordpress.com/2008/11/14/using-a-viewmodel-to-provide-meaningful-validation-error-messages/.
        /// </para>
        /// <para>
        /// To make it so the view model updates if the model is changed directly the PropertyChanged
        /// event is used to notify the view model when changes happen.
        /// </para>
        /// <para>
        /// A more advanced method is at
        /// http://karlshifflett.wordpress.com/mvvm/input-validation-ui-exceptions-model-validation-errors/.
        /// </para>
        /// </remarks>
        public string Port
        {
            get
            { 
                return this.portText;
            }

            set
            {
                if (value != this.portText)
                {
                    this.portText = value;
                    this.NotifyPropertyChanged("Port");
                }
            }
        }

        /// <summary>
        /// Gets or sets the descriptive name of the license.
        /// </summary>
        /// <remarks>
        /// Since the License class has both an automatic and a manual name the name property
        /// is recreated here so that the appropriate name is returned. Otherwise binding would
        /// be more complex to determine when to return Name and when to return ToString.
        /// </remarks>
        public string Name
        {
            get
            {
                return this.License.ToString();
            }

            set
            {
                if (value != this.License.Name)
                {
                    this.License.Name = value;
                    this.NotifyPropertyChanged("Name");
                }
            }
        }

        /// <summary>
        /// Gets an error message indicating what is wrong with this object.
        /// </summary>
        /// <remarks>
        /// Not implemented.
        /// </remarks>
        public string Error
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the error message for the property with the given name.
        /// </summary>
        /// <param name="columnName">The name of the property whose error message to get.</param>
        /// <returns>The error message for the property.</returns>
        public string this[string columnName]
        {
            get
            {
                string result = null;

                if (columnName == "Port")
                {
                    int port = 0;
                    if (string.IsNullOrEmpty(this.portText))
                    {
                        this.License.Port = null;
                        result = this.License["Port"];
                    }
                    else if (int.TryParse(this.portText, out port))
                    {
                        this.License.Port = port;
                        result = this.License["Port"];
                    }
                    else
                    {
                        result = "Port must be a valid number.";
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the current status of the license.
        /// </summary>
        /// <remarks>
        /// This will use the asynchronous method for getting the status.
        /// </remarks>
        public void GetStatus()
        {
#if DEBUG
            // This is only for testing using an existing lmstat output file.
            string testFilesPath = System.IO.Path.GetFullPath(System.Reflection.Assembly.GetExecutingAssembly().Location + @"\..\..\..\..\");
            System.IO.FileInfo lmstatFile = new System.IO.FileInfo(testFilesPath + this.License.Host + ".log");
            if (lmstatFile.Exists)
            {
                Random delay = new Random((int)(DateTime.Now.Ticks % int.MaxValue));
                this.License.GetStatusAsync(lmstatFile, delay.Next(100, 5000));
            }
            else
#endif
            {
                this.License.GetStatusAsync();
            }
        }

        /// <summary>
        /// Event handler that updates the view model properties when the model properties change.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The License's Port property is checked and if it differs from this object's property
        /// then this object is updated.
        /// </para>
        /// <para>
        /// If License's Port or Host property is changed and the Name is manually defined then
        /// notification is sent that this classes Name property has changed.
        /// </para>
        /// </remarks>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="e">The event data.</param>
        private void License_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Port")
            {
                if (this.Port != this.License.Port.ToString())
                {
                    this.Port = this.License.Port.ToString();
                }

                if (string.IsNullOrEmpty(this.License.Name))
                {
                    this.NotifyPropertyChanged("Name");
                }
            }

            if (e.PropertyName == "Host")
            {
                if (string.IsNullOrEmpty(this.License.Name))
                {
                    this.NotifyPropertyChanged("Name");
                }
            }
        }

        /// <summary>
        /// Event handler that updates the FeaturesUsers collection when GetStatus is completed.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The alternative to updating the FeaturesUsers collection is by connecting to the license's PropertyChanged
        /// event and watching for the Features update. This was not done because of threading issues. The event
        /// may not be running on the UI thread which would not allow the collection to be updated. Rather than
        /// using Dispatcher or modifying License's PropertyChanged event the GetStatusCompleted event is used.
        /// That event is designed to return on the UI thread.
        /// </para>
        /// <para>http://www.beacosta.com/blog/?p=34</para>
        /// <para>http://lambert.geek.nz/2007/10/30/wpf-multithreaded-collections/</para>
        /// </remarks>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="e">The event data.</param>
        private void License_GetStatusCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                this.UpdateFeaturesUsers();
            }
        }

        /// <summary>
        /// Merges the license's features and users into a single collection.
        /// </summary>
        private void UpdateFeaturesUsers()
        {
            this.featuresUsers.Clear();
            foreach (Feature feature in this.License.Features)
            {
                if (feature.Users.Count == 0)
                {
                    // Refer to the remarks on the blankUser field for why this was used instead of null.
                    this.featuresUsers.Add(new FeatureUserItem(feature, blankUser));
                }
                else
                {
                    foreach (User user in feature.Users)
                    {
                        this.featuresUsers.Add(new FeatureUserItem(feature, user));
                    }
                }
            }
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
    }
}
