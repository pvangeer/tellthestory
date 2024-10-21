using System;
using System.Globalization;
using System.Windows.Data;
using StoryTree.Gui.ViewModels;

namespace StoryTree.Gui.Converters
{
    public class BusyIndicatorToEnabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is StorageState storageState))
                return value;

            return storageState == StorageState.Idle;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}