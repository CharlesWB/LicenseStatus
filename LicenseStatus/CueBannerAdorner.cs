// <copyright file="CueBannerAdorner.cs" company="Charles W. Bozarth">
// Copyright (C) 2009 Charles W. Bozarth
// Refer to MainWindow.xaml.cs for the full copyright notice.
// </copyright>

namespace LicenseStatus
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Media.Animation;

    /// <summary>
    /// An adorner which displays an object with an opacity to serve as a cue banner.
    /// </summary>
    /// <remarks>Refer to the CueBannerService class for more information.</remarks>
    internal class CueBannerAdorner : Adorner
    {
        /// <summary>
        /// Stores the ContentPresenter which the adorner displays.
        /// </summary>
        private readonly ContentPresenter contentPresenter;

        /// <summary>
        /// Initializes a new instance of the CueBannerAdorner class.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Generically this adorner simply shows an object that occupies the size of the
        /// adorned element. The opacity can then be set to one of three states.
        /// </para>
        /// <para>
        /// The opacity defaults to 0 so that the adorner can be added to an object and
        /// displayed at a later time.
        /// </para>
        /// </remarks>
        /// <param name="adornedElement">The element to bind the adorner to.</param>
        /// <param name="cueBanner">The object to display as the cue banner.</param>
        public CueBannerAdorner(UIElement adornedElement, object cueBanner)
            : base(adornedElement)
        {
            this.IsHitTestVisible = false;

            this.contentPresenter = new ContentPresenter();
            this.contentPresenter.Content = cueBanner;

            // In the original by Jason Kemp the opacity was set directly on the ContentPresenter.
            // But animating it on the ContentPresenter didn't display the change. So it was changed
            // to directly apply to the adorner.
            this.Opacity = 0;
        }

        /// <summary>
        /// Gets the number of visual child elements within this element. Always returns one.
        /// </summary>
        protected override int VisualChildrenCount
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// Sets the cue banner's opacity to it's default value. This would typically be used
        /// when the control does not have content and does not have focus.
        /// </summary>
        public void SetDisplayToNormal()
        {
            this.AnimateOpacity(.67);
        }

        /// <summary>
        /// Sets the cue banner's opacity to a dim value. This would typically be used when
        /// the control has focus but does not yet have content.
        /// </summary>
        public void SetDisplayToDim()
        {
            this.AnimateOpacity(.33);
        }

        /// <summary>
        /// Sets the cue banner's opacity to zero to hide the banner. This would typically be used
        /// when the control has content.
        /// </summary>
        public void SetDisplayToHidden()
        {
            this.AnimateOpacity(0);
        }

        /// <summary>
        /// Size the adorner to match the size of the adorned element.
        /// </summary>
        /// <param name="constraint">The size constraint.</param>
        /// <returns>The size of the adorner.</returns>
        protected override Size MeasureOverride(Size constraint)
        {
            this.contentPresenter.Measure(this.AdornedElement.RenderSize);
            return this.AdornedElement.RenderSize;
        }

        /// <summary>
        /// Arrange and size the adorner on top of the adorned element.
        /// </summary>
        /// <param name="finalSize">The computed size that is used to arrange the content.</param>
        /// <returns>The size of the adorner.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            this.contentPresenter.Arrange(new Rect(finalSize));
            return finalSize;
        }

        /// <summary>
        /// Returns a child at the specified index from a collection of child elements.
        /// </summary>
        /// <param name="index">The zero-based index of the requested child element in the collection.</param>
        /// <returns>Returns the ContentPresenter regardless of the index.</returns>
        protected override Visual GetVisualChild(int index)
        {
            return this.contentPresenter;
        }

        /// <summary>
        /// Animates the opacity property to the given value if it is not already at that value.
        /// </summary>
        /// <param name="opacity">The opacity value to animate to.</param>
        private void AnimateOpacity(double opacity)
        {
            if (this.Opacity != opacity)
            {
                DoubleAnimation fader = new DoubleAnimation();
                fader.To = opacity;
                fader.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 200));
                this.BeginAnimation(Adorner.OpacityProperty, fader);
            }
        }
    }
}
