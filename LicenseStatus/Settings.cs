// <copyright file="Settings.cs" company="Charles W. Bozarth">
// Copyright (C) 2009-2014 Charles W. Bozarth
// Refer to MainWindow.xaml.cs for the full copyright notice.
// </copyright>

namespace LicenseStatus.Properties
{
    /// <summary>
    /// Partial class to customize Settings.
    /// </summary>
    [global::System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules",
        "SA1411:AttributeConstructorMustNotUseUnnecessaryParenthesis",
        Justification = "Formatted the same as the system generated Settings.Designer.cs.")]
    public sealed partial class Settings
    {
        /// <summary>
        /// Gets or sets the window width setting.
        /// </summary>
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("600")]
        [global::System.Obsolete("WindowWidth has been replaced by WindowPlacement.")]
        [global::System.Configuration.NoSettingsVersionUpgrade()]
        public double WindowWidth
        {
            get
            {
                throw new System.NotSupportedException("WindowWidth is obsolete.");
            }

            set
            {
                throw new System.NotSupportedException("WindowWidth is obsolete.");
            }
        }

        /// <summary>
        /// Gets or sets the window height setting.
        /// </summary>
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("380")]
        [global::System.Obsolete("WindowHeight has been replaced by WindowPlacement.")]
        [global::System.Configuration.NoSettingsVersionUpgrade()]
        public double WindowHeight
        {
            get
            {
                throw new System.NotSupportedException("WindowHeight is obsolete.");
            }

            set
            {
                throw new System.NotSupportedException("WindowHeight is obsolete.");
            }
        }

        /// <summary>
        /// Gets or sets the window state setting.
        /// </summary>
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Normal")]
        [global::System.Obsolete("WindowState has been replaced by WindowPlacement.")]
        [global::System.Configuration.NoSettingsVersionUpgrade()]
        public global::System.Windows.WindowState WindowState
        {
            get
            {
                throw new System.NotSupportedException("WindowState is obsolete.");
            }

            set
            {
                throw new System.NotSupportedException("WindowState is obsolete.");
            }
        }

        /// <summary>
        /// Gets or sets the window left setting.
        /// </summary>
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("NaN")]
        [global::System.Obsolete("WindowLeft has been replaced by WindowPlacement.")]
        [global::System.Configuration.NoSettingsVersionUpgrade()]
        public double WindowLeft
        {
            get
            {
                throw new System.NotSupportedException("WindowLeft is obsolete.");
            }

            set
            {
                throw new System.NotSupportedException("WindowLeft is obsolete.");
            }
        }

        /// <summary>
        /// Gets or sets the window top setting.
        /// </summary>
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("NaN")]
        [global::System.Obsolete("WindowTop has been replaced by WindowPlacement.")]
        [global::System.Configuration.NoSettingsVersionUpgrade()]
        public double WindowTop
        {
            get
            {
                throw new System.NotSupportedException("WindowTop is obsolete.");
            }

            set
            {
                throw new System.NotSupportedException("WindowTop is obsolete.");
            }
        }
    }
}
