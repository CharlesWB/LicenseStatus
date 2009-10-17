// <copyright file="AboutWindow.xaml.cs" company="Charles W. Bozarth">
// Copyright (C) 2009 Charles W. Bozarth
// Refer to MainWindow.xaml.cs for the full copyright notice.
// </copyright>

// For the most part this is a generically written About dialog which could be applied to any program.
// This is done by reading the executing assembly's attributes for title, description, version, etc.
// Where it is not generic is the references specifically to the LicenseManager assembly.
// This could also be done generically but for one assembly it was decided not to.
// It is also not generic in the hyperlink.
// 
// This code behind file was intended to be empty, but there was a problem with setting up
// the MethodParameters for GetCustomAttribute in XAML. Instead these were implemented as properties.

namespace LicenseStatus
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the AboutWindow class.
        /// </summary>
        public AboutWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets the executing assembly's title.
        /// </summary>
        public static string AssemblyTitle
        {
            get
            {
                AssemblyTitleAttribute title = Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyTitleAttribute)) as AssemblyTitleAttribute;
                return title.Title;
            }
        }

        /// <summary>
        /// Gets the executing assembly's copyright.
        /// </summary>
        public static string AssemblyCopyright
        {
            get
            {
                AssemblyCopyrightAttribute copyright = Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyCopyrightAttribute)) as AssemblyCopyrightAttribute;
                return copyright.Copyright;
            }
        }

        /// <summary>
        /// Gets the executing assembly's company.
        /// </summary>
        public static string AssemblyCompany
        {
            get
            {
                AssemblyCompanyAttribute company = Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyCompanyAttribute)) as AssemblyCompanyAttribute;
                return company.Company;
            }
        }

        /// <summary>
        /// Gets the executing assembly's description.
        /// </summary>
        public static string AssemblyDescription
        {
            get
            {
                AssemblyDescriptionAttribute description = Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyDescriptionAttribute)) as AssemblyDescriptionAttribute;
                return description.Description;
            }
        }

        /// <summary>
        /// Event handler that closes the window.
        /// </summary>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="e">The event data.</param>
        private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Event handler that opens a hyperlink address.
        /// </summary>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="e">The event data.</param>
        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            string navigateUri = e.Uri.AbsoluteUri;
            Process.Start(new ProcessStartInfo(navigateUri));

            e.Handled = true;
        }
    }
}
