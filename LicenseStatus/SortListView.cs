// <copyright file="SortListView.cs" company="Charles W. Bozarth">
// Copyright (C) 2010 Charles W. Bozarth
// Refer to MainWindow.xaml.cs for the full copyright notice.
// </copyright>

// Notes:
//
// This class could probably be better designed. It is too closely tied to the requirements of the
// License view models.
//
// Originally I had intended to avoid using inheritance and use attached properties in a static class.
// But after trying that and reviewing some examples (such as Family Show) I decided to switch back
// to inheritance and creating my own ListView.
//
// When sorting the GroupDescriptions class is cleared and updated even if the grouping hasn't changed.
// Does this increase the refresh time? It is not noticeable if it does.
//
// View Notes:
//
// Typically examples apply sorting, grouping and filtering using the collection view returned by
// CollectionViewSource.GetDefaultView. The License Status UI consists of a TabControl which contains
// a ListView. Using GetDefaultView on ListView.ItemsSource does sort the ListView until a new tab is
// selected. I'm not entirely convinced on why that is. Either it is due to the ListView inside the
// TabControl, the ListView in a DataTemplate, or the view is reset for some other reason. I do have
// to wonder if the design of the UI is incorrect in some way.
//
// Testing found two methods for applying the sort and having it maintain as the tab changes,
// ListView.Items and modifying a CollectionViewSource directly. ListView.Items (ItemCollection.Items)
// worked well in that it worked if the original binding was done directly to a property or was
// bound to a CollectionViewSource. It is also easy to access within this class. What is not so easy
// is getting the ListView object outside of this class.
//
// For that reason modifying the CollectionViewSource is used. SortListView contains a property for
// setting the CollectionViewSource. This makes it easy for this class to find the resource.
// The code-behind can find the resource using it's key.
//
// Until I learn more about handling this situation I do consider this a hack to get a usable
// sorting, grouping and filtering method.
//
// References:
//
// Dynamic sorting in WPF is not as automated as it could be. In choosing this sort method I looked
// at the following.
//
// http://www.codeplex.com/familyshow
// http://blogs.interknowlogy.com/joelrumerman/archive/2007/04/03/12497.aspx
// http://www.thejoyofcode.com/Sortable_ListView_in_WPF.aspx
// http://mbrownchicago.spaces.live.com/blog/cns!2221DC39E0C749A4!331.entry?wa=wsignin1.0&sa=751959639
// http://ligao101.wordpress.com/2007/07/31/a-much-faster-sorting-for-listview-in-wpf/
// http://www.beacosta.com/blog/?p=17
// http://msdn.microsoft.com/en-us/library/ms745786.aspx
// http://www.switchonthecode.com/tutorials/wpf-tutorial-using-the-listview-part-2-sorting
// http://www.codeproject.com/KB/WPF/WPFListViewSorter.aspx
// http://marlongrech.wordpress.com/2008/04/20/part1-avaloncontrolslibrary-datagridview/

