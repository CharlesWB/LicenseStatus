// <copyright file="SortGridViewColumn.cs" company="Charles W. Bozarth">
// Copyright (C) 2009-2012 Charles W. Bozarth
// Refer to MainWindow.xaml.cs for the full copyright notice.
// </copyright>

namespace LicenseStatus
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    /// <summary>
    /// Represents a column for use with the SortListView class.
    /// </summary>
    /// <remarks>Defines dependency properties for configuring the sort options within XAML.</remarks>
    public class SortGridViewColumn : GridViewColumn
    {
        /// <summary>
        /// The dependency property for SortProperty.
        /// </summary>
        public static readonly DependencyProperty SortPropertyProperty = DependencyProperty.Register("SortProperty", typeof(string), typeof(SortGridViewColumn));

        /// <summary>
        /// The dependency property for SecondarySort.
        /// </summary>
        public static readonly DependencyProperty SecondarySortProperty = DependencyProperty.Register("SecondarySort", typeof(bool), typeof(SortGridViewColumn), new PropertyMetadata(false));

        /// <summary>
        /// The dependency property for GroupTemplate.
        /// </summary>
        public static readonly DependencyProperty GroupTemplateProperty = DependencyProperty.Register("GroupTemplate", typeof(string), typeof(SortGridViewColumn));

        /// <summary>
        /// The dependency property for GroupConverter.
        /// </summary>
        public static readonly DependencyProperty GroupConverterProperty = DependencyProperty.Register("GroupConverter", typeof(IValueConverter), typeof(SortGridViewColumn));

        /// <summary>
        /// Gets or sets the name of the property to sort by when the list view is sorted on this column.
        /// </summary>
        /// <remarks>This is used since the property cannot always be accessed from the binding path.</remarks>
        public string SortProperty
        {
            get { return (string)GetValue(SortPropertyProperty); }
            set { SetValue(SortPropertyProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this column is to be used for sorting in addition to the primary sort column.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If this column is used as the primary sort, then it will not be used for secondary sorting
        /// regardless of this setting.
        /// </para>
        /// <para>
        /// Multiple columns can be defined for secondary sorting. They will be added to the SortDescriptions
        /// collection in the same order as the column order.
        /// </para>
        /// </remarks>
        /// <example>
        /// For example, if the list view is sorted and grouped by feature name, the user name is defined as
        /// a secondary sort so that user names will be sorted within the feature group.
        /// </example>
        public bool SecondarySort
        {
            get { return (bool)GetValue(SecondarySortProperty); }
            set { SetValue(SecondarySortProperty, value); }
        }

        /// <summary>
        /// Gets or sets the template that is used to display the group header when the list view is grouped by this column.
        /// </summary>
        public string GroupTemplate
        {
            get { return (string)GetValue(GroupTemplateProperty); }
            set { SetValue(GroupTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the converter to use for the PropertyGroupDescription when the list view is grouped by this column.
        /// </summary>
        public IValueConverter GroupConverter
        {
            get { return (IValueConverter)GetValue(GroupConverterProperty); }
            set { SetValue(GroupConverterProperty, value); }
        }
    }
}
