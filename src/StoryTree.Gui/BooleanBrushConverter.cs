using System;
using System.Globalization;
using System.Windows.Media;
using System.Windows.Data;

namespace StoryTree.Gui
{
    public class BooleanBrushConverter : IValueConverter
    {
        public Brush FalseBrush { get; set; }
        public Brush TrueBrush { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (FalseBrush == null || TrueBrush == null || !(value is bool isLoading))
            {
                return new SolidColorBrush(Colors.Black);
            }

            return isLoading ? TrueBrush : FalseBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}