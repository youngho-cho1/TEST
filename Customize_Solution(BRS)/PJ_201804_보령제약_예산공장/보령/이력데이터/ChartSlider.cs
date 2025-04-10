using C1.Silverlight;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shapes;

namespace 보령
{
    public class ChartSlider : C1RangeSlider
    {
        // ** fields
        Rectangle _zoomBox;
        double _thumbWidth;

        // ** object model
        public double ThumbWidth
        {
            get { return _thumbWidth; }
        }

        public Rectangle Box
        {
            get { return _zoomBox; }
        }

        // ** ctor
        public ChartSlider()
        {
            _zoomBox = new Rectangle();
            _zoomBox.Fill = new SolidColorBrush(Color.FromArgb(32, 255, 255, 255));
            _zoomBox.Stroke = new SolidColorBrush(Color.FromArgb(200, 255, 255, 255));
            _zoomBox.StrokeThickness = 1;
            _zoomBox.IsHitTestVisible = false;
            Grid.SetColumn(_zoomBox, 1);
            Grid.SetColumnSpan(_zoomBox, 3);
        }

        // ** overrides
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // insert rectangle that represents zoom area
            Grid root = GetTemplateChild("HorizontalTemplate") as Grid;
            if (root != null)
                root.Children.Insert(0, _zoomBox);

            Thumb lthumb = GetTemplateChild("LowerThumb") as Thumb;
            if (lthumb != null)
            {
                _thumbWidth = lthumb.Width;
                if (double.IsNaN(_thumbWidth))
                {
                    lthumb.Measure(new Size(1000, 1000));
                    _thumbWidth = lthumb.DesiredSize.Width * 2;
                }
                double w = Math.Ceiling(_thumbWidth);
                // adjust zoom area margins
                _zoomBox.Margin = new Thickness(w, 0, w, 0);

                // move thumb on the border of zoom area
                lthumb.RenderTransform = new TranslateTransform() { X = 0.5 * _thumbWidth };

                // move thumb on the top of slider visual tree
                root.Children.Remove(lthumb);
                root.Children.Add(lthumb);
            }

            Thumb uthumb = GetTemplateChild("UpperThumb") as Thumb;
            if (uthumb != null)
                // move thumb on the border of zoom area
                uthumb.RenderTransform = new TranslateTransform() { X = -0.5 * _thumbWidth };

            // stretch middle thumb vertically
            Thumb mthumb = GetTemplateChild("MiddleThumb") as Thumb;
            if (mthumb != null)
                mthumb.Height = double.NaN;
        }
    }
}
