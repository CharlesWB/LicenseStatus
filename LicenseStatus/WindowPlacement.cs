// <copyright file="WindowPlacement.cs" company="Charles W. Bozarth">
// Copyright (C) 2009-2012 Charles W. Bozarth
// Refer to MainWindow.xaml.cs for the full copyright notice.
// </copyright>

// Notes:
// 
// Prior to v3.1 the window location was managed by binding to WindowLeft, WindowTop, etc.
// settings. This did not properly handle multiple screens when the screen is disabled.
// At v3.1 window location was changed to using the WindowPlacement setting. This WindowPlacement
// class was created to access GetPlacement and SetPlacement. This class is separate from
// MainWindow only to simplify the code.
//
// References:
//
// http://msdn.microsoft.com/en-us/library/aa972163(v=VS.90).aspx
// http://blogs.msdn.com/b/davidrickard/archive/2010/03/09/saving-window-size-and-location-in-wpf-and-winforms.aspx
// http://jake.ginnivan.net/remembering-wpf-window-positions
// http://www.codeproject.com/Articles/247333/Renaming-User-Settings-properties-between-software

namespace LicenseStatus
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Interop;

    /// <summary>
    /// Structure required by the WINDOWPLACEMENT structure.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Not going to re-document native Windows structures.")]
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        public RECT(int left, int top, int right, int bottom)
        {
            this.Left = left;
            this.Top = top;
            this.Right = right;
            this.Bottom = bottom;
        }
    }

    /// <summary>
    /// Structure required by the WINDOWPLACEMENT structure.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Not going to re-document native Windows structures.")]
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;

        public POINT(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }

    /// <summary>
    /// Structure to store the position, size, and state of a window.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1307:AccessibleFieldsMustBeginWithUpperCaseLetter",
        Justification = "Formatted the same as the native Windows method.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Not going to re-document native Windows structures.")]
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct WINDOWPLACEMENT
    {
        public int length;
        public int flags;
        public int showCmd;
        public POINT minPosition;
        public POINT maxPosition;
        public RECT normalPosition;
    }

    /// <summary>
    /// Provides methods for accessing GetPlacement and SetPlacement methods.
    /// </summary>
    public static class WindowPlacement
    {
        /// <summary>
        /// A showCmd option to display the window at its original size.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore",
            Justification = "Formatted the same as the native Windows constant.")]
        private const int SW_SHOWNORMAL = 1;

        /// <summary>
        /// A showCmd option to display the window minimized.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore",
            Justification = "Formatted the same as the native Windows constant.")]
        private const int SW_SHOWMINIMIZED = 2;

        /// <summary>
        /// An extension to the Window class to set the window placement.
        /// </summary>
        /// <param name="window">The window to place.</param>
        /// <param name="placement">The placement of the window.</param>
        public static void SetPlacement(this Window window, WINDOWPLACEMENT placement)
        {
            WindowPlacement.SetPlacement(new WindowInteropHelper(window).Handle, placement);
        }

        /// <summary>
        /// An extension to the Window class to get the window placement.
        /// </summary>
        /// <param name="window">The window to place.</param>
        /// <returns>The placement of the window.</returns>
        public static WINDOWPLACEMENT GetPlacement(this Window window)
        {
            return WindowPlacement.GetPlacement(new WindowInteropHelper(window).Handle);
        }

        /// <summary>
        /// Sets the show state and the restored and maximized positions of the specified window.
        /// </summary>
        /// <param name="windowHandle">A handle to the window.</param>
        /// <param name="placement">The WINDOWPLACEMENT structure that contains the show state and position information.</param>
        public static void SetPlacement(IntPtr windowHandle, WINDOWPLACEMENT placement)
        {
            try
            {
                placement.length = Marshal.SizeOf(typeof(WINDOWPLACEMENT));
                placement.flags = 0;
                placement.showCmd = placement.showCmd == SW_SHOWMINIMIZED ? SW_SHOWNORMAL : placement.showCmd;
                SetWindowPlacement(windowHandle, ref placement);
            }
            catch (Exception)
            {
                // Ignore failures when setting the window placement.
            }
        }

        /// <summary>
        /// Retrieves the show state and the restored, minimized, and maximized positions of the specified window.
        /// </summary>
        /// <param name="windowHandle">A handle to the window.</param>
        /// <returns>The WINDOWPLACEMENT structure that contains the show state and position information.</returns>
        public static WINDOWPLACEMENT GetPlacement(IntPtr windowHandle)
        {
            WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
            GetWindowPlacement(windowHandle, out placement);

            return placement;
        }

        /// <summary>
        /// Sets the show state and the restored, minimized, and maximized positions of the specified window.
        /// </summary>
        /// <param name="hWnd">A handle to the window.</param>
        /// <param name="lpwndpl">A pointer to a WINDOWPLACEMENT structure that specifies the new show state and window positions.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation",
            Justification = "Formatted the same as the native Windows method.")]
        [DllImport("user32.dll")]
        private static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WINDOWPLACEMENT lpwndpl);

        /// <summary>
        /// Retrieves the show state and the restored, minimized, and maximized positions of the specified window.
        /// </summary>
        /// <param name="hWnd">A handle to the window. </param>
        /// <param name="lpwndpl">A pointer to the WINDOWPLACEMENT structure that receives the show state and position information.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation",
            Justification = "Formatted the same as the native Windows method.")]
        [DllImport("user32.dll")]
        private static extern bool GetWindowPlacement(IntPtr hWnd, out WINDOWPLACEMENT lpwndpl);
    }
}
