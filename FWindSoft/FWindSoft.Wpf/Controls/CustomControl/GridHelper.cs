

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FWindSoft.Wpf.Controls
{
    public class GridHelper
    {
        public static readonly DependencyProperty ShowBorderProperty =
            DependencyProperty.RegisterAttached("ShowBorder", typeof (bool), typeof (GridHelper)
                , new PropertyMetadata(OnShowBorderChanged));

        public static readonly DependencyProperty GridLineThicknessProperty =
            DependencyProperty.RegisterAttached("GridLineThickness", typeof (double), typeof (GridHelper),
                new PropertyMetadata(OnGridLineThicknessChanged));

        public static readonly DependencyProperty GridLineBrushProperty =
            DependencyProperty.RegisterAttached("GridLineBrush", typeof (Brush), typeof (GridHelper),
                new PropertyMetadata(OnGridLineBrushChanged));

        public static bool GetShowBorder(DependencyObject obj)
        {
            return (bool) obj.GetValue(ShowBorderProperty);
        }

        public static void SetShowBorder(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowBorderProperty,value);
        }

        public static void OnShowBorderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as Grid;
            if ((bool) e.OldValue)
                grid.Loaded -= (s, arg) => { };
            else
            {
                grid.Loaded += new RoutedEventHandler(GridLoaded);
            }
        }

        public static double GetGridLineThickness(DependencyObject obj)
        {
            return (double) obj.GetValue(GridLineThicknessProperty);
        }

        public static void SetGridLineThickness(DependencyObject obj,double value)
        {
            obj.SetValue(GridLineThicknessProperty, value);
        }

        public static void OnGridLineThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
        }

        public static Brush GetGridLineBrush(DependencyObject obj)
        {
            var brush= (Brush) obj.GetValue(GridLineBrushProperty);
            return brush ?? Brushes.LightGray;
        }

        public static void SetGridLineBrush(DependencyObject obj, Brush value)
        {
            obj.SetValue(GridLineBrushProperty,value);
        }

        public static void OnGridLineBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
        }

        private static void GridLoaded(object sender, RoutedEventArgs e)
        {
            var grid = sender as Grid;
            var row_count = grid.RowDefinitions.Count;
            var column_count = grid.ColumnDefinitions.Count;

            var controls = grid.Children;
            var count = controls.Count;

            for (int i = 0; i < count; i++)
            {
                var item = controls[i] as FrameworkElement;
                var row = Grid.GetRow((item));
                var column = Grid.GetColumn(item);
                var rowspan = Grid.GetColumnSpan(item);
                var columnspan = Grid.GetColumnSpan(item);

                var settingThickness = GetGridLineThickness(grid);
                var thickness = new Thickness(settingThickness/2);
                if (row == 0)
                    thickness.Top = settingThickness;
                if (row + rowspan == row_count)
                    thickness.Bottom = settingThickness;
                if (column == 0)
                    thickness.Left = settingThickness;
                if (column + columnspan==column_count)
                    thickness.Right = settingThickness;

                var border = new Border()
                {
                    BorderBrush = GetGridLineBrush(grid),
                    BorderThickness = thickness,
                    //Padding = new Thickness(0)
                };
                Grid.SetRow(border,row);
                Grid.SetColumn(border,column);
                Grid.SetRowSpan(border,rowspan);
                Grid.SetColumnSpan(border,columnspan);

                grid.Children.RemoveAt(i);
                border.Child = item;
                grid.Children.Insert(i,border);
            }
        }
    }
}
