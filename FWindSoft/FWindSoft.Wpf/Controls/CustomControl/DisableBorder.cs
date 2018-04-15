

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FWindSoft.Wpf.Controls
{
    public class DisableBorder : Border
    {
        //public static readonly DependencyProperty DisableProperty = DependencyProperty.Register("Disable", typeof (bool),
        //    typeof (DisableBorder), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty DisableBrushProperty = DependencyProperty.Register("DisableBrush", typeof(Brush),
           typeof(DisableBorder), new FrameworkPropertyMetadata(Brushes.Red, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty DisableThicknessProperty = DependencyProperty.Register("DisableThickness", typeof(double),
           typeof(DisableBorder), new FrameworkPropertyMetadata(5d, FrameworkPropertyMetadataOptions.AffectsRender));
        static DisableBorder()
        {

            IsEnabledProperty.OverrideMetadata(typeof(DisableBorder), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));
        }
        #region 包装属性

        //public bool Disable
        //{
        //    set { SetValue(DisableProperty, value); }
        //    get { return (bool)GetValue(DisableProperty); }
        //}
        public Brush DisableBrush
        {
            set { SetValue(DisableBrushProperty, value); }
            get { return (Brush)GetValue(DisableBrushProperty); }
        }
        public double DisableThickness
        {
            set { SetValue(DisableThicknessProperty, value); }
            get { return (double)GetValue(DisableThicknessProperty); }
        }
        #endregion
        protected override void OnRender(DrawingContext dc)
        {
            UIElement child = this.Child;
            double opacity = this.Opacity;
            if (!IsEnabled)
            {
                Point point1 = new Point(0, ActualHeight);
                Point point2 = new Point(ActualWidth, 0);
                dc.DrawLine(new Pen(DisableBrush, DisableThickness), point1, point2);
                opacity = opacity/2;

            }
            if(child!=null)
               child.Opacity = opacity;
            base.OnRender(dc);
        }
    }
}
