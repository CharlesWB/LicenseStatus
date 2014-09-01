// <copyright file="GridViewColumnMenu.cs" company="Charles W. Bozarth">
// Copyright (C) 2009-2014 Charles W. Bozarth
// Refer to MainWindow.xaml.cs for the full copyright notice.
// </copyright>

#region Notes
// Alternative to GridView.ColumnHeaderContextMenu
//
// As an alternative to this class the column header context menu could be defined manually in XAML:
// <GridView.ColumnHeaderContextMenu>
//     <ContextMenu>
//         <MenuItem Header="User Name" IsCheckable="True" IsChecked="{Binding ElementName=UserNameColumn, Path=Width, Converter={StaticResource ColumnWidthToBooleanConverter}}" IsEnabled="False" />
//         <MenuItem Header="Host" IsCheckable="True" IsChecked="{Binding ElementName=HostColumn, Path=Width, Converter={StaticResource ColumnWidthToBooleanConverter}}" />
//     </ContextMenu>
// </GridView.ColumnHeaderContextMenu>
// This would require naming each column and manually defining the MenuItem header.
// The primary reason for creating the GridViewColumnMenu class is to automatically generate the
// context menu by simply setting an attached property.
// This class also provides a property for collecting a list of hidden columns which was designed
// to support application settings with data binding.
//
// Attached Property Initialization
//
// Both RequiredColumns and HiddenColumns are generic Lists which are not watched for changes.
// This means the properties must be defined before the context menu is generated. Testing
// showed that when the properties are bound or defined in XAML they are set before the
// ListView is initialized. I haven't found confirmation that this is guaranteed.
//
// Attached Property that is a Collection
//
// RequiredColumns is a List<string>. This could also be a string[]. This would require that that XAML be:
// <x:Array Type="sys:String" x:Key="RequiredColumns">
//     <sys:String>User Name</sys:String>
//     <sys:String>Feature Name</sys:String>
// </x:Array>
// This would also have to be defined as a Resource instead of placed directly in the property value.
// This appears to be a bug and is mentioned here:
// http://social.msdn.microsoft.com/Forums/en-US/wpf/thread/7290f1d6-0c11-4261-accc-d33324fd2897
//
// References:
//
// http://shemesh.wordpress.com/2011/08/02/wpf-datagrid-with-integrated-field-column-chooser/
// http://blogs.telerik.com/blogs/posts/10-05-26/how-to-column-chooser-for-radgridview-for-silverlight-and-wpf.aspx
// http://www.telerik.com/community/forums/wpf/gridview/column-chooser-in-wpf.aspx
//
// Possible Enhancements:
//
// Handle the case where IsEnabled changes from true to false.
//
// Should Set and Get attached property methods use GridView instead of DependencyObject?
#endregion

