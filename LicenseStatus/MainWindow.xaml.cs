// <copyright file="MainWindow.xaml.cs" company="Charles W. Bozarth">
// Copyright (C) 2009-2012 Charles W. Bozarth
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
// <url>http://licensestatus.sourceforge.net</url>
// <summary>Display the status of FlexLM licenses.</summary>

#region Notes
// Text Box Select All on Focus:
//
// By default text boxes in WPF do not automatically select all the text when they receive focus.
// To work around this both keyboard and mouse events are used to select the text.
// http://www.budnack.net/Lists/Posts/Post.aspx?ID=21
// http://eric.burke.name/dotnetmania/2009/03/17/19.30.27
// http://blogs.vertigo.com/personal/alanl/Blog/archive/2007/02/07/wpf-tip-highlight-textbox-text-on-focus.aspx
// http://www.eggheadcafe.com/conversation.aspx?messageid=33804667&threadid=33804666
// http://www.madprops.org/blog/wpf-textbox-selectall-on-focus/
//
// Validation Error Template and Adorner Decorator:
//
// The editable text boxes are using the default error template for displaying an error state.
// For the Port and Host text boxes this works correctly. But the Program text box would not display
// the error template when the window first opens and the setting is empty. The tooltip and the HasError
// property were correctly set, but the adorner did not exist.
//
// If the text box were placed inside of the TabControl like Port and Host then it would work.
//
// This topic, http://www.eggheadcafe.com/conversation.aspx?messageid=33395846&threadid=33395837,
// suggested using UpdateSource on the text box once the window was loaded. This did work, but
// was too much of a work around.
//
// The solution implemented, http://www.metanous.be/Pharcyde/?tag=/adornerdecorator, was to place
// AdornerDecorator around the text box.
//
// Currently the AdornerDecorator is only placed around the text box. As the web page suggested
// it could be placed around the entire UI. I chose not to do this without a better understanding.
// This does cause a separate adorner layer in the logical tree.
//
// Validation Errors Tooltip:
//
// Microsoft's example for setting the tooltip when a validation error occurs uses (Validation.Errors)[0].ErrorContent.
// If there is no error then a binding error is reported in the Visual Studio Output window.
// This describes using (Validation.Errors).CurrentItem.ErrorContent to avoid that.
// http://joshsmithonwpf.wordpress.com/2008/10/08/binding-to-validationerrors0-without-creating-debug-spew/
//
// ItemContainerStyle and ItemTemplate:
//
// The TabItems are constructed using a DataTemplate to add elements in addition to the text.
// When a ContextMenu was needed I added it to the Grid in the DataTemplate. The problem was
// that the right click only responded when over the text of the text block. That left areas
// in the tab where the context menu was not available. I originally thought that I would
// have to switch to the ItemContainerStyle and define a ControlTemplate. This would require
// fully re-implementing the tab's style which I did not want to do. I didn't realize until
// reading the following site that I could define the ItemContainerStyle just for the style
// and still use the ItemTemplate for the content. This resolved the right click problem.
// http://blogs.msdn.com/wpfsdk/archive/2008/04/30/itemcontrols-don-t-properly-select-items-or-raise-events-when-bound-to-a-data-source.aspx
//
// Wrap Panel and Grid Shared Size Scope:
//
// The License properties are shown as labels and associated text boxes. These are arranged in a
// wrap panel to emulate the details pane at the bottom of Windows Vista Explorer. That pane can
// resize and show more or less information in a grid arrangement.
//
// The property's label and text box are placed in a two column grid. A separate grid exists for
// each property. SharedSizeGroup is used to make the text box columns the same width. As the wrap
// panel arranges the grids into different columns a SizeChanged event is used to redefine the
// SharedSizeGroup, e.g. grid columns in the first wrap panel column are Column0, the next is Column1, etc.
// The purpose of this is to allow the wrap panel columns to better size to the data.
//
// One difference from the Vista Explorer is that all properties are always shown instead of hidden
// as Explorer does. I didn't see a benefit to hiding information. A scroll bar is used to access
// the properties if they are outside the window.
//
// This method does work, but it's indirect. I'd like to see a better connection between the wrap panel
// arrangement and sizing of the property grid.
//
// Visually this method is almost perfect. One imperfection is that the horizontal scroll bar is always
// visible instead of being automatic. If it is set to Auto there is a size where the scroll bar becomes
// hidden, the grids resize cause the scroll bar to be shown. The grids resize again causing the scroll
// bar to be hidden again. This becomes an infinite loop until the user resizes the wrap panel.
//
// Another imperfection is that when an editable text box is reduced in size the label column increases
// to compensate instead of the panel decreases.
//
// Another method was tried where the SharedSizeGroup was bound to the parent grid with a converter that
// set the value per the grid's X coordinate. There were two problems. One is that nothing caused the
// binding to update since it was bound to an unchanging grid. This was worked around by using the
// wrap panel's SizeChanged event to call UpdateTarget on each. This almost worked except that the
// first resize caused the column width to match the first property instead of the widest. This is why
// the XAML was changed to predefine a default SharedSizeGroup and then have SizeChanged directly change it.
//
// ListView VirtualizingStackPanel:
//
// With a relatively long list of Features and Users the ListView can take a noticeable amount of time
// to update. VirtualizingStackPanel is designed to resolve this, except that it is not available
// when grouping is enabled. The choice is to use the organizational benefits of grouping versus
// the speed of updating the ListView.
// http://bea.stollnitz.com/blog/?p=338
// http://bea.stollnitz.com/blog/?p=344
//
// CompositeCollection Error:
//
// The TabControl could be defined by a CompositeCollection to add a New License tab. This worked when the
// collection was a StaticResource, but failed when moved to the ViewModel. Possibly described here:
// http://social.expression.microsoft.com/Forums/en-US/wpf/thread/7f787b17-c262-414d-a600-13934d691f68/
// http://social.msdn.microsoft.com/Forums/en-US/wpf/thread/637b563f-8f2f-4af3-a7aa-5d96b719d5fd/
// This may need to be reexamined to determine easy process for adding a new license.
// http://stackoverflow.com/questions/262343/binding-a-dynamicresource has a solution that seems to work.
//
// Public Application Settings:
//
// The auto generated Settings class is private by default. Binding the window position to the settings
// properties worked but when the ListView sort property was bound an error occurred in the designer.
// The Settings class was made public to resolve this.
//
// List View Auto Size Columns:
//
// One annoyance of the current design is the lack of automatic sizing of columns in the list view as the
// data changes. For example, when the list view is first populated then the column will be resized. But
// when a new tab is selected, the license refreshed, or unused features shown the columns will not be
// resized for the new data. To improve usability the columns should resize if the data changes and the
// user has not manually sized the column.
//
// Without custom coding the columns will resize under the following conditions:
//
// - When the list view data is first loaded.
// - When a license tab is removed. The ListView's OnInitialized is called which causes the resize.
// - When a user double clicks to size a column on an empty list view. When a new license tab is selected
//   that column is resized for the new data.
//
// While not perfect custom code has been added to the DataContextChanged event of the TabControl. This
// resizes the columns when a new tab is selected.
//
// Settings Upgrade Failure When Framework Changes:
//
// Usually Settings.Default.Upgrade() handles transferring application settings from the last version to the
// current version. But when the version also changes the .NET framework Upgrade will fail. For License Status
// this occurred at v3.0. V2.x was .NET 3.5 SP1, and v3.0 changed to .NET 4.
// Since the executable path has not changed it appears that .NET 4 changes how the hash is generated for the local settings path.
// http://stackoverflow.com/questions/2722209/applicationsettingsbase-upgrade-not-upgrading-user-settings-after-recompiling
// http://social.msdn.microsoft.com/Forums/en-SG/vbgeneral/thread/3e3d3471-237d-4905-9785-cd024cbf0ebb
// http://www.dotnetmonster.com/Uwe/Forum.aspx/dotnet-vb/58638/My-Settings-Upgrade-doesn-t-upgrade
// http://www.vesic.org/english/blog/winforms/upgrade-of-applicationuser-settings-between-application-versions/
//
// Localization and Column Names:
//
// If this application is ever localized this will affect column names which will also affect
// the default values of HiddenColumns in application settings.
//
// SettingsVersion:
//
// The SettingsVersion property was added in v3.5. It is used to check the "version" of the
// settings. This way changes can be made to the settings only when appropriate.
//
// Prior to v3.5 the SettingsVersion did not exist and, in effect, has a value of zero.
// At v3.5 the value is 1. This value is only changed when needed.
// 
// The first time this was used was to add a new value to HiddenColumn. The new value would
// not be added during an automatic upgrade so SettingsVersion was used. If the version was
// older then the new entry was added to HiddenColumn. Once the version is updated then the
// new entry is no longer added.
//
// An alternative was to set a boolean indicating that the HiddenColumn change had been done
// and no longer needed to be done. This would have meant a new boolean would be needed each
// time a similar changed happened.
//
// Possible Enhancements:
//
// The DataGrid in .NET 4.0 may eliminate most of the ListView custom code.
//
// Resize the columns when the license is refreshed or when the view changes.
//
// When changing tabs or adding a new tab give the port number text box focus.
//
// Identify which columns are User columns and which are Feature columns. Possibly with embedded columns,
// http://www.designerwpf.com/2008/01/25/embedded-listview-columns-columns-within-columns/.
#endregion