namespace LicenseStatus
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    /// <summary>
    /// A ListView control that supports sorting.
    /// </summary>
    public class SortListView : ListView
    {
        /// <summary>
        /// The dependency property for ColumnHeaderTemplate.
        /// </summary>
        public static readonly DependencyProperty ColumnHeaderTemplateProperty =
            DependencyProperty.Register("ColumnHeaderTemplate", typeof(string), typeof(SortListView));

        /// <summary>
        /// The dependency property for SortProperty.
        /// </summary>
        public static readonly DependencyProperty SortPropertyProperty =
            DependencyProperty.Register("SortProperty", typeof(string), typeof(SortListView));

        /// <summary>
        /// The dependency property for SortDirection.
        /// </summary>
        public static readonly DependencyProperty SortDirectionProperty =
            DependencyProperty.Register("SortDirection", typeof(ListSortDirection), typeof(SortListView), new PropertyMetadata(ListSortDirection.Ascending));

        /// <summary>
        /// The dependency property for CollectionViewSource.
        /// </summary>
        public static readonly DependencyProperty CollectionViewSourceProperty =
            DependencyProperty.Register("CollectionViewSource", typeof(CollectionViewSource), typeof(SortListView));

        /// <summary>
        /// The current column that is sorted.
        /// </summary>
        private SortGridViewColumn sortColumn;

        /// <summary>
        /// The previous column that was sorted.
        /// </summary>
        private SortGridViewColumn previousSortColumn;

        /// <summary>
        /// Gets or sets the template that is used to display the column header for unsorted, ascending and descending sorts.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The template set in this property will be used to remove the sort template from a column. This template
        /// is not required. If it does not exist then the column header template will be set to null.
        /// </para>
        /// <para>
        /// For the ascending and descending sort templates the name is determined by appending 'ascending' or
        /// 'descending' to this template name. These templates are also optional. If they do not exist then
        /// the column header template will not be changed.
        /// </para>
        /// </remarks>
        public string ColumnHeaderTemplate
        {
            get { return (string)GetValue(ColumnHeaderTemplateProperty); }
            set { SetValue(ColumnHeaderTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the name of the property that the list view is currently sorted by.
        /// </summary>
        /// <remarks>
        /// Unlike online examples this has been defined as a dependency property. This allows it to define the sort
        /// during initializing.
        /// </remarks>
        public string SortProperty
        {
            get { return (string)GetValue(SortPropertyProperty); }
            set { SetValue(SortPropertyProperty, value); }
        }

        /// <summary>
        /// Gets or sets the sort direction that the list view is currently sorted by.
        /// </summary>
        /// <remarks>
        /// Unlike online examples this has been defined as a dependency property. This allows it to define the sort
        /// during initializing.
        /// </remarks>
        public ListSortDirection SortDirection
        {
            get { return (ListSortDirection)GetValue(SortDirectionProperty); }
            set { SetValue(SortDirectionProperty, value); }
        }

        /// <summary>
        /// Gets or sets the CollectionViewSource object that the list view is to use for sorting and grouping.
        /// </summary>
        /// <remarks>
        /// This is a hack to get the sort to maintain when the TabControl changes tabs. See the View notes above.
        /// </remarks>
        public CollectionViewSource CollectionViewSource
        {
            get { return (CollectionViewSource)GetValue(CollectionViewSourceProperty); }
            set { SetValue(CollectionViewSourceProperty, value); }
        }

        /// <summary>
        /// Handles the initialized event. If SortProperty is defined then the sort will be applied.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnInitialized(EventArgs e)
        {
            // Handle the event when a header is clicked.
            this.AddHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(this.OnHeaderClicked));

            if (this.SortProperty != null && !String.IsNullOrEmpty(this.SortProperty))
            {
                GridView gridView = this.View as GridView;
                if (gridView != null)
                {
                    foreach (SortGridViewColumn column in gridView.Columns)
                    {
                        if (column.SortProperty == this.SortProperty)
                        {
                            this.sortColumn = column;
                        }
                    }

                    if (this.sortColumn != null)
                    {
                        this.SortList();

                        this.UpdateHeaderTemplate();

                        this.UpdateGroupTemplate();
                    }
                }
            }

            base.OnInitialized(e);
        }

        /// <summary>
        /// A header was clicked. Sort and group by the associated column.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnHeaderClicked(object sender, RoutedEventArgs e)
        {
            // Make sure the column is really being sorted.
            GridViewColumnHeader header = e.OriginalSource as GridViewColumnHeader;
            if (header == null || header.Role == GridViewColumnHeaderRole.Padding)
            {
                return;
            }

            SortGridViewColumn column = header.Column as SortGridViewColumn;
            if (column == null)
            {
                return;
            }

            // See if a new column was clicked, or the same column was clicked.
            if (this.sortColumn != column)
            {
                // A new column was clicked.
                this.previousSortColumn = this.sortColumn;
                this.sortColumn = column;
                this.SortDirection = ListSortDirection.Ascending;
                this.SortProperty = column.SortProperty;
            }
            else
            {
                // The same column was clicked, change the sort order.
                this.previousSortColumn = null;
                this.SortDirection = (this.SortDirection == ListSortDirection.Ascending) ? ListSortDirection.Descending : ListSortDirection.Ascending;
            }

            this.UpdateHeaderTemplate();

            this.UpdateGroupTemplate();

            this.SortList();
        }

        /// <summary>
        /// Sort and group the data.
        /// </summary>
        private void SortList()
        {
            // Get the data to sort.
            CollectionViewSource dataView = this.CollectionViewSource;

            if (dataView.View == null)
            {
                return;
            }

            using (dataView.View.DeferRefresh())
            {
                // Specify the new sorting information.
                dataView.SortDescriptions.Clear();
                SortDescription description = new SortDescription(this.sortColumn.SortProperty, this.SortDirection);
                dataView.SortDescriptions.Add(description);

                // Iterate through the columns to determine if any others are designated for secondary sort.
                GridView gridview = this.View as GridView;
                if (gridview != null)
                {
                    foreach (SortGridViewColumn column in gridview.Columns)
                    {
                        if (column.SortProperty != this.sortColumn.SortProperty && column.SecondarySort)
                        {
                            SortDescription secondarySortDescription = new SortDescription(column.SortProperty, ListSortDirection.Ascending);
                            dataView.SortDescriptions.Add(secondarySortDescription);
                        }
                    }
                }

                // Specify the new grouping information.
                dataView.GroupDescriptions.Clear();
                PropertyGroupDescription group = new PropertyGroupDescription(this.sortColumn.SortProperty, this.sortColumn.GroupConverter, StringComparison.CurrentCulture);
                dataView.GroupDescriptions.Add(group);
            }
        }

        /// <summary>
        /// Update the column header based on the sort column and order.
        /// </summary>
        private void UpdateHeaderTemplate()
        {
            DataTemplate headerTemplate;

            // Restore the previous header.
            if (this.previousSortColumn != null && this.ColumnHeaderTemplate != null)
            {
                headerTemplate = this.TryFindResource(this.ColumnHeaderTemplate) as DataTemplate;
                this.previousSortColumn.HeaderTemplate = headerTemplate;
            }

            // Update the current header.
            if (this.ColumnHeaderTemplate != null)
            {
                // The name of the resource to use for the header.
                string resourceName = this.ColumnHeaderTemplate + this.SortDirection.ToString();

                headerTemplate = this.TryFindResource(resourceName) as DataTemplate;
                if (headerTemplate != null)
                {
                    this.sortColumn.HeaderTemplate = headerTemplate;

                    // Cause the column width to resize to fit the sort arrow if needed. Only do this
                    // if the sort direction is not changing since it would have already been sized during
                    // the first sort. This always resizes a new column even though it would be nice if
                    // it only resized if needed.
                    // http://social.msdn.microsoft.com/Forums/en-US/wpf/thread/50b1ab5a-d06e-41a8-b8ea-0333a0237ad7/
                    if (this.previousSortColumn != null)
                    {
                        this.sortColumn.Width = this.sortColumn.ActualWidth;
                        this.sortColumn.Width = double.NaN;
                    }
                }
            }
        }

        /// <summary>
        /// Update the group template based on the sort column.
        /// </summary>
        /// <remarks>This applies the template to the first GroupStyle level.</remarks>
        private void UpdateGroupTemplate()
        {
            DataTemplate groupTemplate;

            if (this.sortColumn.GroupTemplate != null)
            {
                groupTemplate = this.TryFindResource(this.sortColumn.GroupTemplate) as DataTemplate;
                if (groupTemplate != null && this.GroupStyle.Count > 0)
                {
                    this.GroupStyle[0].HeaderTemplate = groupTemplate;
                }
            }
        }
    }
}