namespace LicenseStatus
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    /// <summary>
    /// Implements a GridView column chooser context menu using attached properties.
    /// </summary>
    /// <remarks>
    /// <para>The <see cref="ColumnWidthToBooleanConverter"/> value converter is required.</para>
    /// <para>
    /// There are two assumptions made by this class. That the column header value is a string.
    /// And that the column header values are unique. Uniqueness affects RequiredColumns and HiddenColumns.
    /// </para>
    /// </remarks>
    public class GridViewColumnMenu
    {
        /// <summary>
        /// Identifies the IsEnabled attached property.
        /// </summary>
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(GridViewColumnMenu), new PropertyMetadata(new PropertyChangedCallback(OnIsEnabledPropertyChanged)));

        /// <summary>
        /// Identifies the RequiredColumns attached property.
        /// </summary>
        public static readonly DependencyProperty RequiredColumnsProperty =
            DependencyProperty.RegisterAttached("RequiredColumns", typeof(List<string>), typeof(GridViewColumnMenu), new PropertyMetadata(new List<string>()));

        /// <summary>
        /// Identifies the HiddenColumns attached property.
        /// </summary>
        public static readonly DependencyProperty HiddenColumnsProperty =
            DependencyProperty.RegisterAttached("HiddenColumns", typeof(List<string>), typeof(GridViewColumnMenu), new PropertyMetadata(new List<string>()));

        /// <summary>
        /// Stores the instance of the <see cref="ColumnWidthToBooleanConverter"/> value converter.
        /// </summary>
        private static IValueConverter converter;

        /// <summary>
        /// Stores the GridView the properties are attached to.
        /// </summary>
        private GridView gridView;

        /// <summary>
        /// Initializes a new instance of the GridViewColumnMenu class.
        /// </summary>
        /// <param name="gridView">The GridView the column chooser will be added to.</param>
        public GridViewColumnMenu(GridView gridView)
        {
            this.gridView = gridView;

            converter = new ColumnWidthToBooleanConverter();
        }

        /// <summary>
        /// Gets a value indicating whether the column chooser is enabled on an element.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The IsEnabled property value for the element.</returns>
        [AttachedPropertyBrowsableForType(typeof(GridView))]
        public static bool GetIsEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsEnabledProperty);
        }

        /// <summary>
        /// Sets a value indicating whether the column chooser is enabled on an element.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The IsEnabled value.</param>
        public static void SetIsEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsEnabledProperty, value);
        }

        /// <summary>
        /// Gets a list of column names which cannot be hidden. They are shown in the context menu, but cannot be deselected.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The RequiredColumns property value for the element.</returns>
        [AttachedPropertyBrowsableForType(typeof(GridView))]
        public static List<string> GetRequiredColumns(DependencyObject obj)
        {
            return (List<string>)obj.GetValue(RequiredColumnsProperty);
        }

        /// <summary>
        /// Sets a list of column names which cannot be hidden. They are shown in the context menu, but cannot be deselected.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The IsEnabled value.</param>
        /// <remarks>
        /// Setting this property after initialization will not change which columns are required.
        /// </remarks>
        public static void SetRequiredColumns(DependencyObject obj, List<string> value)
        {
            obj.SetValue(RequiredColumnsProperty, value);
        }

        /// <summary>
        /// Gets a list of column names which are hidden.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The HiddenColumns property value for the element.</returns>
        [AttachedPropertyBrowsableForType(typeof(GridView))]
        public static List<string> GetHiddenColumns(DependencyObject obj)
        {
            return (List<string>)obj.GetValue(HiddenColumnsProperty);
        }

        /// <summary>
        /// Sets a list of column names which are hidden.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The HiddenColumns value.</param>
        /// <remarks>
        /// Setting this property after initialization will not change the display of a column. This property is maintained
        /// internally. The intent is to make it easier to save and restore the column state with application settings.
        /// </remarks>
        public static void SetHiddenColumns(DependencyObject obj, List<string> value)
        {
            obj.SetValue(HiddenColumnsProperty, value);
        }

        /// <summary>
        /// Called when the IsEnabled property changes.
        /// </summary>
        /// <param name="dependencyObject">The DependencyObject on which the property has changed value.</param>
        /// <param name="e">Event data that is issued by any event that tracks changes to the effective value of this property.</param>
        private static void OnIsEnabledPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            GridView gridView = dependencyObject as GridView;
            if (gridView != null)
            {
                if ((bool)e.NewValue)
                {
                    GridViewColumnMenu menu = new GridViewColumnMenu(gridView);
                    menu.Attach();
                }
            }
        }

        /// <summary>
        /// Attaches to events of the GridView to dynamically build the context menu from the columns.
        /// </summary>
        private void Attach()
        {
            if (this.gridView != null)
            {
                this.gridView.Columns.CollectionChanged += new NotifyCollectionChangedEventHandler(this.GridViewColumns_CollectionChanged);
            }
        }

        /// <summary>
        /// Updates the context menu as columns are changed in the GridView.
        /// </summary>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="e">The event data.</param>
        private void GridViewColumns_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.gridView.ColumnHeaderContextMenu == null)
            {
                this.gridView.ColumnHeaderContextMenu = new ContextMenu();
            }

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    for (int i = 0; i < e.NewItems.Count; i++)
                    {
                        this.gridView.ColumnHeaderContextMenu.Items.Insert(e.NewStartingIndex + i, this.GetMenuItemForColumn(e.NewItems[i] as GridViewColumn));
                    }

                    break;

                case NotifyCollectionChangedAction.Move:
                    // TODO This may be an incomplete implementation since it only processes one item in the change. It's assumed this is true for columns.
                    // This occurs when columns are reordered.
                    object menuItem = this.gridView.ColumnHeaderContextMenu.Items[e.OldStartingIndex];
                    this.gridView.ColumnHeaderContextMenu.Items.RemoveAt(e.OldStartingIndex);
                    this.gridView.ColumnHeaderContextMenu.Items.Insert(e.NewStartingIndex, menuItem);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    for (int i = 0; i < e.OldItems.Count; i++)
                    {
                        this.gridView.ColumnHeaderContextMenu.Items.RemoveAt(e.OldStartingIndex);
                    }

                    break;

                case NotifyCollectionChangedAction.Replace:
                    for (int i = 0; i < e.NewItems.Count; i++)
                    {
                        this.gridView.ColumnHeaderContextMenu.Items[e.NewStartingIndex + i] = this.GetMenuItemForColumn(e.NewItems[i] as GridViewColumn);
                    }

                    break;

                case NotifyCollectionChangedAction.Reset:
                    this.gridView.ColumnHeaderContextMenu = null;
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Creates the MenuItem that corresponds to the GridViewColumn.
        /// </summary>
        /// <param name="column">The GridViewColumn to read the header from.</param>
        /// <returns>The MenuItem with the proper settings defined.</returns>
        private MenuItem GetMenuItemForColumn(GridViewColumn column)
        {
            MenuItem menu = new MenuItem();
            menu.Header = column.Header.ToString();
            menu.IsCheckable = true;

            menu.IsEnabled = true;
            if (GetRequiredColumns(this.gridView) != null && GetRequiredColumns(this.gridView).Contains(menu.Header))
            {
                menu.IsEnabled = false;
            }

            menu.SetBinding(MenuItem.IsCheckedProperty, new Binding("Width") { Mode = BindingMode.TwoWay, Source = column, Converter = converter });

            if (GetHiddenColumns(this.gridView) != null && GetHiddenColumns(this.gridView).Contains(menu.Header))
            {
                menu.IsChecked = false;
            }

            menu.Checked += new RoutedEventHandler(this.Menu_Checked);
            menu.Unchecked += new RoutedEventHandler(this.Menu_Unchecked);

            return menu;
        }

        /// <summary>
        /// Updates the HiddenColumns property when the associated menu item is unchecked.
        /// </summary>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="e">The event data.</param>
        private void Menu_Unchecked(object sender, RoutedEventArgs e)
        {
            MenuItem menu = e.OriginalSource as MenuItem;
            if (GetHiddenColumns(this.gridView) != null && !GetHiddenColumns(this.gridView).Contains(menu.Header.ToString()))
            {
                GetHiddenColumns(this.gridView).Add(menu.Header.ToString());
            }
        }

        /// <summary>
        /// Updates the HiddenColumns property when the associated menu item is checked.
        /// </summary>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="e">The event data.</param>
        private void Menu_Checked(object sender, RoutedEventArgs e)
        {
            MenuItem menu = e.OriginalSource as MenuItem;
            if (GetHiddenColumns(this.gridView) != null)
            {
                GetHiddenColumns(this.gridView).Remove(menu.Header.ToString());
            }
        }
    }
}