namespace LicenseStatus
{
    using System;
    using System.Configuration;
    using System.Globalization;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Input;
    using LicenseStatus.Properties;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Stores the command for toggling the filter of unused features.
        /// </summary>
        public static RoutedCommand ShowUnusedFeaturesCommand = new RoutedCommand();

        /// <summary>
        /// Stores the command that displays the about window.
        /// </summary>
        public static RoutedCommand AboutCommand = new RoutedCommand();

        /// <summary>
        /// Stores the command that exits the window.
        /// </summary>
        public static RoutedCommand ExitCommand = new RoutedCommand();

        /// <summary>
        /// Stores the command that browses for the lmutil command.
        /// </summary>
        public static RoutedCommand BrowseLMUtilCommand = new RoutedCommand();

        /// <summary>
        /// Stores the view model defined in XAML for quick access in the code behind.
        /// </summary>
        private readonly LicenseListViewModel viewModel;

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            // To ensure the settings are upgraded before LicenseListViewModel is instantiated
            // this must be done before InitializeComponent.
            if (Settings.Default.UpgradeSettings)
            {
                Settings.Default.Upgrade();

                if (Settings.Default.UpgradeSettings)
                {
                    this.UpgradeSettingsFromOldVersion();
                }

                // At SettingsVersion 1 (v3.5) a new HiddenColumn entry, 'Checked out', was added.
                // If the user upgrades, this column would be visible as the default. To compensate for
                // that check for the previous SettingsVersion and manually add the column.
                if (Settings.Default.SettingsVersion == 0)
                {
                    if (!Settings.Default.HiddenColumns.Contains("Checked out"))
                    {
                        Settings.Default.HiddenColumns.Add("Checked out");
                    }
                }

                Settings.Default.SettingsVersion = 1;

                Settings.Default.UpgradeSettings = false;
            }

