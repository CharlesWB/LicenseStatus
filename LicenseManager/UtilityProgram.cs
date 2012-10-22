// <copyright file="UtilityProgram.cs" company="Charles W. Bozarth">
// Copyright (C) 2009-2012 Charles W. Bozarth
// Refer to License.cs for the full copyright notice.
// </copyright>

// Originally the lmutil executable was a static property in the License class.
// The problem was that it made data binding overly complicated. See the following
// notes for more information.
//
// Two Way Binding Of A Static Property:
//
// In the License class the LMUtilProgram is a static property. The difficulty with WPF binding
// to a static property is making two way binding work.
//
// One option, {Binding Source={x:Static local:SimpleClass.SimpleStaticProperty}, Path=.}, will
// bind to a static property but will not update the source. If the source is null then an exception
// occurs at startup.
//
// A second option, <ObjectDataProvider x:Key="SimpleData" ObjectType="{x:Type local:SimpleClass}" />
// with {Binding Source={StaticResource SimpleData}, Path=SimpleStaticProperty}, will update the source.
// If the source is null an exception will not occur at startup. But other controls bound to the
// property are not notified of the update. Not sure how to implement INotifyPropertyChanged with
// a static property.
// http://blah.winsmarts.com/2007-2-WPF__DataBinding_with_any_object,_Including_LINQ-ADONET_Entity_FrameWork.aspx
//
// A third option, {Binding Source={StaticResource SimpleClass}, Path=SimpleRefStaticProperty}, is
// implemented by creating a non-static property which gets/sets the static property. This will
// update the source. If the source is null an exception will not occur at startup.
// This was temporarily tried, but it required creating both the executable and the version
// non-static properties so that property change events could be raised for the version.
// http://social.msdn.microsoft.com/forums/en-US/wpf/thread/b42027ab-07be-47a1-889a-a930d19fc4a5/
//
// A fourth option (related to the third option) is creating a singleton class instead of a static property.
// http://social.msdn.microsoft.com/forums/en-US/wpf/thread/257a41be-8168-401c-a915-cdc44e195a3f/

namespace CWBozarth.LicenseManager
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;

    /// <summary>
    /// Represents the FlexLM utility program (lmutil) which provides access to the license information.
    /// </summary>
    /// <remarks>
    /// The utility program is defined as a singleton because there is currently no need for
    /// a different program for each license.
    /// </remarks>
    public sealed class UtilityProgram : ObservableObject, IDataErrorInfo
    {
        /// <summary>
        /// Stores the instance of this singleton class.
        /// </summary>
        private static readonly UtilityProgram instance = new UtilityProgram();

        /// <summary>
        /// Stores the file specification of the lmutil program.
        /// </summary>
        private FileInfo executable;

        /// <summary>
        /// Prevents a default instance of the UtilityProgram class from being created.
        /// </summary>
        private UtilityProgram()
        {
        }

        /// <summary>
        /// Gets the instance of this singleton class.
        /// </summary>
        public static UtilityProgram Instance
        {
            get
            {
                return instance;
            }
        }

        /// <summary>
        /// Gets or sets the FlexLM utility program's executable.
        /// </summary>
        public FileInfo Executable
        {
            get
            {
                return this.executable;
            }

            set
            {
                this.executable = value;
                this.NotifyPropertyChanged("Executable");
                this.NotifyPropertyChanged("Version");
            }
        }

        /// <summary>
        /// Gets the file version of the utility program.
        /// </summary>
        public string Version
        {
            get
            {
                if (this.executable != null && this.executable.Exists)
                {
                    FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(this.executable.FullName);
                    return string.Format(CultureInfo.InvariantCulture, "{0}.{1}.{2}.{3}", versionInfo.FileMajorPart, versionInfo.FileMinorPart, versionInfo.FileBuildPart, versionInfo.FilePrivatePart);
                }
                else
                {
                    return string.Empty;
                }
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

                if (columnName == "Executable")
                {
                    if (this.executable != null && !this.executable.Exists)
                    {
                        result = "File does not exist.";
                    }

                    if (this.executable == null)
                    {
                        result = "Program is a required entry.";
                    }
                }

                return result;
            }
        }
    }
}
