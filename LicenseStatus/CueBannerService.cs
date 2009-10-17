// <copyright file="CueBannerService.cs" company="Charles W. Bozarth">
// Copyright (C) 2009 Charles W. Bozarth
// Refer to MainWindow.xaml.cs for the full copyright notice.
// </copyright>

namespace LicenseStatus
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;

    /// <summary>
    /// Enables the ability to display a cue banner over an element.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is based on Jason Kemp's method for using an adorner,
    /// http://www.ageektrapped.com/blog/the-missing-net-4-cue-banner-in-wpf-i-mean-watermark-in-wpf/
    /// and Kevin Moore's InfoTextBox visual style, http://j832.com/bagotricks/. Another example,
    /// http://vistasquad.co.uk/blogs/nondestructive/archive/2009/01/02/wpf-textbox-with-watermark.aspx.
    /// </para>
    /// <para>
    /// This most closely follows Jason's method, but with the addition of using the
    /// visual style of InfoTextBox. At the moment this only supports TextBox unlike Jason's.
    /// </para>
    /// <para>
    /// I chose not to use InfoTextBox because I didn't want a custom ControlTemplate which did
    /// not match the Window's default or to create themed templates. CueBannerService also gave
    /// me the chance to try adorners and modify it to implement the InfoTextBox visual style.
    /// </para>
    /// <para>
    /// One downside to this method is that the TextBox must be wide enough to display the cue banner.
    /// Yet the TextBox itself does not know what width to use. Should the cue banner be allowed to modify
    /// the width?
    /// </para>
    /// </remarks>
    public static class CueBannerService
    {
        /// <summary>
        /// Defines the CueBanner attached property.
        /// </summary>
        public static readonly DependencyProperty CueBannerProperty =
            DependencyProperty.RegisterAttached("CueBanner", typeof(object), typeof(CueBannerService), new FrameworkPropertyMetadata(String.Empty, CueBanner_PropertyChanged));

        /// <summary>
        /// Gets the CueBanner property.
        /// </summary>
        /// <param name="obj">The object the property is attached to.</param>
        /// <returns>The property's value.</returns>
        public static object GetCueBanner(DependencyObject obj)
        {
            return (object)obj.GetValue(CueBannerProperty);
        }

        /// <summary>
        /// Sets the CueBanner property.
        /// </summary>
        /// <param name="obj">The object the property is attached to.</param>
        /// <param name="value">The property's value.</param>
        public static void SetCueBanner(DependencyObject obj, object value)
        {
            obj.SetValue(CueBannerProperty, value);
        }

        /// <summary>
        /// Event handler that occurs when the cue banner property changes.
        /// </summary>
        /// <param name="d">The DependencyObject on which the property has changed value.</param>
        /// <param name="e">The event data.</param>
        private static void CueBanner_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextBox textBox = d as TextBox;
            if (textBox != null)
            {
                textBox.Loaded += new RoutedEventHandler(Control_Loaded);
                textBox.GotFocus += new RoutedEventHandler(Control_GotFocus);
                textBox.LostFocus += new RoutedEventHandler(Control_LostFocus);
                textBox.TextChanged += new TextChangedEventHandler(Control_TextChanged);
            }
        }

        /// <summary>
        /// Event handler that updates the cue banner when the control's text is changed.
        /// </summary>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="e">The event data.</param>
        private static void Control_TextChanged(object sender, TextChangedEventArgs e)
        {
            Control control = sender as Control;
            CueBannerService.UpdateAdornerState(control);
        }

        /// <summary>
        /// Event handler that updates the cue banner when the control loses focus.
        /// </summary>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="e">The event data.</param>
        private static void Control_LostFocus(object sender, RoutedEventArgs e)
        {
            Control control = sender as Control;
            CueBannerService.UpdateAdornerState(control);
        }

        /// <summary>
        /// Event handler that updates the cue banner when the control gets focus.
        /// </summary>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="e">The event data.</param>
        private static void Control_GotFocus(object sender, RoutedEventArgs e)
        {
            Control control = sender as Control;
            CueBannerService.UpdateAdornerState(control);
        }

        /// <summary>
        /// Event handler that adds the cue banner to the control.
        /// </summary>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="e">The event data.</param>
        private static void Control_Loaded(object sender, RoutedEventArgs e)
        {
            Control control = sender as Control;
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(control);
            CueBannerAdorner cueBannerAdorner = new CueBannerAdorner(control, CueBannerService.GetCueBanner(control));
            adornerLayer.Add(cueBannerAdorner);

            CueBannerService.UpdateAdornerState(control);
        }

        /// <summary>
        /// Gets a value indicating whether the control has content.
        /// </summary>
        /// <remarks>Currently only TextBox is supported.</remarks>
        /// <param name="control">The control to check.</param>
        /// <returns>True if the control has content.</returns>
        private static bool HasContent(Control control)
        {
            bool result = false;
            TextBox textBox = control as TextBox;

            if (textBox != null)
            {
                result = !String.IsNullOrEmpty(textBox.Text);

                // This is only here so that the cue banner can be seen at design time in Visual Studio.
                // The TextBox at design time receives a space as its text.
                // This assumes a single space is not intended by the user code.
                if (result && System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                {
                    result = !(textBox.Text == " ");
                }
            }

            return result;
        }

        /// <summary>
        /// Updates the display of the cue banner based on the control's content, focus and visibility.
        /// </summary>
        /// <param name="control">The control the cue banner is attached to.</param>
        private static void UpdateAdornerState(Control control)
        {
            if (control != null)
            {
                CueBannerAdorner adorner = GetCueBannerAdorner(control);
                if (adorner != null)
                {
                    if (CueBannerService.HasContent(control) || !control.IsEnabled || control.Visibility != Visibility.Visible)
                    {
                        adorner.SetDisplayToHidden();
                    }
                    else
                    {
                        if (control.IsFocused)
                        {
                            adorner.SetDisplayToDim();
                        }
                        else
                        {
                            adorner.SetDisplayToNormal();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the cue banner adorner which is applied to the control.
        /// </summary>
        /// <param name="control">The control the cue banner is attached to.</param>
        /// <returns>The cue banner adorner. If the adorner is not found then null is returned.</returns>
        private static CueBannerAdorner GetCueBannerAdorner(Control control)
        {
            CueBannerAdorner cueBannerAdorner = null;

            if (control != null)
            {
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(control);

                if (adornerLayer != null)
                {
                    Adorner[] adorners = adornerLayer.GetAdorners(control);
                    if (adorners != null)
                    {
                        foreach (Adorner adorner in adorners)
                        {
                            if (adorner is CueBannerAdorner)
                            {
                                cueBannerAdorner = adorner as CueBannerAdorner;
                            }
                        }
                    }
                }
            }

            return cueBannerAdorner;
        }
    }
}