            InitializeComponent();

            this.viewModel = this.FindResource("LicenseListViewModel") as LicenseListViewModel;

            // Bind the commands that the view model requests.
            this.viewModel.BindCommandsToWindow(this);

            this.UpdateUnusedFeaturesFilter(Settings.Default.ShowUnusedFeatures);
        }

        /// <summary>
        /// Enables or disables the view filter for unused features.
        /// </summary>
        /// <param name="showUnusedFeatures">Indicates whether to show unused features.</param>
        private void UpdateUnusedFeaturesFilter(bool showUnusedFeatures)
        {
            CollectionViewSource dataView = this.TryFindResource("FeaturesUsersSource") as CollectionViewSource;

            if (showUnusedFeatures)
            {
                dataView.Filter -= new FilterEventHandler(this.viewModel.UnusedFeatures_Filter);
            }
            else
            {
                dataView.Filter += new FilterEventHandler(this.viewModel.UnusedFeatures_Filter);
            }
        }

        /// <summary>
        /// Event handler that executes the Show Unused Features command.
        /// </summary>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="e">
        /// <para>The event data.</para>
        /// <para>The Parameter property is a boolean value indicating whether to filter the unused features.</para>
        /// </param>
        private void ShowUnusedFeaturesCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.UpdateUnusedFeaturesFilter((bool)e.Parameter);
        }

        /// <summary>
        /// Event handler that executes the about command.
        /// </summary>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="e">The event data.</param>
        private void AboutCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            AboutWindow about = new AboutWindow();
            about.Owner = this;
            about.ShowDialog();
        }

        /// <summary>
        /// Event handler that executes the exit command.
        /// </summary>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="e">The event data.</param>
        private void ExitCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Event handler that executes the command to browse for the lmutil program.
        /// </summary>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="e">The event data.</param>
        private void BrowseLMUtilCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "lmutil Executable (lmutil.exe)|lmutil.exe|Executable Files (*.exe)|*.exe";

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                CWBozarth.LicenseManager.UtilityProgram.Instance.Executable = new FileInfo(dialog.FileName);
            }
        }

        /// <summary>
        /// Event handler for the initialized main window. This restores the window's last location.
        /// </summary>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="e">The event data.</param>
        private void MainWindow_SourceInitialized(object sender, EventArgs e)
        {
            // If WindowPlacement is not set then this is either a brand new install or an upgrade from
            // version before v3.1. If it is an upgrade then the previous settings will be accessible
            // and we can use them to restore the last window position.
            if (Settings.Default.WindowPlacement.length <= 0)
            {
                this.RestoreWindowPositionFromOldSettings();
            }
            else
            {
                this.SetPlacement(Settings.Default.WindowPlacement);
            }
        }

        /// <summary>
        /// Event handler for closing the main window. This saves the user's application settings.
        /// </summary>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="e">The event data.</param>
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Settings.Default.WindowPlacement = this.GetPlacement();

            this.viewModel.SaveSettings();

            Settings.Default.Save();
        }

        /// <summary>
        /// Event handler for selecting all text in a text box when it receives focus via the keyboard.
        /// </summary>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="e">The event data.</param>
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }

        /// <summary>
        /// Event handler for selecting all text in a text box when it receives focus via the mouse.
        /// </summary>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="e">The event data.</param>
        private void TextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (!tb.IsFocused)
            {
                tb.SelectAll();
                Keyboard.Focus(tb);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Event handler for adjusting the shared size column in the license details wrap panel.
        /// </summary>
        /// <remarks>
        /// This is a quick and dirty solution and I hope there is a better way. See the notes above
        /// about shared size scope.
        /// </remarks>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="e">The event data.</param>
        private void LicensePropertiesWrapPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            WrapPanel panel = sender as WrapPanel;

            if (panel != null)
            {
                if (panel.Orientation == Orientation.Vertical)
                {
                    int column = 0;
                    double location = 0;

                    foreach (Grid item in panel.Children)
                    {
                        Rect slot = LayoutInformation.GetLayoutSlot(item);
                        if (slot.X != location)
                        {
                            column++;
                            location = slot.X;
                        }

                        string groupName = string.Format(CultureInfo.InvariantCulture, "Column{0}", column);

                        if (item.ColumnDefinitions.Count == 2 && item.ColumnDefinitions[0].SharedSizeGroup != groupName)
                        {
                            item.ColumnDefinitions[0].SharedSizeGroup = groupName;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Event handler for resizing the report panel via a thumb control.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The report panel was intended to be resized using a GridSplitter. The problem was that
        /// when the panel and splitter were collapsed the remaining columns would remain at the size
        /// the GridSplitter had set them at.
        /// </para>
        /// <para>http://social.msdn.microsoft.com/Forums/en-US/wpf/thread/1f82d834-efae-4eb3-819f-40c2168fb8c9/</para>
        /// <para>http://social.msdn.microsoft.com/forums/en-US/wpf/thread/194d738a-134d-4bc4-940f-85ee8a44d511</para>
        /// </remarks>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="e">The event data.</param>
        private void ThumbVerticalSplitter_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Settings.Default.ReportPanelWidth = Math.Max(Settings.Default.ReportPanelWidth - e.HorizontalChange, 0);
        }

        /// <summary>
        /// Event handler that updates the license name text box if the user leaves it empty.
        /// </summary>
        /// <remarks>
        /// The license name property can be set manually or automatically. If the user erases the contents
        /// of the text box then the name is reset to its automatic value. But the text box does not update
        /// when this happens. This event updates the text box when focus is lost and it is empty.
        /// </remarks>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="e">The event data.</param>
        private void LicenseNameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox != null)
            {
                if (string.IsNullOrEmpty(textBox.Text))
                {
                    BindingExpression binding = textBox.GetBindingExpression(TextBox.TextProperty);
                    if (binding != null)
                    {
                        binding.UpdateTarget();
                    }
                }
            }
        }

        /// <summary>
        /// Event handler that updates the license name text box if the user presses escape when it is empty.
        /// </summary>
        /// <remarks>
        /// See the remarks for LicenseNameTextBox_LostFocus for why this is implemented. This event is designed
        /// with the idea that the user may press escape to try to undo the deletion of the name.
        /// </remarks>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="e">The event data.</param>
        private void LicenseNameTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                TextBox textBox = sender as TextBox;
                if (textBox != null)
                {
                    if (string.IsNullOrEmpty(textBox.Text))
                    {
                        BindingExpression binding = textBox.GetBindingExpression(TextBox.TextProperty);
                        if (binding != null)
                        {
                            binding.UpdateTarget();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Event handler that auto sizes the list view columns when the DataContext changes.
        /// </summary>
        /// <remarks>See the notes about "List View Auto Size Columns" for more information.</remarks>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="e">The event data.</param>
        private void SortListView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ListView listView = sender as ListView;
            if (listView != null)
            {
                if (listView.HasItems)
                {
                    this.AutoSizeColumns(listView);
                }
            }
        }

        /// <summary>
        /// Resize all columns of a list view to fit the contents if the width is not zero.
        /// </summary>
        /// <param name="listView">The list view whose columns are to be resized.</param>
        private void AutoSizeColumns(ListView listView)
        {
            GridView gridView = listView.View as GridView;
            if (gridView != null)
            {
                foreach (GridViewColumn column in gridView.Columns)
                {
                    if (column.Width != 0)
                    {
                        column.Width = column.ActualWidth;
                        column.Width = double.NaN;
                    }
                }
            }
        }

        /// <summary>
        /// Upgrades application settings from an older version that cannot be automatically upgraded.
        /// </summary>
        /// <remarks>
        /// This will manually search for the latest non-upgraded version and copy the old version folder
        /// to the new local settings path.
        /// </remarks>
        private void UpgradeSettingsFromOldVersion()
        {
            Configuration currentConfiguration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);

            if (File.Exists(currentConfiguration.FilePath))
            {
                return;
            }

            DirectoryInfo currentVersionFolder = Directory.GetParent(currentConfiguration.FilePath);
            Version currentVersion;
            if (currentVersionFolder != null && Version.TryParse(currentVersionFolder.Name, out currentVersion))
            {
                DirectoryInfo currentAppSettingsFolder = currentVersionFolder.Parent;
                if (currentAppSettingsFolder != null)
                {
                    DirectoryInfo companyFolder = currentAppSettingsFolder.Parent;
                    if (companyFolder != null && companyFolder.Exists)
                    {
                        // The application folder name is in the form, ExeName_TypeName_Hash. The ExeName can be modified to replace invalid path
                        // characters and shortened (if needed) to a maximum length. See the internal class System.Configuration.ClientConfigPaths.
                        // The following will extract the ExeName from the current settings folder by trimming it down to the next to last underscore.
                        // An underscore could exist in the ExeName, so the first underscore cannot be searched for. For License Status the TypeName
                        // is probably always Url but there is no reason to assume this.
                        // This could have been done with a RegularExpression, but for no particular reason this was done with string searches.
                        // Although unlikely, versions from another program could be found. The other program would have to have the same executable
                        // and company names.
                        int hashUnderscore = currentAppSettingsFolder.Name.LastIndexOf('_');
                        if (hashUnderscore > 1)
                        {
                            int typeUnderscore = currentAppSettingsFolder.Name.LastIndexOf('_', hashUnderscore - 1);
                            if (typeUnderscore > 0)
                            {
                                string searchName = currentAppSettingsFolder.Name.Substring(0, typeUnderscore + 1) + "*";

                                Version oldVersion = new Version(0, 0, 0, 0);
                                DirectoryInfo oldVersionFolder = null;

                                foreach (DirectoryInfo appSettingsFolder in companyFolder.GetDirectories(searchName))
                                {
                                    // Ignore the current settings folder. These should have been correctly handled by Settings.Default.Upgrade.
                                    if (string.Compare(currentAppSettingsFolder.Name, appSettingsFolder.Name, StringComparison.InvariantCultureIgnoreCase) != 0)
                                    {
                                        foreach (DirectoryInfo versionFolder in appSettingsFolder.GetDirectories())
                                        {
                                            Version version;
                                            if (Version.TryParse(versionFolder.Name, out version))
                                            {
                                                if (version > oldVersion && version < currentVersion)
                                                {
                                                    oldVersion = version;
                                                    oldVersionFolder = versionFolder;
                                                }
                                            }
                                        }
                                    }
                                }

                                if (oldVersionFolder != null)
                                {
                                    FileInfo oldConfigFile = new FileInfo(Path.Combine(oldVersionFolder.FullName, "user.config"));
                                    if (oldConfigFile.Exists)
                                    {
                                        try
                                        {
                                            DirectoryInfo oldVersionInNewFolder = new DirectoryInfo(Path.Combine(currentAppSettingsFolder.FullName, oldVersion.ToString()));
                                            if (!oldVersionInNewFolder.Exists)
                                            {
                                                oldVersionInNewFolder.Create();
                                            }

                                            oldConfigFile.CopyTo(Path.Combine(oldVersionInNewFolder.FullName, "user.config"));

                                            Settings.Default.Upgrade();
                                        }
                                        catch (Exception)
                                        {
                                            // This is overkill, but we'll ignore any exceptions that may occur during folder and file creation.
                                            // The only thing this should cause is that the old configuration won't be transferred to the new.
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Attempt to restore the window position using settings that were used prior to v3.1.
        /// If the old settings are used then GetPlacement followed by SetPlacement will be called
        /// to ensure the window is visible on an active screen.
        /// </summary>
        /// <remarks>
        /// This would be only be applicable after the upgrade from v3.0.
        /// </remarks>
        private void RestoreWindowPositionFromOldSettings()
        {
            try
            {
                object left = Settings.Default.GetPreviousVersion("WindowLeft");
                object top = Settings.Default.GetPreviousVersion("WindowTop");
                object width = Settings.Default.GetPreviousVersion("WindowWidth");
                object height = Settings.Default.GetPreviousVersion("WindowHeight");
                object state = Settings.Default.GetPreviousVersion("WindowState");

                if (left != null)
                {
                    this.Left = (double)left;
                }

                if (top != null)
                {
                    this.Top = (double)top;
                }

                if (width != null)
                {
                    this.Width = (double)width;
                }

                if (height != null)
                {
                    this.Height = (double)height;
                }

                if (state != null)
                {
                    this.WindowState = (System.Windows.WindowState)state;
                }

                // In other words, if all the settings were null then skip the call to SetPlacement
                // since the window is being placed at its default location.
                if (left != null || top != null || width != null || height != null || state != null)
                {
                    // By calling SetPlacement with the current GetPlacement we ensure that the window
                    // is visible on an active screen.
                    this.SetPlacement(this.GetPlacement());
                }
            }
            catch (SettingsPropertyNotFoundException)
            {
                // If any of the previous settings are not found then there's no need to restore
                // the previous window location.
            }
        }
    }
}
